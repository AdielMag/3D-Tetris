using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisView : TetrisElement
{
    public Transform currentShape;

    [Space]
    public Transform shapesParent;

    public void Init()
    {
        
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            app.controller.cam.RotateCamera(CameraRotationInput());
        }

        if (Input.GetButtonDown("Horizontal")) {
            Vector2 directionInput =
                new Vector2(Mathf.CeilToInt(Input.GetAxisRaw("Horizontal")), 0);
            app.controller.MoveShape(directionInput);
        }
        if (Input.GetButtonDown("Vertical"))
        {
            Vector2 directionInput =
                new Vector2(0,Mathf.CeilToInt(Input.GetAxisRaw("Vertical")));
            app.controller.MoveShape(directionInput);
        }

        if (Input.GetButtonDown("RotateRightAxis")) 
            app.controller.RotateShape(app.controller.cam.SnappedRight);

        if (Input.GetButtonDown("RotateUpAxis"))
            app.controller.RotateShape(-Vector3.up);

        if (Input.GetButtonDown("RotateForwardAxis"))
            app.controller.RotateShape(-app.controller.cam.SnappedForward);
    }

    Vector2 MovementInput()
    {
        float x = Mathf.Clamp(Mathf.Ceil(Input.GetAxisRaw("Horizontal")), -1, 1);
        float y = Mathf.Clamp(Mathf.Ceil(Input.GetAxisRaw("Vertical")), -1, 1);

        return new Vector2(x, y);
    }

    Vector2 CameraRotationInput() 
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        return new Vector2(x, y);
    }
}
