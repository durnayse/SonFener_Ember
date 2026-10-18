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
    public Vector3 kopruAcikPozisyon = new Vector3(0f, 2f, 3f); // Köprünün kalkacağı yön

    private NavMeshAgent agent;
    private bool saltereGidiyorMu = false;
    private bool kopruAcildiMi = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (durumText != null)
        {
            durumText.text = "[E]  Ember'i Saltere Gonder";
            durumText.color = new Color(0.95f, 0.95f, 0.95f); 
        }
    }

    void Update()
    {
        // 'E' tusuna basildiginda
        if (Input.GetKeyDown(KeyCode.E) && !kopruAcildiMi && !saltereGidiyorMu)
        {
            SalteriAcKomutuVer();
        }

        // Oyuncu takibi (Eğer şaltere gitmiyorsa)
        if (!saltereGidiyorMu && oyuncuHedefi != null)
        {
            agent.stoppingDistance = 2f; // Oyuncuyu takip ederken 2 birim mesafede dur
            agent.SetDestination(oyuncuHedefi.position);
        }

        // Kopruyu yukari kaldirma animasyonu
        if (kopruAcildiMi && kopruObjesi != null)
        {
            kopruObjesi.position = Vector3.MoveTowards(
                kopruObjesi.position, 
                kopruAcikPozisyon, 
                kopruAcilmaHizi * Time.deltaTime
            );
        }
    }

    public void SalteriAcKomutuVer()
    {
        if (salterHedefi != null)
        {
            saltereGidiyorMu = true;
            agent.stoppingDistance = 0.2f; // Şaltere tam varabilsin diye mesafeyi sıfıra yakınlaştırıyoruz!
            agent.SetDestination(salterHedefi.position);
            Debug.Log("Ember komutu aldi: Tuzaklari asip saltere gidiyor!");

            if (durumText != null)
            {
                durumText.text = "Ember tuzaklari asip saltere gidiyor...";
                durumText.color = new Color(1f, 0.8f, 0.3f); 
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Şaltere ulaştığını algıla
        if (other.gameObject.name == "Salter" || other.transform == salterHedefi)
        {
            kopruAcildiMi = true;
            saltereGidiyorMu = false;
            Debug.Log("Ember salteri cevirdi! Kopru aciliyor!");

            if (durumText != null)
            {
                durumText.text = "Kopru Acildi! Yola devam edebilirsin.";
                durumText.color = new Color(0.4f, 0.95f, 0.6f); 
            }
        }
    }
}