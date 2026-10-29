using System;
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

    // --- HOCANIN ÝSTEDÝÐÝ JSON MODELÝ ---
    [Serializable]
    public class SoruJsonModel
    {
        public string soru;
        public int cevap;
    }

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
                altYaziAlani.text = "Sandýk açýlýyor... Ollama JSON sorusunu hazýrlýyor...";

            StartCoroutine(OllamaSoruCek());
        }
    }

    IEnumerator OllamaSoruCek()
    {
        int sayi1 = UnityEngine.Random.Range(1, 15);
        int sayi2 = UnityEngine.Random.Range(1, 15);

        // JSON formatýnda çýktý istemek için özel system prompt
        string jsonSystemPrompt = "Sen JSON formatýnda çýktý üreten bir matematik botusun. " +
                                  "YANITIN SADECE VE SADECE GEÇERLÝ BÝR JSON OLMALIDIR. " +
                                  "Örnek format: {\"soru\": \"5 + 3 kaçtýr?\", \"cevap\": 8}";

        OllamaIstek istekVerisi = new OllamaIstek
        {
            model = modelAdi,
            prompt = $"{sayi1} + {sayi2} kaçtýr?",
            system = jsonSystemPrompt,
            stream = false,
            format = "json" // Ollama'nýn yerel JSON modunu zorluyoruz
        };

        string jsonData = JsonUtility.ToJson(istekVerisi);

        using (UnityWebRequest request = new UnityWebRequest(ollamaUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // 5 Saniye Zaman Aþýmý (Oyunun sonsuza kadar beklemesini önler)
            request.timeout = 30;

            yield return request.SendWebRequest();

            // --- HATA YÖNETÝMÝ (ERROR HANDLING) ---
            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    OllamaYanit yanit = JsonUtility.FromJson<OllamaYanit>(request.downloadHandler.text);

                    // LLM'den gelen JSON string'ini C# nesnesine dönüþtürme (Structured Output)
                    SoruJsonModel gelenSoru = JsonUtility.FromJson<SoruJsonModel>(yanit.response);

                    if (gelenSoru != null && !string.IsNullOrEmpty(gelenSoru.soru))
                    {
                        altYaziAlani.text = gelenSoru.soru;
                        dogruCevap = gelenSoru.cevap > 0 ? gelenSoru.cevap : (sayi1 + sayi2);
                    }
                    else
                    {
                        // Bozuk JSON gelirse Fallback (Yedek Plan)
                        altYaziAlani.text = $"{sayi1} + {sayi2} kaçtýr?";
                        dogruCevap = sayi1 + sayi2;
                    }

                    // UI Kutularýný Aç ve Fareyi Serbest Býrak
                    if (cevapKutusu != null) cevapKutusu.gameObject.SetActive(true);
                    if (gonderButonu != null) gonderButonu.gameObject.SetActive(true);

                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                catch (Exception ex)
                {
                    // JSON parsing hatasý durumunda oyunun çökmesini engeller
                    Debug.LogWarning("JSON Parse Hatasý: " + ex.Message);
                    altYaziAlani.text = $"[Yedek Soru] {sayi1} + {sayi2} kaçtýr?";
                    dogruCevap = sayi1 + sayi2;

                    if (cevapKutusu != null) cevapKutusu.gameObject.SetActive(true);
                    if (gonderButonu != null) gonderButonu.gameObject.SetActive(true);

                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
            else
            {
                // --- OLLAMA KAPALI VEYA SUNUCU ÇÖKMÜÞSE ---
                Debug.LogError("Ollama Sunucu Hatasý: " + request.error);
                if (altYaziAlani != null)
                    altYaziAlani.text = "Ollama Kapalý! (Yedek Soru): " + sayi1 + " + " + sayi2 + " kaçtýr?";

                dogruCevap = sayi1 + sayi2;

                if (cevapKutusu != null) cevapKutusu.gameObject.SetActive(true);
                if (gonderButonu != null) gonderButonu.gameObject.SetActive(true);

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
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

            // Can Artýrma Fonksiyonu
            FindObjectOfType<CheckpointVeOlum>()?.CanArtir();

            cevapKutusu.gameObject.SetActive(false);
            gonderButonu.gameObject.SetActive(false);

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

    [Serializable]
    public class OllamaIstek
    {
        public string model;
        public string prompt;
        public string system;
        public bool stream;
        public string format;
    }

    [Serializable]
    public class OllamaYanit
    {
        public string response;
    }
}