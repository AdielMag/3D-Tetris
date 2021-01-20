using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisView : TetrisElement
{
    [HideInInspector] public UserInputView input;

    public enum InputType
    {
        Play,
        BackeToMenu,
        Exit,
        Pause,
        Unpause
    }

    // MonoBehaviour functions
    private void Update()
    {
        input.HandleInput();
    }

    // Public functions
    public void Init()
    {
        input = GetComponentInChildren<UserInputView>();
    }

    public void PressPlay()
    {
        app.controller.ui.OnInput(InputType.Play);
    }
    public void PressBackToMenu()
    {
        app.controller.ui.OnInput(InputType.BackeToMenu);
    }
    public void PressExit()
    {
        app.controller.ui.OnInput(InputType.Exit);
    }
    public void PressPause()
    {
        app.controller.ui.OnInput(InputType.Pause);
    }
    public void PressUnpause()
    {
        app.controller.ui.OnInput(InputType.Unpause);
    }
    public void EnterScoreName(string name) 
    {
        app.controller.ui.OnEnteredScoreName(name);
    }
}
