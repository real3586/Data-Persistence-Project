using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    public float Speed = 2.0f;
    public float MaxMovement = 2.0f;
    
    void Update()
    {
        float input = Input.GetAxis("Horizontal");

        Vector3 pos = transform.position;
        pos.x += input * Speed * Time.deltaTime;

        if (pos.x > MaxMovement)
            pos.x = MaxMovement;
        else if (pos.x < -MaxMovement)
            pos.x = -MaxMovement;

        if (MainManager.Instance.DevCheat)
        {
            transform.position = new Vector3(MainManager.Instance.Ball.transform.position.x, transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = pos;
        }
    }
}
