using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class EmberController : MonoBehaviour
{
    [Header("Hedefler ve Objeler")]
    public Transform salterHedefi;   
    public Transform kopruObjesi;    
    public Transform oyuncuHedefi;   

    [Header("UI (Arayuz) Elemanlari")]
    public TextMeshProUGUI durumText;

    [Header("Ayarlar")]
    public float kopruAcilmaHizi = 3f;
    public Vector3 kopruAcikPozisyon = new Vector3(0f, 2f, 3f);

    private NavMeshAgent agent;
    private bool saltereGidiyorMu = false;
    private bool kopruAcildiMi = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Oyun başında köprü AÇIK (aktif) başlasın
        if (kopruObjesi != null)
        {
            kopruObjesi.gameObject.SetActive(true);
        }

        // Oyun başında diyalog yazısı gizli olsun
        if (durumText != null)
        {
            durumText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // 'E' tusuna basildiginda komut ver
        if (Input.GetKeyDown(KeyCode.E) && !kopruAcildiMi && !saltereGidiyorMu)
        {
            SalteriAcKomutuVer();
        }

        // Oyuncu takibi (Eğer şaltere gitmiyorsa)
        if (!saltereGidiyorMu && oyuncuHedefi != null)
        {
            agent.stoppingDistance = 2f;
            agent.SetDestination(oyuncuHedefi.position);
        }
    }

    public void SalteriAcKomutuVer()
    {
        if (salterHedefi != null)
        {
            saltereGidiyorMu = true;
            agent.stoppingDistance = 0.2f; 
            agent.SetDestination(salterHedefi.position);
            Debug.Log("Ember komutu aldi: Şaltere gidiyor!");

            // Yolda giderken bilgi yazısı çıkabilir (İsteğe bağlı)
            if (durumText != null)
            {
                durumText.gameObject.SetActive(true);
                durumText.text = "Ember şaltere doğru ilerliyor...";
                durumText.color = new Color(1f, 0.8f, 0.3f); 
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ember şaltere vardığında
        if (saltereGidiyorMu && (other.gameObject.name == "Salter" || other.transform == salterHedefi || other.CompareTag("SalterTrigger")))
        {
            kopruAcildiMi = true;
            saltereGidiyorMu = false;
            Debug.Log("Ember şalteri çevirdi! Köprü kapandı/aktifleşti!");

            // Köprüyü KAPAT (gizle)
            if (kopruObjesi != null)
            {
                kopruObjesi.gameObject.SetActive(false); 
            }

            // Diyalog yazısını göster
            if (durumText != null)
            {
                durumText.gameObject.SetActive(true);
                durumText.text = "Ember: 'Benim minik kanatlarım var ama senin o dev botların buraya sığmaz sanırım, izle ve öğren!'";
                durumText.color = new Color(0.4f, 0.95f, 0.6f); 
            }
        }
    }
}