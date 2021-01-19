using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class UIModel
{
    public UiTweener startWindow;
    public UiTweener highScoresWindow;
    public UiTweener lostWindow;
    public UiTweener inGameWindow;

    public Transform highScoresParent;

    public Text inGameScore;
    public Text lostScore;
}
