using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    public TMP_InputField inputField;
    public string userName;

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void OnNameChange()
    {
        userName = inputField.text;
        if (string.IsNullOrEmpty(userName))
        {
            userName = "Anonymous";
        }
        else if (userName == "PLSDevCheat")
        {
            MainManager.Instance.DevCheat = true;
        }
        MainManager.Instance.playerName = userName;
    }
    public void QuitGame()
    {
        MainManager.Instance.data.SaveData();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
