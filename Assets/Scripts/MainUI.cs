using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUI : MonoBehaviour
{
    public void ExitToMenu()
    {
        SceneManager.LoadScene(0);
    }
}