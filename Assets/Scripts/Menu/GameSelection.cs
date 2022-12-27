using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Handles the complex game selection stuff
/// </summary>
public class GameSelection : MonoBehaviour //Make this handle the game selections section
{
    public Text title;
    Text[] texts;
    Image[] images;

    void Start()
    {
        texts = GetComponentsInChildren<Text>();
        images = GetComponentsInChildren<Image>();
    }
    private void OnEnable()
    {
        string[] choice = { "Pick your poison","Decide your fate","Select your problems","Choose your demise","Elect your ending"};
        title.text = choice[Random.Range(0,choice.Length)];
    }
    void Update()
    {
        Color c = Color.Lerp(Color.red, new Color(0, 0.53f, 1), (((Input.mousePosition.y/Screen.height)-0.5f)*1.5f)+0.5f);
        foreach (Text t in texts)
        {
            t.color = c;
        }
        foreach (Image t in images)
        {
            t.color = c;
        }
    }
}
