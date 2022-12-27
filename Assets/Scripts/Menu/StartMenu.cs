using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;
/// <summary>
/// Controls the start menu?
/// </summary>
public class StartMenu : MonoBehaviour //¯\_(ツ)_/¯
{
    public Transform cam; Vector3 defaultPos;
    public TextAsset tips;
    bool focus = false; public bool Focus { get { return focus; } set { focus = value; } } 
    public GameObject tipUI;
    public Text tipText;
    public bool atMenu;
    AsyncOperation aop;
    public void StartGame()
    {
        if (aop == null)
            Debug.LogWarning("Tried to start game without an operation loaded");
        aop.allowSceneActivation = true;
    }
    public void LoadGame()
    {
        Settings.SavePrefs();
        aop = SceneManager.LoadSceneAsync("Play");
        aop.allowSceneActivation = false;
    }
    public void ViewCredits()
    {
        Application.OpenURL("https://docs.google.com/document/d/e/2PACX-1vQnEDUma47uOpAvYvhTld4z8gKsjmUhi-e_Q3JKyqj51FKBIWwOH5zI6Fwk8VKg6kyIXiLJX46956tp/pub");
    }
    private void Start() 
    {
        defaultPos = cam.position;  
    }
    private void FixedUpdate()
    {
        cam.position = focus ? Vector3.Lerp(cam.position, defaultPos, 0.05f)  : Vector3.Lerp(defaultPos+((Input.mousePosition-(Vector3)Camera.main.pixelRect.size/2)/6),cam.position,0.9f);
    }
    public void ExitGame() { Application.Quit(); } //Simple exit
}
