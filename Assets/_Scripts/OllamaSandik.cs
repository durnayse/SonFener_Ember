using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OllamaSandik : MonoBehaviour
{
    [Header("UI Elemanlarý")]
    public TextMeshProUGUI altYaziAlani;
    public TMP_InputField cevapKutusu;
    public Button gonderButonu;

    private int dogruCevap = -999;
    private bool soruSoruldu = false;
    private bool cevaplandi = false;

    void Start()
    {
        // Baþlangýçta bu sandýða ait UI'lar kapalý baþlasýn
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (soruSoruldu) return;
            soruSoruldu = true;

            SoruOlustur();
        }
    }

    void SoruOlustur()
    {
        int sayi1 = UnityEngine.Random.Range(1, 10);
        int sayi2 = UnityEngine.Random.Range(1, 10);

        dogruCevap = sayi1 + sayi2;

        if (altYaziAlani != null)
        {
            altYaziAlani.gameObject.SetActive(true); // Yazý alanýný aç
            altYaziAlani.text = $"{sayi1} + {sayi2} kaçtýr?";
        }

        if (cevapKutusu != null)
        {
            cevapKutusu.gameObject.SetActive(true);
            cevapKutusu.text = "";
        }

        if (gonderButonu != null)
        {
            gonderButonu.gameObject.SetActive(true);
            gonderButonu.onClick.RemoveAllListeners();
            gonderButonu.onClick.AddListener(CevabiKontrolEt);
        }

        if (cevapKutusu != null)
        {
            cevapKutusu.onSubmit.RemoveAllListeners();
            cevapKutusu.onSubmit.AddListener(delegate { CevabiKontrolEt(); });
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log($"[YENÝ SANDIK SORUSU] {sayi1} + {sayi2} | Beklenen: {dogruCevap}");
    }

    public void CevabiKontrolEt()
    {
        if (cevaplandi) return;
        if (cevapKutusu == null || string.IsNullOrEmpty(cevapKutusu.text)) return;

        string temizCevap = cevapKutusu.text.Replace("\u200B", "").Trim();

        int oyuncuCevabi;
        bool sayiMi = int.TryParse(temizCevap, out oyuncuCevabi);

        Debug.Log($"[SANDIK KONTROL] Girilen: '{temizCevap}' | Sayý mý: {sayiMi} | Oyuncu: {oyuncuCevabi} | Beklenen: {dogruCevap}");

        if (sayiMi && oyuncuCevabi == dogruCevap)
        {
            cevaplandi = true;
            altYaziAlani.text = "TEBRÝKLER! Doðru cevap, +1 CAN KAZANDIN!";

            FindAnyObjectByType<CheckpointVeOlum>()?.CanArtir();

            // 1.5 saniye sonra ekraný tertemiz kapatýp sandýðý yok etsin
            StartCoroutine(BasariylaKapatRutini());
        }
        else
        {
            altYaziAlani.text = "Yanlýþ cevap! Tekrar dene.";
            cevapKutusu.text = "";
        }
    }

    IEnumerator BasariylaKapatRutini()
    {
        // Tebrik yazýsýný oyuncu okuyabilsin diye 1.5 saniye ekranda tutuyoruz
        yield return new WaitForSeconds(1.5f);

        // Þimdi HER ÞEYÝ (Yazý alaný dahil) kökten kapatýyoruz!
        if (altYaziAlani != null) altYaziAlani.gameObject.SetActive(false);
        if (cevapKutusu != null) cevapKutusu.gameObject.SetActive(false);
        if (gonderButonu != null)
        {
            gonderButonu.onClick.RemoveAllListeners();
            gonderButonu.gameObject.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Destroy(gameObject); // Sandýðý sahneden uçur
    }
}