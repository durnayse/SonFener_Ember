using UnityEngine;
using TMPro;

public class MuhafizEtkilesim : MonoBehaviour
{
    public GameObject durumYazisi;
    public GameObject cevapKutusu;
    public GameObject gonderButonu;

    public TMP_InputField mesajYazmaKutusu;
    public FenerNpcController npcController;

    void Start()
    {
        SohbetiAcKapat(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) SohbetiAcKapat(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) SohbetiAcKapat(false);
    }

    void SohbetiAcKapat(bool durum)
    {
        if (durumYazisi != null) durumYazisi.SetActive(durum);
        if (cevapKutusu != null) cevapKutusu.SetActive(durum);
        if (gonderButonu != null) gonderButonu.SetActive(durum);

        // Farenin kilidini arayüz (UI) için serbest býrak veya oyuna geri kilitle
        if (durum)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void MesajiGonder()
    {
        if (mesajYazmaKutusu != null && npcController != null)
        {
            npcController.OyuncuMesajiGonder(mesajYazmaKutusu.text);
            mesajYazmaKutusu.text = "";
        }
    }
}