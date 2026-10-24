using UnityEngine;
using TMPro;

[ExecuteAlways]
[RequireComponent(typeof(TextMeshProUGUI))]
public class paraboltext : MonoBehaviour
{
    private TextMeshProUGUI m_TextMeshPro;

    [Header("Yay (Arc) Ayarlarý")]
    public float radius = 300f; // Yay yarýçapý (büyüttükçe yay düzleþir, küçülttükçe bükülür)

    void Update()
    {
        m_TextMeshPro = GetComponent<TextMeshProUGUI>();
        m_TextMeshPro.ForceMeshUpdate();

        TMP_TextInfo textInfo = m_TextMeshPro.textInfo;
        int characterCount = textInfo.characterCount;

        if (characterCount == 0) return;

        Bounds bounds = m_TextMeshPro.bounds;
        float boundsCenter = (bounds.min.x + bounds.max.x) / 2f;

        for (int i = 0; i < characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int vertexIndex = charInfo.vertexIndex;
            int materialIndex = charInfo.materialReferenceIndex;

            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            // Harfin merkez X konumuna göre çember yayý (sinüs/kosinüs) hesabý
            for (int j = 0; j < 4; j++)
            {
                Vector3 orig = vertices[vertexIndex + j];
                float distanceFromCenter = orig.x - boundsCenter;

                // Yay denklemi (Çember yayý formülü)
                float angle = distanceFromCenter / radius;
                float xOffset = radius * Mathf.Sin(angle) - distanceFromCenter;
                float yOffset = radius * (1f - Mathf.Cos(angle));

                vertices[vertexIndex + j] = new Vector3(orig.x + xOffset, orig.y - yOffset, orig.z);
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].mesh.vertices;
            m_TextMeshPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}