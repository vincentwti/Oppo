using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ball" && GameManager.Instance.IsServer)
        {
            ScoreGoal();
        }
    }

    private void ScoreGoal()
    {
        FootballController.Instance.matchDataList.Add(new MatchData { match = FootballController.Instance.scoreController.GetRound(), winnerId = (int)ScoreController.Player.player1 });
        FootballController.Instance.scoreController.AddScore(ScoreController.Player.player1);
        //FootballController.Instance.NextMatch();
        Debug.Log("clientdId : " + GameManager.Instance.GetClientId());
        Debug.Log("get player 1 score : " + FootballController.Instance.scoreController.GetPlayer1Score());

        FootballPlayerData data = new FootballPlayerData
        {
            isStrikerSelected = FootballController.Instance.clientsRoleDict.ContainsValue((int)FootballController.PlayerType.Striker),
            isGoalKeeperSelected = FootballController.Instance.clientsRoleDict.ContainsValue((int)FootballController.PlayerType.GoalKeeper),
            p1Score = FootballController.Instance.scoreController.GetPlayer1Score(),
            p2Score = FootballController.Instance.scoreController.GetPlayer2Score(),
            matchDataList = FootballController.Instance.matchDataList
        }; 
        EventManager.onFootballDataSent?.Invoke(data);

        //EventManager.onScoreUpdated?.Invoke(GameManager.Instance.GetClientId(), FootballController.Instance.scoreController.GetPlayer1Score(), FootballController.Instance.scoreController.GetPlayer2Score());
    }
}
