using UnityEngine;

public class BasitOyuncuHareket : MonoBehaviour
{
    [Header("Hareket Ayarlari")]
    public float yurumeHizi = 5f;

    void Update()
    {
        // W, A, S, D veya Yön Tuşlarını oku
        float yatay = Input.GetAxisRaw("Horizontal");
        float dikey = Input.GetAxisRaw("Vertical");

        // Hareket yönünü hesapla (.normalized çapraz giderken hızlanmayı önler)
        Vector3 hareketYonu = new Vector3(yatay, 0f, dikey).normalized;

        // Karakteri hareket ettir
        transform.Translate(hareketYonu * yurumeHizi * Time.deltaTime, Space.World);
    }
}