using UnityEngine;

public class KarakterHareketi : MonoBehaviour
{
    public float hiz = 5f;
    public float ziplamaGucu = 5f;
    public float yercekimi = -9.81f;

    private CharacterController controller;
    private float dikeyHiz; // Boşluk hatası düzeltildi

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            gameObject.AddComponent<CharacterController>();
            controller = GetComponent<CharacterController>();
        }
    }

    void Update()
    {
        bool zemindeMi = controller.isGrounded;
        if (zemindeMi && dikeyHiz < 0)
        {
            dikeyHiz = -2f; 
        }

        float yatay = Input.GetAxis("Horizontal"); 
        float dikey = Input.GetAxis("Vertical");   

        Vector3 yon = transform.right * yatay + transform.forward * dikey;
        controller.Move(yon * hiz * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && zemindeMi)
        {
            dikeyHiz = Mathf.Sqrt(ziplamaGucu * -2f * yercekimi);
        }

        dikeyHiz += yercekimi * Time.deltaTime;
        controller.Move(new Vector3(0, dikeyHiz, 0) * Time.deltaTime);
    }
}