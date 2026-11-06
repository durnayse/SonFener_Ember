using System;

[Serializable]
public class ChatMessage
{
    public string role;
    public string content;

    public ChatMessage() { }

    public ChatMessage(string role, string content)
    {
        this.role = role;
        this.content = content;
    }
}

// LLM'in döndürmesi gereken yapýlandýrýlmýþ (JSON) çýktý
[Serializable]
public class NpcReply
{
    public int ikna_puani;   // 0-10
    public string cevap;     // Peri'nin söylediði Türkçe cümleler
}

public enum LlmDurum
{
    Basarili,
    SunucuYok,
    ZamanAsimi,
    ModelHatasi,
    GecersizYanit
}

public class LlmSonuc
{
    public LlmDurum Durum;
    public string HamIcerik;
    public string Hata;
}
