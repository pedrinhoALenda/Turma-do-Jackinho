using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectDotsDrawer : MonoBehaviour
{
    [Header("Dots & Zones")]
    public LayerMask dotLayer;
    public List<Collider2D> validZones;

    [Header("Line Settings")]
    public float minDistance = 0.1f;
    public GameObject linePrefab;
    public int lineSortingOrder = 10;
    public Transform linesParent;

    [Header("Game Flow")]
    public int currentPoint = 1;
    public string nextSceneName;
    public GameObject victoryButton;

    [Header("Settings")]
    public int maxDotsToConnect = 3;

    [Header("Colors")]
    public ColorRangeManager colorRangeManager;

    private LineRenderer currentLineRenderer;
    private List<Vector3> currentLinePoints = new List<Vector3>();

    private Camera cam;
    private Dot currentDot;
    private Dot nextDot;
    private bool isDrawing = false;

    private List<int> dotsSelecionados = new List<int>();

    void Start()
    {
        cam = Camera.main;
        InitializeGame();

        if (victoryButton != null)
            victoryButton.SetActive(false);
    }

    void InitializeGame()
    {
        currentDot = FindDotByNumber(currentPoint);
        nextDot = FindDotByNumber(currentPoint + 1);

        if (currentDot == null)
        {
            Debug.LogError($"Dot with number {currentPoint} not found!");
        }
    }

    void Update()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 10f, dotLayer);
            if (hit.collider != null && hit.collider.GetComponent<Dot>()?.pointNumber == currentPoint)
            {
                StartNewLine(currentDot.transform.position);
                isDrawing = true;
            }
        }

        if (isDrawing && currentLineRenderer != null)
        {
            if (IsInsideAnyZone(mousePos))
            {
                ContinueLine(mousePos);

                if (nextDot != null && Vector2.Distance(mousePos, nextDot.transform.position) < 0.5f)
                {
                    CompleteCurrentLine();

                    if (currentDot != null)
                    {
                        StartNewLine(currentDot.transform.position);
                        isDrawing = true;
                    }
                    else
                    {
                        isDrawing = false;
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            isDrawing = false;
        }
    }

    void StartNewLine(Vector3 startPoint)
    {
        GameObject lineObj = linesParent != null ? Instantiate(linePrefab, linesParent) : Instantiate(linePrefab);
        currentLineRenderer = lineObj.GetComponent<LineRenderer>();
        currentLineRenderer.sortingOrder = lineSortingOrder;

        if (colorRangeManager != null)
        {
            currentLineRenderer.startColor = colorRangeManager.GetColorForPoint(currentPoint);
            currentLineRenderer.endColor = colorRangeManager.GetColorForPoint(currentPoint);
        }

        currentLinePoints.Clear();
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

    void CompleteCurrentLine()
    {
        if (nextDot != null)
        {
            currentLinePoints.Add(nextDot.transform.position);
            UpdateCurrentLine();
        }

        dotsSelecionados.Add(currentPoint);
        currentPoint++;

        currentDot = FindDotByNumber(currentPoint);
        nextDot = FindDotByNumber(currentPoint + 1);

        currentLineRenderer = null;
        currentLinePoints.Clear();

        if (dotsSelecionados.Count >= maxDotsToConnect)
        {
            if (victoryButton != null && !victoryButton.activeSelf)
            {
                victoryButton.SetActive(true);
                Debug.Log($"Vitória ativada ao conectar {dotsSelecionados.Count} dots (máximo definido: {maxDotsToConnect})");
            }
            return;
        }

        if (currentDot == null)
        {
            LoadNextScene();
        }
    }

    Dot FindDotByNumber(int pointNumber)
    {
        if (pointNumber <= 0) return null;

        Dot[] allDots = FindObjectsOfType<Dot>();
        foreach (var dot in allDots)
        {
            if (dot.pointNumber == pointNumber)
                return dot;
        }
        return null;
    }

    bool IsInsideAnyZone(Vector2 position)
    {
        foreach (var zone in validZones)
        {
            if (zone != null && zone.OverlapPoint(position))
                return true;
        }
        return false;
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

    /// <summary>
    /// Reseta a contagem dos dots e o progresso, sem apagar o desenho.
    /// </summary>
    public void ResetDots()
    {
        currentPoint = 1;
        currentDot = FindDotByNumber(currentPoint);
        nextDot = FindDotByNumber(currentPoint + 1);
        
    
        dotsSelecionados.Clear();

        Debug.Log("Dots e contadores resetados.");
    }

    void OnDisable()
    {
        ResetDots();
    }
}
