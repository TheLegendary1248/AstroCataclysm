using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// The Beholder of the project
/// </summary>
public class Master : MonoBehaviour //Make this class control global aspects
{
    static GameObject pause, end;
    public GameObject PauseScreen, EndScreen;
    static float usedTimeScale = 1f;
    private void Awake()
    {
        pause = PauseScreen;
        end = EndScreen;
    }
    /// <summary>
    /// Use to pause the game
    /// </summary>
    /// <param name="isPaused">Will pause the game if true</param>
    public static void Pause(bool isPaused)
    {
        if (isPaused)
        {
            usedTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            pause.SetActive(true);
        }
        else
        {
            Time.timeScale = usedTimeScale;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            pause.SetActive(false);
        }
    }
    public static void EndGame()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        end.SetActive(true);
    }
    public void LoadMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Start Menu");
    }
    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Play");
    }
    
    public void Quit()
    {
        Settings.SavePrefs();
        Application.Quit();
    }
}
