using UnityEngine;

public class TuzakHareket : MonoBehaviour
{
    [Header("1. Sallanma / Dönme Ayarları")]
    public Vector3 sallanmaAcisi = Vector3.zero;
    public float sallanmaHizi = 3f;
    public bool sadeceTekYoneSallan = true;

    [Header("2. İnip Kalkma / Pres Ayarları")]
    public Vector3 hareketMesafesi = Vector3.zero;
    public float hareketHizi = 3f;

    [Header("3. Sağa Sola / İleri Geri Git Gel Hareketi (PingPong)")]
    [Tooltip("Gideceği maksimum mesafe (Örn: X: 4)")]
    public Vector3 sagaSolaMesafe = Vector3.zero;
    public float sagaSolaHiz = 3f;

    private Quaternion baslangicRotasyonu;
    private Vector3 baslangicPozisyonu;

    void Start()
    {
        baslangicRotasyonu = transform.localRotation;
        baslangicPozisyonu = transform.localPosition;
    }

    void Update()
    {
        // 1. Dönme Hareketi
        if (sallanmaAcisi != Vector3.zero)
        {
            float dalga = Mathf.Sin(Time.time * sallanmaHizi);
            if (sadeceTekYoneSallan) dalga = Mathf.Abs(dalga);
            transform.localRotation = baslangicRotasyonu * Quaternion.Euler(sallanmaAcisi * dalga);
        }

        // 2. İnip Kalkma Hareketi
        if (hareketMesafesi != Vector3.zero)
        {
            float mesafeOrani = Mathf.PingPong(Time.time * hareketHizi, 1f);
            transform.localPosition = baslangicPozisyonu + (hareketMesafesi * mesafeOrani);
        }

        // 3. Sağa Sola / İleri Geri Git Gel Hareketi (Sabit Sınırlar İçinde)
        if (sagaSolaMesafe != Vector3.zero)
        {
            // PingPong kullanarak belirlenen mesafe içinde kusursuz gidip gelir
            float mesafeOrani = Mathf.PingPong(Time.time * sagaSolaHiz, 1f);
            transform.localPosition = baslangicPozisyonu + (sagaSolaMesafe * mesafeOrani);
        }
    }
}