using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class MainMenuBehavior : MonoBehaviour
{
    public static string saveFile;
    public static string blankSaveFile;
    public GameObject[] panels;
    public Button continueButton;

    private void Start()
    {
        saveFile = Application.dataPath + "/StreamingAssets/save.ever";
        blankSaveFile = Application.dataPath + "/StreamingAssets/blankSave.ever";
        panels[1].GetComponent<SettingsBehavior>().refresh();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void StartGame(bool newGame)
    {
        if(newGame)
        {
            File.WriteAllLines(saveFile, File.ReadAllLines(blankSaveFile));
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void OpenPanel(GameObject panel)
    {
        foreach(GameObject g in panels)
        {
            g.SetActive(false);
        }

        panel.SetActive(true);
    }
}
