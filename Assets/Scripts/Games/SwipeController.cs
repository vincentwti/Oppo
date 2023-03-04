using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeController : MonoBehaviour//, IPointerDownHandler, IPointerUpHandler
{
    public LineRenderer swipeLineRenderer;
    public Camera worldCamera;

    private Vector3 fromPosition;
    private Vector3 toPosition;

    private float distance;
    private bool canSwipe = true;
    private bool isStartFromBall = false;

    public LayerMask ballLayerMask;
    public LayerMask goalLayerMask;

    public delegate void OnSwipeCompleted(Vector3 shootPosition);
    public OnSwipeCompleted onSwipeCompleted;

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    fromPosition = eventData.position;
    //}

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    toPosition = eventData.position;
    //    Vector3 direction = fromPosition - toPosition;
    //}

    public void CanSwipe(bool canSwipe)
    {
        this.canSwipe = canSwipe;
    }

    private void Update()
    {
        Debug.Log("canswipe : " + canSwipe);
        if (!canSwipe || FootballController.Instance.playerType != FootballController.PlayerType.Striker)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = worldCamera.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 100f, ballLayerMask))
            {
                isStartFromBall = true;
                ClearLine();
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                worldPos.z = 0;
                swipeLineRenderer.positionCount = 2;
                swipeLineRenderer.SetPosition(0, worldPos);
                swipeLineRenderer.SetPosition(1, worldPos);

                fromPosition = worldPos;
            }
            else
            {
                isStartFromBall = false;
            }
        }

        if (Input.GetMouseButton(0) && isStartFromBall)
        {
            float topX;
            float topY;

            Vector3 p = Vector3.zero;

            if (toPosition.x > fromPosition.x)
            {
                topX = -Mathf.Infinity;
            }
            else
            {
                topX = Mathf.Infinity;
            }

            if (toPosition.y > fromPosition.y)
            {
                topY = -Mathf.Infinity;
            }
            else
            {
                topY = Mathf.Infinity;
            }

            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            worldPos.z = 0;
            //Debug.Log("pos : " + mousePos);
            swipeLineRenderer.positionCount += 1;
            swipeLineRenderer.SetPosition(swipeLineRenderer.positionCount - 1, worldPos);

            toPosition = worldPos;

            Vector3 direction = toPosition - fromPosition;
            distance = Vector3.Distance(fromPosition, toPosition);

            for (int i = 0; i < swipeLineRenderer.positionCount; i++)
            {
                Vector3 pos = swipeLineRenderer.GetPosition(i);
                if (toPosition.x > fromPosition.x)
                {
                    if (pos.x > topX)
                    {
                        topX = pos.x;
                    }
                }
                else
                {
                    if (pos.x < topX)
                    {
                        topX = pos.x; 
                    }
                }

                if (toPosition.y > fromPosition.y)
                {
                    if (pos.y > topY)
                    {
                        topY = pos.y;
                    }
                }
            }

            swipeLineRenderer.transform.position = worldPos;
            swipeLineRenderer.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction.normalized);  
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isStartFromBall)
            {
                Vector3 direction = toPosition - fromPosition;
                distance = Vector3.Distance(fromPosition, toPosition);
                OnSwipeReleased(direction);
            }
        }
    }

    public void ClearLine()
    {
        swipeLineRenderer.positionCount = 0;
    }

    private void OnSwipeReleased(Vector3 direction)
    {
        //if (Physics.Raycast(toPosition, Vector3.forward, out RaycastHit raycastHit, 100f, goalLayerMask))
        //{
        //    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    go.transform.position = raycastHit.point;
        //    onSwipeCompleted?.Invoke(raycastHit.point);
        //}
        Ray ray = worldCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 100f, goalLayerMask))
        {
            //GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //go.transform.position = raycastHit.point;
            onSwipeCompleted?.Invoke(raycastHit.point);
        }
    }
}
