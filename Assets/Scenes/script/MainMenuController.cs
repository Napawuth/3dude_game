using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("LvlOneEnvironment"); 
    }

    public void QuitGame()
    {
        Debug.Log("Player Quit the Game!");
        Application.Quit();
    }
}