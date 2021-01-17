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

    public Transform CreateShape(string name) 
    {
        // Create new shape
        Transform shape = new GameObject(name).transform;

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

        if(!Valid())
            app.view.currentShape.transform.position -=
            cam.SnappedRight * input.x + cam.SnappedForward * input.y;
    }

    public void RotateShape(Vector3 axis)
    {
        app.view.currentShape.transform.Rotate(axis, 90,Space.World);
        if (!Valid())
            app.view.currentShape.transform.Rotate(axis, -90, Space.World);

    }

    bool Valid() 
    {
        foreach (Transform block in app.view.currentShape)
        {
            if (block.position.x > app.model.game.boardWidth / 2
                || block.position.x < -app.model.game.boardWidth / 2)
                return false;
            else if (block.position.z > app.model.game.boardDepth / 2
                || block.position.z < -app.model.game.boardDepth / 2)
                return false;
        }

        return true;
    }
}
