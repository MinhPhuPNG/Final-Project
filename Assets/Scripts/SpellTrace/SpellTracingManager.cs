using UnityEngine;
using UnityEngine.SceneManagement;

public class SpellTracingManager : MonoBehaviour
{
    public void OnTracingComplete()
    {
        SceneManager.LoadScene("GameComplete");
    }
}