using System.Collections.Generic;
using UnityEngine;

public class ConnectDotsDrawer : MonoBehaviour
{
    public LayerMask dotLayer;
    public float minDistance = 0.1f;
    public GameObject linePrefab;
    public List<Collider2D> validZones; // Lista atribuída no Inspector

    public int currentPoint = 1;

    private List<GameObject> permanentLines = new List<GameObject>();
    private LineRenderer tempLineRenderer;
    private List<Vector3> tempPoints = new List<Vector3>();
    private bool isDrawing = false;
    private bool validConnection = false;
    private Camera cam;

    private Collider2D currentZone;

    void Start()
    {
        cam = Camera.main;
        DeactivateAllZones();
    }

    void Update()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 10f, dotLayer);
            if (hit.collider != null)
            {
                Dot dot = hit.collider.GetComponent<Dot>();
                if (dot != null && dot.pointNumber == currentPoint)
                {
                    StartDrawing(dot.transform.position);
                    ActivateZoneForCurrentPair();
                }
            }
        }

        if (Input.GetMouseButton(0) && isDrawing)
        {
            if (IsInsideValidZone(mousePos))
            {
                ContinueDrawing(mousePos);

                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 10f, dotLayer);
                if (hit.collider != null)
                {
                    Dot dot = hit.collider.GetComponent<Dot>();
                    if (dot != null && dot.pointNumber == currentPoint + 1)
                    {
                        validConnection = true;
                        FinalizeLine(dot.transform.position);
                        currentPoint++;
                        DeactivateAllZones();
                    }
                }
            }
            else
            {
                CancelTemporaryLine(); // linha fora da zona válida
                DeactivateAllZones();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!validConnection)
            {
                CancelTemporaryLine();
                DeactivateAllZones();
            }

            isDrawing = false;
            validConnection = false;
        }
    }

    void StartDrawing(Vector3 startPoint)
    {
        isDrawing = true;
        validConnection = false;
        tempPoints.Clear();

        GameObject tempLine = Instantiate(linePrefab);
        tempLineRenderer = tempLine.GetComponent<LineRenderer>();
        tempPoints.Add(startPoint);
        UpdateTempLine();
    }

    void ContinueDrawing(Vector3 newPoint)
    {
        if (tempPoints.Count == 0 || Vector3.Distance(tempPoints[tempPoints.Count - 1], newPoint) > minDistance)
        {
            tempPoints.Add(newPoint);
            UpdateTempLine();
        }
    }

    void UpdateTempLine()
    {
        tempLineRenderer.positionCount = tempPoints.Count;
        tempLineRenderer.SetPositions(tempPoints.ToArray());
    }

    void FinalizeLine(Vector3 endPoint)
    {
        tempPoints.Add(endPoint);
        UpdateTempLine();

        permanentLines.Add(tempLineRenderer.gameObject);
        tempLineRenderer = null;
        tempPoints.Clear();
        isDrawing = false;
    }

    void CancelTemporaryLine()
    {
        if (tempLineRenderer != null)
        {
            Destroy(tempLineRenderer.gameObject);
            tempLineRenderer = null;
            tempPoints.Clear();
        }

        isDrawing = false;
    }

    void ActivateZoneForCurrentPair()
    {
        if (currentPoint - 1 < validZones.Count)
        {
            currentZone = validZones[currentPoint - 1];
            if (currentZone != null)
                currentZone.gameObject.SetActive(true);
        }
    }

    void DeactivateAllZones()
    {
        foreach (Collider2D col in validZones)
        {
            if (col != null)
                col.gameObject.SetActive(false);
        }

        currentZone = null;
    }

    bool IsInsideValidZone(Vector2 position)
    {
        if (currentZone == null) return false;
        return currentZone.OverlapPoint(position);
    }
}
