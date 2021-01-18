using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObj : MonoBehaviour
{
    public float rotateSpeed = 1;

    private void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed);
    }
}
