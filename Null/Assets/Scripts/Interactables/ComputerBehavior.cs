using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class ComputerBehavior : MonoBehaviour
{
    public GameObject panel, button;
    public int entryIndex;
    public TextMeshProUGUI journalName;
    journalEntry entry;
    bool open, downloaded;
    float time;

    void Start()
    {
        if(entryIndex >= 0)
        {
            entry = JournalBehavior.getJournal(entryIndex);
            journalName.text = entry.name;
        }
    }

    void Update()
    {
        if (open)
        {
            time -= Time.deltaTime;

            if (time <= 0)
            {
                ToggleComputer(false);
            }
        }
    }

    public void ToggleComputer(bool toggle)
    {
        panel.SetActive(toggle);
        open = toggle;

        if (toggle)
        {
            if(JournalBehavior.currentJournals.Contains(entry))
            {
                downloaded = true;
                journalName.text = "File Downloaded";
                lockComputer(true);
            }

            time = 10f;
        }
    }

    public void Download()
    {
        if (downloaded) { return; };
        Invoke("DownloadDelay", 0.12f);
    }

    public void DownloadDelay()
    {
        time = 10f;
        JournalBehavior.currentJournals.Add(entry);
        JournalBehavior.currentJournals.Sort((a, b) => a.index.CompareTo(b.index));
        journalName.text = "File Downloaded";
        lockComputer(true);
        FindObjectOfType<PDABehavior>().TogglePDA(true);
        FindObjectOfType<PDABehavior>().ChangeScreens(2);
        FindObjectOfType<PDABehavior>().OpenJournal(JournalBehavior.currentJournals.IndexOf(entry));
    }

    public int restoreCount = 0;

    public void RestorePower()
    {
        restoreCount++;

        if(restoreCount == 2)
        {
            journalName.text = "Restore Power?";

        }
        else if (restoreCount == 3)
        {
            journalName.text = "Power Restored";
            FindObjectOfType<LightBehavior>().ToggleAllLights(true);
            FindObjectOfType<SavingBehavior>().saveMilestone(0);
        }
        else
        {
            journalName.text = "Power Partially Restored\nPlease Visit Maintenance Sector B";
        }
    }

    public void lockComputer(bool state)
    {
        downloaded = state;
        button.SetActive(!state);
    }

    public void SetText(string text)
    {
        journalName.text = text;
    }

    public void pressButton()
    {
        button.GetComponent<TriggerBehavior>().Trigger();
    }
}