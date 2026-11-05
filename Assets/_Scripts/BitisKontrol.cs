using UnityEngine;
using UnityEngine.SceneManagement; // Sahne geçişleri için gerekli

public class BitisKontrol : MonoBehaviour
{
    public GameObject basariPaneli; // Başarı ekranı arayüzü

    private void OnTriggerEnter(Collider diger)
    {
        // Çarpışan nesne Player etiketine sahipse
        if (diger.CompareTag("Player"))
        {
            if (basariPaneli != null)
            {
                basariPaneli.SetActive(true); // Başarı panelini aç
            }
            Time.timeScale = 0f; // Oyunu dondur (isteğe bağlı)
        }
    }
}