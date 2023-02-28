using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[Serializable]
public class PlayerHUD
{
    public int maxRound = 5;

    public TMP_Text nameText;
    public TMP_Text scoreText;
    public List<Image> scoreImageValueList;

    public void Reset(ScoreController controller)
    {
        scoreText.text = "0";
        for (int i = 0; i < scoreImageValueList.Count; i++)
        {
            scoreImageValueList[i].sprite = controller.defSprite;
        }
    }

    public void SetScore(ScoreController controller, bool success, int score, int round, Action onMatchEnd)
    {
        Debug.Log("match : " + round + " " + maxRound);
        scoreText.text = score.ToString();
        scoreImageValueList[round].sprite = success ? controller.scoreSprite : controller.missSprite;
        if (round >= maxRound - 1)
        {
            onMatchEnd?.Invoke();
        }
    }
}


public class ScoreController : MonoBehaviour
{
    public PlayerHUD player1Hud;
    public PlayerHUD player2Hud;
    public Sprite scoreSprite;
    public Sprite missSprite;
    public Sprite defSprite;

    public TimeController time;

    public enum Player
    {
        player1 = 1,
        player2 = 2
    }
    public Player player;

    private int player1Score;
    private int player2Score;
    private int round = 0;

    private void Start()
    {
        ResetMatch();
    }

    public int GetPlayer1Score()
    {
        return player1Score;
    }

    public int GetPlayer2Score()
    {
        return player2Score;
    }

    public void ResetMatch()
    {
        round = 0;
        ResetScore();
    }

    public void ResetScore()
    {
        player1Score = player2Score = 0;

        player1Hud.Reset(this);
        player2Hud.Reset(this);
    }

    public void NextRound()
    {
        round += 1;
    }

    public void AddScore(Player player)
    {
        if (GameManager.Instance.IsServer)
        {
            switch (player)
            {
                case Player.player1:
                    player1Score += 1;
                    player1Hud.SetScore(this, true, player1Score, round, () => StartCoroutine(FootballController.Instance.WaitForResetMatch()));
                    player2Hud.SetScore(this, false, player2Score, round, () => StartCoroutine(FootballController.Instance.WaitForResetMatch()));
                    break;
                case Player.player2:
                    player2Score += 1;
                    player1Hud.SetScore(this, false, player1Score, round, () => StartCoroutine(FootballController.Instance.WaitForResetMatch()));
                    player2Hud.SetScore(this, true, player2Score, round, () => StartCoroutine(FootballController.Instance.WaitForResetMatch()));
                    break;
            }
        }
    }

    public void SetScore(List<MatchData> matchDataList, int p1Score, int p2Score)
    {
        Debug.Log("SetScore : " + p1Score + " " + p2Score);
        player1Score = p1Score;
        player2Score = p2Score;
        for (int i = 0; i < matchDataList.Count; i++)
        {
            player1Hud.SetScore(this, matchDataList[i].winnerId == (int)Player.player1, player1Score, i, null);
            player2Hud.SetScore(this, matchDataList[i].winnerId == (int)Player.player2, player2Score, i, null);
        }
    }

    public int GetRound()
    {
        return round;
    }
}
