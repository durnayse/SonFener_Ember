using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CheckpointVeOlum : MonoBehaviour
{
    private Vector3 sonCheckpointNoktasi;
    private CharacterController controller;

    [Header("Can Sistemi")]
    public List<GameObject> kalpler;
    private int mevcutCan;

    [Header("Arayüz (UI)")]
    public GameObject gameOverPanel;

    void Start()
    {
        sonCheckpointNoktasi = transform.position;
        controller = GetComponent<CharacterController>();

        // Oyun başladığında zamanı normale al ve Game Over panelini gizle
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
        // Checkpoint noktasından geçince
        if (other.CompareTag("Checkpoint"))
        {
            sonCheckpointNoktasi = other.transform.position;
            Renderer cpRenk = other.GetComponent<Renderer>();
            if (cpRenk != null) cpRenk.material.color = Color.green;
        }

        // Lavlara/tehlikeli zemine düşünce
        if (other.CompareTag("TehlikeliZemin"))
        {
            CanEksilt();
        }
    }

    public void CanEksilt()
    {
        if (mevcutCan > 0)
        {
            mevcutCan--;

            // İlgili kalp UI'ını gizle
            if (kalpler != null && mevcutCan < kalpler.Count)
            {
                kalpler[mevcutCan].SetActive(false);
            }

            // Can sıfırlandıysa oyunu bitir, bitmediyse checkpointe dön
            if (mevcutCan <= 0)
            {
                StartCoroutine(OyunuBitirVeOtomatikBaslat());
            }
            else
            {
                SonCheckpointeIsinla();
            }
        }
    }

    public void CanArtir()
    {
        if (kalpler != null && mevcutCan < kalpler.Count)
        {
            kalpler[mevcutCan].SetActive(true);
            mevcutCan++;
        }
    }

    void SonCheckpointeIsinla()
    {
        // Işınlanma sırasında controller hata vermesin diye anlık kapatıp açıyoruz
        if (controller != null) controller.enabled = false;
        transform.position = sonCheckpointNoktasi + new Vector3(0, 1f, 0);
        if (controller != null) controller.enabled = true;
    }

    // 3 SANİYE BEKLEYİP OTOMATİK YENİDEN BAŞLATAN BÖLÜM
    private IEnumerator OyunuBitirVeOtomatikBaslat()
    {
        // Game Over yazısını ekrana getir
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Karakterin hareket etmesini engelle (Konsoldaki kırmızı uyarı buradan gelir, oyunu bozmaz)
        if (controller != null) controller.enabled = false;

        // Tam 3 saniye bekle
        yield return new WaitForSeconds(3f);

        // Sahneyi pırıl pırıl baştan yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}