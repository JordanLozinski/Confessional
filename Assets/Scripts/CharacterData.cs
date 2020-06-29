using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;

public class CharacterData : MonoBehaviour
{
    public string characterName;
    public GameObject portraitUI;
    public string textColor = "#000000";
    // TODO: Voice data/params

    [YarnCommand("revealname")]
    public void RevealName(string[] name)
    {
        var sb = new System.Text.StringBuilder();
        foreach (string s in name)
        {
            sb.Append(s);
            sb.Append(" ");
        }
        var actualname = sb.ToString().Trim();
        if (actualname == characterName)
        {
            portraitUI.GetComponentInChildren<Text>().text = characterName;
        }
    }
}