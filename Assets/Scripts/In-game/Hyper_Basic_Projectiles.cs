using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hyper_Basic_Projectiles : Basic_Projectiles
{
    public float IncreasePerSecond;
    public bool ReverseOp;

    void FixedUpdate()
    {
        if (!ReverseOp)
            Speed += IncreasePerSecond * Time.fixedDeltaTime;
        else
            Speed -= IncreasePerSecond * Time.fixedDeltaTime;
        transform.Translate(0, Speed * Time.fixedDeltaTime, 0);
    }
}
