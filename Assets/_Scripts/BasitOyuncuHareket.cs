using UnityEngine;

public class BasitOyuncuHareket : MonoBehaviour
{
    [Header("Hareket Ayarlari")]
    public float yurumeHizi = 5f;
    public float ziplamaGucu = 1.5f;
    public float yercekimi = -9.81f;

    private CharacterController controller;
    private Vector3 hiz;
    private bool yerdeMi;
    
    // Doğma noktası değişkeni buraya tanımlanmalı
    private Vector3 respawnNoktasi;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        respawnNoktasi = transform.position;
    }

    void Update()
    {
        yerdeMi = controller.isGrounded;
        if (yerdeMi && hiz.y < 0)
        {
            hiz.y = -2f;
        }

        float yatay = Input.GetAxisRaw("Horizontal");
        float dikey = Input.GetAxisRaw("Vertical");
        Vector3 hareket = (transform.right * yatay + transform.forward * dikey).normalized;

        controller.Move(hareket * yurumeHizi * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && yerdeMi)
        {
            hiz.y = Mathf.Sqrt(ziplamaGucu * -2f * yercekimi);
        }

        hiz.y += yercekimi * Time.deltaTime;
        controller.Move(hiz * Time.deltaTime);
    }

    // CharacterController için çarpışma algılama
    void OnControllerColliderHit(ControllerColliderHit hit) 
    {
        if (hit.gameObject.CompareTag("TehlikeliZemin")) 
        {
            // Işınlanmanın çalışması için kontrolcüyü anlık kapatıp açıyoruz
            controller.enabled = false;
            transform.position = respawnNoktasi; 
            controller.enabled = true;
        }
    }

    // Görünmez Checkpoint alanına (Is Trigger) girme algılama
    void OnTriggerEnter(Collider alan) 
    {
        if (alan.CompareTag("Checkpoint")) 
        {
            respawnNoktasi = alan.transform.position; 
        }
    }
}