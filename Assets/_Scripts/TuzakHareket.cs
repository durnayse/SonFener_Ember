using UnityEngine;

public class TuzakHareket : MonoBehaviour
{
    [Header("Sallanma Ayarları (Sarkaç)")]
    public Vector3 sallanmaAcisi = new Vector3(0f, 0f, 45f); // Z ekseninde 45 derece sağa-sola açı
    public float hiz = 3f;

    private Quaternion baslangicRotasyonu;

    void Start()
    {
        baslangicRotasyonu = transform.localRotation;
    }

    void Update()
    {
        float salinim = Mathf.Sin(Time.time * hiz);
        // Objeyi kaydırmak yerine ipin ucundan sarkaç gibi sallar
        transform.localRotation = baslangicRotasyonu * Quaternion.Euler(sallanmaAcisi * salinim);
    }
}