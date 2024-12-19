using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    public void LoadNewLevel(string level)
    {
        SceneManager.LoadScene(level);
    }
}