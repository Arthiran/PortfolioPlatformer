using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogController : MonoBehaviour
{
    [SerializeField]
    private GameObject DialogBox;
    [SerializeField]
    private TextMeshProUGUI DialogText;
    [SerializeField]
    private GameObject EnterImg;

    private PlayerController PC;
    private string[] TextBlocks;
    private int Index = -1;
    public float WriteSpeed;

    private void Awake()
    {
        PC = GetComponent<PlayerController>();
        DialogBox.SetActive(false);
        EnterImg.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            NextSentence();
        }
    }

    void NextSentence()
    {
        if (Index <= TextBlocks.Length - 2)
        {
            EnterImg.SetActive(false);
            StopAllCoroutines();
            DialogText.text = "";
            StartCoroutine(WriteSentences());
        }
        else
        {
            DialogBox.SetActive(false);
            PC.DialogOpen = false;
        }
    }

    IEnumerator WriteSentences()
    {
        Index++;
        for (int i = 0; i < TextBlocks[Index].Length; i++)
        {
            DialogText.text += TextBlocks[Index][i];
            yield return new WaitForSeconds(WriteSpeed);
        }
        EnterImg.SetActive(true);
    }

    public void InitDialog(string[] Blocks)
    {    
        TextBlocks = null;
        Index = -1;
        PC.DialogOpen = true;
        TextBlocks = Blocks;
        DialogBox.SetActive(true);
        NextSentence();
    }
}
