using System;
using System.Collections;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public enum GameStatus
    {
        PREPARING,
        PLAY,
        FAIL,
        FINISH
    }
    public GameStatus gameStatus;
    public float currentLevelTime;

    void Update()
    {
        DecreaseTimer();
        CheckIfGameEnd();
    }
    private void DecreaseTimer()
    {
        // MainLevel tutorial rows carry limittime=[0], which means the source
        // game has no countdown for that stage. Keep zero as an inactive timer
        // instead of immediately showing the time-up popup every frame.
        if (GameIsPlaying() && currentLevelTime > 0 && !ReplicaSourceTools.Shuffling && !BoosterManager.Instance.timeFreezed)
        {
            currentLevelTime -= Time.deltaTime;
            TimeSpan duration = TimeSpan.FromSeconds(Mathf.Max(0, currentLevelTime));
            UIManager.Instance.SetTimerText(duration);
            if (currentLevelTime <= 0)
            {
                UIManager.Instance.ShowTimeIsUpPopUp();
            }
        }
    }
    private void CheckIfGameEnd()
    {
        if (GameIsPlaying() && LevelGenerator.Instance.objectsOnScene.Count == 0 &&
            (!InputController.Instance || InputController.Instance.CanEvaluateWin))
        {
            UIManager.Instance.ShowNextlevelPopUp();
        }
    }
    public IEnumerator DelayedGameStart(float t)
    {
        yield return new WaitForSeconds(t);
        gameStatus = GameStatus.PLAY;
        BoosterManager.Instance.BoostersInteractibility(true);
    }
    public bool GameIsPlaying()
    {
        return gameStatus == GameStatus.PLAY? true:false;
    }
}
