using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Script given to all large, custom - physics affected, enemies
/// </summary>
public class HeavyAI : Health, IHeavy, IEnemy
{ 
    public int _entityValue; public int EntityValue => _entityValue; 
    public int _score; public int Score => _score; 
    Turret[] turrets; bool[] notFiring;
    public int firingProp
    {
        set
        {
            notFiring[value] = true;
            allowRedirect = true;
            foreach (bool turret in notFiring)
            {
                if (!turret)
                {
                    allowRedirect = false;
                    break;
                }
            }
        }
    }
    Shield[] shields;
    int bow, port, starBoard; //Implement later
    int maxShield;
    public float bowPos = 0;//Determines where the bow begins
    public float speed;
    public float iterations = 50f;
    public GameObject explosionFX;
    public GameObject[] drops = new GameObject[3];
    public float explosionScale = 1f;
    public float forceRedirectionTime = 5f;
    public float strafeRange;
    public int ShipValue = 40;
    float rotationRate;
    public float agility = 20f;
    bool IsRotating = false;
    bool IsRedirecting = false; 
    bool allowRedirect = false;
    public bool hasDied = false;//required for a dumb reason ; Collision is called even if the Gameobject is set to be destroyed, so frags cause multiple deaths on one ship
    public int score;

    void Start()
    {
        hitPoints = (int)healthMax;
        //StartCoroutine(ProxCheck()); Check
        shields = GetComponentsInChildren<Shield>();
        turrets = GetComponentsInChildren<Turret>(); int i = 0;
        foreach (Turret t in turrets)
        {
            t.parent = transform;
            t.turretID = i++;
        }
        notFiring = new bool[turrets.Length];
        NPCHandler.Population += EntityValue;
        StartCoroutine(ForceRedirect());
        StartCoroutine(Redirect());
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(Vector3.up * speed * Time.fixedDeltaTime);
        if (IsRotating)
            transform.Rotate(Vector3.forward * rotationRate * Time.fixedDeltaTime);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") & !IsRedirecting)//Redirects the ship should the player leave its trigger
        {
            StartCoroutine(Redirect());
        }
    }
    public override void Kill()//Death of enemy 
    {
        if (!hasDied)
        {
            GameObject explosionScaler = Instantiate(explosionFX, transform.position, transform.rotation);
            explosionScaler.transform.localScale *= explosionScale;
            GameObject drop = Instantiate(drops[Random.Range(0, drops.Length)], transform.position, transform.rotation);
            if (drop.CompareTag("Pickup"))
            {
                drop.GetComponent<Pickup>().restore = 45;
            }
            PopUp_Text.Set(transform.position, Color.red, PopUp_Text.TypeOfValue.Kill, 1000f, 3f);
            NPCHandler.Population -= EntityValue;
            Destroy(gameObject);
        }
        
        hasDied = true;
    }
    IEnumerator ForceRedirect() //Schedules redirects
    {
        yield return new WaitForSeconds(forceRedirectionTime);
        if (IsRedirecting)
        {
            yield return new WaitUntil(() => !IsRedirecting);
        }
        else
        {
            StartCoroutine(Redirect());
        }
        StartCoroutine(ForceRedirect());
    }
    IEnumerator Redirect() //Turn AI of Ship
    {
        foreach (Turret i in turrets)i.ceaseFire = true;
        allowRedirect = false;
        IsRedirecting = true;
        yield return new WaitUntil(() => allowRedirect);
        IsRotating = true;
        #region RedirectCalculations
        //Finds a point on a circle around the target that is perpendicular of the normal between self and the player
        //View Equation here : https://www.desmos.com/calculator/hkugtv9tqr
        float ratio = strafeRange/Vector3.Distance(Player.instTransform.position,transform.position);                               //Create a float that is the ratio of strafe to distance. Multiplied by and negated if needed by direction.
        Vector2 dist = (Vector2)Player.instTransform.position - (Vector2)transform.position;                                        //Get the difference between self and the player
        bool rand = Random.Range(0, 2) == 1;                                                                                        //Get a random boolean used to figure out if strafing to left or right
        Vector2 perpendicularPoint = new Vector2(dist.x + (dist.y * (rand?ratio:-ratio)), dist.y + (dist.x *(rand?-ratio:ratio)));  //Add onto the difference with the other component proportioned by the ratio to get the point on the circle
        perpendicularPoint += (Vector2)transform.position;
        Vector2 perpDifference = perpendicularPoint - (Vector2)transform.position;
        float angle = Mathf.Atan2(perpDifference.y,perpDifference.x) * Mathf.Rad2Deg;
        float deltaAngle = Mathf.DeltaAngle(transform.eulerAngles.z + 90f, angle);
        rotationRate = agility * Mathf.Sign(deltaAngle);
        #endregion //Calculation 
        Debug.DrawLine(transform.position, perpendicularPoint, Color.yellow, 3f);
        yield return new WaitForSeconds(Mathf.Abs(deltaAngle) / agility);
        IsRotating = false;
        IsRedirecting = false;
        rotationRate = Mathf.Sign(rotationRate);
        foreach (Turret i in turrets) i.ceaseFire = false;
        for (int i = 0; i < notFiring.Length; i++) notFiring[i] = false;
    }
    IEnumerator ProxCheck()//Checks if close to another of it own. If so, turn slightly to prevent clumping
    {
        yield return new WaitForSeconds(3f);
        Collider2D[] colliders= Physics2D.OverlapCircleAll(transform.position, 5);
        float dir = 0;
        Collider2D self = GetComponent<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
        {
            if(colliders[i].gameObject.layer == gameObject.layer & colliders[i] != self)//Check. Also checks that we aren't reporting this collider
            {
                //Debug.Log(colliders[i]);
                dir += transform.InverseTransformPoint(colliders[i].transform.position).x > 0 ? 1 : -1;//Depending on which side it is, add to dir to determine final movement
            }
        }
        dir *= 30;
        if(dir != 0)
        {
            dir /= iterations;
            for (int i = 0; i < iterations; i++)
            {
                transform.Rotate(Vector3.forward, dir);//Rotate
                yield return new WaitForFixedUpdate();
            }
        }
        StartCoroutine(ProxCheck());
    }
    void StatusReport() //Tells the AI to check the health of all shield on the bow, port, and starboard parts of the ship. Uses this information to know which direction to strafe
    {
        foreach(Shield S in shields)
        {
            if(S.transform.localPosition.y > 0)
            {
                bow = S.ReadableHealth;
            }
            else if (S.transform.localPosition.x > 0)
            {
                starBoard = S.ReadableHealth;
            }
            else
            {
                port = S.ReadableHealth;
            }
        }
    }
    public override void Dmg(int dmg)
    {
        base.Dmg(dmg);
        StopCoroutine(Redirect());
        IsRotating = false;
        IsRedirecting = false;
        rotationRate = Mathf.Sign(rotationRate);
        foreach (Turret i in turrets) i.ceaseFire = false;
        for (int i = 0; i < notFiring.Length; i++) notFiring[i] = false;
    }
}
public interface IEnemy
{
    /// <summary>
    /// Score for destroying enemy
    /// </summary>
    int Score { get; }
    /// <summary>
    /// Used to determine a Coffee Break
    /// </summary>
    int EntityValue { get; }

}
interface IHeavy
{
    /// <summary>
    /// Used when targets are ID'ed
    /// </summary>
    int firingProp { set; } 
 
}
    
