using UnityEngine;
public class Pickup : MonoBehaviour
{
    public string type;
    public int restore;
    public GameObject fx;
    private void Start()//Auto-scales the size of object depending on it's 'restore' value.
    {
        if (80 > restore && restore > 40)
        {
            transform.localScale = (restore / 10 * Vector2.one);
        }
        if (80 < restore)
        {
            transform.localScale = new Vector3(2, 2, 1);
        }
    }
    //Both of below are used for collision detection with player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.collider.isTrigger == false)//Detects if player
        {
            switch (type)//Determines type of pickup. The rest is self-explanatory.
            {
                case "scrap":
                    {
                        PlayerHealth health = collision.gameObject.GetComponent<PlayerHealth>();
                        health.Heal(restore);
                        PopUp_Text.Set(transform.position, Color.green, PopUp_Text.TypeOfValue.Damage, restore, 1.5f);
                        break;
                    }
                case "battery":
                    {
                        PlayerHealth health = collision.gameObject.GetComponent<PlayerHealth>();
                        health.FuelUse(-restore * 3);
                        PopUp_Text.Set(transform.position, Color.clear, PopUp_Text.TypeOfValue.Restore, restore * 3, 1.5f);
                        break;
                    }
            }
            Instantiate(fx, transform.position, Quaternion.identity);//Pickup effect.
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.isTrigger == false)//Detects if player
        {
                switch (type)//Determines type of pickup. The rest is self-explanatory.
            {
                case "scrap":
                {
                    PlayerHealth health = collision.gameObject.GetComponent<PlayerHealth>();
                    health.Heal(restore);
                    PopUp_Text.Set(transform.position, Color.green, PopUp_Text.TypeOfValue.Restore, restore, 1.5f);
                    break;
                }
                case "battery":
                {
                    PlayerHealth health = collision.gameObject.GetComponent<PlayerHealth>();
                    health.FuelUse(-restore * 3);
                    PopUp_Text.Set(transform.position, Color.cyan, PopUp_Text.TypeOfValue.Restore, restore * 3, 1.5f);
                    break;
                }
            }
            Instantiate(fx, transform.position, Quaternion.identity);//Pickup effect.
            Destroy(gameObject);
        }
    }
}
