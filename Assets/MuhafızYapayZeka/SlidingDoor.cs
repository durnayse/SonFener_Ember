using System.Collections;
using UnityEngine;

public class SlidingDoor : MonoBehaviour, IDoor
{
    [SerializeField] private Vector3 hareketYonu = Vector3.up;
    [SerializeField] private float kaymaMesafesi = 6f;
    [SerializeField] private float sure = 2f;

    public bool AcikMi { get; private set; }

    public void Ac()
    {
        if (AcikMi) return;
        AcikMi = true;
        StartCoroutine(Kaydir());
    }

    private IEnumerator Kaydir()
    {
        Vector3 baslangic = transform.position;
        Vector3 hedef = baslangic + hareketYonu.normalized * kaymaMesafesi;
        float gecen = 0f;

        while (gecen < sure)
        {
            transform.position = Vector3.Lerp(baslangic, hedef, gecen / sure);
            gecen += Time.deltaTime;
            yield return null;
        }

        transform.position = hedef;
    }
}