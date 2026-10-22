using UnityEngine;

public class CheckpointVeOlum : MonoBehaviour
{
    // Oyuncunun doğacağı güncel kayıt noktası
    private Vector3 sonCheckpointNoktasi;
    private CharacterController controller;

    void Start()
    {
        // Oyun başladığında ilk doğduğu yeri başlangıç checkpoint'i kabul et
        sonCheckpointNoktasi = transform.position;
        controller = GetComponent<CharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. Eğer oyuncu bir Checkpoint noktasına değerse
        if (other.CompareTag("Checkpoint"))
        {
            sonCheckpointNoktasi = other.transform.position;
            Debug.Log("Checkpoint kaydedildi! Yeni konum: " + sonCheckpointNoktasi);
            
            // İsteğe bağlı: Checkpoint alındığında rengini yeşile çevir
            Renderer cpRenk = other.GetComponent<Renderer>();
            if (cpRenk != null) cpRenk.material.color = Color.green;
        }

        // 2. Eğer oyuncu ilk odadaki tehlikeli zemine (yere) düşerse
        if (other.CompareTag("TehlikeliZemin"))
        {
            Debug.Log("Yere değdin! Son Checkpoint noktasına ışınlanıyorsun...");
            SonCheckpointeIsinla();
        }
    }

    void SonCheckpointeIsinla()
    {
        // CharacterController kullanıyorsak ışınlamadan önce kapatıp açmak gerekir
        if (controller != null) controller.enabled = false;
        
        transform.position = sonCheckpointNoktasi + new Vector3(0, 1f, 0); // Hafif yukarıdan doğsun
        
        if (controller != null) controller.enabled = true;
    }
}