using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisView : TetrisElement
{
    [HideInInspector] public UserInput input;

    [Header("UI")]
    public UiTweener startWindow;
    public UiTweener lostWindow;
    public UiTweener inGameWindow;

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

    public void PressPlay()
    {
        startWindow.Disable();

        app.controller.StartGame();
    }
    public void PressBackToMenu()
    {
        lostWindow.Disable();

        startWindow.gameObject.SetActive(true);
    }
    public void PressExit()
    {
        app.ExitApplication();
    }
}
