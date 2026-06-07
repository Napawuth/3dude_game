using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject victoryScreen;

    [SerializeField] private BossController bossController;

    public void ShowGameOver()
    {
        bossController.StopAllCoroutines();
        gameOverScreen.SetActive(true);
    }

    public void ShowVictory()
    {
        victoryScreen.SetActive(true);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}