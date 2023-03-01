using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerType
    {
        Human,
        AI
    }
    public PlayerType playerType;

    private float thinkTimeMin = 1f;
    private float thinkTimeMax = 3f;
    private float thinkTime;
    private bool isThinking = false;
    private float elapsedThinkingTime = 0f;

    private float checkIdleTime = 30f;
    protected float accelerometerTolerance = 0.1f;
    private float elapsedCheckIdleTime = 0f;

    private void Start()
    {
        playerType = PlayerType.Human;

    }

    protected virtual void Update()
    {
        if (playerType == PlayerType.AI)
        {
            if (FootballController.Instance.playerType == FootballController.PlayerType.Striker)
            {
                if (!FootballController.Instance.scoreController.time.GetIsPaused())
                {
                    isThinking = true;
                    thinkTime = Random.Range(thinkTimeMin, thinkTimeMax);
                }

                if (isThinking)
                {
                    elapsedThinkingTime += Time.deltaTime;
                    if (elapsedThinkingTime >= thinkTime)
                    {
                        DoAction();
                    }
                }
            }
            else if (FootballController.Instance.playerType == FootballController.PlayerType.GoalKeeper)
            {
                DoAction();
            }
        }

        if (CheckIdle() == false)
        {
            playerType = PlayerType.Human;
        }
    }

    protected virtual void DoAction()
    {
        
    }

    protected void ResetCheckIdle()
    {
        elapsedCheckIdleTime = 0;
    }

    protected virtual bool CheckIdle()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ResetCheckIdle();
            return false;
        }
        elapsedCheckIdleTime += Time.deltaTime;
        if (elapsedCheckIdleTime >= checkIdleTime)
        {
            playerType = PlayerType.AI;
        }

        return true;
    }
}
