using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ResourceCounter : MonoBehaviour
{
    public static ResourceCounter Instance { get; private set; }

    [Header("UI Text References")]
    public TextMeshProUGUI mushroomText;
    public TextMeshProUGUI purpleFlowerText;
    public TextMeshProUGUI treeShroomText;

    private int mushrooms;
    private int purpleFlowers;
    private int treeShrooms;
    private bool hasTutorialPotion = false;
    private bool hasSpellBook = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        RebindUI();
        UpdateUI();
    }

    private void RebindUI()
    {
        GameObject mObj = GameObject.Find("MushroomText");
        GameObject pObj = GameObject.Find("PurpleFlowerText");
        GameObject tObj = GameObject.Find("TreeShroomText");

        if (mObj != null) mushroomText = mObj.GetComponent<TextMeshProUGUI>();
        if (pObj != null) purpleFlowerText = pObj.GetComponent<TextMeshProUGUI>();
        if (tObj != null) treeShroomText = tObj.GetComponent<TextMeshProUGUI>();
    }

    public int GetMushroomCount() => mushrooms;
    public int GetPurpleFlowerCount() => purpleFlowers;
    public int GetTreeShroomCount() => treeShrooms;
    public bool HasTutorialPotion() => hasTutorialPotion;
    public bool HasSpellBook() => hasSpellBook;

    public void ConsumeResources(int mushroomAmt, int flowerAmt, int treeShroomAmt)
    {
        mushrooms -= mushroomAmt;
        purpleFlowers -= flowerAmt;
        treeShrooms -= treeShroomAmt;
        
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (mushroomText != null) mushroomText.text = "Mushrooms: " + mushrooms;
        if (purpleFlowerText != null) purpleFlowerText.text = "Flowers: " + purpleFlowers;
        if (treeShroomText != null) treeShroomText.text = "Shrooms: " + treeShrooms;
    }

    public void AddResource(string resourceName, int amount)
    {
        if (resourceName == "Mushroom")
        {
            mushrooms += amount;
        }
        else if (resourceName == "PurpleFlower")
        {
            purpleFlowers += amount;
        }
        else if (resourceName == "TreeShroom")
        {
            treeShrooms += amount;
        }
        else if (resourceName == "TutorialPotion")
        {
            hasTutorialPotion = true;
        }
        else if (resourceName == "SpellBook")
        {
            hasSpellBook = true;
        }

        UpdateUI();
    }
}