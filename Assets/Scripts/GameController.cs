using UnityEngine;
using UnityEngine.SceneManagement; // Include this to use SceneManager

public class GameController : MonoBehaviour
{
    public void RestartGame()
    {
        // Load the current scene again
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
    