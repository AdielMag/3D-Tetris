using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : TetrisElement
{
    public void OnInput(TetrisView.InputType type)
    {
        switch (type)
        {
            case TetrisView.InputType.Play:
                app.model.ui.startWindow.Disable();
                app.model.ui.inGameWindow.Show();
                app.controller.StartGame();
                break;
            case TetrisView.InputType.BackeToMenu:
                app.model.ui.inGameWindow.Disable();
                app.model.ui.scoreWindow.Disable();
                app.model.ui.pauseWindow.Disable();
                app.model.ui.startWindow.Show();
                break;
            case TetrisView.InputType.Exit:
                app.ExitApplication();
                break;
            case TetrisView.InputType.Pause:
                app.controller.enabled = false;
                app.model.ui.pauseWindow.Show();
                break;
            case TetrisView.InputType.Unpause:
                app.controller.enabled = true;
                app.model.ui.pauseWindow.Disable();
                break;
        }
    }
    public void OnEnteredScoreName(string _name)
    {
        app.controller.RegisterHighScore(_name, app.model.score);
        app.model.ui.highScoresWindow.Show();
        app.model.ui.scoreWindow.Disable();
    }


}
