using UnityEngine;

public class KiteController : MonoBehaviour
{
    public float angle;
    public float angleMultiplier;
    public float smoothness = 1f;

    public void SetAngle(float angle)
    {
        this.angle = angle;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            angle = -0.1f;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            angle = 0.1f;
        }


        float finalAngle = -angle * angleMultiplier;
        finalAngle = Mathf.LerpAngle(transform.localEulerAngles.z, finalAngle, Time.deltaTime * smoothness);
        transform.localEulerAngles = new Vector3(0, 0, finalAngle);
        //transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z + 5f * Time.deltaTime);
    
        //transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(0, 0, finalAngle), Time.deltaTime * smoothness);
    }
}
