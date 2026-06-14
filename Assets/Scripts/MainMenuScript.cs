using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(""); //dodati ime scene
    }

    public void OpenSettings()
    {
        // settings - ne radi
        Debug.Log("Settings pressed");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit pressed");
    }
}
