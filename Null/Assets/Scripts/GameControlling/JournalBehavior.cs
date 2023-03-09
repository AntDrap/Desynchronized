using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class JournalBehavior : MonoBehaviour
{
    public static string journalFile = Application.dataPath + "/StreamingAssets/journals.ever";
    public static string[] journalText;
    public static List<journalEntry> journals;
    public static List<journalEntry> currentJournals;

    public static void createJournals()
    {
        journalText = File.ReadAllLines(journalFile);
        journals = new List<journalEntry>();
        currentJournals = new List<journalEntry>();

        int index = 0;
        string name = "";
        List<string> entry = new List<string>();

        for (int i = 0; i < journalText.Length; i++)
        {
            switch (journalText[i][0])
            {
                case '/':
                    continue;
                case '[':
                    string[] temp = journalText[i].Split('-');
                    index = int.Parse(temp[0].Substring(1, temp[0].Length - 1));
                    name = temp[1].Substring(0, temp[1].Length - 1);
                    break;
                case '{':
                    entry = new List<string>();
                    break;
                case '}':
                    journals.Add(new journalEntry(name, entry.ToArray(), index));
                    index = 0;
                    name = "";
                    entry = new List<string>();
                    break;
                default:
                    entry.Add(journalText[i]);
                    break;
            }
        }
    }

    public static journalEntry getJournal(int index)
    {
        if (journals == null)
        {
            createJournals();
        }

        if (index > journals.Count || index < 0)
        {
            return journals[0];
        }

        return journals[index];
    }

    public static void loadJournals(int[] savedJournals)
    {
        currentJournals = new List<journalEntry>();

        for(int i = 0; i < savedJournals.Length; i++)
        {
            currentJournals.Add(journals[savedJournals[i]]);
        }
    }

    public static int[] saveJournals()
    {
        int[] savedJournals = new int[currentJournals.Count];

        for(int i = 0; i < savedJournals.Length; i++)
        {
            savedJournals[i] = currentJournals[i].index;
        }

        return savedJournals;
    }
}

[Serializable]
public class journalEntry
{
    public string name;
    public string[] entry;
    public int index;

    public journalEntry(string n, string[] e, int i)
    {
        name = n;
        entry = e;
        index = i;
    }

    public string getEntry()
    {
        string temp = "";

        for (int i = 0; i < entry.Length; i++)
        {
            temp += entry[i] + "\n";
        }

        return temp;
    }
}