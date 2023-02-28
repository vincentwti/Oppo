using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miss : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ball" && GameManager.Instance.IsServer)
        {
            Out();            
        }
    }

    private void Out()
    {
        if (!FootballController.Instance.CheckCurrentMatch())
        {
            FootballController.Instance.matchDataList.Add(new MatchData { match = FootballController.Instance.scoreController.GetRound(), winnerId = (int)ScoreController.Player.player2 });
            FootballController.Instance.scoreController.AddScore(ScoreController.Player.player2);
            FootballPlayerData data = new FootballPlayerData
            {
                isStrikerSelected = FootballController.Instance.clientsRoleDict.ContainsValue((int)FootballController.PlayerType.Striker),
                isGoalKeeperSelected = FootballController.Instance.clientsRoleDict.ContainsValue((int)FootballController.PlayerType.GoalKeeper),
                p1Score = FootballController.Instance.scoreController.GetPlayer1Score(),
                p2Score = FootballController.Instance.scoreController.GetPlayer2Score(),
                matchDataList = FootballController.Instance.matchDataList
            };
            EventManager.onFootballDataSent?.Invoke(data);
        }
    }
}
