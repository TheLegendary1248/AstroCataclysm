using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class NPCHandler : MonoBehaviour//Make static somehow
{
    public int WaveNumber { get => wavenum; }
    public float StartTime { get => startTime;  }

    public static int Population = 0; //Scoring of all 
    public GameObject[] HeavyShips = new GameObject[10]; //Use resources
    public GameObject[] LightShips = new GameObject[3];
    public GameObject[] Obstacles = new GameObject[1];
    public GameObject SafeArea;
    public Animator animator;
    public Slider wavebar;
    public Text wavenum_text; 
    static int wavenum;
    public float wavetime = 50f;
    public float spawnRange;
    public float timestamp;
    public int CoffeeBreakLimit; public int CoffeeBreakRate; bool OnBreak = false;
    int LightEnemyCount;
    int HeavyEnemyCount;
    int subWaveCount;
    int subWaveCurrent;
    public GameObject celebration;
    float startTime; //Used to figure how long you lasted
    void Start()
    {
        startTime = Time.time;
        wavenum = 0;
        StartWave();
    }
    private void FixedUpdate()//The animation of the purple bar basically
    {
        if(!OnBreak)wavebar.value = Time.time - timestamp;
    }
    void StartWave()//New Wave
    {
        //Initialize all variables for wave
        subWaveCurrent = 0;
        wavetime = Mathf.Min(8 + ++wavenum, 40);//Sets wavetime
        timestamp = Time.time;
        wavebar.maxValue = wavetime;
        wavenum_text.text = wavenum.ToString();
        LightEnemyCount = Mathf.Abs((int)(SettingControler.enemySpawnRateMultiplier * ((Mathf.Cos(wavenum * 2f)) + (Mathf.Cos(wavenum / 4f * 2f) * 2f) + (wavenum / 2f) + 1)));
        HeavyEnemyCount = Mathf.Abs((int)(SettingControler.enemySpawnRateMultiplier * (Mathf.Sin(wavenum) + (wavenum / 5))));
        subWaveCount = (int)(wavetime * .75f / 5f);
        StartCoroutine(DuringWave());
    }
    IEnumerator DuringWave()
    {
        //Debug.Log(LightEnemyCount + "  :  " + HeavyEnemyCount);
        for (int i = 0; i < subWaveCount; i++)
        {
            Spawn();
            yield return new WaitForSeconds(5f);
            if(Population > CoffeeBreakLimit)
            {
                OnBreak = true;
                int t = Population / 4; float stamp = Time.time;
                animator.Play("Base Layer.CoffeeBreak", -1, 0);
                wavenum_text.text += "\nBREAK"; wavenum_text.fontSize /= 2;
                yield return new WaitUntil(() => Population < t);
                wavenum_text.text = wavenum.ToString(); wavenum_text.fontSize *= 2;
                timestamp += Time.time - stamp; OnBreak = false;
                CoffeeBreakLimit += CoffeeBreakRate;
            }
        }
        yield return new WaitForSeconds(wavetime - (5*subWaveCount));
        StartWave();
    }
    void Spawn()
    {
        Transform trans = SafeArea.transform;
        subWaveCurrent++;
        int LInterval = LightEnemyCount / subWaveCount;
        int HInterval = HeavyEnemyCount / subWaveCount;
        _ = (LightEnemyCount % subWaveCount) > (subWaveCount - subWaveCurrent) ? LInterval++ : 0;
        _ = (HeavyEnemyCount % subWaveCount) > (subWaveCount - subWaveCurrent) ? HInterval++ : 0;
        //Debug.Log("L  " + LInterval + "   H  " + HInterval);
        for (int i = 0; i < LInterval; i++)
        {
            Instantiate(LightShips[Random.Range(0, LightShips.Length)], (Random.insideUnitCircle.normalized * (trans.localScale.x + 100)) + (Vector2)trans.position, Quaternion.identity);
        }
        for (int i = 0; i < HInterval & wavenum > 5; i++)
        {
            Instantiate(HeavyShips[Random.Range(0, HeavyShips.Length)], (Random.insideUnitCircle.normalized * (trans.localScale.x + 100)) + (Vector2)trans.position, Quaternion.identity);
        }
        for (int i = 0; i < 3; i++)
        {
            Instantiate(Obstacles[Random.Range(0, Obstacles.Length)], (Random.insideUnitCircle.normalized * (trans.localScale.x + 100)) + (Vector2)trans.position, Quaternion.identity);
        }
    }
    void CoffeeBreak()
    {

    }
}
