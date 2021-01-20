using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : TetrisElement
{
    public void UpdateInGameScore()
    {
        app.model.ui.inGameScore.text = 
            "Score: " + app.model.currentScore.ToString();
    }

    public void UpdateHighScoresWindow()
    {
        // Update high scores window
        for (int i = 0; i < app.model.highScores.Count; i++)
        {
            string target;

            target = (i + 1).ToString() + ". " + app.model.highScores[i].name
                + ": " + app.model.highScores[i].score;

            Text highScoreTxt =
                app.model.ui.highScoresParent.GetChild(i).GetComponent<Text>();

            highScoreTxt.text = target;
        }
    }

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
        app.controller.OnRegisterdHighScore(_name, app.model.currentScore);
        app.model.ui.highScoresWindow.Show();
        app.model.ui.scoreWindow.Disable();
    }

    public void OnGameLost()
    {
        app.model.ui.inGameWindow.Disable();

        app.model.ui.scoreWindow.Show();

        // Update score window score text
        app.model.ui.lostScore.text =
            "Score: " + app.model.currentScore.ToString();
    }
}
