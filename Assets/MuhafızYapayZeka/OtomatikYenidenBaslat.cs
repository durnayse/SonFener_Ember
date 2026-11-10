using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OtomatikYenidenBaslat : MonoBehaviour
{
    [SerializeField] private float beklemeSuresi = 3f;
    [SerializeField] private string sahneAdi = "Ember_Scene";

    private bool _basladi;

    private void OnEnable()
    {
        if (_basladi) return;

        // Panel oyun başında açık geliyorsa hemen yeniden başlatma
        if (Time.timeSinceLevelLoad < 0.5f) return;

        _basladi = true;
        StartCoroutine(YenidenBaslat());
    }

    private IEnumerator YenidenBaslat()
    {
        yield return new WaitForSecondsRealtime(beklemeSuresi);

        Time.timeScale = 1f;
        SceneManager.LoadScene(sahneAdi);
    }
}