using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class OllamaClient : MonoBehaviour, ILlmClient
{
    [Header("Ollama Ayarlarý")]
    [SerializeField] private string modelAdi = "llama3.2:3b";
    [SerializeField] private string sunucuUrl = "http://localhost:11434/api/chat";
    [SerializeField] private int zamanAsimiSaniye = 60;
    [SerializeField, Range(0f, 1.5f)] private float sicaklik = 0.7f;
    [SerializeField] private int maksCevapTokeni = 200;

    // Ollama'nýn JSON þemasý: modeli bu yapýda cevap vermeye zorlar
    private const string JsonSemasi =
        "{\"type\":\"object\",\"properties\":{" +
        "\"ikna_puani\":{\"type\":\"integer\"}," +
        "\"cevap\":{\"type\":\"string\"}}," +
        "\"required\":[\"ikna_puani\",\"cevap\"]}";

    [Serializable] private class Secenekler { public float temperature; public int num_predict; }

    [Serializable]
    private class SohbetIstegi
    {
        public string model;
        public List<ChatMessage> messages;
        public bool stream;
        public string keep_alive;
        public Secenekler options;
    }

    [Serializable] private class SohbetYaniti { public ChatMessage message; }

    public IEnumerator GonderAsync(List<ChatMessage> mesajlar, Action<LlmSonuc> bitince)
    {
        var istek = new SohbetIstegi
        {
            model = modelAdi,
            messages = mesajlar,
            stream = false,
            keep_alive = "10m",
            options = new Secenekler { temperature = sicaklik, num_predict = maksCevapTokeni }
        };

        // JsonUtility "format" alanýný (þema) yazamadýðý için sona elle ekliyoruz
        string json = JsonUtility.ToJson(istek);
        json = json.Substring(0, json.Length - 1) + ",\"format\":" + JsonSemasi + "}";

        var sonuc = new LlmSonuc();

        using (var request = new UnityWebRequest(sunucuUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = zamanAsimiSaniye;

            yield return request.SendWebRequest();   // oyunu dondurmaz

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                string hata = request.error ?? "";
                sonuc.Durum = hata.ToLower().Contains("timeout") ? LlmDurum.ZamanAsimi : LlmDurum.SunucuYok;
                sonuc.Hata = hata;
            }
            else if (request.result == UnityWebRequest.Result.ProtocolError)
            {
                // Örn. 404 = model indirilmemiþ
                sonuc.Durum = LlmDurum.ModelHatasi;
                sonuc.Hata = "HTTP " + request.responseCode + " - " + request.downloadHandler.text;
            }
            else if (request.result != UnityWebRequest.Result.Success)
            {
                sonuc.Durum = LlmDurum.GecersizYanit;
                sonuc.Hata = request.error;
            }
            else
            {
                try
                {
                    var yanit = JsonUtility.FromJson<SohbetYaniti>(request.downloadHandler.text);
                    sonuc.HamIcerik = yanit?.message?.content;
                    sonuc.Durum = string.IsNullOrWhiteSpace(sonuc.HamIcerik)
                        ? LlmDurum.GecersizYanit
                        : LlmDurum.Basarili;
                }
                catch (Exception ex)
                {
                    sonuc.Durum = LlmDurum.GecersizYanit;
                    sonuc.Hata = ex.Message;
                }
            }
        }

        if (sonuc.Durum != LlmDurum.Basarili)
            Debug.LogWarning("[Ollama] " + sonuc.Durum + " : " + sonuc.Hata);

        bitince?.Invoke(sonuc);
    }
}
