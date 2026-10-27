using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class OllamaSandik : MonoBehaviour
{
    [Header("UI Elemanlarý")]
    public TextMeshProUGUI altYaziAlani;
    public TMP_InputField cevapKutusu;
    public Button gonderButonu;

    [Header("Ollama Ayarlarý")]
    public string modelAdi = "phi3:mini";
    public string ollamaUrl = "http://localhost:11434/api/generate";

    private int dogruCevap;
    private bool soruSoruldu = false;

    void Start()
    {
        if (cevapKutusu != null) cevapKutusu.gameObject.SetActive(false);
        if (gonderButonu != null) gonderButonu.gameObject.SetActive(false);

        if (gonderButonu != null)
        {
            gonderButonu.onClick.AddListener(CevabiKontrolEt);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !soruSoruldu)
        {
            soruSoruldu = true;

            if (altYaziAlani != null)
                altYaziAlani.text = "Sandýk açýlýyor... Ollama matematik sorusunu hazýrlýyor...";

            StartCoroutine(OllamaSoruCek());
        }
    }

    IEnumerator OllamaSoruCek()
    {
        int sayi1 = Random.Range(1, 15);
        int sayi2 = Random.Range(1, 15);
        dogruCevap = sayi1 + sayi2;

        OllamaIstek istekVerisi = new OllamaIstek
        {
            model = modelAdi,
            prompt = $"{sayi1} + {sayi2} kaçtýr?",
            system = "Sen sadece matematik sorusunu soran bir oyun nesnesisin. Asla baþlýk, markdown (#) veya açýklama ekleme. Sadece soruyu yaz.",
            stream = false
        };

        string jsonData = JsonUtility.ToJson(istekVerisi);

        using (UnityWebRequest request = new UnityWebRequest(ollamaUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                OllamaYanit yanit = JsonUtility.FromJson<OllamaYanit>(request.downloadHandler.text);

                // Gelen metni temizleyip soru satýrýný çekiyoruz
                string gelenMetin = yanit.response;
                string temizSoru = $"{sayi1} + {sayi2} kaçtýr?";

                string[] satirlar = gelenMetin.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (string satir in satirlar)
                {
                    if (satir.Contains("?") || satir.Contains("kaçtýr"))
                    {
                        temizSoru = satir.Replace("#", "").Replace("*", "").Replace("-", "").Trim();
                        break;
                    }
                }

                if (altYaziAlani != null)
                    altYaziAlani.text = temizSoru;

                // Kutularý ekrana getiriyoruz
                if (cevapKutusu != null) cevapKutusu.gameObject.SetActive(true);
                if (gonderButonu != null) gonderButonu.gameObject.SetActive(true);

                // --- FAREYÝ SERBEST BIRAKIYORUZ (Týklayabilmen için) ---
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                if (altYaziAlani != null)
                    altYaziAlani.text = "Ollama ile baðlantý kurulamadý!";
            }
        }
    }

    public void CevabiKontrolEt()
    {
        if (cevapKutusu == null) return;

        int oyuncuCevabi;
        bool sayiMi = int.TryParse(cevapKutusu.text.Trim(), out oyuncuCevabi);

        if (sayiMi && oyuncuCevabi == dogruCevap)
        {
            altYaziAlani.text = "TEBRÝKLER! Doðru cevap, +1 CAN KAZANDIN!";

            cevapKutusu.gameObject.SetActive(false);
            gonderButonu.gameObject.SetActive(false);

            // --- FAREYÝ TEKRAR OYUNA KÝLÝTLÝYORUZ ---
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Destroy(gameObject, 3f);
        }
        else
        {
            altYaziAlani.text = "Yanlýþ cevap! Tekrar dene.";
            cevapKutusu.text = "";
        }
    }

    [System.Serializable]
    public class OllamaIstek
    {
        public string model;
        public string prompt;
        public string system;
        public bool stream;
    }

    [System.Serializable]
    public class OllamaYanit
    {
        public string response;
    }
}