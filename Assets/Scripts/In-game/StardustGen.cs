using UnityEngine;
/// <summary>
/// In charge of creating the procedural dust overlay and background (and planets)
/// </summary>
public class StardustGen : MonoBehaviour
{
    public Renderer background;
    public bool isMain = false;

    public static bool UseDust = true;
    public static bool UseBackground = false;

    Renderer render;
    Texture2D txt;
    Vector3 euler;
    Vector2 redOff;
    Vector3 otherOff;
    private Color32[] pixels;

    int cnt = 0;
    private void Start()
    {
        //Dust
        euler = transform.eulerAngles;
        redOff = new Vector2(Random.Range(-200f, 200f), Random.Range(-200f, 200f));
        otherOff = new Vector2(Random.Range(-200f, 200f), Random.Range(-200f, 200f));
        txt = new Texture2D(50, 50);
        pixels = new Color32[txt.width * txt.height];
    }
    void Noise()
    {

        render = GetComponent<Renderer>();
        render.enabled = true;
        render.material.mainTexture = txt;

        float y = 0;
        float t = Time.time * 5f;
        Vector2 vec = -transform.position / 5;
        while (y < txt.width)
        {
            float x = 0;
            while (x < txt.height)
            {
                float xCenter = Mathf.Abs(txt.width / 2f - x);
                xCenter *= xCenter;
                float yCenter = Mathf.Abs(txt.height / 2f - y);
                yCenter *= yCenter;
                float alpha = Mathf.PerlinNoise((x + t + vec.x + otherOff.x) / txt.width * 3, (y + t + vec.y + otherOff.y) / txt.height * 3);
                float red = Mathf.PerlinNoise((x + t + vec.x + redOff.x) / txt.width * 3, (y + t + vec.y + redOff.y) / txt.height * 3);
                float blue = Mathf.PerlinNoise((x + t + vec.x + otherOff.x) / txt.width * 4, (y + t + vec.y + otherOff.y) / txt.height * 4);
                pixels[(int)y * txt.height + (int)x] =
                new Color32(
                    (byte)Mathf.Max(red * 255, 0),                                                                                    //Red
                    (byte)Mathf.Max((.75f - alpha) * 150, 0),                                                                      //Green
                    (byte)Mathf.Max(blue * 255, 0),                                                                                   //Blue
                    (byte)Mathf.Max(((alpha / 2.4f) - (100 > xCenter + yCenter ? (1f - (xCenter + yCenter) / 100f) / 5f : 0)) * 255, 0)    //Alpha
                );
                x++;
            }
            y++;
        }
        txt.SetPixels32(pixels);
        txt.Apply();

    }
    private void FixedUpdate()
    {
        if ((UseDust ? cnt : -1) > 0)
        {
            Noise();
            return;
        }
        cnt++;
    }
    private void Update()
    {
        transform.eulerAngles = new Vector3(270, 0, 0);
    }
}
