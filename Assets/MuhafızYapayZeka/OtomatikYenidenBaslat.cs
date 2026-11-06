using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Bu script Game Over panelinin üstüne eklenir.
// Panel açılınca bekler, sonra sahneyi baştan yükler.
public class OtomatikYenidenBaslat : MonoBehaviour
{
    [SerializeField] private float beklemeSuresi = 3f;

    private void OnEnable()
    {
        StartCoroutine(YenidenBaslat());
    }

    private IEnumerator YenidenBaslat()
    {
        // timeScale 0 olsa bile çalışsın diye gerçek zaman kullanıyoruz
        yield return new WaitForSecondsRealtime(beklemeSuresi);

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
