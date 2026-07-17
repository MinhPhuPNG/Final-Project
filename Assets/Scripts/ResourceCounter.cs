using UnityEngine;
using TMPro;
public class ResourceCounter : MonoBehaviour
{
    public TextMeshProUGUI mushroomText;
    public TextMeshProUGUI purpleFlowerText;
    public TextMeshProUGUI treeShroomText;
    private int mushrooms;
    private int purpleFlowers;
    private int treeShrooms;
    private bool hasTutorialPotion = false;
    void Start()
    {
        UpdateUI();
    }

    public int GetMushroomCount() => mushrooms;
    public int GetPurpleFlowerCount() => purpleFlowers;
    public int GetTreeShroomCount() => treeShrooms;
    public bool HasTutorialPotion() => hasTutorialPotion;
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

        UpdateUI();
    }
}
