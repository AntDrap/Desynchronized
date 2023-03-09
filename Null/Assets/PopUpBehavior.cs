using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopUpBehavior : MonoBehaviour
{
    TextMeshProUGUI displayText;
    string currentWord;
    List<string> wordQueue;
    void Start()
    {
        displayText = GetComponent<TextMeshProUGUI>();
        wordQueue = new List<string>();
        displayText.text = "";
    }

    void Update()
    {
        if(wordQueue.Count > 0)
        {
            if(currentWord != wordQueue[0])
            {
                currentWord = wordQueue[0];
                StartCoroutine("startDisplay");
            }
        }
    }

    public void addWord(string word)
    {
        if(wordQueue.Contains(word))
        {
            return;
        }

        wordQueue.Add(word);
    }

    public IEnumerator startDisplay()
    {
        displayText.text = "";

        foreach (char c in currentWord)
        {
            displayText.text = displayText.text + c;
            yield return new WaitForSeconds(0.025f);
        }

        yield return new WaitForSeconds(1);

        while (displayText.text.Length > 0)
        {
            displayText.text = displayText.text.Substring(0, displayText.text.Length - 1);
            yield return new WaitForSeconds(0.0125f);
        }

        yield return new WaitForSeconds(0.5f);

        wordQueue.RemoveAt(0);
        wordQueue.TrimExcess();
        currentWord = "";
    }
}
