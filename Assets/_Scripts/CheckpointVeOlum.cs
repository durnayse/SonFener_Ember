using UnityEngine;
using System.Collections.Generic;

public class CheckpointVeOlum : MonoBehaviour
{
    private Vector3 sonCheckpointNoktasi;
    private CharacterController controller;

    [Header("Can Sistemi")]
    public List<GameObject> kalpler;
    private int mevcutCan;

    void Start()
    {
        sonCheckpointNoktasi = transform.position;
        controller = GetComponent<CharacterController>();

        if (kalpler != null && kalpler.Count > 0)
        {
            mevcutCan = kalpler.Count;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Checkpoint
        if (other.CompareTag("Checkpoint"))
        {
            sonCheckpointNoktasi = other.transform.position;
            Renderer cpRenk = other.GetComponent<Renderer>();
            if (cpRenk != null) cpRenk.material.color = Color.green;
        }

        // Düşme / Yanma
        if (other.CompareTag("TehlikeliZemin"))
        {
            CanEksilt();
            SonCheckpointeIsinla();
        }
    }

    // CAN EKSİLT
    public void CanEksilt()
    {
        if (mevcutCan > 0)
        {
            mevcutCan--;
            if (kalpler != null && mevcutCan < kalpler.Count)
            {
                kalpler[mevcutCan].SetActive(false);
            }
        }
    }

    // CAN ARTIR
    public void CanArtir()
    {
        if (kalpler != null && mevcutCan < kalpler.Count)
        {
            kalpler[mevcutCan].SetActive(true);
            mevcutCan++;
            Debug.Log("Can arttı! Güncel Can: " + mevcutCan);
        }
    }

    void SonCheckpointeIsinla()
    {
        if (controller != null) controller.enabled = false;
        transform.position = sonCheckpointNoktasi + new Vector3(0, 1f, 0);
        if (controller != null) controller.enabled = true;
    }
}