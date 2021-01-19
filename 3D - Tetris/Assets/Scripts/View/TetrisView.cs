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
        app.model.ui.inGameWindow.Disable();
        app.model.ui.scoreWindow.Disable();
        app.model.ui.pauseWindow.Disable();

        app.model.ui.startWindow.Show();
    }
    public void PressExit()
    {
        app.ExitApplication();
    }
    public void PressPause()
    {
        app.controller.enabled = false;
        app.model.ui.pauseWindow.Show();
    }
    public void PressUnpause()
    {
        app.controller.enabled = true;
        app.model.ui.pauseWindow.Disable();
    }
    public void EnterScoreName(string name) 
    {
        app.controller.RegisterHighScore(name, app.model.score);

        app.model.ui.highScoresWindow.Show();

        app.model.ui.scoreWindow.Disable();
    }
}
