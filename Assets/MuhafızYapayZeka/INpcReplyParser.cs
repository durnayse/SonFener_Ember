using UnityEngine;

public interface INpcReplyParser
{
    bool TryParse(string hamMetin, out NpcReply cevap);
}

public class JsonNpcReplyParser : INpcReplyParser
{
    public bool TryParse(string hamMetin, out NpcReply cevap)
    {
        cevap = null;
        if (string.IsNullOrWhiteSpace(hamMetin)) return false;

        // Model JSON'un etrafýna fazladan yazý koyarsa sadece { ... } kýsmýný al
        int bas = hamMetin.IndexOf('{');
        int son = hamMetin.LastIndexOf('}');
        if (bas < 0 || son <= bas) return false;

        try
        {
            cevap = JsonUtility.FromJson<NpcReply>(hamMetin.Substring(bas, son - bas + 1));
        }
        catch
        {
            return false;
        }

        if (cevap == null || string.IsNullOrWhiteSpace(cevap.cevap)) return false;

        cevap.ikna_puani = Mathf.Clamp(cevap.ikna_puani, 0, 10);
        return true;
    }
}
