using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class UIModel
{
    [Header("Scene objects")]
    public UiTweener startWindow;
    public UiTweener highScoresWindow;
    public UiTweener scoreWindow;
    public UiTweener inGameWindow;
    public UiTweener pauseWindow;

    public Transform highScoresParent;

    public Text inGameScore;
    public Text lostScore;
}
