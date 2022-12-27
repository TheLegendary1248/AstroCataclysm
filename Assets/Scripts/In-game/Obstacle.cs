using System.Collections;
using UnityEngine;
public class Obstacle : Health //Make it 'asteriod', interface 'name', ...
{
    public GameObject fx;
    public CircleCollider2D circleCollider;
    public SpriteRenderer sprite;
    public Color color;
    public Vector2 force;
    public Rigidbody2D self_rb;
    public float sizeFactor;
    public float reentryTime;
    private void Start() //Upon creation, it the obstacle gets launched toward the center.
    {
        sizeFactor = Random.Range(2f, 0.5f);
        hitPoints = (int)healthMax;
        hitPoints = Mathf.RoundToInt(sizeFactor * hitPoints);
        transform.localScale *= sizeFactor;
        GetComponent<TrailRenderer>().widthMultiplier *= sizeFactor;
        color = sprite.color;
        Vector2 dist = Vector2.zero - self_rb.position;
        float seek = Mathf.Atan2(dist.y, dist.x) * Mathf.Rad2Deg - 90f + Random.Range(-25f, 25f);
        self_rb.rotation = seek;
        self_rb.AddRelativeForce(force * sizeFactor, ForceMode2D.Impulse);
        StartCoroutine("AutoReenter"); 
    }
    public override void Kill()//Death of obstacle
    {
        GameObject effect = Instantiate(fx, transform.position, transform.rotation);
        effect.transform.localScale = transform.localScale / 50;
        Destroy(gameObject);
    }
    /*
    private void OnCollisionEnter2D(Collision2D collision)//If it's the player, it will become transparent and a Trigger(Which doesn't affect objects)
    {
        if (collision.collider.CompareTag("Player"))
        {
            circleCollider.isTrigger = true;
            color.a = 0.5f;
            sprite.color = color;
            StartCoroutine("Check");
        }

    }
    private void OnTriggerExit2D(Collider2D collision)//If it's the player, it will become opaque and a Collider(Which does affect objects)
    {
        if (collision.CompareTag("Player"))
        {
            circleCollider.isTrigger = false;
            color.a = 1f;
            sprite.color = color;
            StopCoroutine("Check");
        }
    }*/
    IEnumerator Check()
    {
        yield return new WaitForSeconds(0.5f);
        ContactFilter2D filter = new ContactFilter2D();
        filter.NoFilter();
        Collider2D[] colliders = new Collider2D[10];
        circleCollider.OverlapCollider(filter, colliders);
        bool isPlayer = false;
        foreach (Collider2D i in colliders)
        {
            if (i)
            {
                if (i.CompareTag("Player"))
                    isPlayer = true;
            }
        }
        if (isPlayer)
            StartCoroutine("Check");
        else
        {
            circleCollider.isTrigger = false;
            color.a = 1f;
            sprite.color = color;
        }
    }

    IEnumerator AutoReenter() //Adds a spontaneous force after specified time that brings it back to the playing area. Usually unnoticable
    {
        yield return new WaitForSeconds(reentryTime);
        float dis = ((Vector2)transform.position - Vector2.zero).sqrMagnitude;
        if (dis >= 100*100)
        {
            Vector2 dist = Vector2.zero - self_rb.position;
            float seek = Mathf.Atan2(dist.y, dist.x) * Mathf.Rad2Deg - 90f + Random.Range(-25f, 25f);
            self_rb.rotation = seek; 
            self_rb.AddRelativeForce(force * sizeFactor, ForceMode2D.Impulse);
        }
        else
        {
            self_rb.AddForce(new Vector2(Random.Range(-force.x,force.x),Random.Range(-force.y,force.y)), ForceMode2D.Impulse);
        }
        StartCoroutine("AutoReenter");
    }

}
