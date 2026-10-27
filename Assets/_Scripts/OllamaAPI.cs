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
        Debug.Log("Ollama'ya istek atýlýyor...");
        // Modele Türkçe kurallarýna uymasý ve sadece tek bir kýsa cümle kurmasý için net bir talimat veriyoruz
        StartCoroutine(SendOllamaRequest("Sadece Türkçe konuþ. RPG oyunundaki bir köylüsün. Oyuncuya sadece 'Yabancý, köye hoþ geldin!' de. Baþka hiçbir þey ekleme."));
    }

    IEnumerator SendOllamaRequest(string promptText)
    {
        string url = "http://localhost:11434/api/generate";
        string jsonData = "{\"model\":\"phi3:mini\", \"prompt\":\"" + promptText + "\", \"stream\":false}";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Baðlantý Hatasý: " + request.error);
            }
            else
            {
                string hamJson = request.downloadHandler.text;
                OllamaCevapSablonu parcalanmisVeri = JsonUtility.FromJson<OllamaCevapSablonu>(hamJson);

                Debug.Log("Yapay Zekanýn Temiz Cevabý: " + parcalanmisVeri.response);

                if (diyalogMetinAlani != null)
                {
                    diyalogMetinAlani.text = parcalanmisVeri.response;
                }
            }
        }
    }
}