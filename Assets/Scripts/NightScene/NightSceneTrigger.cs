using UnityEngine;
using UnityEngine.SceneManagement;

public class NightSceneTrigger : MonoBehaviour
{
    public void LoadNightScene()
    {
        SceneManager.LoadScene("NightScene"); 
    }
}