using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisController : TetrisElement
{
    public  CameraController cam;

    private FallLocationIndicatorController _fallLocIndic;

    // MonoBehaviour functions
    private float _previousUpdateTime;
    private void Update()
    {
        float timeToWait = app.view.input.fallFaster ?
            app.model.game.timeBetweenUpdates / 10 : app.model.game.timeBetweenUpdates;

        if (Time.time - _previousUpdateTime > timeToWait)
        {
            _previousUpdateTime = Time.time;
            UpdateGame();
        }
    }

    // Public functions
    public void Init()
    {
        app.transform.position +=
            Vector3.forward * (app.model.game.boardWidth / 2)
            + Vector3.right * (app.model.game.boardDepth / 2)
            + Vector3.up * (app.model.game.boardHeight / 2);

        cam = GetComponentInChildren<CameraController>();
        cam.Init();

        _fallLocIndic = GetComponentInChildren<FallLocationIndicatorController>();
        _fallLocIndic.Init();

        CreateNewShape("L");
    }
    public void OnMovementInput(Vector2 input)
    {
        MoveCurrentShape(input);
    }
    public void OnRotationInput(UserInput.RotationAxis axis)
    {
        Vector3 targetAxis;
        switch (axis)
        {
            default:
                targetAxis = cam.SnappedRight;
                break;
            case UserInput.RotationAxis.Up:
                targetAxis = -Vector3.up;
                break;
            case UserInput.RotationAxis.Forward:
                targetAxis = -cam.SnappedForward;
                break;
        }

        RotateCurrentShape(targetAxis);
    }


    // Private functions
    private void CreateNewShape(string name)
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


        Vector3 initialStartingPos;

        // Set initial starting pos
        initialStartingPos = new Vector3(
            app.model.game.boardWidth / 2,
            app.model.game.boardHeight,
            app.model.game.boardDepth / 2);

        // Check if the board dimensions are even
        if (app.model.game.boardWidth % 2 == 0)
            initialStartingPos += Vector3.forward * .5f;
        if (app.model.game.boardDepth % 2 == 0)
            initialStartingPos -= Vector3.right * .5f;
        if (app.model.game.boardHeight % 2 == 0)
            initialStartingPos += Vector3.up * .5f;

        shape.transform.position = initialStartingPos;
        // Set shape parent
        shape.transform.SetParent(app.model.game.shapesParent, true);

        app.model.game.currentShape = shape;

        // Set new indicator
        _fallLocIndic.SetNewIndicator();
        // Update fall location indicator
        _fallLocIndic.UpdateIndicator();
    }

    private void UpdateGame() 
    {
        // Move shape down
        app.model.game.currentShape.transform.position -= Vector3.up;

        // Update fall location indicator
        _fallLocIndic.UpdateIndicator();

        if (!ValidBlocksPosition()) // Check if its not valid
        {
            // Move shape up
            app.model.game.currentShape.transform.position += Vector3.up;

            ShapeColided();
        }
    }
    private void ShapeColided()
    {
        bool reachedTop = AddCurrentShapeBlocksToGrid();

        if (reachedTop)
            GameLost();
        else
        {
            CheckForRows();

            // Create new shape
            CreateNewShape("L");
        }
    }
    
    private void CheckForRows()
    {
        int maxHeightToCheck = 0;
        int minHeightToCheck = app.model.game.boardHeight;

        for (int i = 0;i< app.model.game.currentShape.childCount; i++)
        {
            float yPos = app.model.game.currentShape.GetChild(i).position.y;

            if (yPos > maxHeightToCheck)
                maxHeightToCheck = Mathf.FloorToInt(yPos);

            if(yPos< minHeightToCheck)
                minHeightToCheck = Mathf.FloorToInt(yPos);
        }

        // Loop through max grid height to min (the shape cube max & min heights)
        for (int height = maxHeightToCheck; height >= minHeightToCheck; height--)
            if (RowIsFull(height))
            {
                // Remove row
                RemoveRow(height);
                // Move top rows down
                MoveRowsDown(height);
            }
    }
    private void RemoveRow(int rowHeight) 
    {
        for (int width = 0; width < app.model.game.boardWidth; width++)
            for (int depth = 0; depth < app.model.game.boardDepth; depth++)
            {
                // Disable object - will remove all shpes when lost
                // (dont delete now it will affect performance)
                app.model.game.grid[width, rowHeight, depth].gameObject.SetActive(false);
                app.model.game.grid[width, rowHeight, depth] = null;
            }
    }
    private void MoveRowsDown(int minRow)
    {
        for (int height = minRow ; height < app.model.game.boardHeight; height++)
            for (int width = 0; width < app.model.game.boardWidth; width++)
                for (int depth = 0; depth < app.model.game.boardDepth; depth++)
                {
                    // Grab cube transform
                    Transform cube = app.model.game.grid[width, height, depth];
                    if (cube != null)
                    {
                        // Remove cube from array
                        app.model.game.grid[width, height, depth] = null;

                        // Place the cube one row below
                        app.model.game.grid[width, height - 1, depth] = cube;

                        // Move down the cube
                        cube.position -= Vector3.up;
                    }
                }
    }

    private void GameLost() { Debug.Log("Lost!"); }

    private void MoveCurrentShape(Vector2 input)
    {
        app.model.game.currentShape.transform.position +=
            cam.SnappedRight * input.x + cam.SnappedForward * input.y;

        if(!ValidBlocksPosition())
            app.model.game.currentShape.transform.position -=
            cam.SnappedRight * input.x + cam.SnappedForward * input.y;

        // Update fall location indicator
        _fallLocIndic.UpdateIndicator();
    }
    private void RotateCurrentShape(Vector3 actualAxis)
    {
        app.model.game.currentShape.transform.Rotate(actualAxis, 90,Space.World);
        if (!ValidBlocksPosition())
            app.model.game.currentShape.transform.Rotate(actualAxis, -90, Space.World);

        // Update fall location indicator
        _fallLocIndic.UpdateIndicator();
    }

    private bool RowIsFull(int rowHeight)
    {
        // Loop thourgh entire row and check if has empty variable
        for (int width = 0; width < app.model.game.boardWidth; width++)
            for (int depth = 0; depth < app.model.game.boardDepth; depth++)
                if (app.model.game.grid[width, rowHeight, depth] == null)
                    return false;

        return true;
    }
    private bool AddCurrentShapeBlocksToGrid()
    {
        foreach (Transform block in app.model.game.currentShape)
        {
            int x = (int)block.transform.position.x;
            int y = (int)block.transform.position.y;
            int z = (int)block.transform.position.z;

            if (y >= app.model.game.boardHeight)
                return true;
            else // Not necessary but helps readability
                app.model.game.grid[x, y, z] = block.transform;
        }
        return false;
    }
    private bool ValidBlocksPosition() 
    {
        // Validate if current shape blocks positions are allowed

        // Itertae
        foreach (Transform block in app.model.game.currentShape)
        {
            int roundX = Mathf.FloorToInt(block.position.x);
            int roundY = Mathf.FloorToInt(block.position.y);
            int roundZ = Mathf.FloorToInt(block.position.z);

            // Check if its withing width dimensions
            if (roundX >= app.model.game.boardWidth || roundX < 0)
                return false;
            // Check if its withing depth dimensions
            if (roundZ >= app.model.game.boardDepth || roundZ < 0)
                return false;
            // Check if it touched the ground
            if (roundY < 0)
                return false;

            // Check if block is inside the grid
            // (When you create shapes there are some blocks that are a bit higher than the grid)
            if (roundY < app.model.game.boardHeight)
                // Check if block position is free
                if (app.model.game.grid[roundX, roundY, roundZ] != null)
                    return false;
        }

        return true;
    }
}
