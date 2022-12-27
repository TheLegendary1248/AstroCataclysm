using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public static CamFollow self;
    public static float euclidRot = 0;
    
    void Start()
    {
        self = this;
    }

    void FixedUpdate()
    {
        euclidRot = transform.eulerAngles.z;
        
    }

    public static void HighlightObject() { }
    public static void SetFocus() { }
}
