using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class DrawInArea : MonoBehaviour
{
    public Collider2D drawArea; // Área permitida para desenhar
    public float minDistance = 0.1f; // Distância mínima entre pontos desenhados

    private LineRenderer lineRenderer;
    private List<Vector3> points = new List<Vector3>();
    private bool isDrawing = false;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (drawArea.OverlapPoint(worldPoint))
            {
                StartDrawing(worldPoint);
            }
        }

        if (Input.GetMouseButton(0) && isDrawing)
        {
            if (drawArea.OverlapPoint(worldPoint))
            {
                ContinueDrawing(worldPoint);
            }
            else
            {
                StopDrawing(); // Se sair da área, para de desenhar
            }
        }

        if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            StopDrawing();
        }
    }

    void StartDrawing(Vector2 startPoint)
    {
        isDrawing = true;
        points.Clear();
        points.Add(startPoint);
        UpdateLine();
    }

    void ContinueDrawing(Vector2 newPoint)
    {
        if (Vector2.Distance(points[points.Count - 1], newPoint) > minDistance)
        {
            points.Add(newPoint);
            UpdateLine();
        }
    }

    void StopDrawing()
    {
        isDrawing = false;
    }

    void UpdateLine()
    {
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}
