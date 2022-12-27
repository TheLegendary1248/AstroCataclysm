using System.Collections;
using UnityEngine;

public class Explosion : Projectiles
{
    public float shrinkSpeed = -50f;
    public string target = "Player";
    public float size = 0.2f;
    public override void Convert(bool isShockwave)
    {
        gameObject.layer = 8;
        hasConverted = true;
        target = "Enemy";
        gameObject.GetComponent<SpriteRenderer>().color = FriendlyColor;
        size *= 1.5f;
    }
    void Start()
    {
        StartCoroutine("Expand");
    }
    void FixedUpdate()
    {
        transform.localScale -= Vector3.one * Time.deltaTime * shrinkSpeed;
        if (transform.localScale.y <= .3f)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.CompareTag(target) | collision.CompareTag("Obstacle")) & !collision.isTrigger)
        {
            Health health = collision.GetComponent<Health>();
            if(health)
            {
                int dmg = Lib[name.Replace("(Clone)", "")].Damage;
                collision.GetComponent<Health>().Dmg(dmg);
                PopUp_Text.Set(transform.position, hasConverted ? FriendlyColor : EnemyColor, PopUp_Text.TypeOfValue.Damage, dmg, 2f);
            }
        }
    }
    IEnumerator Expand() 
    {
        yield return new WaitForSeconds(size);
        shrinkSpeed *= -.1f;
        Destroy(gameObject.GetComponent<CircleCollider2D>()); //Renders the explosion harmless once it has stopped expanding
    }
}
