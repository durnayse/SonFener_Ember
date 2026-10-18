using UnityEngine;
using UnityEngine.AI;

public class EmberController : MonoBehaviour
{
    [Header("Hedefler ve Objeler")]
    public Transform salterHedefi;   
    public Transform kopruObjesi;    
    public Transform oyuncuHedefi;   

    [Header("Ayarlar")]
    public float kopruAcilmaHizi = 3f;
    public Vector3 kopruAcikPozisyon = new Vector3(6f, 0f, 0f); // Koprunun yukari cikmis hali

    private NavMeshAgent agent;
    private bool saltereGidiyorMu = false;
    private bool kopruAcildiMi = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // TEST ICIN: Klavyeden 'E' tusuna basinca salter komutu verilir
        if (Input.GetKeyDown(KeyCode.E) && !kopruAcildiMi)
        {
            SalteriAcKomutuVer();
        }

        // Oyuncu takibi
        if (!saltereGidiyorMu && oyuncuHedefi != null)
        {
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
            agent.SetDestination(salterHedefi.position);
            Debug.Log("Ember komutu aldi: Tuzaklari asip saltere gidiyor!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Salter" || other.transform == salterHedefi)
        {
            kopruAcildiMi = true;
            saltereGidiyorMu = false;
            Debug.Log("Ember salteri cevirdi! Kopru aciliyor!");
        }
    }
}