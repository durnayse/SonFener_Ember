using UnityEngine;

public class KameraKontrol : MonoBehaviour
{
    [Header("Fare Ayarları")]
    public float hassasiyet = 300f;
    public Transform oyuncuGövdesi;

    private float xRotasyon = 0f;

    void Start()
    {
        // Fare imlecini oyun ekranına kilitle ve gizle
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float fareX = Input.GetAxis("Mouse X") * hassasiyet * Time.deltaTime;
        float fareY = Input.GetAxis("Mouse Y") * hassasiyet * Time.deltaTime;

        // Yukarı/aşağı bakma (Sadece kamerayı döndürür)
        xRotasyon -= fareY;
        xRotasyon = Mathf.Clamp(xRotasyon, -80f, 80f); // Tam tepeye veya tam aşağı dönmeyi sınırlar
        transform.localRotation = Quaternion.Euler(xRotasyon, 0f, 0f);

        // Sağa/sola bakma (Karakterin tamamını döndürür)
        oyuncuGövdesi.Rotate(Vector3.up * fareX);
    }
}