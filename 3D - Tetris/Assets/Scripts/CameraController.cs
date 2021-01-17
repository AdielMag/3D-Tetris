using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : TetrisElement
{
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    public void Init()
    {
        yaw = app.model.cam.camParent.eulerAngles.y;
        pitch = app.model.cam.camParent.eulerAngles.x;
    }

    public void OnPlayerInput(Vector2 inputDelta)
    {
        RotateCamera(inputDelta);
    }

    private void RotateCamera(Vector2 inputDelta)
    {
        // Update yaw * pitch based on inputDelta
        yaw += app.model.cam.camRotationSpeed * inputDelta.x;
        pitch -= app.model.cam.camRotationSpeed * inputDelta.y;

        // Clamp pitch 
        pitch = Mathf.Clamp(pitch, app.model.cam.camPitchLimits.x, app.model.cam.camPitchLimits.y);

        // Set euler angles
        app.model.cam.camParent.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }

    public Vector3 SnappedForward
    { 
        get
        {
            return SnapDirectionToGridDirections(app.model.cam.camParent.forward);
        } 
    }
    public Vector3 SnappedRight
    {
        get
        {
            return SnapDirectionToGridDirections(app.model.cam.camParent.right);
        }
    }
    public Vector3 SnappedUp
    {
        get
        {
            return SnapDirectionToGridDirections(app.model.cam.camParent.up);
        }
    }

    private Vector3 SnapDirectionToGridDirections(Vector3 direciton)
    {
        float x = Mathf.Abs(direciton.x);
        float z = Mathf.Abs(direciton.z);

        if (x > z)
        {
            x = Mathf.Round(direciton.x);
            z = 0;
        }
        else if (z > x)
        {
            z = Mathf.Round(direciton.z);
            x = 0;
        }

        return new Vector3(x, 0, z);
    }
}
