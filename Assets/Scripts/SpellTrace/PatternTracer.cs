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
        public GameObject puzzleFolder; 
        public string instructionMessage = "Trace the spell symbol!";
        [HideInInspector] public List<Transform> checkpoints = new List<Transform>();
    }

    [Header("Puzzle Pool & Settings")]
    public List<PuzzleData> puzzles = new List<PuzzleData>();
    public int puzzlesToComplete = 3;
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
    [Tooltip("Radius in SCREEN PIXELS. Set between 40 and 80 for UI RectTransforms!")]
    public float checkpointRadius = 50f;      
    public string returnSceneName = "NightScene";

    private LineRenderer lineRenderer;
    private List<Vector3> drawnPoints = new List<Vector3>();
    
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
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;

        foreach (var puzzle in puzzles)
        {
            if (puzzle.puzzleFolder != null)
            {
                puzzle.puzzleFolder.SetActive(false);
                puzzle.checkpoints.Clear();
                foreach (Transform child in puzzle.puzzleFolder.transform)
                {
                    puzzle.checkpoints.Add(child);
                }
                Debug.Log($"[TRACER LOG]: Puzzle '{puzzle.puzzleFolder.name}' configured with {puzzle.checkpoints.Count} checkpoint(s).");
            }
        }

        StartGameSession();
    }

    void StartGameSession()
    {
        currentStrikes = 0;
        ResetHeartUI();

        shuffledPuzzles = new List<PuzzleData>(puzzles);
        for (int i = 0; i < shuffledPuzzles.Count; i++)
        {
            PuzzleData temp = shuffledPuzzles[i];
            int randomIndex = Random.Range(i, shuffledPuzzles.Count);
            shuffledPuzzles[i] = shuffledPuzzles[randomIndex];
            shuffledPuzzles[randomIndex] = temp;
        }

        completedCount = 0;
        currentPuzzleIndex = 0;
        timeRemaining = timeLimitSeconds;
        isGameActive = true;

        LoadNextPuzzle();
    }

    void Update()
    {
        if (!isGameActive) return;

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
            ContinueTrace();
            CheckProgress(GetPointerScreenPosition());
        }
        else if (pointer.press.wasReleasedThisFrame && isTracing)
        {
            Debug.Log("[TRACER LOG]: Mouse/Touch released mid-trace.");
            StopTrace();
        }
    }

    private void LoadNextPuzzle()
    {
        foreach (var p in puzzles)
        {
            if (p.puzzleFolder != null) p.puzzleFolder.SetActive(false);
        }

        if (currentPuzzleIndex < shuffledPuzzles.Count && completedCount < puzzlesToComplete)
        {
            PuzzleData currentPuzzle = shuffledPuzzles[currentPuzzleIndex];
            currentPuzzle.puzzleFolder.SetActive(true);

            if (instructionText != null)
                instructionText.text = currentPuzzle.instructionMessage;

            lineRenderer.positionCount = 0;
            drawnPoints.Clear();
            currentCheckpointIndex = 0;

            Debug.Log($"[TRACER LOG]: Loaded Puzzle '{currentPuzzle.puzzleFolder.name}'. Active Checkpoint count: {currentPuzzle.checkpoints.Count}");
        }
        else
        {
            OnAllPuzzlesCompleted();
        }
    }

    private void StartTrace()
    {
        PuzzleData activePuzzle = shuffledPuzzles[currentPuzzleIndex];
        Vector2 screenPointerPos = GetPointerScreenPosition();
        Vector3 worldPointerPos = GetPointerWorldPosition();

        if (activePuzzle.checkpoints.Count > 0)
        {
            Vector2 cpScreenPos = activePuzzle.checkpoints[0].position;
            float dist = Vector2.Distance(screenPointerPos, cpScreenPos);

            Debug.Log($"[TRACER LOG]: Start click detected at screen position {screenPointerPos}. CP_1 Screen Pos: {cpScreenPos} | Distance: {dist:F1}px (Max Allowed: {checkpointRadius}px)");

            if (dist <= checkpointRadius)
            {
                Debug.Log("[TRACER LOG]: CP_1 Hit! Tracing session started.");
                isTracing = true;
                drawnPoints.Clear();
                lineRenderer.positionCount = 0;
                currentCheckpointIndex = 0;

                AddPoint(worldPointerPos);
                CheckProgress(screenPointerPos);
            }
            else
            {
                Debug.LogWarning($"[TRACER LOG]: Click was too far from CP_1 ({dist:F1}px > {checkpointRadius}px radius).");
            }
        }
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

        if (currentCheckpointIndex < activePuzzle.checkpoints.Count)
        {
            RegisterMistake($"Tracing stopped prematurely. Reached checkpoint {currentCheckpointIndex}/{activePuzzle.checkpoints.Count}");
        }
    }

    private void AddPoint(Vector3 worldPosition)
    {
        drawnPoints.Add(worldPosition);
        lineRenderer.positionCount = drawnPoints.Count;
        lineRenderer.SetPosition(drawnPoints.Count - 1, worldPosition);
    }

    private void CheckProgress(Vector2 currentScreenPoint)
    {
        PuzzleData activePuzzle = shuffledPuzzles[currentPuzzleIndex];
        if (currentCheckpointIndex >= activePuzzle.checkpoints.Count) return;

        Transform currentCP = activePuzzle.checkpoints[currentCheckpointIndex];
        Vector2 cpScreenPos = currentCP.position;
        float distance = Vector2.Distance(currentScreenPoint, cpScreenPos);

        if (distance <= checkpointRadius)
        {
            Debug.Log($"[TRACER LOG]: Checkpoint #{currentCheckpointIndex + 1} ('{currentCP.name}') HIT! Distance: {distance:F1}px");

            PlayCheckpointSFX(currentCheckpointIndex == 0);

            currentCheckpointIndex++;

            if (currentCheckpointIndex >= activePuzzle.checkpoints.Count)
            {
                OnPuzzleSuccess();
            }
        }
    }

    private void PlayCheckpointSFX(bool isFirst)
    {
        if (sfxAudioSource == null)
        {
            Debug.LogWarning("[TRACER LOG]: sfxAudioSource is missing on the script component!");
            return;
        }

        AudioClip clipToPlay = isFirst ? firstCheckpointSound : subsequentCheckpointSound;

        if (clipToPlay != null)
        {
            sfxAudioSource.PlayOneShot(clipToPlay);
        }
        else
        {
            Debug.LogWarning($"[TRACER LOG]: Missing audio clip for {(isFirst ? "First" : "Subsequent")} Checkpoint sound!");
        }
    }

    private void RegisterMistake(string reason)
    {
        currentStrikes++;
        Debug.LogWarning($"[TRACER LOG]: Mistake registered! Reason: {reason}. Current Strikes: {currentStrikes}/{heartImages.Length}");

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

        lineRenderer.positionCount = 0;
        drawnPoints.Clear();
        currentCheckpointIndex = 0;

        if (currentStrikes >= heartImages.Length)
        {
            OnGameFailed("Too Many Mistakes!");
        }
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

    private void OnPuzzleSuccess()
    {
        isTracing = false;
        Debug.Log($"[TRACER LOG]: Puzzle Solved Successfully! ({completedCount + 1}/{puzzlesToComplete})");

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
        Debug.Log($"[TRACER LOG]: All puzzles complete! Loading return scene '{returnSceneName}'");
        SceneManager.LoadScene(returnSceneName);
    }

    private void OnGameFailed(string reason)
    {
        isGameActive = false;
        Debug.LogError($"[TRACER LOG]: Game Session Failed ({reason}). Restarting scene...");
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