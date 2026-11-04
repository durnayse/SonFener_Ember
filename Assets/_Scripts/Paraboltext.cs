using UnityEngine;
using TMPro;

[ExecuteAlways]
[RequireComponent(typeof(TextMeshProUGUI))]
public class paraboltext : MonoBehaviour
{
    private TextMeshProUGUI m_TextMeshPro;

    [Header("Yay (Arc) Ayarlarý")]
    public float radius = 300f; 

    void Update()
    {
        m_TextMeshPro = GetComponent<TextMeshProUGUI>();
        m_TextMeshPro.ForceMeshUpdate();

        TMP_TextInfo textInfo = m_TextMeshPro.textInfo;
        int characterCount = textInfo.characterCount;

        if (characterCount == 0) return;

        float boundsCenter = (m_TextMeshPro.bounds.min.x + m_TextMeshPro.bounds.max.x) / 2f;

        for (int i = 0; i < characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int vertexIndex = charInfo.vertexIndex;
            int materialIndex = charInfo.materialReferenceIndex;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            
            Vector2 charMidBaseline = (vertices[vertexIndex + 0] + vertices[vertexIndex + 2]) / 2;
            float distFromCenter = charMidBaseline.x - boundsCenter;

            
            float angleRadian = distFromCenter / radius;
            float angleDegree = angleRadian * Mathf.Rad2Deg;

            Vector3 translation = new Vector3(
                (Mathf.Sin(angleRadian) * radius) - distFromCenter,
                (Mathf.Cos(angleRadian) * radius) - radius,
                0
            );

           
            Matrix4x4 matrix = Matrix4x4.TRS(translation, Quaternion.Euler(0, 0, -angleDegree), Vector3.one);

           
            for (int j = 0; j < 4; j++)
            {
                Vector3 orig = vertices[vertexIndex + j];
                Vector3 offsetFromMid = orig - (Vector3)charMidBaseline;
                vertices[vertexIndex + j] = (Vector3)charMidBaseline + matrix.MultiplyPoint3x4(offsetFromMid);
            }
        }

        m_TextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }
}