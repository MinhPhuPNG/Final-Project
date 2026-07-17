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
    void Start()
    {
        UpdateUI();
    }

    public int GetMushroomCount() => mushrooms;
    public int GetPurpleFlowerCount() => purpleFlowers;
    public int GetTreeShroomCount() => treeShrooms;
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

        UpdateUI();
    }
}
