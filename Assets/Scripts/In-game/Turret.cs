using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Interface of Turrets. Make sure to implement to avoid clutter
/// </summary>
interface IAimable { }
public class Turret : MonoBehaviour
{
    public string ProjectileName;
    bool ReverseDir = false; public bool flipAngle = false;
    public Queue<GameObject> ProjectilePool;
    public GameObject projectile;
    public Basic_Projectiles.MovementTypes Type;
    public Basic_Projectiles.SpecialMovementVars MovementVars;
    public float delay;
    public float burst_qt;
    public float burst_delay = 0.1f;
    public float ang_toggle = 0f;
    public float spread = 0f;
    public bool isFriendly = false;
    public bool isShield = false;
    public bool ceaseFire = false;
    public Transform parent;
    public int turretID;
    public void Awake()
    {
        ProjectilePool = Projectiles.Lib[ProjectileName].Pool;
        delay /= SettingControler.enemyTurretFireRateMultiplier;
        burst_delay /= SettingControler.enemyTurretFireRateMultiplier;
    }
    public IEnumerator Fire() //The firing routine, determined by its variables
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < burst_qt; i++)
        {
            GameObject shot = Projectiles.GetAndSetProjectile(ProjectileName, transform.position, transform.eulerAngles.z + ang_toggle + Random.Range(-spread, spread));
            Projectiles shotScript = shot.GetComponent<Basic_Projectiles>();
            
            if(shotScript is Basic_Projectiles basic)
            {
                basic.ChangeMovementType(Type, MovementVars);
                if (ReverseDir & flipAngle)
                {
                    basic.Flip();
                }
            }
            if(isFriendly)
            {
                shotScript.Convert(true);
            }
            ang_toggle *= -1f;
            ReverseDir = !ReverseDir;
            yield return new WaitForSeconds(burst_delay);
        }
        if(parent & ceaseFire)parent.GetComponent<IHeavy>().firingProp = turretID;
        yield return new WaitUntil(() => !ceaseFire);
        StartCoroutine(Fire());
    }
}
public interface IAimed { }
public interface ILauncher { }
