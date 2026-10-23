using UnityEngine;

public class SalterKontrol : MonoBehaviour
{
    [Header("Ayarlar")]
    public GameObject acilirKapi;
    public float kalkisHizi = 3f;
    public float maksimumYukseklik = 8f;

    private bool tetiklendi = false;

    void Update()
    {
        if (tetiklendi && acilirKapi != null)
        {
            // Kapıyı yukarı doğru hareket ettir
            acilirKapi.transform.Translate(Vector3.up * kalkisHizi * Time.deltaTime, Space.World);

            // Belirlenen yüksekliğe ulaşınca hareketi durdur
            if (acilirKapi.transform.position.y >= maksimumYukseklik)
            {
                tetiklendi = false;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Temas eden obje "Player" etiketine sahipse kapıyı aç
        if (other.CompareTag("Player"))
        {
            tetiklendi = true;
        }
    }
}