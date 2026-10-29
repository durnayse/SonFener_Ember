using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class OllamaNpc : MonoBehaviour
{
    [Header("Görsel ve Kapý Referanslarý")]
    public GameObject kapi;
    public GameObject sohbetPaneli;
    public TextMeshProUGUI periYaziAlani;
    public TMP_InputField oyuncuCevapKutusu;
    public Button gonderButonu;

    [Header("Ollama Ayarlarý")]
    public string modelAdi = "phi3:mini";
    public string ollamaUrl = "http://localhost:11434/api/generate";

    private bool kapiAcildiMi = false;
    private bool oyuncuYakinMi = false;

    [Serializable]
    public class NpcJsonModel
    {
        public string cevap;
        public bool kapiAcilsin;
    }

    void Start()
    {
        if (sohbetPaneli != null) sohbetPaneli.SetActive(false);

        // Butona Týklama Desteði
        if (gonderButonu != null)
        {
            gonderButonu.onClick.AddListener(MesajGonder);
        }

        // Enter Tuþuna Basarak Gönderme Desteði
        if (oyuncuCevapKutusu != null)
        {
            oyuncuCevapKutusu.onSubmit.AddListener(delegate { MesajGonder(); });
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !kapiAcildiMi)
        {
            oyuncuYakinMi = true;

            if (sohbetPaneli != null) sohbetPaneli.SetActive(true);
            if (oyuncuCevapKutusu != null) oyuncuCevapKutusu.gameObject.SetActive(true);
            if (gonderButonu != null) gonderButonu.gameObject.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (periYaziAlani != null)
                periYaziAlani.text = "Selam seyyah! Ben bu kapýnýn koruyucusu Peri. Geçmek istiyorsan soruma cevap vermelisin:\n\n'Dünyada kaç kýta vardýr?'";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            oyuncuYakinMi = false;

            if (!kapiAcildiMi && sohbetPaneli != null)
                sohbetPaneli.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void MesajGonder()
    {
        if (!oyuncuYakinMi || kapiAcildiMi) return;

        if (oyuncuCevapKutusu == null || string.IsNullOrEmpty(oyuncuCevapKutusu.text.Trim())) return;

        string oyuncuMesaji = oyuncuCevapKutusu.text.Trim();

        if (periYaziAlani != null)
            periYaziAlani.text = "Peri cevabýný kontrol ediyor...";

        oyuncuCevapKutusu.text = "";

        StartCoroutine(OllamaNpcSorgula(oyuncuMesaji));
    }

    IEnumerator OllamaNpcSorgula(string oyuncuMesaji)
    {
        string temizCevap = oyuncuMesaji.ToLower(new System.Globalization.CultureInfo("tr-TR")).Trim();
        bool cevapDogruMu = temizCevap.Contains("7") || temizCevap.Contains("yedi");

        string durumTalimati = cevapDogruMu
            ? "Oyuncunun cevabý DOÐRU (Dünyada 7 kýta vardýr)! Oyuncuyu tebrik et ve kapiAcilsin deðerini true yap."
            : "Oyuncunun cevabý YANLIÞ! Doðru cevabýn 7 olduðunu ima etmeden tekrar denemesini söyle ve kapiAcilsin deðerini false yap.";

        string systemPrompt = "Sen bilge ve tatlý bir kapý koruyucu Perisin. Sorduðun soru: 'Dünyada kaç kýta vardýr?'. " + durumTalimati + " SADECE GEÇERLÝ BÝR JSON FORMATINDA YANIT VER. Örnek format: {\"cevap\": \"Tebrikler doðru bildin!\", \"kapiAcilsin\": true}";

        OllamaIstek istek = new OllamaIstek
        {
            model = modelAdi,
            prompt = "Oyuncunun Cevabý: " + oyuncuMesaji,
            system = systemPrompt,
            stream = false,
            format = "json"
        };

        string jsonData = JsonUtility.ToJson(istek);

        using (UnityWebRequest request = new UnityWebRequest(ollamaUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 20;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    OllamaYanit yanit = JsonUtility.FromJson<OllamaYanit>(request.downloadHandler.text);
                    NpcJsonModel gelenYanit = JsonUtility.FromJson<NpcJsonModel>(yanit.response);

                    if (gelenYanit != null)
                    {
                        if (cevapDogruMu)
                            gelenYanit.kapiAcilsin = true;

                        periYaziAlani.text = gelenYanit.cevap;

                        if (gelenYanit.kapiAcilsin)
                        {
                            kapiAcildiMi = true;
                            StartCoroutine(KapiyiKaydir());
                            Invoke("SohbetiKapat", 4f);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("NPC JSON Parse Hatasý: " + ex.Message);
                    if (cevapDogruMu)
                    {
                        periYaziAlani.text = "Doðru cevap! Kapý açýlýyor...";
                        StartCoroutine(KapiyiKaydir());
                        kapiAcildiMi = true;
                        Invoke("SohbetiKapat", 4f);
                    }
                    else
                    {
                        periYaziAlani.text = "Yanlýþ cevap, tekrar dene!";
                    }
                }
            }
            else
            {
                Debug.LogError("Ollama Offline: " + request.error);
                periYaziAlani.text = "[Ollama Baðlantýsý Yok] Peri uykuda, kapý açýldý!";
                StartCoroutine(KapiyiKaydir());
                kapiAcildiMi = true;
                Invoke("SohbetiKapat", 4f);
            }
        }
    }

    IEnumerator KapiyiKaydir()
    {
        if (kapi == null) yield break;

        Vector3 baslangicPos = kapi.transform.position;
        Vector3 hedefPos = baslangicPos + (kapi.transform.right * 4f);

        float gecenSure = 0f;
        float hareketSuresi = 2f;

        while (gecenSure < hareketSuresi)
        {
            kapi.transform.position = Vector3.Lerp(baslangicPos, hedefPos, gecenSure / hareketSuresi);
            gecenSure += Time.deltaTime;
            yield return null;
        }

        kapi.transform.position = hedefPos;
    }

    void SohbetiKapat()
    {
        if (sohbetPaneli != null) sohbetPaneli.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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