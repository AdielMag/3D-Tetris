using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisView : TetrisElement
{
    [HideInInspector] public UserInputView input;

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
        app.model.ui.startWindow.Disable();

        app.model.ui.inGameWindow.Show();

        app.controller.StartGame();
    }
    public void PressBackToMenu()
    {
        app.model.ui.lostWindow.Disable();

        app.model.ui.startWindow.Show();
    }
    public void PressExit()
    {
        app.ExitApplication();
    }
    public void EnterScoreName(string name) 
    {
        app.controller.RegisterHighScore(name, app.model.score);

        app.model.ui.highScoresWindow.Show();

        app.model.ui.lostWindow.Disable();
    }
}
