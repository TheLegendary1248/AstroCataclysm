using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Preferences : MonoBehaviour//Remove later
{
    /*
    public bool SmoothMove;
    public bool isPerspective;
    public int currentLvl;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnLoad;
    }
    void OnLoad(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "Play")
        {
            if(isPerspective)
            {
                GameObject.FindWithTag("MainCamera").GetComponent<Camera>().orthographic = false;
            }
        }
        else 
        { Debug.Log("Returned to Main"); }

    }
    public void Revive()
    {
        SceneManager.LoadScene("Play");
        Spawner spawner = GameObject.Find("Spawner").GetComponent<Spawner>();
    }
    void LoadPrefs()
    {
        PlayerPrefs.GetFloat("MoveS", 100);
        PlayerPrefs.GetFloat("RotationS", 100);
    }
    */
    public void mode(int diff)
    {
        SettingControler.SetDifficulty(diff);
    }
}
