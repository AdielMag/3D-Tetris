using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisModel : TetrisElement
{
    [HideInInspector] public Transform currentShape;
    [HideInInspector] public List<Score> highScores;

    public int score;

    [Space]
    public Transform shapesParent;
    public Transform fallLocationCubesParent;
    public Transform shapePreviewParent;

    public GameModel game;
    public CameraModel cam;
    public UIModel ui;

    // Public functions
    public void Init()
    {
        cam.camParent = Camera.main.transform.parent;

        // Load data
        //game.shapes = Data.LoadShapes();
        game.SetAvailableShapes();
        game.cubePrefab = Resources.Load<GameObject>("Prefabs/CubePrefab");
        game.fallIndicatorCubePrefab =
            Resources.Load<GameObject>("Prefabs/FallIndicatorCubePrefab");
        game.grid = 
            new Transform[game.boardWidth, game.boardHeight, game.boardDepth];

        game.CreateCubeMats();

        game.cubeSquaresMatt =
            game.cubePrefab.GetComponent<MeshRenderer>().sharedMaterials[0];
        game.SetAvailableFillmats();

        game.fallIndicatorMat = game.fallIndicatorCubePrefab
            .GetComponent<MeshRenderer>().sharedMaterials[1];
    }
}

[System.Serializable]
public struct Score
{
    public string name;
    public int score;
}
