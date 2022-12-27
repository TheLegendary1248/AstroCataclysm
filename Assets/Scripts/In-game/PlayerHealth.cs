using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Health
{
    public Camera cam;//Ref to the Camera 
    public float FuelRegen;
    public Slider healthBar;
    public Slider healthBarM;
    public Slider energyBar;
    public Animator animator;
    public float energy = 100f;
    void Start()
    {
        hitPoints = SettingControler.playerHealth;
        healthMax = hitPoints;
        healthBar.maxValue = hitPoints;
        healthBarM.maxValue = hitPoints;
        healthBar.value = hitPoints;
        healthBarM.value = hitPoints;
        
    }
    public override void Dmg(int dmg)
    {
        base.Dmg(dmg);
        healthMax -= Mathf.CeilToInt(dmg / 2f);
        healthBar.value = hitPoints;
        healthBarM.value = healthMax;
        cam.orthographicSize -= dmg/2;
        animator.Play("Health.HealthAnim", -1, 0f);
    }
    public override void Heal(int gain)
    {
        base.Heal(gain);
        healthBar.value = hitPoints;
        animator.Play("Health.HealthAnim", -1, 0f);
    }
    public void Restore(float gain)
    {
        healthMax += gain;
        healthMax = Mathf.Min(SettingControler.playerHealth, healthMax);
        healthBarM.value = healthMax;
        animator.Play("Health.HealthAnim", -1, 0f);
    }
    public void FuelUse(float J)//Called every FixedUpdate on the 'Control' script.
    {
        energy -= J;
        if (energy <= 0f)
        {
            animator.Play("Energy.EnergyDepleted",-1,0f);
            StartCoroutine(Exhaust());
        }
        energy = Mathf.Clamp(energy, 0f, 100f);
        energyBar.value = energy;
    }
    public virtual IEnumerator Exhaust() 
    {
        yield return new WaitForFixedUpdate();
    }
}
