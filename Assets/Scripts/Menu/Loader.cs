using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Used to preload everything, typically prefabs
/// </summary>
public class Loader : MonoBehaviour
{
    static bool alreadyLoaded = false;
    public GameObject popUpText;
    public ObjectPoolSetUp BulletLib;
    private void Awake()
    {
        if (alreadyLoaded)
            return;
        PopUp_Text.Self = popUpText;
        BulletLib.Load();
        alreadyLoaded = true;
        SceneManager.sceneLoaded += ClearPools;
    }
    private static void ClearPools(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Play")
        {
            Projectiles.Clear();
            PopUp_Text.ResetPool();
        }
    }
}
