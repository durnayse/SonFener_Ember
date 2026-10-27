using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class OllamaSandik : MonoBehaviour
{
    [Header("UI Ayarlarý")]
    public TextMeshProUGUI altYaziAlani;

    [Header("Ollama Ayarlarý")]
    public string ollamaUrl = "http://localhost:11434/api/generate";
    public string modelAdi = "phi3:mini";

    private bool soruSoruldu = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !soruSoruldu)
        {
            soruSoruldu = true;
            altYaziAlani.text = "Sandýk açýlýyor... Ollama matematik sorusunu hazýrlýyor...";

            // Ollama'dan soruyu çekmek için Coroutine baþlatýyoruz
            StartCoroutine(OllamadanSoruCek());
        }
    }

    IEnumerator OllamadanSoruCek()
    {
        // Ollama'ya vereceðimiz komut (prompt)
        string promptMetni = "Sen bir oyun karakterisin. Oyuncunun sandýðý açabilmesi için ilkokul seviyesinde çok kýsa tek bir matematik sorusu sor (Örn: 12 + 15 kaçtýr?). Cevabý yazma, sadece soruyu sor.";

        OllamaRequest requestData = new OllamaRequest
        {
            model = modelAdi,
            prompt = promptMetni,
            stream = false
        };

        string jsonBody = JsonUtility.ToJson(requestData);
        byte[] rawData = System.Text.Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request = new UnityWebRequest(ollamaUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(rawData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Ýstek tamamlanana kadar bekle
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Gelen JSON cevabýný çözüyoruz
            OllamaResponse responseData = JsonUtility.FromJson<OllamaResponse>(request.downloadHandler.text);
            altYaziAlani.text = responseData.response;
        }
        else
        {
            altYaziAlani.text = "Ollama baðlantýsý kurulamadý! Lütfen terminalde Ollama'nýn açýk olduðunu kontrol et.";
            Debug.LogError("Ollama Hatasý: " + request.error);
        }
    }
}

// Ollama JSON Yapýsý
[System.Serializable]
public class OllamaRequest
{
    public string model;
    public string prompt;
    public bool stream;
}

[System.Serializable]
public class OllamaResponse
{
    public string response;
}