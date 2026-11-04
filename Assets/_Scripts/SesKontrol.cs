using UnityEngine;

public class SesKontrol : MonoBehaviour
{
    public AudioSource arkaPlanMuzigi;
    public GameObject kapatmaCizgisi;

    private bool sesAcik = true;

    // Sahne geçiþlerinde objeyi korumak için (Singleton mantýðý)
    public static SesKontrol instance;

    void Awake()
    {
        // Eðer sahnede zaten bir SesYoneticisi varsa yenisini sil (müzik üst üste binmesin)
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Sahne deðiþse de beni yok etme!
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
            if (kapatmaCizgisi != null) kapatmaCizgisi.SetActive(false);
        }
        else
        {
            arkaPlanMuzigi.mute = true;
            if (kapatmaCizgisi != null) kapatmaCizgisi.SetActive(true);
        }
    }
}