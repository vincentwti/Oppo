using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public enum PlayerType
    {
        Human,
        AI
    }
    public PlayerType playerType;

    private float thinkTimeMin = 1f;
    private float thinkTimeMax = 4f;
    private float thinkTime;
    private bool isThinking = false;
    private float elapsedThinkingTime = 0f;

    private float checkIdleTime = 30f;
    protected float accelerometerTolerance = 0.1f;
    protected bool pauseAi = false;
    private float elapsedCheckIdleTime = 0f;

    public Image flagImage;
    public Animator animator;

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
                    isThinking = false;
                    elapsedThinkingTime += Time.deltaTime;
                    if (elapsedThinkingTime >= thinkTime)
                    {
                        DoAction();
                        elapsedThinkingTime = 0;
                    }
                }
            }
            else if (FootballController.Instance.playerType == FootballController.PlayerType.GoalKeeper)
            {
                DoAction();
            }
        }

        CheckIdle();
    }

    protected virtual void DoAction()
    {
        
    }

    protected void ResetCheckIdle()
    {
        elapsedCheckIdleTime = 0;
        playerType = PlayerType.Human;
    }

    protected virtual bool CheckIdle()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ResetCheckIdle();
            flagImage.color = Color.blue;
            return false;
        }
        elapsedCheckIdleTime += Time.deltaTime;
        if (elapsedCheckIdleTime >= checkIdleTime)
        {
            playerType = PlayerType.AI;
            flagImage.color = Color.red;
        }

        return true;
    }

    public virtual void PlayWinAnimation()
    {
        animator.SetBool("Win", true);
        pauseAi = true;
    }

    public virtual void PlayLoseAnimation()
    {
        animator.SetBool("Lose", true);
        pauseAi = true;
    }

    public virtual void PlayIdleAnimation()
    {
        animator.SetBool("Win", false);
        animator.SetBool("Lose", false);
        pauseAi = false;
    }
}
