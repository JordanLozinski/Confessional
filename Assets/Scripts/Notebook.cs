using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;
public class Notebook : MonoBehaviour 
{
    public List<string> lines;

    [YarnCommand("takenote")]
    public void TakeNote(string note)
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        lines.Add(note);
        Debug.Log("Note added: " + note);
    }

    public void ShowNotebook()
    {
        GetComponent<Button>().interactable = false;
        GetComponent<Image>().enabled = false;
        // Display notebook view
        // Render text on it
        GetComponent<Button>().interactable = true;
        GetComponent<Image>().enabled = true;
    }
}