using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TetrisModel : TetrisElement
{
    [HideInInspector] public Transform currentShape;

    public Transform shapesParent;
    public Transform fallLocationCubesParent;

    public GameModel game;
    public CameraModel cam;

    // Public functions
    public void Init()
    {
        cam.camParent = Camera.main.transform.parent;

        // Load data
        game.shapes = game.LoadShapes();
        game.cubePrefab = Resources.Load<GameObject>("Prefabs/CubePrefab");
        game.fallIndicatorCubePrefab =
            Resources.Load<GameObject>("Prefabs/FallIndicatorCubePrefab");
        game.grid = 
            new Transform[game.boardWidth, game.boardHeight, game.boardDepth];

        game.CreateCubeMats();

        game.cubeSquaresMatt =
            game.cubePrefab.GetComponent<MeshRenderer>().sharedMaterials[0];
        game.availableFillMats = game.cubeFillMats.ToList<Material>();

        game.fallIndicatorMat = game.fallIndicatorCubePrefab
            .GetComponent<MeshRenderer>().sharedMaterials[1];
    }
}
