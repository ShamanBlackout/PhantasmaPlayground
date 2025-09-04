using Unity.VisualScripting;
using UnityEngine;

public class Pause : MonoBehaviour
{

    public GameObject pauseMenuUI;
    public static bool GameIsPaused = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //pauseMenuUI.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        //Time.timeScale = 0f; // Pause the game by setting time scale to 0
        GameIsPaused = true;
    }
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        //interrupts animations and Coroutines needed for wallet login and pinging
        //Time.timeScale = 1f; // Resume the game by setting time scale back to 1
        GameIsPaused = false;
    }
}
