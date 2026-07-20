using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class ResourceCounter : MonoBehaviour
{
    public static ResourceCounter Instance { get; private set; }

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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        RebindUI();
        UpdateUI();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetResources();
        RebindUI();
        UpdateUI();
    }

    public int GetMushroomCount() => mushrooms;
    public int GetPurpleFlowerCount() => purpleFlowers;
    public int GetTreeShroomCount() => treeShrooms;
    public bool HasTutorialPotion() => hasTutorialPotion;
    public bool HasSpellBook() => hasSpellBook;

    private void ResetResources()
    {
        mushrooms = 0;
        purpleFlowers = 0;
        treeShrooms = 0;
        hasTutorialPotion = false;
        hasSpellBook = false;
    }

    private void RebindUI()
    {
        if (mushroomText != null && purpleFlowerText != null && treeShroomText != null)
        {
            return;
        }

        TextMeshProUGUI[] textFields = FindObjectsOfType<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI textField in textFields)
        {
            if (textField == null)
            {
                continue;
            }

            string lowerName = textField.gameObject.name.ToLowerInvariant();
            string lowerText = textField.text.ToLowerInvariant();

            if (mushroomText == null && (lowerName.Contains("mushroom") || lowerText.Contains("mushrooms")))
            {
                mushroomText = textField;
            }
            else if (purpleFlowerText == null && (lowerName.Contains("flower") || lowerText.Contains("flowers")))
            {
                purpleFlowerText = textField;
            }
            else if (treeShroomText == null && (lowerName.Contains("shroom") || lowerText.Contains("shrooms")))
            {
                treeShroomText = textField;
            }
        }
    }

    public void ConsumeResources(int mushroomAmt, int flowerAmt, int treeShroomAmt)
    {
        mushrooms -= mushroomAmt;
        purpleFlowers -= flowerAmt;
        treeShrooms -= treeShroomAmt;
        
        UpdateUI();
    }
    private void UpdateUI()
    {
        mushroomText.text = "Mushrooms: " + mushrooms;
        purpleFlowerText.text = "Flowers: " + purpleFlowers;
        treeShroomText.text = "Shrooms: " + treeShrooms;
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
