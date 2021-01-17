using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisView : TetrisElement
{
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
