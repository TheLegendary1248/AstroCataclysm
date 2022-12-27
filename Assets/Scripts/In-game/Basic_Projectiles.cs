using System;
using System.Collections;
using UnityEngine;
public class Basic_Projectiles : Projectiles
{
    [Serializable]
    public struct SpecialMovementVars
    {
        public float One;
        public float Two;
    }
    public float[] Special = new float[2];
    public enum MovementTypes
    {
        /// <summary>
        /// Defines straight forward movement
        /// </summary>
        Normal, 
        /// <summary>
        /// First variable defines curve amount
        /// </summary>
        Curve, 
        /// <summary>
        /// First variable defines Frequency, Second defines Width
        /// </summary>
        Sine, 
        /// <summary>
        /// Same as normal, but the first defines the interval between toggles
        /// </summary>
        Sparkler, 
        /// <summary>
        /// First defines speed change. Then its multiplied by the sign of the second variable
        /// </summary>
        Hyper
    }
    public MovementTypes Type;
    public delegate void movement();
    movement Movement = new movement(Stall);
    public GameObject DeathFX;
    public string Frag;
    public int FragNum = 8;
    public float combo = 1f;
    public SubProjectiles[] Frags;
    //Almost every projectile has this script for basic, straight-forward movement.
    [Serializable]
    public class SubProjectiles
    {
        public string FragName;
        public int Amount = 6;
        public SpecialMovementVars specialMovement;
        public MovementTypes Type = MovementTypes.Normal;
    }
    public void Flip()
    {
        switch (Type)
        {
            case MovementTypes.Sine:
                Special[0] *= -1;
                break;
            case MovementTypes.Curve:
                Special[0] *= -1;
                break;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type">Type of Movement</param>
    /// <param name="MovementVars">Two variables determining the special movement</param>
    public void ChangeMovementType(MovementTypes type, SpecialMovementVars MovementVars)
    {
        Type = type;
        switch (type)
        {
            case MovementTypes.Normal:
                Movement = Normal;
                //No effect
                break;
            case MovementTypes.Curve:
                Movement = Curve;
                Special[0] = MovementVars.One;
                //Curve amount
                break;
            case MovementTypes.Sine:
                Special = new float[4] { MovementVars.One, MovementVars.Two, Time.time, 0 };
                //Resized to fit needs. They're (in respective order) Frequency, Width, TimeStamp, Difference
                Movement = Sine;
                break;
            case MovementTypes.Sparkler:
                Special[0] = MovementVars.One;
                Special[1] = 1;
                Movement = Normal;
                //Sparkle Interval, Toggle Placeholder
                StartCoroutine(SparklerInterval());
                break;
            case MovementTypes.Hyper:
                Special[0] = MovementVars.One;
                Special[1] = MovementVars.Two;
                Movement = Hyper;
                //Speed Change, Additive/Subtractive
                break;
            default:
                goto case MovementTypes.Normal;
        }
    }
    void OnEnable()
    {
        StartCoroutine(LifeTime(Detonate));
    }
    private void Awake()
    {
        if(Movement == Stall)
        {
            Movement = Normal;
        }
    }

    public void EndOfLife(bool HitSomething)
    {
        StopAllCoroutines();
        Movement = Stall;
        if (!string.IsNullOrEmpty(Frag))
        {
            if (FragNum == 1)
            {
                GameObject AOE = Instantiate(Lib[Frag].Object, transform.position, transform.rotation);
                if (hasConverted)
                {
                    AOE.GetComponent<Projectiles>().Convert(PlayerShot);
                }
            }
            else
            {
                for (int i = 0; i < FragNum; i++)
                {
                    GameObject obj;
                    obj = GetAndSetProjectile(Frag, transform.position, transform.eulerAngles.z + (360 / FragNum * i));

                    if (hasConverted)
                    {
                        Projectiles projectiles = obj.GetComponent<Projectiles>();
                        if (projectiles)
                            projectiles.Convert(PlayerShot);
                    }
                }
            }
        }
        if (DeathFX)
            Instantiate(DeathFX, transform.position, transform.rotation);
        if (PlayerShot & !HitSomething)
        {
            data.BuildCombo(false, false);
        }
        transform.parent = null;
        Behaviour com = (Behaviour)GetComponent("Halo");
        if (com)
        {
            com.enabled = false;
        }
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite)
        {
            sprite.enabled = false;
        }
        GetComponent<Rigidbody2D>().simulated = false;
        TrailRenderer trail = GetComponent<TrailRenderer>();
        if (trail)
        {
            trail.emitting = false;
            StartCoroutine(WaitForTrailToFade(trail.time));
        }
        else
        {
            Pool();
            gameObject.SetActive(false);
        }

    }
    IEnumerator LifeTime(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        EndOfLife(false);
    }
    void ChangeColor()//Changes color when converted
    {
        Color color;
        switch (projectileState)
        {
            case ProjectileState.Friendly:
                color = FriendlyColor;
                break;
            case ProjectileState.Converted:
                color = ConvertedColor;
                break;
            case ProjectileState.Enemy:
                color = EnemyColor;
                break;
            default:
                color = EnemyColor;
                break;
        }
        TrailRenderer trail = GetComponent<TrailRenderer>();
        if (trail)
        {
            trail.startColor = color;  
        }
        else
        { GetComponent<SpriteRenderer>().color = color; }
    }
    public override void Convert(bool isShockwave)//Conversion
    {
        if (isShockwave)
        {
            gameObject.layer = 8;
            projectileState = ProjectileState.Friendly;
        }
        else
        {
            gameObject.layer = 12;
            projectileState = ProjectileState.Converted;
        }
        ChangeColor();
        hasConverted = true;
    }
    void FixedUpdate() //Basic forward movement called each frame
    {
        Movement();
    }
    
    #region Movements
    /// <summary>
    /// Normal, straight forward, movement
    /// </summary>
    void Normal()
    {
        transform.Translate(0, Speed * Time.fixedDeltaTime * SpeedMultiplier, 0);
    }
    /// <summary>
    /// Movement that makes the bullet move horizontally to itself, in a sine like pattern
    /// </summary>
    void Sine()
    {
        float x = Mathf.Sin((Time.fixedTime - Special[2]) * Special[0]) * Special[1];
        transform.Translate(x - Special[3], Speed * Time.fixedDeltaTime * SpeedMultiplier, 0);
        Special[3] = x;
    }
    /// <summary>
    /// Movement that makes the bullet curve in one direction
    /// </summary>
    void Curve()
    {
        transform.Rotate(0, 0, Special[0] * Time.fixedDeltaTime);
        transform.Translate(0, Speed * Time.fixedDeltaTime * SpeedMultiplier, 0);
    }
    /// <summary>
    /// Movement that adds or subtracts continuosly to the bullet's speed
    /// </summary>
    void Hyper()
    {
        SpeedMultiplier += Special[0] * Mathf.Sign(Special[1]) * Time.fixedDeltaTime;
        transform.Translate(0, Speed * Time.fixedDeltaTime * SpeedMultiplier, 0);
    }
    /// <summary>
    /// Empty so that the projectiles does nothing
    /// </summary>
    static void Stall() { /* Empty, so that the projectile does nothing */}
    #endregion
    /// <summary>
    ///Coroutine for Sparkler Types
    /// </summary>
    IEnumerator SparklerInterval()
    {
        yield return new WaitForSeconds(Special[0]);
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        TrailRenderer trail = GetComponent<TrailRenderer>();
        Rigidbody2D collider2 = GetComponent<Rigidbody2D>();
        bool toggle = Special[1] == 1;
        Special[1] = Special[1] == 1 ? 0 : 1;
        if (sprite)
            sprite.enabled = toggle;
        if (trail)
            trail.emitting = toggle;
        if (collider2)
            collider2.simulated = toggle;
        StartCoroutine(SparklerInterval());
    }
    /// <summary>
    /// Coroutine used to stall disabling the GameObject to let the trail fade naturally
    /// </summary>
    /// <param name="time"> Trail.time goes here</param>
    IEnumerator WaitForTrailToFade(float time)
    {
        yield return new WaitForSeconds(time);
        Pool();
        GetComponent<TrailRenderer>().Clear();
        gameObject.SetActive(false);
    }
    /// <summary>
    /// Function to pool the object in its respective group
    /// </summary>
    public override void Pool()//Function to pool the object in its respective group
    {
        ObjectContainers container = Lib[name.Replace("(Clone)", "")];
        if (container != null)
        {
            container.Pool.Enqueue(gameObject);
        }
    }
    public override void Revert()
    {
        ObjectContainers containers = Lib[name.Replace("(Clone)", "")];
        gameObject.layer = containers.Object.layer;
        projectileState = containers.State;
        Special = new float[2] { 0f, 0f };
        PlayerShot = false;
        TrailRenderer trail = GetComponent<TrailRenderer>();
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (trail)
        {
            trail.emitting = true;
            trail.startColor = containers.Object.GetComponent<TrailRenderer>().startColor;
            if (sprite)
            {
                sprite.enabled = true;
            }
        }
        else
        {
            sprite.enabled = true;
            sprite.color = containers.Object.GetComponent<SpriteRenderer>().color;
        }
        Behaviour com = (Behaviour)GetComponent("Halo");
        if (com)
        {
            com.enabled = true;
        }
        SpeedMultiplier = 1f;
        GetComponent<Rigidbody2D>().simulated = true;
        hasConverted = false;
        Movement = Normal;
        gameObject.SetActive(true);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Shockwave"))
        {
            //Checks if the object hit is infact the target intended
            if (collision.CompareTag("Projectile"))
            {
                Projectiles movement = collision.GetComponent<Projectiles>();
                if (movement)
                {
                    if (PlayerShot & movement.isConvertible & !movement.hasConverted)
                    {
                        movement.Convert(false);
                        EndOfLife(true);
                    }
                }
                else
                    Debug.Log("Projectile does not have script attached???");
            }
            else
            {
                bool isTarget = false; 
                bool isObstacle = false;
                if (!collision.CompareTag("Obstacle") & !collision.isTrigger)
                    foreach (string s in TargetPairs[projectileState])//Check if compatible
                    {
                        if (collision.CompareTag(s))
                        {
                            isTarget = true;
                            break;
                        }
                    }
                else if (collision.isTrigger)//Check if it's shield
                    isTarget = collision.gameObject.layer == 16;
                else //Otherwise an obstacle
                    isTarget = isObstacle = true;
                Rigidbody2D body = collision.GetComponent<Rigidbody2D>();
                if (body)
                {
                    body.AddForce((collision.transform.position - transform.position) * 50, ForceMode2D.Impulse);
                }
                if (isTarget)//If so, look for a Health script which to deal damage
                {
                    Health health = collision.gameObject.GetComponent<Health>();
                    if (health)
                    {
                        int damage = Lib[name.Replace("(Clone)", "")].Damage;
                        damage = Mathf.RoundToInt(damage * SettingControler.globalDamageMultiplier);
                        if (PlayerShot)  //If shot by player, BuildCombo by whether the intended target was hit
                        {
                            bool hit = !collision.CompareTag("Obstacle");
                            data.BuildCombo(hit, hit);
                            damage = (int)(damage * SettingControler.self.Combo);
                        }
                        health.Dmg(damage);
                        if(!isObstacle)
                            PopUp_Text.Set(transform.position, PlayerShot | hasConverted ? FriendlyColor : EnemyColor, PopUp_Text.TypeOfValue.Damage, damage, PlayerShot | hasConverted ? 1.5f : 3f);
                        EndOfLife(true);
                    }
                }
            }
        }
        else
        {
            collision.gameObject.GetComponent<Shockwave>().Convert(GetComponent<Collider2D>());
        }
    }
    /*
    public void OnCollisionEnter2D(Collision2D collision) //Determines how the projectile should react on Collision
    {
        /*Health hp = collision.collider.GetComponent<Health>(); //Work on this later
        if(hp)*

        if (!collision.gameObject.CompareTag("Shockwave")) //Asks if the object is a shockwave. If so, do nothing since the shockwave contains the neccesary code for action
        {
            Health hitPoints = collision.collider.GetComponent<Health>();
            if (collision.collider.CompareTag(target) | collision.collider.CompareTag("Obstacle")) //If target or an object on scene whatever
            {
                int damage = Lib[name.Replace("(Clone)", "")].Damage;
                damage = Mathf.RoundToInt(damage * SettingControler.globalDamageMultiplier);
                if (PlayerShot)  //If shot by player, BuildCombo by whether the intended target was hit
                {
                    data.BuildCombo(collision.collider.CompareTag(target));
                }
                hitPoints.Dmg(damage);
                PopUp_Text.Set(transform, PlayerShot ? FriendlyColor : EnemyColor, PopUp_Text.TypeOfValue.Damage, damage, 1f);

                EndOfLife(true);
            }
            else
            {
                Basic_Projectiles movement = collision.collider.GetComponent<Basic_Projectiles>();
                if (movement)
                {
                    //Debug.Log(name);
                    if (PlayerShot & collision.collider.gameObject.layer == 11 & movement.isConvertible & !movement.hasConverted)
                    {
                        movement.Convert(false);
                        EndOfLife(true);
                    }
                }
            }
        }
    }
    */
}