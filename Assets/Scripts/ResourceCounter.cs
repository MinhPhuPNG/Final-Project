using UnityEngine;
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

    // Called by each scene's ResourceUIBinder when the scene loads
    public void SetUIReferences(TextMeshProUGUI mText, TextMeshProUGUI pText, TextMeshProUGUI tText)
    {
        mushroomText = mText;
        purpleFlowerText = pText;
        treeShroomText = tText;

        UpdateUI();
    }

    public int GetMushroomCount() => mushrooms;
    public int GetPurpleFlowerCount() => purpleFlowers;
    public int GetTreeShroomCount() => treeShrooms;
    public bool HasTutorialPotion() => hasTutorialPotion;
    public bool HasSpellBook() => hasSpellBook;

    public void ConsumeResources(int mushroomAmt, int flowerAmt, int treeShroomAmt)
    {
        mushrooms = Mathf.Max(0, mushrooms - mushroomAmt);
        purpleFlowers = Mathf.Max(0, purpleFlowers - flowerAmt);
        treeShrooms = Mathf.Max(0, treeShrooms - treeShroomAmt);

        UpdateUI();
    }

    public void AddResource(string resourceName, int amount)
    {
        if (string.IsNullOrWhiteSpace(resourceName))
        {
            return;
        }

        string normalizedName = resourceName.Trim().ToLowerInvariant();

        switch (normalizedName)
        {
            case "mushroom":
                mushrooms += amount;
                break;
            case "purpleflower":
                purpleFlowers += amount;
                break;
            case "treeshroom":
                treeShrooms += amount;
                break;
            case "tutorialpotion":
                hasTutorialPotion = true;
                break;
            case "spellbook":
                hasSpellBook = true;
                break;
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (mushroomText != null)
        {
            mushroomText.text = "Mushrooms: " + mushrooms;
        }

        if (purpleFlowerText != null)
        {
            purpleFlowerText.text = "Flowers: " + purpleFlowers;
        }

        if (treeShroomText != null)
        {
            treeShroomText.text = "Shrooms: " + treeShrooms;
        }
    }
}