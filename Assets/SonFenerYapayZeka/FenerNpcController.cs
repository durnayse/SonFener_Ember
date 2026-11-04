using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenerNpcController : MonoBehaviour
{
    [Header("Bağlantılar")]
    [SerializeField] private OllamaYeniClient llmClient; // Az önce yazdığımız istemci
    [SerializeField] private GameObject acilacakKapi;    // İkna olunca açılacak kapı objesi

    [Header("Oyun Durumu")]
    [SerializeField] private int mevcutIknaPuani = 0;
    [SerializeField] private int hedefIknaPuani = 10;
    [SerializeField] private bool kapıAcildi = false;

    private List<ChatMessage> sohbetGecmisi = new List<ChatMessage>();
    private bool istekGidiyor = false;

    void Start()
    {
        // Bileşen otomatik atanmadıysa sahnede arayalım
        if (llmClient == null)
            llmClient = GetComponent<OllamaYeniClient>();

        // Başlangıç promptlarını yükleyelim
        sohbetGecmisi = NpcPromptBuilder.OlusturBaslangicMesajlari();

        Debug.Log("[FenerNPC] Muhafız Peri yerini aldı. Oyuncuyu bekliyor...");
    }

    // Oyuncu bir şeyler yazdığında (UI'dan veya konsoldan) bu fonksiyonu çağıracağız
    public void OyuncuMesajiGonder(string oyuncuMetni)
    {
        if (kapıAcildi || istekGidiyor || string.IsNullOrWhiteSpace(oyuncuMetni)) return;

        StartCoroutine(MesajGonderCoroutine(oyuncuMetni));
    }

    private IEnumerator MesajGonderCoroutine(string oyuncuMetni)
    {
        istekGidiyor = true;
        Debug.Log("[Oyuncu]: " + oyuncuMetni);

        // Oyuncunun mesajını geçmişe ekleyelim
        sohbetGecmisi.Add(new ChatMessage("user", oyuncuMetni));

        LlmSonuc sonuc = null;
        yield

 return StartCoroutine(llmClient.GonderAsync(sohbetGecmisi, (yanit) => {
     sonuc = yanit;
 }));

        istekGidiyor = false;

        if (sonuc != null && sonuc.Durum == LlmDurum.Basarili)
        {
            // Gelen ham cevabı parse edelim
            NpcReply npcYanit = LlmResponseParser.Parse(sonuc.HamIcerik);

            // Puanı güncelleyelim
            mevcutIknaPuani = npcYanit.ikna_puani;
            Debug.Log("[Peri]: " + npcYanit.cevap + " (İkna Puanı: " + mevcutIknaPuani + "/" + hedefIknaPuani + ")");

            // Asistanın cevabını hafızaya ekleyelim ki geçmişi bilsin
            sohbetGecmisi.Add(new ChatMessage("assistant", sonuc.HamIcerik));

            // İkna kontrolü
            if (mevcutIknaPuani >= hedefIknaPuani && !kapıAcildi)
            {
                KapiyiAc();
            }
        }
        else
        {
            Debug.LogWarning("[FenerNPC] Yapay zekadan yanıt alınamadı. Durum: " + (sonuc?.Durum.ToString() ?? "Bilinmiyor"));
        }
    }

    private void KapiyiAc()
    {
        kapıAcildi = true;
        Debug.Log("🎉 TEBRİKLER! Peri ikna oldu ve Fener Kapısı açıldı!");

        if (acilacakKapi != null)
        {
            acilacakKapi.SetActive(false); // Kapıyı yok et veya aç
        }
    }
}