using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Controls the settings of the player and the settings panel. Not to be confused with SettingControler
/// </summary>
public class Settings : MonoBehaviour
{
    public static bool hasAlreadyLoaded { get; private set; } = false;
    public delegate void load(); public static load Load;

    public Slider maskSlider;
    public Slider musicSlider;
    public Slider SFXSlider;
    public Slider rotationSlider;
    public Slider movementSlider;

    public static float clarityMask = 15;
    public static float musicVolume = 1f;
    public static float SFXVolume = 1f;
    public static float rotationSensitivity = 1f;
    public static float movementSensitivity = 1f;


    public static float maskProp { get { return clarityMask; } set { clarityMask = value; } }
    public static float musicProp { get { return musicVolume; } set { musicVolume = value; } }
    public static float SFXProp { get { return SFXVolume; } set { SFXVolume = value; } }
    public static float rotProp { get { return rotationSensitivity; } set { rotationSensitivity = value; } }
    public static float moveProp { get { return movementSensitivity; } set { movementSensitivity = value; } }
    Settings()
    {
        Load = Awake;

    }
    public void Awake()
    {
        if (!hasAlreadyLoaded)
        {
            clarityMask = PlayerPrefs.GetFloat("Mask");
            musicVolume = PlayerPrefs.GetFloat("Music Volume");
            SFXVolume = PlayerPrefs.GetFloat("SFX Volume");
            rotationSensitivity = PlayerPrefs.GetFloat("Rotation");
            movementSensitivity = PlayerPrefs.GetFloat("Movement");
            
            hasAlreadyLoaded = true;
        }
        maskSlider.SetValueWithoutNotify(clarityMask);
        musicSlider.SetValueWithoutNotify(musicVolume);
        SFXSlider.SetValueWithoutNotify(SFXVolume);
        rotationSlider.SetValueWithoutNotify(rotationSensitivity);
        movementSlider.SetValueWithoutNotify(movementSensitivity);
    }

    static public void SavePrefs()
    {
        Debug.Log(musicVolume.ToString() + "/" + SFXVolume.ToString() + "/" + movementSensitivity.ToString() + "/" + rotationSensitivity.ToString() + "/" + clarityMask.ToString());
        PlayerPrefs.SetFloat("Mask", clarityMask);
        PlayerPrefs.SetFloat("Music Volume", musicVolume);
        PlayerPrefs.SetFloat("SFX Volume", SFXVolume);
        PlayerPrefs.SetFloat("Rotation", rotationSensitivity);
        PlayerPrefs.SetFloat("Movement", movementSensitivity);
    }
}
