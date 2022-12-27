using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUp_Text : MonoBehaviour
{
    public static GameObject Self;
    public TextMesh textMesh;
    bool Rainbow = false;
    int setSize;
    readonly static Queue<GameObject> Pool = new Queue<GameObject>();
    public enum TypeOfValue
    {
        Damage, Restore, Points, Kill
    }
    /// <summary>
    /// Set the TextMesh Object
    /// </summary>
    /// <param name="color">Color of the text. Color.clear enables rainbow</param>
    /// <param name="type">Type of value to be represented, since it differs</param>
    /// <param name="value">Value to be changed to text(string)</param>
    public static void Set(Vector3 pos, Color color, TypeOfValue type, float value, float sizeMultiplier)
    {
        GameObject obj;
        bool wasQueued;
        if (Pool.Count > 0)
        {
            obj = Pool.Dequeue();
            obj.transform.SetPositionAndRotation(pos, Quaternion.identity);
            obj.SetActive(true);
            wasQueued = true;
        }
        else
        {
            obj = Instantiate(Self, pos, Quaternion.identity);
            wasQueued = false;
        }
        PopUp_Text popUp = obj.GetComponent<PopUp_Text>();
        if (color != Color.clear)
        {
            popUp.Rainbow = false;
            popUp.textMesh.color = color;
        }
        else
            popUp.Rainbow = true;
        switch (type)
        {
            case TypeOfValue.Damage:
                popUp.textMesh.text = value.ToString();
                break;
            case TypeOfValue.Restore:
                popUp.textMesh.text = value.ToString() + "+";
                break;
            case TypeOfValue.Points:
                break;
            case TypeOfValue.Kill:
                popUp.textMesh.text = "+" + value;
                popUp.textMesh.fontStyle = FontStyle.Bold;
                break;
            default:
                Debug.Log("u didnt specify type dumbass, somehow");
                break;
        }
        popUp.transform.localScale = Vector3.zero;
        if (wasQueued)
        {
            popUp.textMesh.fontSize = popUp.setSize;
        }
        else
        {
            popUp.setSize = popUp.textMesh.fontSize;
        }
        popUp.textMesh.fontSize = Mathf.RoundToInt(popUp.textMesh.fontSize*sizeMultiplier);
    }
    private void FixedUpdate()
    {
        if (Rainbow)
        {
            textMesh.color = Color.HSVToRGB(Mathf.Repeat(Time.time / 2f, 1f), 1f, 1f);
        }
        Vector3 rot = transform.eulerAngles;
        rot.z = CamFollow.euclidRot;
    }
    private void OnEnable()
    {
        transform.Translate(Random.Range(-4f, 4f), Random.Range(-4f, 4f), 0);
        transform.Rotate(Vector3.forward * Random.Range(-20f, 20f));
        StartCoroutine(Life());
    }

    IEnumerator Life()
    {
        while (transform.localScale.z < 1f)
        {
            transform.localScale += Vector3.one * Time.fixedDeltaTime * 3f;
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(1f);
        while (transform.localScale.z > 0f)
        {
            transform.localScale -= Vector3.one * Time.fixedDeltaTime * 3f;
            yield return new WaitForFixedUpdate();
        }
        Pool.Enqueue(gameObject);
        gameObject.SetActive(false);
    }
    public static void ResetPool() => Pool.Clear();
}
