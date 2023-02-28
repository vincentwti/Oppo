using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalKeeper : MonoBehaviour
{
    private Vector3 acceleration;

    public Transform target;
    public Transform goalKeeper;
    public Transform hands;
    public float minX, maxX, minY, maxY;
    public float minAngle = -60f;
    public float maxAngle = 60f;
    public float smoothness = 5f;
    public float handSpeed = 2f;

    private Vector3 calibrationOffset;

    private void Start()
    {
        Debug.LogWarning("Start");
        EventManager.onCalibrate += Calibrate;
    }

    private void OnDestroy()
    {
        EventManager.onCalibrate -= Calibrate;
    }

    // Update is called once per frame
    private void Update()
    {
        if (FootballController.Instance.playerType == FootballController.PlayerType.GoalKeeper)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                acceleration.x = Input.acceleration.x - calibrationOffset.x;
                acceleration.y = Input.acceleration.y - calibrationOffset.y;
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                acceleration.x = -Input.acceleration.y - calibrationOffset.x;
                acceleration.y = Input.acceleration.z - calibrationOffset.z;
            }
        }
    }

    private void LateUpdate()
    {
        if (!GameManager.Instance.IsServer && FootballController.Instance.playerType == FootballController.PlayerType.GoalKeeper)
        {
            UpdateTargetPointer();
            UpdateGoalKeeper();
        }
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 50;
        style.normal.textColor = Color.white;
        GUI.Label(new Rect(50, 1000, 600, 60), "acc : " + acceleration, style);
    }

    private void UpdateTargetPointer()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Vector2 offset = new Vector2((maxX + minX) / 2, (maxY + minY) / 2);

        Vector3 targetPos = target.position;
        
        targetPos.x = offset.x + -acceleration.x * maxX * 4;
        targetPos.y = offset.y + acceleration.y * maxY * 4;
        if (targetPos.x > maxX) targetPos.x = maxX;
        else if (targetPos.x < minX) targetPos.x = minX;
        if (targetPos.y > maxY) targetPos.y = maxY;
        else if (targetPos.y < minY) targetPos.y = minY;

        target.position = Vector3.Lerp(target.position, targetPos, Time.deltaTime * handSpeed);

        //float x = Mathf.Lerp(target.position.x, targetPos.x, Time.deltaTime * handSpeed);
        //float y = Mathf.Lerp(target.position.y, targetPos.y, Time.deltaTime * handSpeed);
        //target.position = new Vector3(x, y, target.position.z);
#endif
    }

    private void UpdateGoalKeeper()
    {
        Vector3 pos = transform.position;
        pos.x = target.position.x;
        transform.position = pos;

        Vector3 handPos = hands.position;
        handPos.y = target.position.y;
        hands.position = handPos;

        EventManager.onGoalKeeperPositionUpdated?.Invoke(GameManager.Instance.GetClientId(), pos, handPos);

        //float angle = acceleration.x * 60f * 2f;
        //if (angle > maxAngle) angle = maxAngle;
        //if (angle < minAngle) _ = minAngle;

        //float finalAngle = Mathf.LerpAngle(transform.localEulerAngles.z, -angle, Time.deltaTime * smoothness);

        ////goalKeeper.localEulerAngles = Vector3.Lerp(goalKeeper.localEulerAngles, new Vector3(0, 0, finalAngle), Time.deltaTime * smoothness);
        //goalKeeper.localEulerAngles = new Vector3(0, 0, finalAngle);
    }

    public void UpdatePosition(Vector3 position, Vector3 handPosition)
    {
        goalKeeper.position = position;
        hands.position = handPosition;
    }

    public void Calibrate()
    {
        calibrationOffset = Input.acceleration;
    }
}
