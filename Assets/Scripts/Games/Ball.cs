using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WTI.NetCode;

public class Ball : MonoBehaviour
{
    private Rigidbody rigidBody;
    public Vector3 DefPos { get; private set; }

    private float smoothness = 100f;
    public bool isShooting;

    public float test = 0;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        DefPos = transform.position;
        isShooting = false;
    }

    public void Reset()
    {
        Debug.Log("reset");
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        transform.position = DefPos;
    }

    public void Shoot(Vector3 shootPosition)
    {
        Reset();
        isShooting = true;
        FootballController.Instance.scoreController.time.Pause(true);
        if (FootballController.Instance.playerType == FootballController.PlayerType.Striker)
        {
            Vector3 direction = shootPosition - transform.position;
            Vector3 upForce = Vector3.zero;
            if (shootPosition.y > transform.position.y)
            {
                upForce = Vector3.up * 5f;
            }
            rigidBody.AddForce(direction.normalized * 25f + upForce, ForceMode.Impulse);
        }
        //rigidBody.AddTorque(Vector3.down * 10000, ForceMode.Impulse);
        //float upForce = Random.Range(1, 7);
        //float sideForce = Random.Range(-8, 8);
        //rigidBody.AddForce(Vector3.forward * Random.Range(25, 40) + new Vector3(sideForce, upForce, 0), ForceMode.Impulse);    
    }

    public void AddForce(Vector3 force)
    {
        //rigidBody.velocity = Vector3.zero;
        //force.x *= 2f;
        //rigidBody.AddForce(force, ForceMode.VelocityChange);        
    }

    public void Shoot()
    {
        Reset();
        isShooting = true;
        FootballController.Instance.scoreController.time.Pause(true);
        Vector3 upForce = Vector3.up * 5f;
        rigidBody.AddForce(Vector3.forward * 25f + upForce, ForceMode.Impulse);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
            transform.position = DefPos;
        }

        if (!GameManager.Instance.IsServer && FootballController.Instance.playerType == FootballController.PlayerType.Striker)
        {
            rigidBody.isKinematic = false;
            EventManager.onFootballUpdated?.Invoke(GameManager.Instance.GetClientId(), transform.position);
        }
        else
        {
            rigidBody.isKinematic = true;
        }
    }

    public void UpdatePosition(Vector3 position)
    {
        if (isShooting)
        {
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * smoothness);
        }
        else
        {
            transform.position = position;
        }
    }
}
