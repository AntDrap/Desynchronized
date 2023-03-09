using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEditor;

public class SavingBehavior : MonoBehaviour
{
    public static string saveFile;
    public GameObject masterHold;
    public ComputerBehavior powerRestore, powerDiagnosis;
    public bool[] mileStones;
    List<string> items;

    void Start()
    {
        mileStones = new bool[6];
        saveFile = Application.dataPath + "/StreamingAssets/save.ever";
        items = new List<string>();

#if UNITY_EDITOR
        saveGame();
        File.WriteAllLines(saveFile, File.ReadAllLines(Application.dataPath + "/StreamingAssets/blankSave.ever"));
#else
        loadGame();
#endif
    }

    public void saveMilestone(int index)
    {
        mileStones[index] = true;
        saveGame();
    }

    public void loadMilestones()
    {
        for(int i = 0; i < mileStones.Length; i++)
        {
            if(mileStones[i])
            {
                switch(i)
                {
                    case 0:
                        powerRestore.pressButton();
                        powerDiagnosis.pressButton();
                        powerRestore.pressButton();
                        break;
                }
            }
        }
    }

    public void saveGame(GameObject savePoint = null)
    {
        items = new List<string>();
        items.Add("M:" + mileStones[0].ToString() + ":" + mileStones[1].ToString() + ":" + mileStones[2].ToString() + ":" + mileStones[3].ToString() + ":" + mileStones[4].ToString() + ":" + mileStones[5].ToString());

        PlayerBehavior pb = FindObjectOfType<PlayerBehavior>();

        for (int i = 0; i < 5; i++)
        {
            items.Add
            (
                i + ":" +
                pb.equipmentUnlocked[i] + ":" +
                pb.equipment[i].GetComponent<WeaponBehavior>().ammo + ":" +
                pb.equipment[i].GetComponent<WeaponBehavior>().totalAmmo + ":" +
                pb.equipment[i].GetComponent<WeaponBehavior>().modOne + ":" +
                pb.equipment[i].GetComponent<WeaponBehavior>().modTwo
            );
        }

        int playerEquip = -1;

        if (pb.currentEquip)
        {
            playerEquip = pb.equipment.IndexOf(pb.currentEquip);
        }
        else if (pb.pda.GetComponent<PDABehavior>().playerLastEquip)
        {
            playerEquip = pb.equipment.IndexOf(pb.pda.GetComponent<PDABehavior>().playerLastEquip);
        }

        items.Add("P:" + pb.health + ":" + pb.maxHealth + ":" + pb.stamina + ":" + pb.maxStamina + ":" + pb.moneyCount + ":" + pb.sconeCount + ":" + pb.accessLevel + ":" + playerEquip.ToString());

        string journalSave = "J:";

        int[] journalNums = JournalBehavior.saveJournals();

        for(int i = 0; i < journalNums.Length; i++)
        {
            if(i != 0)
            {
                journalSave += ":";
            }

            journalSave += journalNums[i];
        }

        items.Add(journalSave);

        foreach (GameObject g in actuallyFindAllObjects())
        {
            if (g.tag == "Saved" || g.tag == "Player" || g.tag == "MainCamera" || g.GetComponent<EnemyBehavior>())
            {
                Vector3 position = g.transform.position;

                if((g.tag == "Player" || g.tag == "MainCamera") && savePoint)
                {
                    position = savePoint.transform.position;
                }

                string itemAdd = "I:" + g.name + ":" + position.ToString() + ":" + g.transform.localRotation.eulerAngles.ToString() + ":" + g.transform.localScale.ToString() + ":" + g.activeSelf.ToString();

                if(g.GetComponent<EnemyBehavior>())
                {
                    itemAdd += ":" + "E" + ":" + g.GetComponent<EnemyBehavior>().health.ToString();
                }
                else if(g.GetComponent<TargetBehavior>())
                {
                    itemAdd += ":" + "T" + ":" + g.GetComponent<TargetBehavior>().shot.ToString() + ":" + g.GetComponent<TargetBehavior>().prizeGiven.ToString();
                }

                items.Add(itemAdd);
            }
        }

        File.WriteAllLines(saveFile, items.ToArray());

        FindObjectOfType<PopUpBehavior>().addWord("Progress Saved");
    }

    public void loadGame()
    {
        PlayerBehavior pb = FindObjectOfType<PlayerBehavior>();
        string[] saveLines = File.ReadAllLines(saveFile);
        string[] temp;

        for (int i = 0; i < saveLines.Length; i++)
        {
            switch(saveLines[i][0])
            {
                case 'I':
                    temp = saveLines[i].Split(":");

                    foreach (GameObject g in actuallyFindAllObjects())
                    {
                        if(g.name.Equals(temp[1]))
                        {
                            g.transform.position = translateToVector(temp[2]);
                            g.transform.localRotation = Quaternion.Euler(translateToVector(temp[3]));
                            g.transform.localScale = translateToVector(temp[4]);
                            g.SetActive(bool.Parse(temp[5]));

                            if (temp.Length > 6)
                            {
                                if (temp[6] == "E")
                                {
                                    g.GetComponent<EnemyBehavior>().health = float.Parse(temp[7]);
                                    g.GetComponent<EnemyBehavior>().ChangeHealth(0);

                                    if(float.Parse(temp[7]) <= 0)
                                    {
                                        g.GetComponent<EnemyBehavior>().health = 0.5f;
                                        g.GetComponent<EnemyBehavior>().ChangeHealth(1);
                                    }
                                }
                                else if(temp[6] == "T")
                                {
                                    g.GetComponent<TargetBehavior>().hitTarget(bool.Parse(temp[7]), bool.Parse(temp[8]));
                                }
                            }
                        }
                    }
                    break;
                case 'M':
                    temp = saveLines[i].Split(":");
                    mileStones = new bool[temp.Length - 1];

                    for(int j = 0; j < mileStones.Length; j++)
                    {
                        mileStones[j] = bool.Parse(temp[j + 1]); 
                    }

                    loadMilestones();
                    break;
                case 'P':
                    temp = saveLines[i].Split(":");
                    pb.health = float.Parse(temp[1]);
                    pb.maxHealth = float.Parse(temp[2]);
                    pb.stamina = float.Parse(temp[3]);
                    pb.maxStamina = float.Parse(temp[4]);
                    pb.moneyCount = int.Parse(temp[5]);
                    pb.sconeCount = int.Parse(temp[6]);
                    pb.accessLevel = int.Parse(temp[7]);
                    pb.updateUI();
                    break;
                case 'J':
                    temp = saveLines[i].Split(":");
                    List<int> journalLoad = new List<int>();

                    for(int j = 1; j < temp.Length; j++)
                    {
                        if(temp[j].Length > 0)
                        {
                            journalLoad.Add(int.Parse(temp[j]));
                        }
                    }

                    JournalBehavior.loadJournals(journalLoad.ToArray());
                    break;
                case '0':
                    temp = saveLines[i].Split(":");
                    FlashlightBehavior fb = pb.equipment[0].GetComponent<FlashlightBehavior>();
                    pb.equipmentUnlocked[0] = bool.Parse(temp[1]);
                    fb.ammo = int.Parse(temp[2]);
                    fb.totalAmmo = int.Parse(temp[3]);
                    if(bool.Parse(temp[4])) { fb.addMod(1); }
                    if (bool.Parse(temp[5])) { fb.addMod(2); }
                    break;
                case '1':
                    temp = saveLines[i].Split(":");
                    PistolBehavior pistolB = pb.equipment[1].GetComponent<PistolBehavior>();
                    pb.equipmentUnlocked[1] = bool.Parse(temp[1]);
                    pistolB.ammo = int.Parse(temp[2]);
                    pistolB.totalAmmo = int.Parse(temp[3]);
                    if (bool.Parse(temp[4])) { pistolB.addMod(1); }
                    if (bool.Parse(temp[5])) { pistolB.addMod(2); }
                    break;
                case '2':
                    temp = saveLines[i].Split(":");
                    ShotgunBehavior sb = pb.equipment[2].GetComponent<ShotgunBehavior>();
                    pb.equipmentUnlocked[2] = bool.Parse(temp[1]);
                    sb.ammo = int.Parse(temp[2]);
                    sb.totalAmmo = int.Parse(temp[3]);
                    if (bool.Parse(temp[4])) { sb.addMod(1); }
                    if (bool.Parse(temp[5])) { sb.addMod(2); }
                    break;
                case '3':
                    temp = saveLines[i].Split(":");
                    DrillBehavior db = pb.equipment[3].GetComponent<DrillBehavior>();
                    pb.equipmentUnlocked[3] = bool.Parse(temp[1]);
                    db.ammo = int.Parse(temp[2]);
                    db.totalAmmo = int.Parse(temp[3]);
                    if (bool.Parse(temp[4])) { db.addMod(1); }
                    if (bool.Parse(temp[5])) { db.addMod(2); }
                    break;
                case '4':
                    temp = saveLines[i].Split(":");
                    FlamethrowerBehavior flameb = pb.equipment[4].GetComponent<FlamethrowerBehavior>();
                    pb.equipmentUnlocked[4] = bool.Parse(temp[1]);
                    flameb.ammo = int.Parse(temp[2]);
                    flameb.totalAmmo = int.Parse(temp[3]);
                    if (bool.Parse(temp[4])) { flameb.addMod(1); }
                    if (bool.Parse(temp[5])) { flameb.addMod(2); }
                    break;
            }
        }

        pb.ChangeEquipment(int.Parse(saveLines[6].Split(":")[8]));
    }

    public Vector3 translateToVector(string word)
    {
        string temp2 = "";

        for (int i = 0; i < word.Length; i++)
        {
            if (word[i] != ' ' && word[i] != '(' && word[i] != ')')
            {
                temp2 += word[i];
            }
        }

        return new Vector3(float.Parse(temp2.Split(',')[0]), float.Parse(temp2.Split(',')[1]), float.Parse(temp2.Split(',')[2]));
    }

    public List<GameObject> actuallyFindAllObjects()
    {
        List<GameObject> objs = new List<GameObject>();

        iterate(masterHold, ref objs);

        return objs;
    }

    public void iterate(GameObject g, ref List<GameObject> objs)
    {
        objs.Add(g);

        for(int i = 0; i < g.transform.childCount; i++)
        {
            iterate(g.transform.GetChild(i).gameObject, ref objs);
        }
    }

    public static void randomizeNames()
    {
        foreach (GameObject g in Resources.FindObjectsOfTypeAll(typeof(GameObject)))
        {
            string[] temp = g.name.Split('-');

            string temp2 = "";

            for (int i = 0; i < temp[0].Length; i++)
            {
                if (temp[0][i] != ' ' && temp[0][i] != ':' && temp[0][i] != '[' && temp[0][i] != ']')
                {
                    temp2 += temp[0][i];
                }
            }

            if (g.tag == "Saved" || g.tag == "Player" || g.tag == "MainCamera" || g.GetComponent<EnemyBehavior>())
            {
                g.name = temp2.Split('(')[0] + "-" + UnityEngine.Random.Range(0, 1000000);
            }
            else
            {
                g.name = temp2.Split('(')[0];
            }
        }
    }
}

#if UNITY_EDITOR

public class RandomizeNames : EditorWindow
{
    [MenuItem("Window/Randomize Object Names")]
    public static void ShowWindow()
    {
        GetWindow<RandomizeNames>("Randomize Object Names");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Run Function"))
        {
            SavingBehavior.randomizeNames();
        }
    }
}
#endif