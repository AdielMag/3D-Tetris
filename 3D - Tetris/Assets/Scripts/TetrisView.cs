using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisView : TetrisElement
{
    public Transform currentShape;

    [Space]
    public Transform shapesParent;

    public UserInput input;

    public void Init()
    {
        input = GetComponentInChildren<UserInput>();
    }

    private void Update()
    {
        input.HandleInput();
    }
}
