using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisApplication : MonoBehaviour
{
    #region Singleton & Awake
    public static TetrisApplication instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    public TetrisModel model;
    public TetrisView view;
    public TetrisController controller;

    private void Start()
    {
        model.Init();
        view.Init();
        controller.Init();
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}
