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

    private void UpdateColor() { }
    private void UpdateBlocksPositions() { }
}
