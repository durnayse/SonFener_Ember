using UnityEngine;
using TMPro;
using System.Collections;

public class VadiyeMesaj : MonoBehaviour
{
    public TextMeshProUGUI mesajYazisi;
    public CanvasGroup mesajPaneli;
    public float girisGecikmesi = 1f;
    public float beklemeSuresi = 4f;
    public float solmaSuresi = 0.8f;

    IEnumerator Start()
    {
        mesajPaneli.gameObject.SetActive(true);
        mesajPaneli.alpha = 0f;

        yield return new WaitForSeconds(girisGecikmesi);

        yield return StartCoroutine(Goster("Vadiye ulaştın, tebrik ederim!"));
        yield return StartCoroutine(Goster("Kuleye girebilmek için muhafız peri'yi ikna etmelisin."));

        mesajPaneli.gameObject.SetActive(false);
    }

    IEnumerator Goster(string metin)
    {
        mesajYazisi.text = metin;

        yield return StartCoroutine(Solma(0f, 1f));
        yield return new WaitForSeconds(beklemeSuresi);
        yield return StartCoroutine(Solma(1f, 0f));
    }

    IEnumerator Solma(float baslangic, float bitis)
    {
        float t = 0f;
        while (t < solmaSuresi)
        {
            t += Time.deltaTime;
            mesajPaneli.alpha = Mathf.Lerp(baslangic, bitis, t / solmaSuresi);
            yield return null;
        }
        mesajPaneli.alpha = bitis;
    }
}