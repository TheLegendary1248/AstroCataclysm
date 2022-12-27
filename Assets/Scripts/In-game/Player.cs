using System;
using System.Collections;
using UnityEngine;
/// <summary>
/// Script in charge of controlling the player
/// </summary>
public class Player : PlayerHealth
{
    //Below are initialized in the inspector or as is
    public static Transform instTransform; Rigidbody2D rb;
    public string bullet;//Ref to the player's current choice of fire
    Projectiles.ObjectContainers bulletRef;
    public GameObject shockwave;//Ref to shockwave
    public GameObject death_fx; // Particle Effect after death
    public GameObject end_screen; //Reference to End Screen
    public LineRenderer sight; public Gradient[] sightcolors;
    public float fire_rate;
    public float base_FuelRate = 10f;//Base fuel rate. VarFuelRate returns to this value if no effect to energy is applied 
    public float restore = 6f;//Regeneration rate(Max Health)
    public float camFocus = 1f;//Camera Size. Set to one for zoom-out introduction to the game
    public float velocityLimit;
    [Header("Unneccessary to Assign")]
    //Below are initialized later
    Transform camTransform;//Ref to the Camera's Position
    Vector2 vel;//A 2D Vector used for translation of the player. Determined by mouse movement
    float rot;//Rotation
    public float combo; //Current Combo Value. Used for damage amplification.
    float VarFuelRate;//Amount by which energy is resupplied. This variable changes according to effect
    bool active = true;//Indicator if the player exhausted their energy supply
    bool coolDown = true;//Cooldown Indicator(For fire rate)
    public bool[] overCharge = new bool[] { false, false };
    Vector2 lastVel;
    public static Player controlInst;
    int LastBodyCount = 0;
    static bool isDead = false;
    public void ToggleCameraType() {cam.orthographic = !cam.orthographic;}
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        isDead = false;
        instTransform = transform;
        controlInst = this;
        Swarming_AI.target = transform;
        Missile_PhysicsAffected.player = transform;
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            CursorLock(false); //Hides cursor and locks it to middle of screen
        }
        camTransform = cam.gameObject.GetComponent<Transform>();
        camTransform.position = transform.position;
        VarFuelRate = base_FuelRate;
        bulletRef = Projectiles.Lib[bullet];
        if (!Settings.hasAlreadyLoaded) Settings.Load();
    }
    private void OnTriggerEnter2D(Collider2D collision) {if (collision.CompareTag("Enemy")){camFocus -= 2f;}} 
    //The above and below OnTriggers add an automatic, dramatic zooming effect if an enemy comes close within the radius of the trigger
    private void OnTriggerExit2D(Collider2D collision)  {if (collision.CompareTag("Enemy")){camFocus += 2f;}}
    void FixedUpdate()
    {
        FuelUse(-FuelRegen * Time.fixedDeltaTime);
        if (active)
        {
            vel = Vector2.Lerp(vel, lastVel, Time.fixedDeltaTime * 40);
            lastVel = vel;
            //Debug.Log(Input.touchCount.ToString())
            float diff = (Mathf.Abs(Mathf.DeltaAngle(Mathf.Atan2(vel.y,vel.x)*Mathf.Rad2Deg,transform.eulerAngles.z + 90)) + 90)/360f + .5f;
            //transform.Translate(vel,Space.World);
            rb.AddRelativeForce(vel * rb.mass, ForceMode2D.Impulse); 
            if(rb.velocity.magnitude > velocityLimit) { Vector2 vel = rb.velocity; rb.velocity = vel.normalized * velocityLimit; }
            if(vel.magnitude > 1f | energy < 50)
            {
                animator.Play("Energy.EnergyAnim", -1, 0f);
            }
            FuelUse(vel.magnitude * VarFuelRate * diff  / 100f);
            vel = Vector2.zero;
        }
        transform.Rotate(Vector3.forward, rot*2);
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 1f, 75f);
        cam.orthographicSize = Mathf.Lerp(camFocus, cam.orthographicSize, .96f);
        camTransform.position = Vector3.Lerp(camTransform.position, transform.position + (Vector3)rb.velocity, .035f) - new Vector3(0, 0, 2);
        if (!cam.orthographic)
        { camTransform.rotation = Quaternion.Lerp(camTransform.rotation, Quaternion.Euler((camTransform.position.y - transform.position.y) / 3, -(camTransform.position.x - transform.position.x) / 3, 0), 0.25f); }
        else { camTransform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(camTransform.eulerAngles.z,transform.eulerAngles.z,0.1f)); }
        camFocus += 3f * Time.fixedDeltaTime; //Because if an enemy dies in Trigger, it doens't call OnTriggerExit2D()
        camFocus = Mathf.Clamp(camFocus, 30f, 75f); //Confines camera size
        Collider2D[] templist = Physics2D.OverlapCircleAll(transform.position, 55);
        camFocus += (LastBodyCount - templist.Length) * 3f;
        LastBodyCount = templist.Length;
    }
    public static void CursorLock(bool ShowCursor)//Hides cursor and locks it to middle of screen if false is given
    {
        Cursor.lockState = ShowCursor ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = ShowCursor;
    }
    void Update()
    {
        if (active & Time.timeScale != 0)
        {
            if (SystemInfo.deviceType == DeviceType.Handheld)
            {
                //Handheld Controls. Haven't tested yet
                if (Input.touchCount > 0)
                {
                    for (int i = 0; i < Input.touchCount & i < 2; i++)
                    {
                        Touch touch = Input.GetTouch(i);
                        if(touch.position.x < (cam.pixelWidth / 2)) //Left Side Controls
                        {                           
                            if (coolDown)
                            {
                                Shoot();
                            }
                            transform.Rotate(Vector3.forward, Settings.rotationSensitivity * touch.deltaPosition.x * Time.deltaTime);
                        }                      
                        if (touch.position.x > (cam.pixelWidth / 2)) //Right Side Controls
                        {
                            vel = touch.deltaPosition * Time.deltaTime * Settings.movementSensitivity;
                            VarFuelRate = base_FuelRate;
                        }
                        else if(touch.position.x > (cam.pixelWidth / 2) || touch.phase == TouchPhase.Stationary)
                        {
                            Restore(Time.deltaTime * restore);
                            vel.y = 0f; vel.x = 0f; VarFuelRate = 0;
                            FuelUse(-base_FuelRate * Time.deltaTime);
                        }                        
                    }                                       
                }                
            }
            //___________________________________________________________________________________________________________________________________________
            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                //Desktop Controls
                vel.x += Input.GetAxisRaw("Mouse X") * Settings.movementSensitivity;
                vel.y += Input.GetAxisRaw("Mouse Y") * Settings.movementSensitivity;
                rot = Settings.rotationSensitivity * -Input.GetAxis("Horizontal");
                //transform.Rotate(Vector3.forward, wheel_sensitivity * -Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime);
                camFocus -= 50f * Input.GetAxisRaw("Vertical") * Time.deltaTime;

                if (Input.GetMouseButtonDown(0))
                {
                    if (coolDown) //If not already in cooldown, fire. Prevents spamming click
                    {
                        Shoot();
                    }
                }
                if (Input.GetMouseButton(1)) //Right Click
                {
                    Restore(Time.deltaTime * restore);
                    vel.y = 0f; vel.x = 0f; VarFuelRate = 0;
                    FuelUse(-base_FuelRate * Time.deltaTime );
                    if (energy > 90 & overCharge[0])//Second phase of overcharging
                    {
                        overCharge[1] = true;
                        sight.colorGradient = sightcolors[1];
                        SettingControler.updateOvercharge = true;
                    }
                    animator.Play("Energy.EnergyAnim", -1, 0f);
                }
                if (Input.GetMouseButtonUp(1)) //Right Click Up
                {
                    VarFuelRate = base_FuelRate;
                }
            }
        }
        else { vel.x = 0; vel.y = 0; rot = 0; } //Prevents the ship from drifting away if fuel runs out
        if (Input.GetKeyDown(KeyCode.Space) & !isDead) //Pause
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Master.Pause(true);
            }
            else
            {
                Master.Pause(false);
            }
        }
        if (isDead)
        {
            cam.transform.Translate(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * 100f * Time.unscaledDeltaTime);
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + (Input.GetKey(KeyCode.Q) ? 50f * Time.unscaledDeltaTime : 0) - (Input.GetKey(KeyCode.E) ? 50f * Time.unscaledDeltaTime : 0), 10, 75);
        }
    }
    public override void Kill()//Death of player
    {
        active = false;
        GetComponent<SpriteRenderer>().enabled = false;
        
        Time.timeScale = 0f;
        CursorLock(true);
        Instantiate(death_fx, transform.position, transform.rotation);
        Master.EndGame();
        isDead = true;
    }
    public override IEnumerator Exhaust()//Called when fuel runs out.
    {
        active = false;
        yield return new WaitForSeconds(1f);
        active = true;
    }
    void Shoot()
    {
        if (overCharge[1] & !Input.GetMouseButton(1))
        {
            Instantiate(shockwave, transform.position, transform.rotation);
            sight.colorGradient = sightcolors[0];
            overCharge = new bool[] { false, false };
            camFocus = 75f;
            StartCoroutine(FWait()); //Fire rate
        }
        else
        {
            GameObject shot = bulletRef.GetAndSetProjectile(transform.position, transform.eulerAngles.z);
            Basic_Projectiles Ref = shot.GetComponent<Basic_Projectiles>(); //Dmg multiplier
            Ref.combo = combo;
            Ref.PlayerShot = true;
            if(Input.GetMouseButton(1))
            {
                Ref.SpeedMultiplier *= 2;
            }
            StartCoroutine(FWait()); //Fire rate
        }
        FuelUse(2f);
    }
    IEnumerator FWait() //Fire Rate
    {
        coolDown = false;
        yield return new WaitForSeconds(fire_rate);
        if (Input.GetMouseButton(0) && active)
        {
            Shoot();
        }
        else
        {
            coolDown = true;
        }
    }
}