using System.Collections;
using UnityEngine;

public class RehberTetikleyici : MonoBehaviour
{
    [Header("Sıralı Rehber Mesajları")]
    [TextArea(2, 4)]
    public string[] mesajlar;
    public float mesajlarArasiBekleme = 4f;

    [Header("Tetikleme Ayarları")]
    public bool sadeceBirKezCalissin = true;
    private bool tetiklendiMi = false;

    private void OnTriggerEnter(Collider other)
    {
        if (tetiklendiMi && sadeceBirKezCalissin) return;

        if (other.CompareTag("Player") || other.name.Contains("Oyuncu") || other.name.Contains("Player"))
        {
            OyunRehberi rehber = FindFirstObjectByType<OyunRehberi>();
            if (rehber != null && mesajlar != null && mesajlar.Length > 0)
            {
                StartCoroutine(SraliMesajlariGoster(rehber));
                tetiklendiMi = true;
            }
        }
    }

    private IEnumerator SraliMesajlariGoster(OyunRehberi rehber)
    {
        foreach (string mesaj in mesajlar)
        {
            rehber.MesajGoster(mesaj, mesajlarArasiBekleme);
            yield return new WaitForSeconds(mesajlarArasiBekleme + 0.5f);
        }
    }
}