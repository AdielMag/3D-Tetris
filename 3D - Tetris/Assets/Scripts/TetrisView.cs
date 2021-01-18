using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisView : TetrisElement
{
    public UserInput input;

    // MonoBehaviour functions
    private void Update()
    {
        input.HandleInput();
    }

    // Public functions
    public void Init()
    {
        input = GetComponentInChildren<UserInput>();
    }

}
