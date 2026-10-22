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

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        yerdeMi = controller.isGrounded;
        if (yerdeMi && hiz.y < 0)
        {
            hiz.y = -2f; // Karakter yerdeyken aşağı doğru biriken gücü sıfırla
        }

        float yatay = Input.GetAxisRaw("Horizontal");
        float dikey = Input.GetAxisRaw("Vertical");
        Vector3 hareket = (transform.right * yatay + transform.forward * dikey).normalized;

        controller.Move(hareket * yurumeHizi * Time.deltaTime);

        // Boşluk tuşuna basıldığında ve karakter yerdeyse zıpla
        if (Input.GetKeyDown(KeyCode.Space) && yerdeMi)
        {
            hiz.y = Mathf.Sqrt(ziplamaGucu * -2f * yercekimi);
        }

        // Yerçekimini sürekli uygula
        hiz.y += yercekimi * Time.deltaTime;
        controller.Move(hiz * Time.deltaTime);
    }
}