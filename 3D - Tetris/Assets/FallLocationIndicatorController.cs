using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallLocationIndicatorController : TetrisElement
{
    private Transform _cubesParent { get { return app.model.game.fallLocationCubesParent; } }

    public void Init()
    {
        InstantiateCubes();
    }

    private void InstantiateCubes()
    {
        int maxCubeCountInShapes = 0;

        // Iterate through all shapes and check thier block count
        foreach (KeyValuePair<string, ShapeModel> shape in app.model.game.shapes)
        {
            int shapeBlockCount = shape.Value.blocksPositions.Length;

            // Check if block count is bigger than current max block count
            if (shapeBlockCount > maxCubeCountInShapes)
                maxCubeCountInShapes = shapeBlockCount; // Set as max
        }

        // Create the max amount of cubes that the game will need
        for (int i = 0; i < maxCubeCountInShapes; i++)
            Instantiate(app.model.game.fallIndicatorCubePrefab, _cubesParent);
    }

    public void SetNewIndicator() 
    {
        for (int i = 0; i < _cubesParent.childCount; i++)
        {
            bool activeState = i < app.model.game.currentShape.childCount;
            _cubesParent.GetChild(i).gameObject.SetActive(activeState);
        }
    }

    public void UpdateIndicator()
    {
        UpdateBlocksPositions();
    }

    private void UpdateColor() { }
    private void UpdateBlocksPositions()
    {
        // Set the maximum distance from floor
        float minDisFrmFlr = app.model.game.boardHeight;

        // Iterate thorugh all cubes y pos to check whose the closest to the floor
        foreach (Transform cube in app.model.game.currentShape)
        {
            float targetY = cube.position.y;

            // Check if there's a cube under the shape cube that prevents
            // the shape from falling to the floor and store
            // its y position to position the indicator in the right place
            for (int y = Mathf.FloorToInt(targetY); y > 0; y--)
                if (!Valid(new Vector3(cube.position.x, y - 1, cube.position.z)))
                {
                    targetY -= y;
                    break;
                }

            // Check if this cube distance from the floor is the closest one
            if (targetY < minDisFrmFlr)
                minDisFrmFlr = targetY;
        }

        // Check if has blocks that prevent the shape from going down
        // and update the indicator accordingly


        // Add offset if boardHeigt is even
        if (app.model.game.boardHeight % 2 == 0)
            minDisFrmFlr -= .5f;

        // Set indicator cubes positions
        for (int i=0;i< app.model.game.currentShape.childCount;i++)
        {
            Vector3 indicatorCubeTargetPos =
                app.model.game.currentShape.GetChild(i).position - Vector3.up * minDisFrmFlr;

            _cubesParent.GetChild(i).position = indicatorCubeTargetPos;
        }
    }

    bool Valid(Vector3 blockPos)
    {
        int roundX = Mathf.FloorToInt(blockPos.x);
        int roundY = Mathf.FloorToInt(blockPos.y);
        int roundZ = Mathf.FloorToInt(blockPos.z);

        // Check if block is inside the grid
        // (When you create shapes there are some blocks that are a bit higher than the grid)
        if (roundY < app.model.game.boardHeight && roundY >= 0)
            // Check if block position is free
            if (app.model.game.grid[roundX, roundY, roundZ] != null)
                return false;


        return true;
    }
}
