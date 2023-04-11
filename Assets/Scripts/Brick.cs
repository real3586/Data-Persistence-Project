using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Brick : MonoBehaviour
{    
    public int PointValue;
    public GameObject Bricks;

    void Start()
    {
        Bricks = GameObject.Find("Bricks");
        gameObject.transform.SetParent(Bricks.transform);
        var renderer = GetComponentInChildren<Renderer>();

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        switch (PointValue)
        {
            case 1 :
                block.SetColor("_BaseColor", Color.green);
                break;
            case 2:
                block.SetColor("_BaseColor", Color.yellow);
                break;
            case 5:
                block.SetColor("_BaseColor", Color.blue);
                break;
            default:
                block.SetColor("_BaseColor", Color.red);
                break;
        }
        renderer.SetPropertyBlock(block);
    }

    private void OnCollisionEnter(Collision other)
    {        
        //slight delay to be sure the ball have time to bounce
        Destroy(gameObject, 0.2f);
    }
    private void OnDestroy()
    {
        if (!MainManager.Instance.m_GameOver)
        MainManager.Instance.AddPoint(PointValue);
    }
}
