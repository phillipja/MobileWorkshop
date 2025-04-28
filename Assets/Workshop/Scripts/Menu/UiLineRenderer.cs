using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer), typeof(AspectRatioFitter))]
public class UiLineRenderer : Graphic
{
    [SerializeField] private float thickness = 2f;
    [SerializeField, Range(4, 16)] private int cornerSegments = 8;
    [SerializeField, Range(10, 100)] private int maxPoints = 100;
    [SerializeField] private List<float> points = new List<float>();

    private AspectRatioFitter _ratioFitter;

    protected override void OnValidate()
    {
        base.OnValidate();

        if (_ratioFitter == null
            && TryGetComponent(out _ratioFitter) == false)
        {
            Debug.LogError($"Missing Component {nameof(AspectRatioFitter)}");
            return;
        }

        _ratioFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        _ratioFitter.aspectRatio = 1;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (points.Count < 2)
            return;

        float halfThickness = thickness / 2f;
        float radius = halfThickness / Mathf.Cos(Mathf.PI / cornerSegments);
        int vertCount = -1;

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        for (int i = 0; i < points.Count - 1; i++)
        {
            // Draw Line Segment
            Vector2 current = new(rectTransform.rect.size.x * i / maxPoints, points[i]);
            Vector2 next = new(rectTransform.rect.size.x * (i + 1) / maxPoints, points[i + 1]);

            Vector2 dir = (current - next).normalized;
            Vector2 norm = new(-dir.y, dir.x);

            vertex.position = current + norm * halfThickness;
            vh.AddVert(vertex);
            vertCount++;

            vertex.position = current - norm * halfThickness;
            vh.AddVert(vertex);
            vertCount++;

            vertex.position = next - norm * halfThickness;
            vh.AddVert(vertex);
            vertCount++;

            vertex.position = next + norm * halfThickness;
            vh.AddVert(vertex);
            vertCount++;

            vh.AddTriangle(vertCount - 3, vertCount - 2, vertCount - 1);
            vh.AddTriangle(vertCount - 1, vertCount, vertCount - 3);

            vertex.position = current;
            vh.AddVert(vertex);
            vertCount++;
            int centerVert = vertCount;

            // Draw Corner
            for (int j = 0; j < cornerSegments; j++)
            {
                float angle = (2 * Mathf.PI / cornerSegments) * j;
                vertex.position.x = current.x + radius * Mathf.Cos(angle);
                vertex.position.y = current.y + radius * Mathf.Sin(angle);
                vh.AddVert(vertex);
                vertCount++;

                if (j > 0)
                {
                    vh.AddTriangle(vertCount, vertCount - 1, centerVert);
                }
            }

            vh.AddTriangle(centerVert + 1, vertCount, centerVert);
        }

        Vector2 last = new(rectTransform.rect.size.x * (points.Count - 1) / maxPoints, points[^1]);
        vertex.position = last;
        vh.AddVert(vertex);
        vertCount++;
        int lastCenterVert = vertCount;

        // Draw Corner
        for (int j = 0; j < cornerSegments; j++)
        {
            float angle = (2 * Mathf.PI / cornerSegments) * j;
            vertex.position.x = last.x + radius * Mathf.Cos(angle);
            vertex.position.y = last.y + radius * Mathf.Sin(angle);
            vh.AddVert(vertex);
            vertCount++;

            if (j > 0)
            {
                vh.AddTriangle(vertCount, vertCount - 1, lastCenterVert);
            }
        }

        vh.AddTriangle(lastCenterVert + 1, vertCount, lastCenterVert);
    }

    public void PlotEvaluationOption(EvaluationOption option)
    {
        points.Clear();
        for (int i = 0; i < maxPoints; i++)
        {
            AddPoint(option.Evaluate((float)i / maxPoints));
        }
    }

    public void AddPoint(float t)
    {
        if (points.Count >= maxPoints)
        {
            points.RemoveAt(0);
        }

        float height = rectTransform.rect.size.y * t;
        points.Add(height);

        SetVerticesDirty();
    }
}
