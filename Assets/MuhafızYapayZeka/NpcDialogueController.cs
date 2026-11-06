using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NpcDialogueController : MonoBehaviour
{
    [Header("Baðlantýlar")]
    [SerializeField] private OllamaClient ollamaClient;
    [SerializeField] private SlidingDoor kapi;

    [Header("Arayüz")]
    [SerializeField] private GameObject sohbetPaneli;
    [SerializeField] private TextMeshProUGUI periYaziAlani;
    [SerializeField] private TMP_InputField oyuncuCevapKutusu;
    [SerializeField] private Button gonderButonu;

    [Header("Oyun Ayarlarý")]
    [SerializeField, Range(1, 10)] private int kapiAcmaPuani = 8;
    [SerializeField] private int maksGecmisMesaj = 8;
    [SerializeField] private int maksDeneme = 2;
    [SerializeField] private bool puaniGoster = true;   // test/rapor için açýk kalsýn

    private const string SelamMesaji =
        "[Oyun bilgisi: Oyuncu az önce kapýya yaklaþtý. Onu kendi sözlerinle karþýla ve neden geçmek istediðini sor.]";

    private ILlmClient _llm;
    private IDoor _kapi;
    private INpcReplyParser _parser;
    private readonly NpcPromptBuilder _promptBuilder = new NpcPromptBuilder();
    private readonly List<ChatMessage> _gecmis = new List<ChatMessage>();

    private bool _oyuncuYakin;
    private bool _kapiAcildi;
    private bool _istekDevam;
    private bool _selamlandi;

    private void Awake()
    {
        _llm = ollamaClient;
        _kapi = kapi;
        _parser = new JsonNpcReplyParser();
    }

    private void Start()
    {
        if (sohbetPaneli != null) sohbetPaneli.SetActive(false);

        if (gonderButonu != null)
        {
            gonderButonu.onClick.RemoveAllListeners();
            gonderButonu.onClick.AddListener(MesajGonder);
        }
        if (oyuncuCevapKutusu != null)
        {
            oyuncuCevapKutusu.onSubmit.RemoveAllListeners();
            oyuncuCevapKutusu.onSubmit.AddListener(delegate { MesajGonder(); });
        }
    }

    // ---------- Yaklaþma / uzaklaþma ----------

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || _kapiAcildi) return;

        _oyuncuYakin = true;
        StartCoroutine(SohbetiAc());
    }

    private IEnumerator SohbetiAc()
    {
        yield return null;   // bir kare bekle: fizik olayý bitsin

        if (!_oyuncuYakin || _kapiAcildi) yield break;

        if (sohbetPaneli != null) sohbetPaneli.SetActive(true);
        if (oyuncuCevapKutusu != null) oyuncuCevapKutusu.text = "";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Ýlk karþýlama cümlesini de LLM üretir (sabit metin yok)
        if (!_selamlandi && !_istekDevam)
            StartCoroutine(PeriyeSor(SelamMesaji, true));
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _oyuncuYakin = false;
        StartCoroutine(SohbetiKapatGecikmeli());
    }

    private IEnumerator SohbetiKapatGecikmeli()
    {
        yield return null;   // bir kare bekle

        if (_oyuncuYakin) yield break;   // bu arada tekrar yaklaþtýysa kapatma

        // Sohbet zaten açýk deðilse imlece hiç dokunma
        bool panelAcikti = sohbetPaneli != null && sohbetPaneli.activeSelf;
        if (!panelAcikti) yield break;

        if (!_kapiAcildi) sohbetPaneli.SetActive(false);

        // Oyun durmuþsa (Game Over gibi) imleci kilitleme
        if (Time.timeScale == 0f) yield break;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ---------- Mesajlaþma ----------

    public void MesajGonder()
    {
        if (!_oyuncuYakin || _kapiAcildi || _istekDevam) return;
        if (oyuncuCevapKutusu == null) return;

        string mesaj = oyuncuCevapKutusu.text.Trim();
        if (string.IsNullOrEmpty(mesaj)) return;

        oyuncuCevapKutusu.text = "";
        StartCoroutine(PeriyeSor(mesaj, false));
    }

    private IEnumerator PeriyeSor(string kullaniciMesaji, bool selamlama)
    {
        _istekDevam = true;
        EtkilesimiAyarla(false);
        if (periYaziAlani != null) periYaziAlani.text = "Peri düþünüyor...";

        _gecmis.Add(new ChatMessage("user", kullaniciMesaji));
        GecmisiKirp();

        LlmSonuc sonuc = null;
        NpcReply cevap = null;
        bool basarili = false;

        for (int deneme = 0; deneme < maksDeneme; deneme++)
        {
            sonuc = null;
            yield return StartCoroutine(_llm.GonderAsync(MesajListesiOlustur(), r => sonuc = r));

            if (sonuc == null) break;

            if (sonuc.Durum == LlmDurum.Basarili)
            {
                if (_parser.TryParse(sonuc.HamIcerik, out cevap)) { basarili = true; break; }
                sonuc.Durum = LlmDurum.GecersizYanit;   // bozuk JSON -> tekrar dene
            }

            if (sonuc.Durum != LlmDurum.GecersizYanit) break;   // sunucu/zaman aþýmý: tekrar deneme
        }

        if (!basarili)
        {
            if (_gecmis.Count > 0) _gecmis.RemoveAt(_gecmis.Count - 1);   // cevapsýz kalan mesajý çýkar
            if (periYaziAlani != null)
                periYaziAlani.text = HataMesaji(sonuc != null ? sonuc.Durum : LlmDurum.SunucuYok);
            _istekDevam = false;
            EtkilesimiAyarla(true);
            yield break;
        }

        _gecmis.Add(new ChatMessage("assistant", JsonUtility.ToJson(cevap)));
        if (selamlama) _selamlandi = true;

        string metin = cevap.cevap;
        if (puaniGoster && !selamlama)
            metin += "\n\n<size=70%><i>(Ýkna: " + cevap.ikna_puani + "/10)</i></size>";
        if (periYaziAlani != null) periYaziAlani.text = metin;

        Debug.Log("[NPC] Mesaj: '" + kullaniciMesaji + "' | Puan: " + cevap.ikna_puani + " | Cevap: " + cevap.cevap);

        _istekDevam = false;

        if (!selamlama && cevap.ikna_puani >= kapiAcmaPuani)
        {
            _kapiAcildi = true;
            if (_kapi != null) _kapi.Ac();
            yield return new WaitForSeconds(4f);
            SohbetiKapat();
        }
        else
        {
            EtkilesimiAyarla(true);
        }
    }

    private List<ChatMessage> MesajListesiOlustur()
    {
        var liste = new List<ChatMessage> { new ChatMessage("system", _promptBuilder.Olustur()) };
        liste.AddRange(_gecmis);
        return liste;
    }

    private void GecmisiKirp()
    {
        while (_gecmis.Count > maksGecmisMesaj) _gecmis.RemoveAt(0);
    }

    private static string HataMesaji(LlmDurum durum)
    {
        switch (durum)
        {
            case LlmDurum.SunucuYok:
                return "Peri'ye ulaþýlamýyor. Ollama sunucusu açýk mý? Biraz sonra tekrar dene.";
            case LlmDurum.ZamanAsimi:
                return "Peri çok geç cevap verdi. Tekrar dene.";
            case LlmDurum.ModelHatasi:
                return "Peri'nin modeli bulunamadý. Model yüklü mü? (ollama pull)";
            default:
                return "Peri'nin sözleri anlaþýlamadý. Mesajýný tekrar yazar mýsýn?";
        }
    }

    private void EtkilesimiAyarla(bool aktif)
    {
        if (gonderButonu != null) gonderButonu.interactable = aktif;
        if (oyuncuCevapKutusu != null)
        {
            oyuncuCevapKutusu.interactable = aktif;
            if (aktif && _oyuncuYakin) oyuncuCevapKutusu.ActivateInputField();
        }
    }

    private void SohbetiKapat()
    {
        if (sohbetPaneli != null) sohbetPaneli.SetActive(false);

        if (Time.timeScale == 0f) return;   // oyun durmuþsa imlece dokunma

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}