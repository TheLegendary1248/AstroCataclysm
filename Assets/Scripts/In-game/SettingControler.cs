using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Controls the setting each game, or enviroment
/// </summary>
public class SettingControler : MonoBehaviour //Controls the setting each game, environment and score
{
    public static bool exMode = false;
    public static bool SpecialCircumstances;
    public static float globalDamageMultiplier = 1f;
    public static float enemyHealthMultiplier = 1f;
    public static float enemyTurretFireRateMultiplier = 1f;
    public static float enemySpawnRateMultiplier = 1f;
    public static int playerHealth = 250;
    public static SettingControler self;
    public enum Difficulties { Easy, Normal, Hard, Impossible}
    public static Difficulties difficulty = Difficulties.Normal;
    public float testStartTimeScale = 1;
    public Animator textExa;
    private float combo = 1f; public float Combo => combo; 
    public int chain;
    public int lastCharge;
    public Player player;
    public Slider overcharge; public static bool updateOvercharge = true;
    public Text comboText;  public Slider comboBar; public GameObject comboParent;float comboTimestamp;bool comboActive = false;public float comboTime = 5f;
    public float totalScore;
    public GameObject PauseScreen;
    public GameObject EndScreen;


    //Mostly contains code for Combos. Might rename it.
    void Start()
    {
        self = this;
        Projectiles.data = this;
        Time.timeScale = testStartTimeScale;
        comboBar.maxValue = comboTime;
    }
    public void BuildCombo(bool isHit, bool addTospecial)//Add parameter soon
    {
        if(isHit)
        {
            comboParent.SetActive(true);
            comboBar.value = comboTime;
            comboTimestamp = Time.time + comboTime;
            if(addTospecial)
                chain++;
            combo += .1f;
            combo = Mathf.Clamp(combo, 1f, 2.5f);
            ChangeCombText();
            textExa.SetTrigger("Add");
            comboActive = true;
        }
        else
        {
            combo -= .02f;
            combo = Mathf.Clamp(combo, 1f, 2.5f);
            ChangeCombText();
        }
        if(chain >= lastCharge + 12)
        {
            overcharge.value = chain - lastCharge;
            player.overCharge[0] = true;//First phase of overcharging
            lastCharge = chain;
            updateOvercharge = false;
        }
        if (updateOvercharge)
        {
            overcharge.value = chain - lastCharge;
        }
        player.combo = combo;
    }
    void FixedUpdate()
    {
        if (comboActive)
        {
            comboBar.value =  comboTimestamp - Time.time;
        }
        if(comboBar.value == 0)
        {
            comboParent.SetActive(false);
            comboActive = false;
            combo = 1f;
            chain = 0;
            lastCharge = 0;
            overcharge.value = 0;
        }
    }
    public static void SetDifficulty(int diff)
    {
        if (diff > 3)
        {
            Debug.Log("Difficulty is out of range: Normal Selected");
            difficulty = Difficulties.Normal;
            globalDamageMultiplier = 1f;
            enemyHealthMultiplier = 1f;
            enemySpawnRateMultiplier = 1f;
            enemyTurretFireRateMultiplier = 1f;
            playerHealth = 250;
        }
        else
        {
            difficulty = (Difficulties)diff;
            switch (difficulty)
            {
                case Difficulties.Easy:
                    globalDamageMultiplier = 0.75f;
                    enemyHealthMultiplier = 0.75f;
                    enemySpawnRateMultiplier = 0.75f;
                    enemyTurretFireRateMultiplier = 0.75f;
                    playerHealth = 200;
                    break;
                case Difficulties.Normal:
                    globalDamageMultiplier = 1f;
                    enemyHealthMultiplier = 1f;
                    enemySpawnRateMultiplier = 1f;
                    enemyTurretFireRateMultiplier = 1f;
                    playerHealth = 250;
                    break;
                case Difficulties.Hard:
                    globalDamageMultiplier = 1.2f;
                    enemyHealthMultiplier = 1.5f;
                    enemySpawnRateMultiplier = 1.2f;
                    enemyTurretFireRateMultiplier = 1.2f;
                    playerHealth = 300;
                    break;
                case Difficulties.Impossible:
                    globalDamageMultiplier = 1.5f;
                    enemyHealthMultiplier = 2f;
                    enemySpawnRateMultiplier = 1.5f;
                    enemyTurretFireRateMultiplier = 1.5f;
                    playerHealth = 350;
                    break;
                default:
                    Debug.Log("Error selecting difficulty: Normal Selected");
                    goto case Difficulties.Normal;
            }
        }
    }
    void ChangeCombText()
    {
        comboText.text = chain.ToString() + " Chain / Combo   " + combo.ToString() + "x";
    }
    void SetStats()
    {

    }
}
interface ISpecial { }
