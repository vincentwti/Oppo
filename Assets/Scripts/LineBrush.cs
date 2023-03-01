using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineBrush : SimplePooling
{
    private LineRenderer lineRenderer;

    private float lineThreshold = 2f;
    private float elapsedTimeTouchRelease = 0f;
    private bool isTouchReleased = false;
    private int frame = 1;
    private float elapsedFrameTime;

    private List<Vector3> positionList = new List<Vector3>();

    private List<LineRenderer> lineList = new List<LineRenderer>();

    public float width = 1;
    public Transform lineParent;

    protected override void Start()
    {
        base.Start();
        Input.multiTouchEnabled = false;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        StartCoroutine(SyncLinePosition(frame));
        elapsedFrameTime = 0f;

        EventManager.onClearLine += ClearLine;
    }

    private void SpawnLine(Vector3 position)
    {
        GameObject lineObj = GetItem("line");
        LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer>();
        lineRenderer.transform.parent = lineParent;
        lineRenderer.transform.position = position;
        
        lineList.Add(lineRenderer);
    }

    private void OnDestroy()
    {
        EventManager.onClearLine -= ClearLine;
    }

    private void Update()
    {
        if (EventManager.onGetIsConnected != null)
        {
            if (!EventManager.onGetIsConnected.Invoke())
            {
                return;
            }
        }
        if (GameManager.Instance.IsServer)
        {
            CheckElapsedTime();
        }
        else if(GameManager.Instance.controlType == GameManager.ControlType.SHAKEDRAW)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isTouchReleased = false;
                elapsedTimeTouchRelease = 0;
                //ClearLine();
                Vector3 mousePos = Input.mousePosition;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                worldPos.x = float.Parse(worldPos.x.ToString("0.000"));
                worldPos.y = float.Parse(worldPos.y.ToString("0.000"));
                worldPos.z = 0;
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, worldPos);
                lineRenderer.SetPosition(1, worldPos);

                positionList.Add(worldPos);
                positionList.Add(worldPos);
            }

            if (elapsedFrameTime >= frame / 60f)
            {
                if (Input.GetMouseButton(0))
                {
                    if (positionList.Count < 100)
                    {
                        Vector3 mousePos = Input.mousePosition;
                        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                        worldPos.x = float.Parse(worldPos.x.ToString("0.000"));
                        worldPos.y = float.Parse(worldPos.y.ToString("0.000"));
                        worldPos.z = 0;
                        lineRenderer.positionCount += 1;
                        lineRenderer.SetPosition(lineRenderer.positionCount - 1, worldPos);

                        positionList.Add(worldPos);
                    }
                }
                elapsedFrameTime = 0f;
            }
            else
            {
                elapsedFrameTime += Time.deltaTime;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isTouchReleased = true;
            }

            //if (isTouchReleased)
            //{
            //    CheckElapsedTime();
            //}
        }
    }

    private void CheckElapsedTime()
    {
        //elapsedTimeTouchRelease += Time.deltaTime;

        //if (elapsedTimeTouchRelease > lineThreshold)
        //{
        //    elapsedTimeTouchRelease = 0f;
        //    ClearLine();
        //}
    }

    private IEnumerator SyncLinePosition(float rate)
    {
        while (true)
        {
            if (!isTouchReleased)
            {
                yield return new WaitForSeconds(0.1f);
                if (!GameManager.Instance.IsServer)
                {
                    if (GameManager.Instance.controlType == GameManager.ControlType.SHAKEDRAW && positionList.Count > 0)
                    {
                        EventManager.onDrawingLine?.Invoke(GameManager.Instance.GetClientId(), positionList);
                    }
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    private void ClearLine()
    {
        for (int i = 0; i < lineList.Count; i++)
        {
            lineList[i].positionCount = 0;
            lineList[i].gameObject.SetActive(false);
        }

        //lineRenderer.positionCount = 0;
        //positionList.Clear();
    }

    public void SetLine(List<Vector3> points)
    {
        elapsedTimeTouchRelease = 0f;
        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPosition(i, points[i]);
        }
    }
}
