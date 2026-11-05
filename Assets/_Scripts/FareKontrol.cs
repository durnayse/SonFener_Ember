using UnityEngine;

public class FareKontrol : MonoBehaviour
{
    public float fareHassasiyeti = 200f;
    public Transform oyuncuGovdesi;

    private float xRotasyonu = 0f;

    void Start()
    {
        // Oyun başladığında imleci ekranın ortasına kilitle ve gizle
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float fareX = Input.GetAxis("Mouse X") * fareHassasiyeti * Time.deltaTime;
        float fareY = Input.GetAxis("Mouse Y") * fareHassasiyeti * Time.deltaTime;

        xRotasyonu -= fareY;
        xRotasyonu = Mathf.Clamp(xRotasyonu, -90f, 90f); // Kameranın çok fazla takla atmasını engeller

        transform.localRotation = Quaternion.Euler(xRotasyonu, 0f, 0f);
        
        if (oyuncuGovdesi != null)
        {
            oyuncuGovdesi.Rotate(Vector3.up * fareX);
        }
    }
}