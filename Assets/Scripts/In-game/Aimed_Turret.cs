using UnityEngine;

public class Aimed_Turret : Turret
{
    public float rot_speed = 20f;
    public Targeting type;
    public enum Targeting
    {
        Offset,
        Snap,
        Independent
    }
    void Start()
    {
        StartCoroutine("Fire");
    }
    private void FixedUpdate()
    {
        Vector2 dif = transform.position - parent.position + Player.instTransform.position - transform.position;
        transform.eulerAngles = new Vector3(0, 0, (Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg) - 90f);

        /*
        Vector2 dir =  (Vector2)Player.instTransform.position - (Vector2)transform.position;
        dir.Normalize();
        float rot = Vector3.Cross(dir, transform.up).z *rot_speed; //I have no idea. Kinda took this from Brackey's "How to make a homing missile"
        rot *= 1.5f;
        transform.Rotate(Vector3.forward ,-rot);*/
    }
}
