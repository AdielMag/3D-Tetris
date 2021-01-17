using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraModel
{
    public float camRotationSpeed = 1;
    public Vector2 camPitchLimits = new Vector2(-15, 45);
    public Transform camParent;
}
