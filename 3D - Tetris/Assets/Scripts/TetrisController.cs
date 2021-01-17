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

    private float _previousUpdateTime;
    private void Update()
    {
        if (Time.time - _previousUpdateTime > app.model.game.timeBetweenUpdates)
        {
            _previousUpdateTime = Time.time;
            UpdateGame();
        }
    }

    public Transform CreateShape(string name)
    {
        // Create new shape
        Transform shape = new GameObject(name).transform;

        // Get shape data
        ShapeModel shapeData = app.model.game.shapes[name];

        for (int i = 0; i < shapeData.blocksPositions.Length; i++)
        {
            // Create cube
            Transform cube = Instantiate(app.model.game.cubePrefab, shape.transform).transform;

            // Place cube based on data
            cube.localPosition = shapeData.blocksPositions[i];
        }

        // Check if the board dimensions are even
        if (app.model.game.boardWidth % 2 == 0)
            shape.transform.position += Vector3.forward * .5f; // Correct starting position
        if (app.model.game.boardDepth % 2 == 0)
            shape.transform.position -= Vector3.right * .5f; // Correct starting position


        // Set shape Y position
        shape.transform.position +=
             Vector3.up * (((float)app.model.game.boardHeight / 2) + .5f);

        // Set shape parent
        //shape.transform.SetParent(app.view.shapesParent, true);

        return shape;
    }

    private void UpdateGame() 
    {
        // Move shape down
        app.view.currentShape.transform.position -= Vector3.up;
        if (!Valid()) // Check if its not valid
        {
            // Move shape up
            app.view.currentShape.transform.position += Vector3.up;

            AddCurrentShapeBlocksToGrid();

            // Create new shape
            app.view.currentShape = CreateShape("L");
        }
    }

    public void AddCurrentShapeBlocksToGrid()
    {
        foreach (Transform block in app.view.currentShape)
        {
            int x = (int)block.transform.position.x + Mathf.FloorToInt(app.model.game.boardWidth);
            int y = (int)block.transform.position.y + Mathf.FloorToInt(app.model.game.boardHeight);
            int z = (int)block.transform.position.z + Mathf.FloorToInt(app.model.game.boardDepth);

            app.model.game.grid[x, y, z] = block.transform;
        }
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
            int roundX = Mathf.RoundToInt(block.position.x);
            int roundY = Mathf.RoundToInt(block.position.y);
            int roundZ = Mathf.RoundToInt(block.position.z);

            // Check if its withing width dimensions
            if (roundX > app.model.game.boardWidth / 2 || roundX < -app.model.game.boardWidth / 2)
                return false;
            // Check if its withing depth dimensions
            if (roundZ > app.model.game.boardDepth / 2 || roundZ < -app.model.game.boardDepth / 2)
                return false;
            // Check if it touched the ground
            if (roundY < -app.model.game.boardHeight / 2)
                return false;

            // Add offset to make variables positive
            roundX += Mathf.FloorToInt(app.model.game.boardWidth);
            roundY += Mathf.FloorToInt(app.model.game.boardHeight);
            roundZ += Mathf.FloorToInt(app.model.game.boardDepth);

            if (app.model.game.grid[roundX, roundY, roundZ] != null)
                return false;
        }

        return true;
    }

}
