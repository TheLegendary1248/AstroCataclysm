using System.Collections;
using UnityEngine;

public class Missile_PhysicsAffected : Projectiles
{
    public static Transform player;
    public Transform target;
    public Rigidbody2D self_rb;
    public Vector2 force;
    public float startingForceMulti;
    public float detonate;
    public string targetTag = "Player";
    public Component[] MarkedForDestruction;
    public GameObject explosion;
    public float explosionSize = .2f;
    public int damage;
    public Color altColor;
    //It's called physics affected incase I decide to make another homing missile that doesn't use physics for rotation
    private void Awake()
    {
        damage = Mathf.RoundToInt(damage * SettingControler.globalDamageMultiplier);
    }
    public override void Convert(bool isShockwave)
    {
        targetTag = "Enemy";
        gameObject.layer = 8;
        gameObject.GetComponent<SpriteRenderer>().color = altColor;
        hasConverted = true;
        GameObject[] list = GameObject.FindGameObjectsWithTag(targetTag);
        if (list.Length != 0)
        {
            int closest = 0;
            float minDist = float.MaxValue;
            if (list.Length == 1)
                {
                    closest = 0;
                }
            else for (int i = 0; i < list.Length; i++)
                {
                    float dist = (transform.position - list[i].transform.position).sqrMagnitude;
                    if (dist < minDist)
                    {
                        closest = i;
                        minDist = dist;
                    }
                }
            target = list[closest].GetComponent<Transform>();
            self_rb.velocity = (target.position - transform.position).normalized * force.y * 20f;
        }
        else
        { force *= -1f; }
    }
    void Start()
    {
        StartCoroutine(Explode()); //Delay before blowing up
        self_rb.AddRelativeForce(Vector2.up * startingForceMulti, ForceMode2D.Impulse); //Launches the missile in the direction it was shot
        if (!hasConverted)
        {
            target = player;
        }
    }
    IEnumerator Explode()
    {
        yield return new WaitForSeconds(detonate);
        Kill();
    }

    void Kill() //Destroys every component in Game Object, except for trail renderer, which automatically destroys the Game Object after there isn't a trail present anymore.
    {
        GameObject ex = Instantiate(explosion, transform.position, transform.rotation);
        if(hasConverted)
            ex.GetComponent<Projectiles>().Convert(true);
        foreach (Component i in MarkedForDestruction)
        {
            Destroy(i);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision) //Explosion on impact
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            Kill();
        }
    }
    void FixedUpdate() //Rotation and force
    {
        if (target)
        {
            Vector2 dist = (Vector2)target.position - self_rb.position;
            float seek = Mathf.Atan2(dist.y, dist.x) * Mathf.Rad2Deg - 90f;
            self_rb.rotation = seek;
            self_rb.AddRelativeForce(force, ForceMode2D.Impulse);
        }
        else
        {
            Kill();
        }
    }
}
 