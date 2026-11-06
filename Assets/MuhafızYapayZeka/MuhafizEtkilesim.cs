using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MuhafizEtkilesim : MonoBehaviour
{
    [Header("UI Elemanlari")]
    public GameObject muhafizPanel;
    public TMP_InputField inputField;
    public TextMeshProUGUI durumYazisi;
    public Button gonderButonu;

    [Header("Ayarlar")]
    public Transform oyuncu; // Oyuncunun Transform'unu buraya sürükleyeceðiz
    public float etkilesimMesafesi = 5f; // Yaklaþma mesafesi

    private bool sohbetAcik = false;

    void Start()
    {
        if (muhafizPanel != null)
            muhafizPanel.SetActive(false);

        if (gonderButonu != null)
            gonderButonu.onClick.AddListener(MesajiGonder);

        // Eðer oyuncu atanmadýysa tag ile otomatik bulmaya çalýþalým
        if (oyuncu == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                oyuncu = playerObj.transform;
        }
    }

    void Update()
    {
        if (oyuncu == null) return;

        // Muhafýz ile oyuncu arasýndaki mesafeyi ölçüyoruz
        float mesafe = Vector3.Distance(transform.position, oyuncu.position);

        // Eðer belirlenen mesafenin içindeyse ve E tuþuna basýldýysa
        if (mesafe <= etkilesimMesafesi)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                SohbetiAc();
            }
        }
        else
        {
            // Uzaklaþýrsa paneli otomatik kapat
            if (sohbetAcik)
            {
                SohbetiKapat();
            }
        }
    }

    void SohbetiAc()
    {
        sohbetAcik = true;
        if (muhafizPanel != null)
        {
            muhafizPanel.SetActive(true);
            if (durumYazisi != null)
                durumYazisi.text = "Muhafiz: Dur yolcu! Gecmek istiyorsan once bulmacayi coz.";
        }
    }

    void SohbetiKapat()
    {
        sohbetAcik = false;
        if (muhafizPanel != null)
            muhafizPanel.SetActive(false);
    }

    void MesajiGonder()
    {
        if (inputField != null && !string.IsNullOrEmpty(inputField.text))
        {
            Debug.Log("Gonderilen mesaj: " + inputField.text);
            if (durumYazisi != null)
                durumYazisi.text = "Muhafiz: Yanitini aldim, dusunuyorum...";

            inputField.text = "";
        }
    }
}