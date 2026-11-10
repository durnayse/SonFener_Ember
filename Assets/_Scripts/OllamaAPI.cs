using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

[System.Serializable]
public class OllamaCevapSablonu
{
    public string model;
    public string created_at;
    public string response;
    public bool done;
}

public class OllamaAPI : MonoBehaviour
{
    public TextMeshProUGUI diyalogMetinAlani;
    void Start()
    {
        Debug.Log("Ollama'ya istek at�l�yor...");
        // Modele T�rk�e kurallar�na uymas� ve sadece tek bir k�sa c�mle kurmas� i�in net bir talimat veriyoruz
        StartCoroutine(SendOllamaRequest("Sadece T�rk�e konu�. RPG oyunundaki bir k�yl�s�n. Oyuncuya sadece 'Yabanc�, k�ye ho� geldin!' de. Ba�ka hi�bir �ey ekleme."));
    }

    IEnumerator SendOllamaRequest(string promptText)
    {
        string url = "http://localhost:11434/api/generate";
        string jsonData = "{\"model\":\"llama3.2\", \"prompt\":\"" + promptText + "\", \"stream\":false}";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Ba�lant� Hatas�: " + request.error);
            }
            else
            {
                string hamJson = request.downloadHandler.text;
                OllamaCevapSablonu parcalanmisVeri = JsonUtility.FromJson<OllamaCevapSablonu>(hamJson);

                Debug.Log("Yapay Zekan�n Temiz Cevab�: " + parcalanmisVeri.response);

                if (diyalogMetinAlani != null)
                {
                    diyalogMetinAlani.text = parcalanmisVeri.response;
                }
            }
        }
    }
}