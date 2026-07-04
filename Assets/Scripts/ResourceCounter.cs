using UnityEngine;
using TMPro;
public class ResourceCounter : MonoBehaviour
{
    public TextMeshProUGUI mushroomText;
    public TextMeshProUGUI purpleFlowerText;
    private int mushrooms;
    private int purpleFlowers;
    void Start()
    {
        UpdateUI();
    }

    // Update is called once per frame
    private void UpdateUI()
    {
        mushroomText.text = "Mushrooms: " + mushrooms;
        purpleFlowerText.text = "Purple Flowers: " + purpleFlowers;
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

        UpdateUI();
    }
}
