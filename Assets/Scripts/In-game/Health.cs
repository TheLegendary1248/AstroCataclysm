using UnityEngine.UI;
using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public int ReadableHealth { get { return hitPoints; } }
    protected int hitPoints = 100;
    public float healthMax = 100f;
    public ParticleSystem dmgIndicator;
    //Every destructible item in-game has this script, although some code is exclusive to the player.
    private void Start()
    {
        hitPoints = (int)(healthMax * SettingControler.enemyHealthMultiplier);
    }
    public virtual void Dmg(int dmg) //Deals damage to object. Called by bullets that hit the Game Object
    {
        hitPoints -= dmg;
        if (dmgIndicator)
        {
            var em = dmgIndicator.emission;
            em.rateOverTime = Mathf.Round(15*(.75f-(hitPoints/healthMax)));
        }
        if(hitPoints <= 0)
        {
            if(dmgIndicator)
            {
                dmgIndicator.Stop();
            }
            Kill();
        }
    }
    public virtual void Heal(int gain)//Heals SELF. Exclusive to player for now and called by in-game Pickups
    {
        hitPoints += gain;
        hitPoints = Mathf.Clamp(hitPoints, 0, Mathf.FloorToInt(healthMax));
    }
    public virtual void Kill() { } 
}
