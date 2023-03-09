using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PDABehavior : MonoBehaviour
{
    public GameObject[] screens;
    public GameObject PDA;
    public GameObject playerLastEquip;
    public bool open;
    PlayerBehavior pb;
    Vector3 startPos;
    int currentJournal;

    void Awake()
    {
        pb = FindObjectOfType<PlayerBehavior>();
        startPos = transform.localPosition;
        transform.localPosition = startPos - Vector3.up * 0.75f;
        screens[3].GetComponent<SettingsBehavior>().refresh();
        ChangeScreens(0);
        TogglePDA(false, true);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePDA(!open);
        }

        if (open)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, 8 * Time.deltaTime);

            if (screens[1].activeSelf)
            {
                for (int i = 0; i < pb.equipmentUnlocked.Length; i++)
                {
                    if (pb.equipmentUnlocked[i] && i != 0)
                    {
                        ammo[i - 1].text = pb.equipment[i].GetComponent<WeaponBehavior>().ammo + "/" + pb.equipment[i].GetComponent<WeaponBehavior>().totalAmmo;
                    }
                }
            }

            if ((Input.GetKeyDown(KeyCode.Escape) && !screens[0].activeSelf))
            {
                ChangeScreens(0);
            }
        }
    }

    public GameObject[] itemDisplays;
    public GameObject[] upgradeDisplays;
    public TextMeshProUGUI[] ammo;

    public void ChangeScreens(int index)
    {
        foreach (GameObject scr in screens)
        {
            scr.SetActive(false);
        }

        switch(index)
        {
            case 1:
                foreach (GameObject g in itemDisplays)
                {
                    for(int i = 0; i < g.transform.childCount; i++)
                    {
                        g.transform.GetChild(i).gameObject.SetActive(false);
                    }

                    g.GetComponent<Image>().color = new Vector4(1, 1, 1, 0.1f);
                }

                foreach (GameObject g in upgradeDisplays)
                {
                    for (int i = 0; i < g.transform.childCount; i++)
                    {
                        g.transform.GetChild(i).gameObject.SetActive(false);
                    }

                    g.GetComponent<Image>().color = new Vector4(1, 1, 1, 0.1f);
                }

                for (int i = 0; i < pb.equipmentUnlocked.Length; i++)
                {
                    if (pb.equipmentUnlocked[i])
                    {
                        itemDisplays[i].SetActive(true);
                        itemDisplays[i].GetComponent<Image>().color = new Vector4(1, 1, 1, 0.4f);
                        upgradeDisplays[i].SetActive(true);
                        upgradeDisplays[i].GetComponent<Image>().color = new Vector4(1, 1, 1, 0.4f);

                        for (int j = 0; j < itemDisplays[i].transform.childCount; j++)
                        {
                            itemDisplays[i].transform.GetChild(j).gameObject.SetActive(true);
                        }

                        upgradeDisplays[i].transform.GetChild(0).gameObject.SetActive(pb.equipment[i].GetComponent<WeaponBehavior>().modOne);
                        upgradeDisplays[i].transform.GetChild(1).gameObject.SetActive(pb.equipment[i].GetComponent<WeaponBehavior>().modTwo);
                        upgradeDisplays[i].transform.GetChild(2).gameObject.SetActive(true);

                        if (i != 0)
                        {
                            ammo[i - 1].text = pb.equipment[i].GetComponent<WeaponBehavior>().ammo + "/" + pb.equipment[i].GetComponent<WeaponBehavior>().totalAmmo;
                        }
                    }
                }

                break;
            case 2:
                currentJournal = 0;
                ChangeJournal(currentJournal);
                break;
        }

        screens[index].SetActive(true);
    }

    public void TogglePDA(bool toggle, bool skip = false)
    {
        if(toggle)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        foreach (GameObject scr in screens)
        {
            scr.SetActive(false);
        }

        if (toggle)
        {
            playerLastEquip = pb.currentEquip;
            ChangeScreens(0);
        }
        else
        {
            transform.localPosition = startPos - Vector3.up * 0.7f;
        }

        if(!skip)
        {
            pb.ChangeEquipment(toggle || !playerLastEquip ? -1 : pb.equipment.IndexOf(playerLastEquip));
        }

        PDA.SetActive(toggle);
        open = toggle;
    }

    public GameObject leftArrow, rightArrow;
    public TextMeshProUGUI journalName, journalText, journalNumber;

    public void ChangeJournal(int dir)
    {
        OpenJournal(Mathf.Clamp(currentJournal + dir, 0, JournalBehavior.currentJournals.Count - 1));
    }

    public void OpenJournal(int number)
    {
        currentJournal = number;
        if (JournalBehavior.currentJournals.Count > 0)
        {
            journalName.text = JournalBehavior.currentJournals[number].name;
            journalText.text = JournalBehavior.currentJournals[number].getEntry();
            journalNumber.text = (number + 1) + "/" + JournalBehavior.currentJournals.Count;

            leftArrow.SetActive(number > 0);
            rightArrow.SetActive(number < JournalBehavior.currentJournals.Count - 1);
        }
        else
        {
            leftArrow.SetActive(false);
            rightArrow.SetActive(false);
            journalName.text = "";
            journalText.text = "You Have No Journals";
            journalNumber.text = "";
        }
    }

    public void OpenJournal(bool dir)
    {
        if(dir)
        {
            OpenJournal(++currentJournal);
        }
        else
        {
            OpenJournal(--currentJournal);
        }
    }

    public void ExitGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
