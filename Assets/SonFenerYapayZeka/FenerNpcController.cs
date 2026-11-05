using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FenerNpcController : MonoBehaviour
{
    [Header("Bağlantılar")]
    [SerializeField] private OllamaYeniClient llmClient;
    [SerializeField] private GameObject acilacakKapi;

    [SerializeField] private TextMeshProUGUI durumYazisiUI;

    [Header("Oyun Durumu")]
    [SerializeField] private int mevcutIknaPuani = 0;
    [SerializeField] private int hedefIknaPuani = 10;
    [SerializeField] private bool kapıAcildi = false;

    private List<ChatMessage> sohbetGecmisi = new List<ChatMessage>();
    private bool istekGidiyor = false;

    void Start()
    {
        if (llmClient == null)
            llmClient = GetComponent<OllamaYeniClient>();

        sohbetGecmisi = NpcPromptBuilder.OlusturBaslangicMesajlari();

        Debug.Log("[FenerNPC] Muhafız Peri yerini aldı. Oyuncuyu bekliyor...");
    }

    public void OyuncuMesajiGonder(string oyuncuMetni)
    {
        if (kapıAcildi || istekGidiyor || string.IsNullOrWhiteSpace(oyuncuMetni)) return;

        if (durumYazisiUI != null) durumYazisiUI.text = "Muhafız düşünüyor...";

        StartCoroutine(MesajGonderCoroutine(oyuncuMetni));
    }

    private IEnumerator MesajGonderCoroutine(string oyuncuMetni)
    {
        istekGidiyor = true;
        Debug.Log("[Oyuncu]: " + oyuncuMetni);

        sohbetGecmisi.Add(new ChatMessage("user", oyuncuMetni));

        LlmSonuc sonuc = null;
        yield return StartCoroutine(llmClient.GonderAsync(sohbetGecmisi, (yanit) => {
            sonuc = yanit;
        }));

        istekGidiyor = false;

        if (sonuc != null && sonuc.Durum == LlmDurum.Basarili)
        {
            NpcReply npcYanit = LlmResponseParser.Parse(sonuc.HamIcerik);
            mevcutIknaPuani = npcYanit.ikna_puani;

            if (durumYazisiUI != null)
            {
                durumYazisiUI.text = npcYanit.cevap;
            }

            Debug.Log("[Peri]: " + npcYanit.cevap + " (İkna Puanı: " + mevcutIknaPuani + "/" + hedefIknaPuani + ")");
        }
    }

}