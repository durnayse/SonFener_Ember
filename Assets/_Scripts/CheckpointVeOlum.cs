using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Sahne yüklemek için eklediğimiz kütüphane

public class CheckpointVeOlum : MonoBehaviour
{
    private Vector3 sonCheckpointNoktasi;
    private CharacterController controller;

    [Header("Can Sistemi")]
    public List<GameObject> kalpler;
    private int mevcutCan;

    [Header("Arayüz (UI)")]
    public GameObject gameOverPanel; // Game Over panelimiz

    void Start()
    {
        sonCheckpointNoktasi = transform.position;
        controller = GetComponent<CharacterController>();

        // Oyun başladığında zaman akışını normale alıp Game Over panelini gizleyelim (güvenlik için)
        Time.timeScale = 1f;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (kalpler != null && kalpler.Count > 0)
        {
            mevcutCan = kalpler.Count;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Checkpoint
        if (other.CompareTag("Checkpoint"))
        {
            sonCheckpointNoktasi = other.transform.position;
            Renderer cpRenk = other.GetComponent<Renderer>();
            if (cpRenk != null) cpRenk.material.color = Color.green;
        }

        // Düşme / Yanma
        if (other.CompareTag("TehlikeliZemin"))
        {
            CanEksilt();
            SonCheckpointeIsinla();
        }
    }

    // CAN EKSİLT
    public void CanEksilt()
    {
        if (mevcutCan > 0)
        {
            mevcutCan--;
            if (kalpler != null && mevcutCan < kalpler.Count)
            {
                kalpler[mevcutCan].SetActive(false);
            }

            // Can sıfırlandıysa oyunu bitir
            if (mevcutCan <= 0)
            {
                OyunuBitir();
            }
        }
    }

    // CAN ARTIR
    public void CanArtir()
    {
        if (kalpler != null && mevcutCan < kalpler.Count)
        {
            kalpler[mevcutCan].SetActive(true);
            mevcutCan++;
            Debug.Log("Can arttı! Güncel Can: " + mevcutCan);
        }
    }

    void SonCheckpointeIsinla()
    {
        // Eğer can sıfırlandıysa ışınlamaya gerek yok, zaten oyun donacak
        if (mevcutCan <= 0) return;

        if (controller != null) controller.enabled = false;
        transform.position = sonCheckpointNoktasi + new Vector3(0, 1f, 0);
        if (controller != null) controller.enabled = true;
    }

    // OYUN BİTİŞ MANTIĞI
    private void OyunuBitir()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true); // Ekrana Game Over panelini getir
        }
        Time.timeScale = 0f; // Arkada akan oyunu ve zamanı tamamen durdur
    }

    // TEKRAR BAŞLAMA MANTIĞI (Butona bunu bağlayacağız)
    public void RestartGame()
    {
        Time.timeScale = 1f; // Zamanı normale döndür
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Şu anki sahneyi en baştan yükle
    }
}