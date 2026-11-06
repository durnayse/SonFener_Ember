using System.Collections;
using UnityEngine;
using TMPro;

public class OyunRehberi : MonoBehaviour
{
    [Header("UI Bağlantıları")]
    public GameObject bilgiKutusu;
    public TextMeshProUGUI bilgiYazisi;

    [Header("Başlangıç Mesajı Ayarları")]
    [TextArea(2, 4)]
    public string baslangicMesaji = "Alttaki lava değmeden taşların üstünden ilerleyerek Checkpoint noktasına ulaşman lazım! Ama dikkat et, bazı taşlar tuzaklı olabilir...";
    public float ekrandaKalmaSuresi = 7f;

    private Coroutine aktifMesajCoroutine;

    void Start()
    {
        // Oyun başladığında ilk mesajı otomatik göster
        MesajGoster(baslangicMesaji, ekrandaKalmaSuresi);
    }

    public void MesajGoster(string mesaj, float sure)
    {
        if (aktifMesajCoroutine != null)
        {
            StopCoroutine(aktifMesajCoroutine);
        }
        aktifMesajCoroutine = StartCoroutine(MesajSuresi(mesaj, sure));
    }

    private IEnumerator MesajSuresi(string mesaj, float sure)
    {
        bilgiKutusu.SetActive(true);
        bilgiYazisi.text = mesaj;
        yield return new WaitForSeconds(sure);
        bilgiKutusu.SetActive(false);
    }
}