using System.Collections.Generic;

public static class NpcPromptBuilder
{
    public const string SistemTalimati =
        "Sen 'Fener Kapýsý'nýn bilge ve gizemli Muhafýz Perisisin. Oyuncu bu kapýdan geçmek için seni ikna etmek zorunda. " +
        "Oyuncunun sözlerinin derinliðini, samimiyetini ve mantýðýný tartarak ona 0 ile 10 arasýnda bir 'ikna_puani' ver.\n\n" +
        "KURALLAR:\n" +
        "1. Kesinlikle sadece geçerli bir JSON nesnesi döndür.\n" +
        "2. JSON yapýsý þu þekilde olmalýdýr: {\"ikna_puani\": [0-10 arasý sayý], \"cevap\": \"[Perinin Türkçe konuþmasý]\"}\n" +
        "3. Oyuncu seni tamamen ikna ettiðinde (puan 10 olduðunda) kapýnýn açýlacaðýný belirten bir ifade kullan.\n" +
        "4. Asla JSON dýþýnda bir metin, açýklama veya markdown bloklarý (```json gibi) ekleme.";

    public static List<ChatMessage> OlusturBaslangicMesajlari()
    {
        return new List<ChatMessage>
        {
            new ChatMessage("system", SistemTalimati),
            new ChatMessage("assistant", "{\"ikna_puani\": 0, \"cevap\": \"Dur yolcu! Fener Kapýsý'nýn ardýndaki sýrlar ancak kalbi ve zihni berrak olanlara açýlýr. Neden buradan geçmeyi hak ettiðini bana kanýtla...\"}")
        };
    }
}
