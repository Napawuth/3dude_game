using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject helpScreen;

    public void StartGame()
    {
        SceneManager.LoadScene("LvlOneEnvironment");
    }

    public void QuitGame()
    {
        Debug.Log("Player Quit the Game!");
        Application.Quit();
    }

    public void ShowInstructions()
    {
        helpScreen.SetActive(true);
    }

    public void HideInstuctions()
    {
        helpScreen.SetActive(false);
    }
}