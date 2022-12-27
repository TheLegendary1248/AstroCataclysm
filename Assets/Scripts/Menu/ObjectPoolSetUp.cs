using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolSetUp : Projectiles
{
    public List<ObjectContainers> BulletsToAddToLibrary;
    public void Load()
    {
        //Add objects to 'Lib'
        foreach (ObjectContainers objects in BulletsToAddToLibrary)
        {
            Lib.Add(objects.Object.name, objects);
        }
    }
}
