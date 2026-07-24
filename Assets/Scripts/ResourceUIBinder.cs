using UnityEngine;
using TMPro;

public class ResourceUIBinder : MonoBehaviour
{
    [Header("Drag Scene Text Elements Here")]
    public TextMeshProUGUI mushroomText;
    public TextMeshProUGUI purpleFlowerText;
    public TextMeshProUGUI treeShroomText;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (ResourceCounter.Instance != null)
        {
            ResourceCounter.Instance.SetUIReferences(mushroomText, purpleFlowerText, treeShroomText);
        }
    }
}