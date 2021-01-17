using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisController : TetrisElement
{
    [HideInInspector] public CameraController cam;
    public void Init()
    {
        cam = GetComponentInChildren<CameraController>();
        cam.Init();

        app.view.currentShape = CreateShape("L");
    }

    public ShapeView CreateShape(string name) 
    {
        // Create new shape
        ShapeView shape = new GameObject(name).AddComponent<ShapeView>();

        // Get shape data
        ShapeModel shapeData = app.model.game.shapes[name];

        for(int i= 0;i< shapeData.blocksPositions.Length; i++)
        {
            // Create cube
            Transform cube = Instantiate(app.model.game.cubePrefab, shape.transform).transform;

            // Place cube based on data
            cube.localPosition = shapeData.blocksPositions[i];
        }

        shape.transform.position = app.view.shapeCreationPos.position;

        // Set shape parent
        //shape.transform.SetParent(app.view.shapesParent, true);

        return shape;
    }

    public void MoveShape(Vector2 input)
    {
        app.view.currentShape.transform.position +=
            cam.SnappedRight * input.x + cam.SnappedForward * input.y;
    }

    public void RotateShape(Vector3 axis)
    {
        app.view.currentShape.transform.Rotate(axis, 90,Space.World);
    }
}
