using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInputView : TetrisElement
{
    public enum RotationAxis { Right, Up, Forward }

    // Fall faster flag
    [HideInInspector] public bool fallFaster;

    // Public functions
    public void HandleInput()
    {
        // Camera
        if (Input.GetMouseButton(0))
            app.controller.cam.OnPlayerInput(CameraRotationInput());

        // Shape movement
        if (Input.GetButtonDown("Horizontal"))
        {
            Vector2 directionInput =
                new Vector2(Mathf.CeilToInt(Input.GetAxisRaw("Horizontal")), 0);
            app.controller.OnMovementInput(directionInput);
        }
        if (Input.GetButtonDown("Vertical"))
        {
            Vector2 directionInput =
                new Vector2(0, Mathf.CeilToInt(Input.GetAxisRaw("Vertical")));
            app.controller.OnMovementInput(directionInput);
        }

        // Shape rotation
        if (Input.GetButtonDown("RotateRightAxis"))
            app.controller.OnRotationInput(RotationAxis.Right);
        if (Input.GetButtonDown("RotateUpAxis"))
            app.controller.OnRotationInput(RotationAxis.Up);
        if (Input.GetButtonDown("RotateForwardAxis"))
            app.controller.OnRotationInput(RotationAxis.Forward);

        // Fall faster flag
        fallFaster = Input.GetButton("Space");
    }

    // Private functions
    private Vector2 CameraRotationInput()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        return new Vector2(x, y);
    }
}
