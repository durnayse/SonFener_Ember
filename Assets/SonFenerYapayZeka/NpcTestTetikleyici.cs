using UnityEngine;

public class NpcTestTetikleyici : MonoBehaviour
{
    public FenerNpcController npcController;

    void Update()
    {
        // Klavyeden T tuþuna basýldýðýnda çalýþýr
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Test mesajý Ollama'ya gönderiliyor...");
            if (npcController != null)
            {
                npcController.OyuncuMesajiGonder("Selam peri, ben geldim. Lütfen kapýyý açar mýsýn?");
            }
        }
    }
}