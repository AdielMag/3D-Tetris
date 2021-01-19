using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : TetrisElement
{
    private Transform _cameraChild;

    private float _baseDistance;

    private float _yaw = 0.0f;
    private float _pitch = 0.0f;

    // Public functions
    public void Init()
    {
        _cameraChild = app.model.cam.camParent.GetChild(0);

        _yaw = app.model.cam.camParent.eulerAngles.y;
        _pitch = app.model.cam.camParent.eulerAngles.x;

        CalculateBaseDistance();

        SetCameraDistance();
    }
    public void OnPlayerInput(Vector2 inputDelta)
    {
        RotateCamera(inputDelta);
    }

    // Pass current camera directions through 'SnapDirectionToGridDirections'
    // and return snapped direction
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

    // Private functions
    private void RotateCamera(Vector2 inputDelta)
    {
        // Update yaw * pitch based on inputDelta
        _yaw += app.model.cam.camRotationSpeed * inputDelta.x;
        _pitch -= app.model.cam.camRotationSpeed * inputDelta.y;

        // Clamp pitch 
        _pitch = Mathf.Clamp(
            _pitch, app.model.cam.camPitchLimits.x, app.model.cam.camPitchLimits.y);

        // Set euler angles
        app.model.cam.camParent.eulerAngles
            = new Vector3(_pitch, _yaw, 0.0f);

        // Set camera distance
        SetCameraDistance();
    }
    private void CalculateBaseDistance()
    {
        // Change the base distance of the camera based on board dimensions

        int dimensionsMagnitude =
            app.model.game.boardDepth
            + app.model.game.boardWidth
            + app.model.game.boardHeight;

        // When the dimensions magnitucde is 15 the distance is -9
        float referenceDistance = -9;
        int referenceMagnitude = 15;

        // Divide current magnitude by refernce
        float targetDistanceMultiplier =
            (float)dimensionsMagnitude / referenceMagnitude;

        // Multiply refernce distance by targetDistanceMultiplier
        _baseDistance = referenceDistance * targetDistanceMultiplier;
    }
    private void SetCameraDistance()
    {
        // Use curve to change the distance based on pitch
        //  precentage from max limit (pitch/maxPitch)
        _cameraChild.transform.localPosition = new Vector3(0, 0,
            _baseDistance * app.model.cam.ditanceScale.Evaluate
            (_pitch / app.model.cam.camPitchLimits.y));
    }
    private Vector3 SnapDirectionToGridDirections(Vector3 direciton)
    {
        // Create absolute variables to check which axis is bigger
        float x = Mathf.Abs(direciton.x);
        float z = Mathf.Abs(direciton.z);

        // Check which axis is bigger
        if (x > z)
        {
            // Round target to make sure it wil be 1 or -1
            x = Mathf.Round(direciton.x);
            // Make sure other axis will not affect direction
            z = 0;
        }
        else if (z > x)
        {
            // Round target to make sure it wil be 1 or -1
            z = Mathf.Round(direciton.z);
            // Make sure other axis will not affect direction
            x = 0;
        }

        return new Vector3(x, 0, z);
    }
}
