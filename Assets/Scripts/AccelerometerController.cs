using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WTI.NetCode;


[Serializable]
public class PairDirectionList
{
    public string keyName;
    public List<DirectionList> directionList;
}

[Serializable]
public class DirectionList
{
    public List<AccelerometerController.DirectionType> directionList;
}

public class AccelerometerController : MonoBehaviour
{
    private Vector2 acceleration;

    public enum CheckType
    {
        Direction,
        Angle
    }
    public CheckType checkType;

    public enum DirectionType
    {
        up = 0,
        left = 1,
        down = 2,
        right = 3
    }
    public List<int> tempDirectionList = new List<int>();
    public float commandTimeout = 0.5f;
    public float elapsedTime = 0f;
    public List<DirectionType> allowedDirection;

    public List<PairDirectionList> acceptableDirectionList;
    //list of directions to be considered as shake; ex: left -> right -> left -> right
    public List<List<DirectionType>> shakeCommandList = 
        new List<List<DirectionType>> { 
            new List<DirectionType> { 
                DirectionType.left, DirectionType.right, DirectionType.left, DirectionType.right 
            }, 
            new List<DirectionType> { 
                DirectionType.right, DirectionType.left, DirectionType.right, DirectionType.left
            } 
        };

    //public UnityEvent onShakedTriggered;
    //public UnityEvent<float> onAccelerometerChanged;

    private string directionList = "";

    private void Start()
    {
        elapsedTime = 0f;
        ClearDirections();
    }

    private void Update()
    {
        if (tempDirectionList.Count > 0)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= commandTimeout)
            {
                elapsedTime = 0f;
                ClearDirections();

            }
        }

        if (!GameManager.Instance.IsServer)
        {
            if (GameManager.Instance.controlType == GameManager.ControlType.KITE)
            {
                //onAccelerometerChanged?.Invoke(acceleration.x);
                EventManager.onPhoneTilted?.Invoke(GameManager.Instance.GetClientId(),  acceleration.x);
            }
        }
    }

    private void ClearDirections()
    {
        tempDirectionList.Clear();
        directionList = "";
    }

    private void LateUpdate()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveUp();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveDown();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveRight();
        }
#endif
        if (Application.platform == RuntimePlatform.Android)
        {
            acceleration.x = Input.acceleration.x;
            acceleration.y = Input.acceleration.y;
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            acceleration.x = -Input.acceleration.y;
            acceleration.y = Input.acceleration.z;
        }

        if (acceleration.x > 0.2f)
        {
            MoveRight();
        }
        else if (acceleration.x < -0.2f)
        {
            MoveLeft();
        }
        if (acceleration.y > 0.2f)
        {
            MoveUp();
        }
        else if (acceleration.y < -0.2f)
        {
            MoveDown();
        }
       
    }

    private void MoveUp()
    {
        if (allowedDirection.Contains(DirectionType.up))
        {
            if (tempDirectionList.Count == 0 || tempDirectionList[tempDirectionList.Count - 1] != (int)DirectionType.up)
            {
                tempDirectionList.Add((int)DirectionType.up);
                directionList += ((int)DirectionType.up).ToString();

                CheckDirection();
            }
        }
    }

    private void MoveDown()
    {
        if (allowedDirection.Contains(DirectionType.down))
        {
            if (tempDirectionList.Count == 0 || tempDirectionList[tempDirectionList.Count - 1] != (int)DirectionType.down)
            {
                tempDirectionList.Add((int)DirectionType.down);
                directionList += ((int)DirectionType.down).ToString();

                CheckDirection();
            }
        }
    }

    private void MoveLeft()
    {
        if (allowedDirection.Contains(DirectionType.left))
        {
            if (tempDirectionList.Count == 0 || tempDirectionList[tempDirectionList.Count - 1] != (int)DirectionType.left)
            {
                tempDirectionList.Add((int)DirectionType.left);
                directionList += ((int)DirectionType.left).ToString();

                CheckDirection();
            }
        }
    }

    private void MoveRight()
    {
        if (allowedDirection.Contains(DirectionType.right))
        {
            if (tempDirectionList.Count == 0 || tempDirectionList[tempDirectionList.Count - 1] != (int)DirectionType.right)
            {
                tempDirectionList.Add((int)DirectionType.right);
                directionList += ((int)DirectionType.right).ToString();

                CheckDirection();
            }
        }
    }

    private void CheckDirection()
    {
        if (checkType == CheckType.Direction)
        {
            string commandList = "";
            for (int i = 0; i < shakeCommandList.Count; i++)
            {
                if (shakeCommandList[i].Count == tempDirectionList.Count)
                {
                    commandList = "";
                    for (int j = 0; j < shakeCommandList[i].Count; j++)
                    {
                        commandList += ((int)shakeCommandList[i][j]).ToString();
                    }
                }

                if (directionList == commandList && directionList.Length > 0)
                {
                    //onShakedTriggered?.Invoke();
                    if (GameManager.Instance.controlType == GameManager.ControlType.SHAKEDRAW)
                    {
                        EventManager.onPhoneSideShaked?.Invoke(GameManager.Instance.GetClientId());
                        ClearDirections();
                        elapsedTime = 0f;
                        directionList = "";
                    }
                    break;
                }
            }
        }
    }

    //private void OnGUI()
    //{
    //    GUIStyle style = new GUIStyle();
    //    style.fontSize = 50;
    //    GUI.Label(new Rect(70, 70, 300, 70), "dir : " + directionList, style);
    //    GUI.Label(new Rect(70, 140, 300, 70), "acc y : " + acceleration.y, style);
    //}
}
