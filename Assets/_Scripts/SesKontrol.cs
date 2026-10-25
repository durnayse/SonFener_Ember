using UnityEngine;

public class SesKontrol : MonoBehaviour
{
    public AudioSource arkaPlanMuzigi;
    public GameObject kapatmaCizgisi;

    private bool sesAcik = true;

    void Start()
    {
        sesAcik = PlayerPrefs.GetInt("SesAyari", 1) == 1;

        MuzikDurumunuGuncelle();
    }

    public void SesAcKapat()
    {
        sesAcik = !sesAcik; 

        PlayerPrefs.SetInt("SesAyari", sesAcik ? 1 : 0);

        MuzikDurumunuGuncelle();
    }

    private void MuzikDurumunuGuncelle()
    {
        if (sesAcik)
        {
            arkaPlanMuzigi.mute = false; 
            kapatmaCizgisi.SetActive(false); 
        }
        else
        {
            arkaPlanMuzigi.mute = true; 
            kapatmaCizgisi.SetActive(true); 
        }
    }
}