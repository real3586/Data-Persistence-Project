using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathZone : MonoBehaviour
{
    public MainManager Manager;
    void OnEnable()
    {
        Manager = GameObject.Find("MainManager").GetComponent<MainManager>();
    }
    private void OnCollisionEnter(Collision other)
    {
        other.gameObject.SetActive(false);
        Manager.GameOver(false);
    }
}