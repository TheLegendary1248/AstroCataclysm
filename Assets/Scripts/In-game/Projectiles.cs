using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Base class for all projectiles. Also contains the object pools for most projectiles
/// </summary>
public class Projectiles : MonoBehaviour
{
    /// <summary>
    /// Determines if the projectile was shot by player. Enabled by default by the player once instantiated. Used to determine if a hit should affect combo/score
    /// </summary>
    public bool PlayerShot = false;
    public bool isConvertible = false;
    /// <summary>
    /// Anything that has this set to true cannot be converted by other projectiles
    /// </summary>
    public bool hasConverted = false;
    public float Detonate = 1f;
    public float Speed = 2f; 
    public float SpeedMultiplier = 1f;
    [Flags]
    public enum ProjectileState
    {
        Friendly =  0b_0000_0001,
        Converted = 0b_0000_0010,
        Enemy =     0b_0000_0100
    }
    public enum StandardProjectiles
    {
        
    }
    public readonly Dictionary<ProjectileState, string[]> TargetPairs = new Dictionary<ProjectileState, string[]>() 
    { 
        { ProjectileState.Friendly, new string[]{"Enemy"} },
        { ProjectileState.Converted, new string[]{"Player", "Enemy", "Friendly"} },
        { ProjectileState.Enemy, new string[]{"Player", "Friendly"} }
    };
    public ProjectileState projectileState = ProjectileState.Enemy;
    public static Dictionary<string, ObjectContainers> Lib = new Dictionary<string, ObjectContainers>();
    public static void Clear()
    {
        Dictionary<string, ObjectContainers>.KeyCollection keys = Lib.Keys; 
        foreach (string k in keys)
        {
            Lib[k].Pool.Clear();
        }
    }
    public static SettingControler data;
    /// <summary>
    /// Container of a certain projectile. Used to eliminate the look up of Lib
    /// </summary>
    [Serializable]
    public class ObjectContainers
    {
        public int Damage;
        public Queue<GameObject> Pool = new Queue<GameObject>();
        public GameObject Object;
        public ProjectileState State; 
        public GameObject GetAndSetProjectile(Vector2 position, float rotation)
        {
            GameObject obj;
            if (Pool.Count <= 0)
            {
                obj = Instantiate(Object, position, Quaternion.Euler(0, 0, rotation));
            }
            else
            {
                obj = Pool.Dequeue();
                obj.transform.position = position;
                obj.transform.eulerAngles = new Vector3(0, 0, rotation);
                obj.GetComponent<Projectiles>().Revert();
            }
            return obj;
        }
        public void SetProjectile(Vector2 position, float rotation)
        { 
            if (Pool.Count <= 0)
            {
                Instantiate(Object, position, Quaternion.Euler(0, 0, rotation));
            }
            else
            {
                GameObject obj = Pool.Dequeue();
                obj.transform.position = position;
                obj.transform.eulerAngles = new Vector3(0, 0, rotation);
                obj.GetComponent<Projectiles>().Revert();
            }
        } 
    }
    /// <summary>
    /// Retrieves a bullet from the object pooler
    /// </summary>
    /// <param name="name">Name of bullet to search for</param>
    /// <param name="position">Set position of bullet</param>
    /// <param name="rotation">Set z rotation of bullet</param>
    public static GameObject GetAndSetProjectile(string name, Vector2 position, float rotation)
    {
        bool s = !Lib.ContainsKey(name);
        if (s)
            throw new Exception("Projectile is not in library");
        GameObject obj;
        if(Lib[name].Pool.Count <= 0)
        {
            obj = Instantiate(Lib[name].Object, position, Quaternion.Euler(0,0,rotation));
        }
        else
        {
            obj = Lib[name].Pool.Dequeue();
            obj.transform.position = position;
            obj.transform.eulerAngles = new Vector3(0,0,rotation);
            obj.GetComponent<Projectiles>().Revert();
        }
        return obj;
    }
    public static void SetProjectile(string name, Vector2 position, float rotation)
    {
        bool s = !Lib.ContainsKey(name);
        if (s)
            throw new Exception("Projectile is not in library");
        if (Lib[name].Pool.Count <= 0)
        {
            Instantiate(Lib[name].Object, position, Quaternion.Euler(0, 0, rotation));
        }
        else
        {
            GameObject obj = Lib[name].Pool.Dequeue();
            obj.transform.position = position;
            obj.transform.eulerAngles = new Vector3(0, 0, rotation);
            obj.GetComponent<Projectiles>().Revert();
        }
    }
    public static ObjectContainers GetContainer(string name) => Lib.ContainsKey(name) ? Lib[name] : throw new Exception();
    /// <summary>
    /// Converts the projectile
    /// </summary>
    /// <param name="isShockwave">If it was hit by a shockwave, the projectile gets fully converted, else it can hit both friendly and enemy targets</param>
    public virtual void Convert(bool isShockwave) { }
    public virtual void Pool() { }
    public virtual void Revert() { }
    public virtual void Fragmentation() { }
    public static readonly Color FriendlyColor = new Color(0f, 0.6f, 1f, 1f);
    public static readonly Color EnemyColor = Color.red;
    public static readonly Color ConvertedColor = Color.magenta;
}
