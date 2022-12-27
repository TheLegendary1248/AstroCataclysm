using UnityEngine;
public class Shockwave : MonoBehaviour
{
    public Rigidbody2D self_rb;
    public Transform origin;
    public float pushForce;
    public float enemyPushForce;
    public void Kill()
    {
        Destroy(gameObject);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        Convert(collision.collider);
    }
    public void Convert(Collider2D collider)
    {
        GameObject obj = collider.gameObject;
        if(obj.layer == 11)
        {
            //Debug.Log("Made it here");
            Projectiles projectile = obj.GetComponent<Projectiles>();
            if(!projectile.hasConverted)
            {
                obj.transform.Rotate(0, 0, Reflection(obj.transform.position, origin.position));//Fix this sometime later. Any bullets reflected during the first half of the shockwave's life gets sent towards near the bullet's origin.
                projectile.Convert(true);
            }
        }
        if (obj.GetComponent<Rigidbody2D>()) //If it has a rigidbody, apply force to it;
        {
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            Physics2D.IgnoreCollision(obj.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            float Force = obj.CompareTag("Enemy") | obj.CompareTag("Projectile") ? enemyPushForce : pushForce;
            Swarming_AI AI = obj.GetComponent<Swarming_AI>();
            if(AI) { AI.StartCoroutine("CombatTag"); }
            rb.AddForce((Vector2)(obj.transform.position - origin.position) * Force * rb.mass);
            rb.AddTorque(/*Vector2.Dot(new Vector2(transform.forward.y, -transform.forward.x), (obj.transform.position - transform.position).normalized) * */10f, ForceMode2D.Impulse);
        }
    }
    private float Reflection(Vector2 obj, Vector2 wall)
    {
        Vector2 dif = obj - wall;
        float result = ((Mathf.Atan(dif.y / dif.x) * Mathf.Rad2Deg) - Mathf.Repeat(transform.eulerAngles.z, 360)) * 2f;
        return result;
    }
}
