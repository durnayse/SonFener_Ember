using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OllamaNpc : MonoBehaviour
{
    [Header("Görsel ve Kapý Referanslarý")]
    public GameObject kapi;
    public GameObject sohbetPaneli;
    public TextMeshProUGUI periYaziAlani;
    public TMP_InputField oyuncuCevapKutusu;
    public Button gonderButonu;

    private bool kapiAcildiMi = false;
    private bool oyuncuYakinMi = false;

    // Sorularýmýzý buraya sabitliyoruz
    private string orjinalSoru = "Selam seyyah! Ben bu kapýnýn koruyucusu Peri. Geçmek istiyorsan soruma cevap vermelisin:\n\n'Dünyada kaç kýta vardýr?'";
    private string yanlisCevapMetni = "Yanlýþ cevap seyyah! Kapalý kapýlar ardýnda kaldýn. Tekrar dene:\n\n'Dünyada kaç kýta vardýr?'";

    void Start()
    {
        if (sohbetPaneli != null) sohbetPaneli.SetActive(false);

        if (gonderButonu != null)
        {
            gonderButonu.onClick.RemoveAllListeners();
            gonderButonu.onClick.AddListener(MesajGonder);
        }

        if (oyuncuCevapKutusu != null)
        {
            oyuncuCevapKutusu.onSubmit.RemoveAllListeners();
            oyuncuCevapKutusu.onSubmit.AddListener(delegate { MesajGonder(); });
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !kapiAcildiMi)
        {
            oyuncuYakinMi = true;

            if (sohbetPaneli != null) sohbetPaneli.SetActive(true);

            if (periYaziAlani != null)
            {
                periYaziAlani.gameObject.SetActive(true);
                periYaziAlani.text = orjinalSoru; // Oyuncu gelince ilk soruyu sor
            }

            if (oyuncuCevapKutusu != null)
            {
                oyuncuCevapKutusu.gameObject.SetActive(true);
                oyuncuCevapKutusu.text = "";
            }
            if (gonderButonu != null) gonderButonu.gameObject.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            oyuncuYakinMi = false;

            if (!kapiAcildiMi && sohbetPaneli != null)
                sohbetPaneli.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void MesajGonder()
    {
        if (!oyuncuYakinMi || kapiAcildiMi) return;
        if (oyuncuCevapKutusu == null || string.IsNullOrEmpty(oyuncuCevapKutusu.text.Trim())) return;

        string oyuncuMesaji = oyuncuCevapKutusu.text;
        oyuncuCevapKutusu.text = ""; // Kutuyu anýnda temizle ki yeni cevap yazýlabilsin

        // Küçük harfe çevirip 7 veya yedi kelimesini arýyoruz
        string temizCevap = oyuncuMesaji.Replace("\u200B", "").Trim().ToLower(new System.Globalization.CultureInfo("tr-TR"));
        bool cevapDogruMu = temizCevap.Contains("7") || temizCevap.Contains("yedi");

        if (cevapDogruMu)
        {
            if (periYaziAlani != null)
                periYaziAlani.text = "Doðru cevap! Kapý açýlýyor...";

            kapiAcildiMi = true;
            StartCoroutine(KapiyiKaydir());
            Invoke(nameof(SohbetiKapat), 3f); // 3 saniye sonra UI ekranýný kapat
        }
        else
        {
            if (periYaziAlani != null)
                periYaziAlani.text = yanlisCevapMetni; // Yanlýþsa uyar ve soruyu ekranda tut
        }
    }

    IEnumerator KapiyiKaydir()
    {
        if (kapi == null) yield break;

        Vector3 baslangicPos = kapi.transform.position;
        Vector3 hedefPos = baslangicPos + (kapi.transform.right * 4f);

        float gecenSure = 0f;
        float hareketSuresi = 2f;

        while (gecenSure < hareketSuresi)
        {
            kapi.transform.position = Vector3.Lerp(baslangicPos, hedefPos, gecenSure / hareketSuresi);
            gecenSure += Time.deltaTime;
            yield return null;
        }

        kapi.transform.position = hedefPos;
    }

    void SohbetiKapat()
    {
        if (sohbetPaneli != null) sohbetPaneli.SetActive(false);
        if (periYaziAlani != null) periYaziAlani.gameObject.SetActive(false);
        if (oyuncuCevapKutusu != null) oyuncuCevapKutusu.gameObject.SetActive(false);
        if (gonderButonu != null) gonderButonu.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}