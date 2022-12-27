using System.Collections;
using UnityEngine;

public class Swarming_AI : Health, IEnemy
{
    public int EntityValue => _entityValue; public int _entityValue;
    public int Score => _score; 
    public static Transform target;
    public Rigidbody2D RBody;
    public Vector2 force;
    public float startingForceMulti;
    public string bullet;
    public GameObject explosion;
    public float spread;
    public float rate;
    public GameObject[] drops = new GameObject[1];
    public bool combatTagged = false;
    public int _score;
    //Script used for light ships.

    void Start()
    {
        hitPoints = (int)(healthMax * SettingControler.enemyHealthMultiplier);
        RBody.AddRelativeForce(force * startingForceMulti, ForceMode2D.Impulse);
        StartCoroutine(Fire());
        StartCoroutine(ProxCheck());
        NPCHandler.Population += EntityValue;
    }

    void FixedUpdate()//Same code that applies force to missiles. Basically they share the same method of movement
    {
        if (!combatTagged)
        {
            Vector2 dist = (Vector2)target.position - RBody.position;
            float seek = Mathf.Atan2(dist.y, dist.x) * Mathf.Rad2Deg - 90f;
            RBody.rotation = seek;
            RBody.AddRelativeForce(force, ForceMode2D.Impulse);
        }
    }
    public override void Kill()//Death of enemy
    {
        GameObject explosionScaler = Instantiate(explosion, transform.position, transform.rotation);
        explosionScaler.transform.localScale = transform.localScale;
        if (Random.Range(0, 3) == 0)
        {
            GameObject drop = Instantiate(drops[Random.Range(0, drops.Length)], transform.position, transform.rotation);
            if (drop.CompareTag("Pickup"))
            {
                drop.GetComponent<Pickup>().restore = 20;
            }
        }
        if (combatTagged)
        { PopUp_Text.Set(transform.position, Color.clear, PopUp_Text.TypeOfValue.Kill, Score * 2.5f, 3f); GameStatistics.Score = (int)(Score * 2.5f); }
        else
        { PopUp_Text.Set(transform.position, Color.red, PopUp_Text.TypeOfValue.Kill, Score, 2f); GameStatistics.Score = Score; }
        if (SettingControler.exMode)
        {
            int i = -3;
            while(i < 3)
            {
                Projectiles.GetAndSetProjectile(bullet, transform.position, transform.eulerAngles.z + (i * 15));
                i++;
            }
        }
        NPCHandler.Population -= EntityValue;
        Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision) // Should the enemy be combat tagged, take damage depending on the force of the collision
    {
        if (combatTagged & collision.collider.CompareTag("Obstacle"))
        {
            Vector2 vel = collision.relativeVelocity;
            Dmg(Mathf.RoundToInt(vel.magnitude / 10));
        }
    }
    IEnumerator Fire() //Delay between shots
    {
        yield return new WaitForSeconds(rate);
        Projectiles.GetAndSetProjectile(bullet, transform.position, transform.eulerAngles.z + Random.Range(-spread, spread));
        StartCoroutine(Fire());
    }
    IEnumerator ProxCheck()//Proximity check if any objects on the same layer exist in a radius. If true, add horizontal relative velocity
    {
        yield return new WaitForSeconds(2f);
        if (!combatTagged)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);
            if (Vector2.Distance(Player.instTransform.position, transform.position) < 30)
            {
                SocialDistancing(true);
                yield return new WaitForSeconds(2f);
                SocialDistancing(false);
            }
            else
            {
                foreach (Collider2D x in colliders)
                {
                    if (x.gameObject.layer == gameObject.layer)
                    {
                        SocialDistancing(true);
                        yield return new WaitForSeconds(2f);
                        SocialDistancing(false);
                        break;
                    }
                }
            }
            if ((target.position - transform.position).sqrMagnitude > 50*50)
            {
                RBody.AddRelativeForce(force * 50, ForceMode2D.Impulse);
            }
        }
        StartCoroutine(ProxCheck());
    }

    public IEnumerator CombatTag()//Used to allow temporary damage by gameobjects on the "obstacle" layer when launched by a shockwave.
    {
        combatTagged = true;
        RBody.freezeRotation = false;
        yield return new WaitForSeconds(2f);
        combatTagged = false;
        RBody.freezeRotation = true;
    }
    void SocialDistancing(bool i) //Adds horizontal velocity
    {
        force.x = i ? Random.Range(-3, 3) * force.y : 0;
    }
    
}
