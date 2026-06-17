using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[ExecuteAlways]
public class FireButtonBackground : BaseMeshEffect
{
    [Header("Gradient")]
    public Color topColor = new Color32(255, 170, 170, 255);
    public Color middleColor = new Color32(210, 15, 15, 255);
    public Color bottomColor = new Color32(20, 0, 0, 255);

    [Header("Corner Shape")]
    [Range(0f, 80f)]
    public float cornerCutSize = 25f;

    [Header("Border")]
    public bool drawBorder = true;
    public Color borderColor = new Color32(255, 255, 255, 180);
    [Range(1f, 10f)]
    public float borderThickness = 3f;

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;

        vh.Clear();

        Rect rect = graphic.rectTransform.rect;

        float xMin = rect.xMin;
        float xMax = rect.xMax;
        float yMin = rect.yMin;
        float yMax = rect.yMax;

        float cut = Mathf.Min(
            cornerCutSize,
            rect.width * 0.25f,
            rect.height * 0.45f
        );

        Vector2[] points =
        {
            new Vector2(xMin + cut, yMax),
            new Vector2(xMax - cut, yMax),
            new Vector2(xMax, yMax - cut),
            new Vector2(xMax, yMin + cut),
            new Vector2(xMax - cut, yMin),
            new Vector2(xMin + cut, yMin),
            new Vector2(xMin, yMin + cut),
            new Vector2(xMin, yMax - cut)
        };

        Vector2 center = GetCenter(points);

        int centerIndex = AddVertex(vh, center, GetColor(0.5f));

        for (int i = 0; i < points.Length; i++)
        {
            float t = Mathf.InverseLerp(yMin, yMax, points[i].y);
            AddVertex(vh, points[i], GetColor(t));
        }

        for (int i = 0; i < points.Length; i++)
        {
            int next = i + 1;

            if (next >= points.Length)
                next = 0;

            vh.AddTriangle(centerIndex, i + 1, next + 1);
        }

        if (drawBorder)
            AddBorder(vh, points);
    }

    private int AddVertex(VertexHelper vh, Vector2 position, Color color)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.position = position;
        vertex.color = color;

        vh.AddVert(vertex);

        return vh.currentVertCount - 1;
    }

    private Color GetColor(float t)
    {
        if (t < 0.7f)
        {
            return Color.Lerp(
                bottomColor,
                middleColor,
                t / 0.7f
            );
        }

        return Color.Lerp(
            middleColor,
            topColor,
            (t - 0.7f) / 0.3f
        );
    }

    private Vector2 GetCenter(Vector2[] points)
    {
        Vector2 center = Vector2.zero;

        for (int i = 0; i < points.Length; i++)
            center += points[i];

        center /= points.Length;

        return center;
    }

    private void AddBorder(VertexHelper vh, Vector2[] outer)
    {
        int count = outer.Length;
        Vector2 center = GetCenter(outer);

        Vector2[] inner = new Vector2[count];

        for (int i = 0; i < count; i++)
        {
            Vector2 directionToCenter = (center - outer[i]).normalized;
            inner[i] = outer[i] + directionToCenter * borderThickness;
        }

        int startIndex = vh.currentVertCount;

        for (int i = 0; i < count; i++)
        {
            AddVertex(vh, outer[i], borderColor);
            AddVertex(vh, inner[i], borderColor);
        }

        for (int i = 0; i < count; i++)
        {
            int next = i + 1;

            if (next >= count)
                next = 0;

            int outerCurrent = startIndex + i * 2;
            int innerCurrent = startIndex + i * 2 + 1;
            int outerNext = startIndex + next * 2;
            int innerNext = startIndex + next * 2 + 1;

            vh.AddTriangle(outerCurrent, outerNext, innerNext);
            vh.AddTriangle(outerCurrent, innerNext, innerCurrent);
        }
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        if (graphic != null)
            graphic.SetVerticesDirty();
    }
#endif
}