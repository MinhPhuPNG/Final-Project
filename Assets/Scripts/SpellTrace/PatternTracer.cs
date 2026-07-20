using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; 
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class PatternTracer : MonoBehaviour
{
    [System.Serializable]
    public class PuzzleData
    {
        public string puzzleName = "Spell Pattern";

        [Tooltip("The parent GameObject containing all graphic overlays and checkpoint dots for this puzzle.")]
        public GameObject puzzleFolder; 

        [TextArea(2, 3)]
        public string instructionMessage = "Trace the spell symbol!";
        
        [Tooltip("If true, lifting the cursor won't trigger a mistake if a full line/segment was finished!")]
        public bool allowLiftingCursor = true;

        [HideInInspector] public List<Transform> checkpoints = new List<Transform>();
    }

    [Header("Puzzle Pool & Sequence Settings")]
    public List<PuzzleData> puzzles = new List<PuzzleData>();

    [Tooltip("How many puzzles from the list to play during this run.")]
    public int puzzlesToComplete = 8;

    [Tooltip("Global game timer in seconds for the whole run.")]
    public float timeLimitSeconds = 30f;

    [Header("TextMeshPro UI References")]
    public TextMeshProUGUI timerText;         
    public TextMeshProUGUI instructionText;   

    [Header("Heart UI & Audio")]
    public Graphic[] heartImages;             
    public AudioSource sfxAudioSource;        
    public AudioClip mistakeSound;            
    public AudioClip successSound;            

    [Header("Checkpoint SFX")]
    public AudioClip firstCheckpointSound;       
    public AudioClip subsequentCheckpointSound;  

    [Header("Line & UI Settings")]
    public float minDistanceBetweenPoints = 0.05f;
    
    [Tooltip("Radius in SCREEN PIXELS for hitting checkpoints.")]
    public float checkpointRadius = 20f; 

    [Tooltip("If true, straying too far away from the line between checkpoints resets the stroke.")]
    public bool enforcePathLine = true;
    public float maxStrayDistance = 35f;


    // Dynamic Line Generation
    private LineRenderer activeLineRenderer;
    private List<LineRenderer> activeStrokes = new List<LineRenderer>();
    private List<Vector3> drawnPoints = new List<Vector3>();
    private LineRenderer lineTemplate;

    // Game Session Tracking
    private List<PuzzleData> shuffledPuzzles = new List<PuzzleData>();
    private int currentPuzzleIndex = 0;
    private int currentCheckpointIndex = 0;
    private int completedCount = 0;
    private int currentStrikes = 0;
    
    private float timeRemaining;
    private bool isGameActive = false;
    private bool isTracing = false;

    void Start()
    {
        lineTemplate = GetComponent<LineRenderer>();
        if (lineTemplate != null) lineTemplate.enabled = false;

        // Index all checkpoints once at startup
        foreach (var puzzle in puzzles)
        {
            if (puzzle.puzzleFolder != null)
            {
                puzzle.puzzleFolder.SetActive(false);
                puzzle.checkpoints.Clear();
                
                CollectCheckpointsRecursively(puzzle.puzzleFolder.transform, puzzle.checkpoints);

                string cpNames = "";
                for (int i = 0; i < puzzle.checkpoints.Count; i++)
                {
                    cpNames += $"[{i + 1}] {puzzle.checkpoints[i].name} ";
                }

                Debug.Log($"<color=cyan>[TRACER LOG]:</color> Found <b>{puzzle.checkpoints.Count}</b> checkpoints in '{puzzle.puzzleFolder.name}': {cpNames}");
            }
        }

        StartGameSession();
    }
    void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void CollectCheckpointsRecursively(Transform parent, List<Transform> checkpointList)
    {
        foreach (Transform child in parent)
        {
            string nameLower = child.name.ToLower();

            if (nameLower.Contains("bg") || nameLower.Contains("background") || nameLower.Contains("pattern") || nameLower.Contains("symbol"))
            {
                if (child.childCount > 0)
                {
                    CollectCheckpointsRecursively(child, checkpointList);
                }
                continue;
            }

            if (nameLower.StartsWith("cp") || nameLower.Contains("checkpoint") || nameLower.Contains("dot") || nameLower.Contains("point"))
            {
                checkpointList.Add(child);
            }
            else if (child.childCount > 0)
            {
                CollectCheckpointsRecursively(child, checkpointList);
            }
        }
    }

    void StartGameSession()
    {
        currentStrikes = 0;
        ResetHeartUI();

        shuffledPuzzles = new List<PuzzleData>(puzzles);

        completedCount = 0;
        currentPuzzleIndex = 0;
        timeRemaining = timeLimitSeconds;
        isGameActive = true;

        LoadNextPuzzle();
    }

    void Update()
    {
        if (!isGameActive) return;

        // Global countdown timer
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            OnGameFailed("Time Ran Out!");
            return;
        }

        if (timerText != null)
            timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining) + "s";

        Pointer pointer = Pointer.current;
        if (pointer == null) return;

        if (pointer.press.wasPressedThisFrame)
        {
            StartTrace();
        }
        else if (pointer.press.isPressed && isTracing)
        {
            Vector2 screenPos = GetPointerScreenPosition();

            if (enforcePathLine && IsStrayingFromLine(screenPos))
            {
                RegisterMistake("Strayed off the pattern path!");
                return;
            }

            ContinueTrace();
            CheckProgress(screenPos);
        }
        else if (pointer.press.wasReleasedThisFrame && isTracing)
        {
            StopTrace();
        }
    }

    private void StartTrace()
    {
        PuzzleData activePuzzle = shuffledPuzzles[currentPuzzleIndex];
        Vector2 screenPointerPos = GetPointerScreenPosition();
        Vector3 worldPointerPos = GetPointerWorldPosition();

        if (activePuzzle.checkpoints.Count == 0) return;

        Vector2 nextCPPos = activePuzzle.checkpoints[currentCheckpointIndex].position;
        float distToTarget = Vector2.Distance(screenPointerPos, nextCPPos);

        if (distToTarget <= checkpointRadius)
        {
            Debug.Log($"[TRACER LOG]: Starting stroke at Checkpoint #{currentCheckpointIndex + 1} ('{activePuzzle.checkpoints[currentCheckpointIndex].name}')");
            
            isTracing = true;
            CreateNewStrokeLine();
            AddPoint(worldPointerPos);

            CheckProgress(screenPointerPos);
        }
        else
        {
            Debug.LogWarning($"[TRACER LOG]: Cannot start trace! Click was {distToTarget:F1}px away from Checkpoint #{currentCheckpointIndex + 1} (Must be within {checkpointRadius}px).");
        }
    }

    private void CreateNewStrokeLine()
    {
        GameObject newStrokeObj = new GameObject($"Stroke_{activeStrokes.Count + 1}");
        newStrokeObj.transform.SetParent(transform, false);

        activeLineRenderer = newStrokeObj.AddComponent<LineRenderer>();
        if (lineTemplate != null)
        {
            activeLineRenderer.sharedMaterial = lineTemplate.sharedMaterial;
            activeLineRenderer.startWidth = lineTemplate.startWidth;
            activeLineRenderer.endWidth = lineTemplate.endWidth;
            activeLineRenderer.startColor = lineTemplate.startColor;
            activeLineRenderer.endColor = lineTemplate.endColor;
            activeLineRenderer.useWorldSpace = lineTemplate.useWorldSpace;
            activeLineRenderer.sortingOrder = lineTemplate.sortingOrder;
        }

        activeLineRenderer.positionCount = 0;
        drawnPoints.Clear();
        activeStrokes.Add(activeLineRenderer);
    }

    private void CheckProgress(Vector2 currentScreenPoint)
    {
        PuzzleData activePuzzle = shuffledPuzzles[currentPuzzleIndex];
        
        if (currentCheckpointIndex >= activePuzzle.checkpoints.Count) return;

        Transform nextTargetCP = activePuzzle.checkpoints[currentCheckpointIndex];
        Vector2 targetScreenPos = nextTargetCP.position;
        float distance = Vector2.Distance(currentScreenPoint, targetScreenPos);

        if (distance <= checkpointRadius)
        {
            Debug.Log($"[TRACER LOG]: Checkpoint #{currentCheckpointIndex + 1} ('{nextTargetCP.name}') HIT!");

            PlayCheckpointSFX(currentCheckpointIndex == 0);

            currentCheckpointIndex++;

            if (currentCheckpointIndex >= activePuzzle.checkpoints.Count)
            {
                OnPuzzleSuccess();
            }
        }
    }

    private bool IsStrayingFromLine(Vector2 pointerScreenPos)
    {
        PuzzleData activePuzzle = shuffledPuzzles[currentPuzzleIndex];

        if (currentCheckpointIndex == 0 || currentCheckpointIndex >= activePuzzle.checkpoints.Count)
            return false;

        Vector2 lastHitCP = activePuzzle.checkpoints[currentCheckpointIndex - 1].position;
        Vector2 nextTargetCP = activePuzzle.checkpoints[currentCheckpointIndex].position;

        float distToSegment = DistanceToSegment(pointerScreenPos, lastHitCP, nextTargetCP);

        return distToSegment > maxStrayDistance;
    }

    private float DistanceToSegment(Vector2 point, Vector2 a, Vector2 b)
    {
        Vector2 ap = point - a;
        Vector2 ab = b - a;
        float sqrLen = ab.sqrMagnitude;

        if (sqrLen == 0) return Vector2.Distance(point, a);

        float t = Mathf.Clamp01(Vector2.Dot(ap, ab) / sqrLen);
        Vector2 projection = a + t * ab;
        return Vector2.Distance(point, projection);
    }

    private void ContinueTrace()
    {
        Vector3 worldPos = GetPointerWorldPosition();

        if (drawnPoints.Count == 0 || Vector3.Distance(drawnPoints[drawnPoints.Count - 1], worldPos) > minDistanceBetweenPoints)
        {
            AddPoint(worldPos);
        }
    }

    private void StopTrace()
    {
        isTracing = false;
        PuzzleData activePuzzle = shuffledPuzzles[currentPuzzleIndex];

        if (!activePuzzle.allowLiftingCursor && currentCheckpointIndex < activePuzzle.checkpoints.Count)
        {
            RegisterMistake($"Released mouse early before completing the trace!");
        }
    }

    private void AddPoint(Vector3 worldPosition)
    {
        if (activeLineRenderer == null) return;

        drawnPoints.Add(worldPosition);
        activeLineRenderer.positionCount = drawnPoints.Count;
        activeLineRenderer.SetPosition(drawnPoints.Count - 1, worldPosition);
    }

    private void PlayCheckpointSFX(bool isFirst)
    {
        if (sfxAudioSource == null) return;

        AudioClip clipToPlay = isFirst ? firstCheckpointSound : subsequentCheckpointSound;
        if (clipToPlay != null)
        {
            sfxAudioSource.PlayOneShot(clipToPlay);
        }
    }

    private void RegisterMistake(string reason)
    {
        isTracing = false;
        currentStrikes++;
        Debug.LogWarning($"[TRACER LOG]: Mistake! {reason}");

        if (sfxAudioSource != null && mistakeSound != null)
        {
            sfxAudioSource.PlayOneShot(mistakeSound);
        }

        int heartToHideIndex = heartImages.Length - currentStrikes;
        if (heartToHideIndex >= 0 && heartToHideIndex < heartImages.Length)
        {
            if (heartImages[heartToHideIndex] != null)
            {
                heartImages[heartToHideIndex].enabled = false;
            }
        }

        ClearAllStrokes();
        currentCheckpointIndex = 0;

        if (currentStrikes >= heartImages.Length)
        {
            OnGameFailed("Too Many Mistakes!");
        }
    }

    private void ClearAllStrokes()
    {
        foreach (var stroke in activeStrokes)
        {
            if (stroke != null)
            {
                Destroy(stroke.gameObject);
            }
        }
        activeStrokes.Clear();
        drawnPoints.Clear();
    }

    private void ResetHeartUI()
    {
        foreach (Graphic heart in heartImages)
        {
            if (heart != null)
            {
                heart.enabled = true;
            }
        }
    }

    private void LoadNextPuzzle()
    {
        // Hide every puzzle folder in the list
        foreach (var p in puzzles)
        {
            if (p.puzzleFolder != null)
                p.puzzleFolder.SetActive(false);
        }

        ClearAllStrokes();

        if (currentPuzzleIndex < shuffledPuzzles.Count && completedCount < puzzlesToComplete)
        {
            PuzzleData currentPuzzle = shuffledPuzzles[currentPuzzleIndex];

            // Simply activate the current puzzle object
            if (currentPuzzle.puzzleFolder != null)
            {
                currentPuzzle.puzzleFolder.SetActive(true);
            }

            if (instructionText != null)
            {
                instructionText.text = currentPuzzle.instructionMessage;
            }

            currentCheckpointIndex = 0;
        }
        else
        {
            OnAllPuzzlesCompleted();
        }
    }

    private void OnPuzzleSuccess()
    {
        isTracing = false;
        Debug.Log($"[TRACER LOG]: Puzzle Solved! ({completedCount + 1}/{puzzlesToComplete})");

        if (sfxAudioSource != null && successSound != null)
        {
            sfxAudioSource.PlayOneShot(successSound);
        }

        completedCount++;
        currentPuzzleIndex++;

        if (completedCount >= puzzlesToComplete)
        {
            OnAllPuzzlesCompleted();
        }
        else
        {
            LoadNextPuzzle();
        }
    }

    private void OnAllPuzzlesCompleted()
    {
        isGameActive = false;
        SceneManager.LoadScene("GameComplete");
    }

    private void OnGameFailed(string reason)
    {
        isGameActive = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private Vector2 GetPointerScreenPosition()
    {
        Pointer pointer = Pointer.current;
        if (pointer == null) return Vector2.zero;
        return pointer.position.ReadValue();
    }

    private Vector3 GetPointerWorldPosition()
    {
        Vector2 screenPos = GetPointerScreenPosition();
        Camera cam = Camera.main != null ? Camera.main : FindFirstObjectByType<Camera>();

        if (cam == null) return Vector3.zero;

        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));
        return worldPos;
    }
}