using UnityEngine;

public class SalterKontrol : MonoBehaviour
{
    [Header("Ayarlar")]
    public GameObject acilirKapi;
    public float kalkisHizi = 3f;
    public float maksimumYukseklik = 8f;

    [Header("UI / Bilgi Mesajı")]
    [TextArea(2, 4)]
    public string salterMesaji = "Ember şalteri çekti! Köprü açılıyor ve yol aktif hale geliyor.";

    private bool oyuncuAlanda = false;
    private bool tetiklendi = false;

    void Update()
    {
        // Oyuncu şalter alanındayken E tuşuna basarsa
        if (oyuncuAlanda && !tetiklendi && Input.GetKeyDown(KeyCode.E))
        {
            tetiklendi = true;

            // Ember rehber mesajını ekrana yansıt
            OyunRehberi rehber = FindFirstObjectByType<OyunRehberi>();
            if (rehber != null)
            {
                rehber.MesajGoster(salterMesaji, 5f);
            }
        }

        // Tetiklendiyse kapıyı/köprüyü yukarı kaldır
        if (tetiklendi && acilirKapi != null)
        {
            acilirKapi.transform.Translate(Vector3.up * kalkisHizi * Time.deltaTime, Space.World);

            if (acilirKapi.transform.position.y >= maksimumYukseklik)
            {
                // Hedef yüksekliğe ulaştı
                tetiklendi = false; 
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.name.Contains("Oyuncu") || other.name.Contains("Player"))
        {
            oyuncuAlanda = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.name.Contains("Oyuncu") || other.name.Contains("Player"))
        {
            oyuncuAlanda = false;
        }
    }
}