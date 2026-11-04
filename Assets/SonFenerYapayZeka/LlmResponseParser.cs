using UnityEngine;

public static class LlmResponseParser
{
    public static NpcReply Parse(string hamJson)
    {
        if (string.IsNullOrWhiteSpace(hamJson))
        {
            return OlusturVarsayilan("Kapý muhafýzý sessizliðe büründü, seni duymazlýktan geliyor...");
        }

        try
        {
            // Ollama'ya JSON formatý zorladýðýmýz için doðrudan parse edebiliriz
            NpcReply yanit = JsonUtility.FromJson<NpcReply>(hamJson);

            if (yanit != null)
            {
                // Puanýn 0-10 arasýnda kalmasýný garantiye alalým (Güvenlik önlemi)
                yanit.ikna_puani = Mathf.Clamp(yanit.ikna_puani, 0, 10);
                if (string.IsNullOrEmpty(yanit.cevap))
                {
                    yanit.cevap = "...";
                }
                return yanit;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[Parser] JSON çözme hatasý: " + ex.Message + " | Ham içerik: " + hamJson);
        }

        // Eðer parse edilemezse ham metni direkt cevap olarak verelim
        return new NpcReply
        {
            ikna_puani = 0,
            cevap = hamJson
        };
    }

    private static NpcReply OlusturVarsayilan(string mesaj)
    {
        return new NpcReply
        {
            ikna_puani = 0,
            cevap = mesaj
        };
    }
}
