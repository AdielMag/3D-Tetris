using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallLocationIndicatorController : TetrisElement
{
    private Transform _cubesParent 
        { get { return app.model.fallLocationCubesParent; } }

    private float _alphaValue;

    // Public functions
    public void Init()
    {
        InstantiateCubes();

        // Get target material alpha and set it as base alpha
        _alphaValue = _cubesParent.GetChild(0)
            .GetComponent<MeshRenderer>().sharedMaterials[1].color.a;
    }
    public void SetNewIndicator()
    {
        // Enable indicator
        app.model.fallLocationCubesParent.gameObject.SetActive(true);

        // Enable \ disable based on needed cubes (current shape number of cubes)
        for (int i = 0; i < _cubesParent.childCount; i++)
        {
            bool activeState = i < app.model.currentShape.childCount;
            _cubesParent.GetChild(i).gameObject.SetActive(activeState);
        }

        UpdateIndicator();
    }
    public void UpdateIndicator()
    {
        UpdateBlocksPositions();
        UpdateColor();
    }
    public void HideIndicatorCubes()
    {
        // Disable indicator
        app.model.fallLocationCubesParent.gameObject.SetActive(false);
    }

    // Private functions
    private void InstantiateCubes()
    {
        int maxCubeCountInShapes = 0;

        // Iterate through all shapes and check thier block count
        foreach (ShapeModel shape in app.model.game.availableShapes)
        {
            int shapeBlockCount = shape.blocksPositions.Length;

            // Check if block count is bigger than current max block count
            if (shapeBlockCount > maxCubeCountInShapes)
                maxCubeCountInShapes = shapeBlockCount; // Set as max
        }

        // Create the max amount of cubes that the game will need
        for (int i = 0; i < maxCubeCountInShapes; i++)
        {
            // Get prefab
            GameObject cube =
                Instantiate(app.model.game.fallIndicatorCubePrefab, _cubesParent);

            // Set material
            cube.GetComponent<MeshRenderer>()
                .sharedMaterials[1] = app.model.game.fallIndicatorMat;
        }

        HideIndicatorCubes();
    }
    private void UpdateColor() 
    { 
        Color targetColor;

        // Get color from current shape
        targetColor = 
            app.model.currentShape.GetChild(0)
            .GetComponent<MeshRenderer>().materials[1].color;

        // Change color alpha
        targetColor.a = _alphaValue;

        // Set color
        app.model.game.fallIndicatorMat.color = targetColor;
    }
    private void UpdateBlocksPositions()
    {
        // Set the maximum distance from floor
        float minDisFrmFlr = app.model.game.boardHeight;

        // Iterate thorugh all cubes y pos to check whose the closest to the floor
        foreach (Transform cube in app.model.currentShape)
        {
            float targetY = cube.position.y;

            // Check if there's a cube under the shape cube that prevents
            // the shape from falling to the floor and store
            // its y position to position the indicator in the right place
            for (int y = Mathf.FloorToInt(targetY); y > 0; y--)
                if (!ValidPos(new Vector3(cube.position.x, y - 1, cube.position.z)))
                {
                    targetY -= y;
                    break;
                }

            // Check if this cube distance from the floor is the closest one
            if (targetY < minDisFrmFlr)
                minDisFrmFlr = targetY;
        }

        // Set indicator cubes positions
        for (int i=0;i< app.model.currentShape.childCount;i++)
        {
            Vector3 indicatorCubeTargetPos =
                app.model.currentShape.GetChild(i).position - Vector3.up * minDisFrmFlr;

            _cubesParent.GetChild(i).position = indicatorCubeTargetPos;
        }
    }
    private bool ValidPos(Vector3 blockPos)
    {
        int roundX = Mathf.RoundToInt(blockPos.x);
        int roundY = Mathf.RoundToInt(blockPos.y);
        int roundZ = Mathf.RoundToInt(blockPos.z);

        // Check if block is inside the grid
        // (When you create shapes there are some blocks that are a bit higher than the grid)
        if (roundY < app.model.game.boardHeight && roundY > -1)
            // Check if block position is free
            if (app.model.game.grid[roundX, roundY, roundZ] != null)
                return false;

        return true;
    }
}
