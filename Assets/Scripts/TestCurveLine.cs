using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCurveLine : MonoBehaviour
{
    private float maxTime = 2f;
    private float interval = 0.05f;
    public List<Vector3> dots = new List<Vector3>();
    private List<Vector3> tempDots = new List<Vector3>();
    private List<Vector3> dotsInGoal = new List<Vector3>();
    public float totalDistance = 0f;
    public float heightPerDistance = 0f;
    private float shootDirection; //0 = linear, -1 = right, 2 = left

    public enum ShootType
    {
        Linear,
        Curve
    }
    public ShootType shootType;

    public float rotationSpeed = 500f;
    public float speed = 10f;

    private bool canDrawLine = true;
    public float elapsedTime = 0;
    public float intervalElapsedTime = 0f;

    public LayerMask shootLayerMask;
    public LineRenderer lineRenderer;

    public Ball ball;

    private Vector3 targetNode;
    public Vector3 targetDir;
    private bool reachGoal = false;
    public bool isReleased = false;

    private void Reset()
    {
        dots.Clear();
        tempDots.Clear();
        dotsInGoal.Clear();
        isReleased = false;
        reachGoal = false;
    }

    private float GetAngle(Vector3 origin, Vector3 p1, Vector3 p2)
    {
        return Vector3.Angle(p1 - origin, p2 - p1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Reset();
            ball.Reset();
        }

        ball.GetComponent<Rigidbody>().isKinematic = false;
        if (Input.GetMouseButtonDown(0))
        {
            Reset();
            Vector3 mousePos = Input.mousePosition;
            Ray ray = FootballController.Instance.strikerCamera.ScreenPointToRay(mousePos);
            RaycastHit[] raycastHits = Physics.RaycastAll(ray, 200f, shootLayerMask);
            for (int i = 0; i < raycastHits.Length; i++)
            {
                dots.Add(raycastHits[i].point);
                //ball.transform.rotation = Quaternion.LookRotation(raycastHits[i].point - ball.transform.position);
                break;
            }
        }

        if (Input.GetMouseButton(0) && !isReleased)
        {
            totalDistance = 0f;
            elapsedTime += Time.deltaTime;
            intervalElapsedTime += Time.deltaTime;
            if (intervalElapsedTime >= interval && canDrawLine)
            {
                Vector3 mousePos = Input.mousePosition;
                Ray ray = FootballController.Instance.strikerCamera.ScreenPointToRay(mousePos);
                RaycastHit[] raycastHits = Physics.RaycastAll(ray, 200f, shootLayerMask);
                for (int i = 0; i < raycastHits.Length; i++)
                {
                    if (raycastHits[i].collider.tag == "ShootArea")
                    {
                        if (dots.Count > 2)
                        {
                            float angle = GetAngle(dots[dots.Count - 2], dots[dots.Count - 1], raycastHits[i].point);
                            Debug.LogError("final angle : " + angle);
                            if (angle > 25)
                            {
                                OnTouchReleased();
                                break;
                            }
                            else
                            {
                                float angleDir = AngleDir(dots[dots.Count - 1] - dots[dots.Count - 2], raycastHits[i].point - dots[dots.Count - 2], Vector3.up);
                                if (angleDir != shootDirection)
                                {
                                    break;
                                }

                                Debug.LogError("angle : " + angleDir);
                            }
                        }
                        if (Vector3.Distance(dots[dots.Count - 1], raycastHits[i].point) > 2f)
                        {
                            reachGoal = true;
                            dotsInGoal.Add(raycastHits[i].point);
                            break;
                        }

                    }
                    else
                    {
                        if (dots.Count > 2)
                        {
                            float angle = GetAngle(dots[dots.Count - 3], dots[dots.Count - 2], dots[dots.Count - 1]);
                            Debug.Log("angle : " + angle);

                            if (dots.Count == 3)
                            {
                                if (angle <= 5)
                                {
                                    shootType = ShootType.Linear;
                                }
                                else
                                {
                                    shootType = ShootType.Curve;
                                }
                            }

                            if (angle > 35)
                            {
                                OnTouchReleased();
                            }

                            if (i < dots.Count - 2)
                            {
                                float angleDir = AngleDir(dots[dots.Count - 2] - dots[dots.Count - 3], dots[dots.Count - 1] - dots[dots.Count - 3], Vector3.up);
                                if (dots[dots.Count - 3] == dots[0])
                                {
                                    shootDirection = angleDir;
                                }
                                else
                                {
                                    if (angleDir != shootDirection && shootType == ShootType.Curve)
                                    {
                                        break;
                                    }
                                }

                                Debug.LogError("angle : " + angleDir);
                            }
                        }
                        if (Vector3.Distance(dots[dots.Count - 1], raycastHits[i].point) > 2f)
                        {
                            dots.Add(raycastHits[i].point);
                            break;
                        }
                    }
                }
                intervalElapsedTime = 0f;
            }
            if (elapsedTime >= maxTime)
            {
                StartCoroutine(OnTouchReleased());
                canDrawLine = false;
                elapsedTime = 0f;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (!isReleased)
            {
                if (dotsInGoal.Count > 0)
                {
                    dots.Add(dotsInGoal[dotsInGoal.Count - 1]);
                }
                StartCoroutine(OnTouchReleased());
            }
        }
    }

    private IEnumerator OnTouchReleased()
    {
        Vector3 shootDir = (dots[dots.Count - 1] - dots[0]).normalized;
        Debug.LogWarning("shootDir : " + shootDir);
        ball.AddForce(dots[dots.Count - 1] - dots[0]);
        for (int i = 0; i < dots.Count - 1; i++)
        {
            if (i == dots.Count - 2)
            {
                Vector3 tempPos = dots[i + 1];
                tempPos.y = dots[i].y;
                float dist = Vector3.Distance(dots[i], tempPos);
                totalDistance += dist;
            }
            else
            {
                Vector3 dir = (dots[i + 1] - dots[i]).normalized;
                Debug.LogWarning("node dir : " + dir);
                float dist = Vector3.Distance(dots[i], dots[i + 1]);
                float distWithOrigin = Vector3.Distance(dots[0], dots[i + 1]);
                totalDistance += dist;

                //if (shootDir.x - dir.x < 0.03f && shootDir.y - dir.y < 0.03f && shootDir.z - dir.z < 0.03f)
                //{
                //    dots[i + 1] = dots[0] + shootDir * distWithOrigin;
                //}

                //dots[dots.Count - 3], dots[dots.Count - 2], dots[dots.Count - 1]
                //if (i < dots.Count - 2)
                //{
                //    float angleDir = AngleDir(dots[i + 2] - dots[i], dots[i + 1] - dots[i], Vector3.up);
                //    Debug.LogError("angle : " + angleDir);
                //}

            }
        }

        heightPerDistance = (dots[dots.Count - 1].y - dots[0].y) / totalDistance;
        Debug.LogWarning("y : " + dots[dots.Count - 1].y);
        if (shootType == ShootType.Curve && dots[dots.Count - 1].y > 2f)
        {
            for (int i = 0; i < dots.Count - 1; i++)
            {
                Vector3 pos = dots[i];
                float dist = Vector3.Distance(dots[0], dots[i]);
                pos.y = heightPerDistance * dist + dots[0].y;
                dots[i] = pos;
            }
        }
        else
        {
            Debug.LogWarning(dots.Count);
            int flag = dots.Count - 1;
            for (int i = 0; i < flag; i++)
            {
                dots.RemoveAt(0);
            }
        }
        tempDots = new List<Vector3>(dots);

        canDrawLine = true;
        elapsedTime = intervalElapsedTime = 0f;
        //dots.Clear();
        isReleased = true;
        yield return null;
    }

    private void LateUpdate()
    {
        if (isReleased)
        {
            float minDistance = Mathf.Infinity;
            if (dots.Count > 0)
            {
                Vector3 targetNode = dots[0];
                float ballDistance = Vector3.Distance(ball.transform.position, ball.DefPos);
                float nodeDistance = Vector3.Distance(dots[0], ball.DefPos);
                //Debug.Log("dist : " + distance);

                

                if (ballDistance > nodeDistance)
                {
                    Debug.LogError("distance : " + ballDistance + " " + nodeDistance + " " + " dot 0 : " + dots[0] + " " + ball.DefPos);
                    dots.RemoveAt(0);
                }

                if (dots.Count > 0)
                {
                    targetDir = dots[0] - ball.transform.position;
                    //Quaternion targetRot = Quaternion.LookRotation(targetDir.normalized, ball.transform.up);
                    //ball.transform.rotation = Quaternion.Lerp(ball.transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
                    //ball.AddForce(ball.transform.forward * 20f);

                    ball.AddForce(targetDir.normalized * speed);
                }
                //Vector3 goalDir = (dots[dots.Count - 1] - dots[0]);
                //goalDir.x = goalDir.z = 0;
                //ball.AddForce(goalDir.normalized * 10);

                //for (int i = 0; i < tempDots.Count; i++)
                //{
                //    float distance = Vector3.Distance(ball.transform.position, tempDots[i]);
                //    Debug.Log("dist : " + distance);
                //    targetNode = tempDots[i];
                //    if (distance < 0.1f)
                //    {
                //        tempDots.RemoveAt(i);
                //    }

                //    //if (distance < minDistance)
                //    //{
                //    //    minDistance = distance;
                //    //    targetNode = dots[i];
                //    //    if (distance < 0.1f)
                //    //    {
                //    //        dots.RemoveAt(i);
                //    //    }
                //    //}
                //}
            }
            else
            {
                //ball.AddForce(ball.GetComponent<Rigidbody>().velocity.normalized * speed);
                //ball.AddForce(ball.transform.forward * speed);
            }
            //if (targetNode != null)
            //{
            //    targetDir = (Vector3)targetNode - ball.transform.position;
            //    Debug.Log("target direction : " + targetDir + " targetNode : " + targetNode);
            //    ball.AddForce(targetDir.normalized * 10f);
            //    ball.AddForce((dots[dots.Count - 1] - ball.transform.position).normalized * 10);
            //}
            //else
            //{
            //    ball.AddForce(targetDir.normalized * 10f);
            //}

            //targetNode = dots[1];

            //Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0f);

            //ball.transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);
        }
    }

    private float AngleDir(Vector3 forward, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(forward, targetDir);
        float dir = Vector3.Dot(perp, up);
        if (dir > 0f)
        {
            return 1f;
        }
        else if (dir < 0f)
        {
            return -1f;
        }
        return 0f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (dots.Count > 0)
        {
            for (int i = 0; i < dots.Count - 1; i++)
            {
                Gizmos.DrawWireSphere(dots[i], 0.3f);
            }
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(dots[dots.Count - 1], 0.5f);
        }
    }


}
