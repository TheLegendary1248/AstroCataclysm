using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Markers, aka warnings, for delayed weapons
/// </summary>
public class Markers : MonoBehaviour
{
    public enum Marks
    {
        Strike,
        Laser
    }
    static Dictionary<Marks, GameObject> markObjects = new Dictionary<Marks, GameObject>();
    public float lifetime;
    public static void Set(Vector2 position, float rotation, Marks mark)
    {
        if (markObjects.ContainsKey(mark))
        {
            Instantiate(markObjects[mark], position, Quaternion.Euler(0, 0, rotation));
        }
        else
        {
            GameObject obj = Resources.Load<GameObject>(mark.ToString() + "Mark");
            markObjects.Add(mark, obj);
            Instantiate(obj, position, Quaternion.Euler(0, 0, rotation));
        }
    }
    void Awake()
    {
        StartCoroutine(Removal());
    }
    IEnumerator Removal()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
