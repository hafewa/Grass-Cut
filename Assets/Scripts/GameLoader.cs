using UnityEngine.SceneManagement;
using UnityEngine;

public class GameLoader : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.LoadScene(PlayerPrefs.GetString("LastLevel", "1"));
    }
}
