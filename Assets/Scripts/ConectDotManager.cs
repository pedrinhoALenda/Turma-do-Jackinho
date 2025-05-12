using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectDotsDrawer : MonoBehaviour
{
    public LayerMask dotLayer;
    public float minDistance = 0.1f;
    public GameObject linePrefab;
    public List<Collider2D> validZones;
    public int currentPoint = 1;
    public string nextSceneName;
    public int lineSortingOrder = 10;
    public Color[] lineColors;
    public float autoCompleteSpeed = 5f;
    public float errorMargin = 0.5f;

    private List<GameObject> permanentLines = new List<GameObject>();
    private LineRenderer currentLineRenderer;
    private List<Vector3> currentLinePoints = new List<Vector3>();
    private bool isDrawing = false;
    private Camera cam;
    private Collider2D currentZone;
    private bool isAutoCompleting = false;
    private Vector3 lastValidPosition;
    private Dot currentDot;
    private Dot nextDot;
    private Vector3 targetPosition; // Renomeado de autoCompleteTarget para targetPosition

    void Start()
    {
        cam = Camera.main;
        DeactivateAllZones();
        currentDot = FindDotByNumber(currentPoint);
    }

    void Update()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        if (isAutoCompleting)
        {
            AutoCompleteLine();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 10f, dotLayer);
            if (hit.collider != null && hit.collider.GetComponent<Dot>()?.pointNumber == currentPoint)
            {
                StartNewLine(currentDot.transform.position);
                ActivateZoneForCurrentPair();
                lastValidPosition = currentDot.transform.position;
            }
        }

        if (Input.GetMouseButton(0) && isDrawing)
        {
            if (IsInsideValidZone(mousePos) || Vector3.Distance(mousePos, lastValidPosition) < errorMargin)
            {
                ContinueLine(mousePos);
                lastValidPosition = mousePos;

                if (nextDot != null && Vector3.Distance(mousePos, nextDot.transform.position) < errorMargin)
                {
                    StartAutoComplete(nextDot.transform.position);
                }
            }
            else
            {
                ContinueLine(lastValidPosition);
            }
        }

        if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            if (nextDot == null || Vector3.Distance(currentLinePoints[currentLinePoints.Count - 1], nextDot.transform.position) > errorMargin)
            {
                isDrawing = false;
                return;
            }
            StartAutoComplete(nextDot.transform.position);
        }
    }

    void StartNewLine(Vector3 startPoint)
    {
        isDrawing = true;
        currentLinePoints.Clear();

        GameObject lineObj = Instantiate(linePrefab);
        currentLineRenderer = lineObj.GetComponent<LineRenderer>();
        currentLineRenderer.sortingOrder = lineSortingOrder;

        if (lineColors.Length > 0)
        {
            int colorIndex = (currentPoint - 1) % lineColors.Length;
            currentLineRenderer.startColor = lineColors[colorIndex];
            currentLineRenderer.endColor = lineColors[colorIndex];
        }

        currentLinePoints.Add(startPoint);
        UpdateCurrentLine();
    }

    void ContinueLine(Vector3 newPoint)
    {
        if (currentLinePoints.Count == 0 || Vector3.Distance(currentLinePoints[currentLinePoints.Count - 1], newPoint) > minDistance)
        {
            currentLinePoints.Add(newPoint);
            UpdateCurrentLine();
        }
    }

    void UpdateCurrentLine()
    {
        currentLineRenderer.positionCount = currentLinePoints.Count;
        currentLineRenderer.SetPositions(currentLinePoints.ToArray());
    }

    void StartAutoComplete(Vector3 target)
    {
        isAutoCompleting = true;
        isDrawing = false;
        targetPosition = target; // Usando a variável renomeada
    }

    void AutoCompleteLine()
    {
        Vector3 lastPoint = currentLinePoints[currentLinePoints.Count - 1];
        Vector3 newPos = Vector3.MoveTowards(lastPoint, targetPosition, autoCompleteSpeed * Time.deltaTime);

        ContinueLine(newPos);

        if (Vector3.Distance(newPos, targetPosition) < 0.01f)
        {
            CompleteCurrentLine();
        }
    }

    void CompleteCurrentLine()
    {
        isAutoCompleting = false;
        permanentLines.Add(currentLineRenderer.gameObject);

        currentPoint++;
        currentDot = nextDot;
        nextDot = FindDotByNumber(currentPoint + 1);

        DeactivateAllZones();

        if (currentPoint >= validZones.Count + 1)
        {
            LoadNextScene();
        }
        else
        {
            StartNewLine(currentDot.transform.position);
            ActivateZoneForCurrentPair();
            lastValidPosition = currentDot.transform.position;
        }
    }

    Dot FindDotByNumber(int pointNumber)
    {
        Dot[] allDots = FindObjectsOfType<Dot>();
        foreach (var dot in allDots)
        {
            if (dot.pointNumber == pointNumber)
                return dot;
        }
        return null;
    }

    void ActivateZoneForCurrentPair()
    {
        if (currentPoint - 1 < validZones.Count)
        {
            currentZone = validZones[currentPoint - 1];
            if (currentZone != null)
            {
                currentZone.gameObject.SetActive(true);
                nextDot = FindDotByNumber(currentPoint + 1);
            }
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

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Next scene name not set in the inspector.");
        }
    }
}