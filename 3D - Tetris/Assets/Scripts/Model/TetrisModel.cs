using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisModel : TetrisElement
{
    public int currentScore;

    [Header("Scene objects")]
    public Transform shapesParent;
    public Transform fallLocationCubesParent;
    public Transform shapePreviewParent;

    [Space]
    public GameModel game;
    public CameraModel cam;
    public UIModel ui;

    [HideInInspector] public Transform currentShape;

    [HideInInspector] public List<Score> highScores;

    // Public functions
    public void Init()
    {
        cam.camParent = Camera.main.transform.parent;

        game.Init();
    }
}
