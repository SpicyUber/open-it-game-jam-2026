using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreenEffect : MonoBehaviour
{
    public void OnScreenClick()
    {
        SceneManager.LoadScene("MainScene");
    }
}
