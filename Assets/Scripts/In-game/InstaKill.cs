using System.Collections;
using UnityEngine;
using UnityEngine.UI;
//Simple code used by the playing area for insta-killing the player if they leave the safe area.
public class InstaKill : MonoBehaviour
{
    public GameObject LaserPrefab;
    public GameObject WarningObj;
    Text Warning;
    Vector2 last;
    Vector2 next;
    float timestamp;
    IEnumerator InstKill()
    {
        yield return new WaitForSeconds(3f);
        GameObject instance = Instantiate(LaserPrefab);
        Vector2 target = Player.instTransform.position;
        instance.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg);
        instance.transform.position = target;
        WarningObj.SetActive(false);
        Player.controlInst.Kill();
    }
    private void Start()
    {
        Warning = WarningObj.GetComponent<Text>();
        if(SettingControler.exMode)
        {
            transform.localScale = Vector2.one * 200;
            transform.position = new Vector2(Mathf.PerlinNoise(Time.time / 30f, 0) - 0.5f, Mathf.PerlinNoise(0, Time.time / 30f) - 0.5f) * 600f;
        }
        Player.controlInst.transform.position = transform.position;
    }
    private void FixedUpdate()
    {
        Warning.color = Color.Lerp(Color.white, Color.red, (Mathf.Sin(Time.time * 25f) + 1f ) / 2f);
        if (SettingControler.exMode)
        {
            transform.position = new Vector2(Mathf.PerlinNoise(Time.time / 30f, 0) - 0.5f, Mathf.PerlinNoise(0, Time.time / 30f) - 0.5f) * 600f;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            WarningObj.SetActive(true);
            StartCoroutine("InstKill");
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            WarningObj.SetActive(false);
            StopCoroutine("InstKill");
        }
    }
}
