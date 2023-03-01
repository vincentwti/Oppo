using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Striker : Player
{
    public Transform striker;
    public Ball ball;
    public SwipeController swipeController;

    public Animator animator;
    private Vector3 shootPosition;

    private void Start()
    {
        animator = GetComponent<Animator>(); 
        swipeController.onSwipeCompleted += PlayShootAnimation;
    }

    private void OnDestroy()
    {
        swipeController.onSwipeCompleted -= PlayShootAnimation;
    }

    public void PlayShootAnimation(Vector3 shootPosition)
    {
        if (FootballController.Instance.playerType == FootballController.PlayerType.Striker)
        {
            EventManager.onShootAnimationPlayed?.Invoke(GameManager.Instance.GetClientId());
        }
        FootballController.Instance.swipeController.CanSwipe(false);
        FootballController.Instance.scoreController.time.Pause(true);
        this.shootPosition = shootPosition;
        animator.SetTrigger("Shoot");
    }

    public void PlayShootAnimation()
    {
        Debug.Log("animator : " + animator);
        animator.SetTrigger("Shoot");
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayShootAnimation(shootPosition);
        }
    }

    protected override void DoAction()
    {
        Shoot();
    }

    protected override bool CheckIdle()
    {
        return base.CheckIdle();
    }

    public void Shoot()
    {
        ball.Shoot(shootPosition);
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.blue;
        GUI.Label(new Rect(60, 300, 200, 60), FootballController.Instance.playerType.ToString(), style);
    }
}
