using System;
using TMPro;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public TextMeshProUGUI levelText; 
    public TextMeshProUGUI timerText;
    public GameObject OutOfBoxesPopUp;
    public GameObject TimeIsUpPopUp;
    public GameObject NextLevelPopUp;

    #region UI Text
    public void SetLevelText(string level)
    {
        levelText.text = level;
    }
    public void SetTimerText(TimeSpan span)
    {
        timerText.text = string.Format("{0:D2}:{1:D2}", span.Minutes + (span.Hours * 60), span.Seconds);
    }
    #endregion

    #region PopUp
    public void ShowOutOfBoxesPopUp()
    {
        if (ReplicaFlow.Instance != null) { ReplicaFlow.Instance.HandleResult(false); return; }
        GameManager.Instance.gameStatus = GameManager.GameStatus.FAIL;
        OutOfBoxesPopUp.SetActive(true);
    }
    public void ShowTimeIsUpPopUp()
    {
        if (ReplicaFlow.Instance != null) { ReplicaFlow.Instance.HandleResult(false); return; }
        GameManager.Instance.gameStatus = GameManager.GameStatus.FAIL;
        TimeIsUpPopUp.SetActive(true);
    }
    public void ShowNextlevelPopUp()
    {
        if (ReplicaFlow.Instance != null) { ReplicaFlow.Instance.HandleResult(true); return; }
        GameManager.Instance.gameStatus = GameManager.GameStatus.FINISH;
        NextLevelPopUp.SetActive(true);
    }
    #endregion

    #region Button OnClick Events
    public void OnClickRestart()
    {
        OutOfBoxesPopUp.SetActive(false);
        TimeIsUpPopUp.SetActive(false);
        LevelGenerator.Instance.RestartLevel();
    }
    public void OnClickNext()
    {
        NextLevelPopUp.SetActive(false);
        PlayerPrefs.SetInt("currentLevel", PlayerPrefs.GetInt("currentLevel", 1)+1);
        LevelGenerator.Instance.RestartLevel();
    }
    #endregion
}
