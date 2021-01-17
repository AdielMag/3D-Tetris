using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisModel : TetrisElement
{
    public GameModel game;
    public CameraModel cam;

    public void Init()
    {
        cam.camParent = Camera.main.transform.parent;

        // Load data
        game.shapes = game.LoadShapes();
        game.cubePrefab = Resources.Load<GameObject>("CubePrefab");
    }
}
