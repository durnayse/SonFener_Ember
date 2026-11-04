using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yönetimi için şart

public class SahneGecisTrigger : MonoBehaviour
{
    [Header("Gecis Yapilacak Sahne Adi")]
    public string hedefSahneAdi = "Valley"; // Unity Build Settings'deki sahne adı

    private void OnTriggerEnter(Collider other)
    {
        // Eğer alana giren ana karakter (Oyuncu) ise sahneyi değiştir
        if (other.CompareTag("Player"))
        {
            Debug.Log("Checkpoint tetiklendi! Valley sahnesine geçiliyor...");
            SceneManager.LoadScene(hedefSahneAdi);
        }
    }
}