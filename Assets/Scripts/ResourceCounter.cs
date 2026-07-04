using UnityEngine;
using TMPro;
public class ResourceCounter : MonoBehaviour
{
    public TextMeshProUGUI mushroomText;
    private int mushrooms;
    void Start()
    {
        UpdateUI();
    }

    // Update is called once per frame
    private void UpdateUI()
    {
        mushroomText.text = "Mushrooms: " + mushrooms;
    }

    public void AddResource(string resourceName, int amount)
    {
        if (resourceName == "Mushroom")
        {
            mushrooms += amount;
        }
        
        UpdateUI();
    }
}
