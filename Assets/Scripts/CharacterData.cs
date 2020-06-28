using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterData : MonoBehaviour
{
    public string characterName;
    public GameObject portraitUI;
    public string textColor = "#000000";
    // TODO: Voice data/params

    [YarnCommand("revealname")]
    public void RevealName(string name)
    {
        if (name == characterName)
        {
            portraitUI.GetComponentInChildren<Text>().text = characterName;
        }
    }
}