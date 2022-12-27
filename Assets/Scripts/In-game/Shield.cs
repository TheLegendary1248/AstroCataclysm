using System.Collections.Generic;
using System.Collections;
using UnityEngine;
/// <summary>
/// Class for shields
/// </summary>
public class Shield : Health
{
    /// <summary>
    /// Specify how the shield should behave
    /// </summary>
    public enum ShieldType
    {
        /// <summary>
        /// A shield type that keeps its shape and is really durable. Doesn't regenerate while active and has a fair cooldown
        /// </summary>
        Fortress,
        /// <summary>
        /// A shield type that keeps its shape and fairly tough. Regenerates while active and has a short cooldown
        /// </summary>
        Stasis,
        /// <summary>
        /// A shield type that grows and shrinks depending on it's health. No cooldown and regenerates continously
        /// </summary>
        Battery
    }
    public LineRenderer line;
    public SpriteRenderer sprite;
    Color[] colors = new Color[2];
    public Coroutine Anim;
    public ShieldType type;
    public int regen = 1;
    public float coolDown = 10f;
    public float maxSize = 30f;
    public float lerped = 0;
    Rigidbody2D rigidBody2D;
    bool active = true;
    public void Start()
    {
        colors[1] = line.startColor;
        colors[0] = sprite.color;
        rigidBody2D = GetComponent<Rigidbody2D>();
        hitPoints = (int)healthMax;
        StartCoroutine(Regen());
    }
    private void FixedUpdate()
    {
        if(active & type == ShieldType.Battery)
        {
            lerped = Mathf.Lerp(lerped, hitPoints, 0.05f);
            transform.localScale = Vector2.one * Mathf.Lerp(0 , maxSize, lerped / healthMax);
        }
        else
        {
            lerped = Mathf.Lerp(lerped, active ? 1 : 0, 0.05f);
            transform.localScale = Vector2.one * Mathf.Lerp(0, maxSize, lerped);
        }
    }
    IEnumerator Regen()
    {
        yield return new WaitForSeconds(0.5f);
        if (active)
        {
            hitPoints += type != ShieldType.Fortress ? regen : 0;
            hitPoints = Mathf.Clamp(hitPoints, 0, (int)healthMax);
            StartCoroutine(Regen());
        }
    }
    IEnumerator Cooldown()
    {
        active = false;
        rigidBody2D.simulated = false;
        yield return new WaitForSeconds(coolDown);
        active = true;
        rigidBody2D.simulated = true;
        hitPoints = (int)healthMax;
        StartCoroutine(Regen());
    }
    IEnumerator Flash()
    {
        float timestamp = Time.time + 0.25f;
        do
        {
            line.startColor = Color.Lerp(colors[1], Color.white, (timestamp - Time.time) * 4);
            line.endColor = Color.Lerp(colors[1], Color.white, (timestamp - Time.time) * 4);
            sprite.color = Color.Lerp(colors[0], Color.white, (timestamp - Time.time) * 4);
            yield return new WaitForFixedUpdate();
        } while (timestamp > Time.time);
    }
    public override void Dmg(int dmg)
    {
        if (active)
        {
            StartCoroutine(Flash());
            hitPoints -= dmg;
            if(hitPoints <= 0)
            {
                hitPoints = 0;
                if (type == ShieldType.Battery)
                    return;
                StopCoroutine(Regen());
                StopCoroutine(Cooldown());
                StartCoroutine(Cooldown());
            }
        }   
    }
}
