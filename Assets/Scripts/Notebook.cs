using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn;
using Yarn.Unity;
public class Notebook : MonoBehaviour 
{
    public List<string> lines;

    [YarnCommand("takenote")]
    public void TakeNote(string note)
    {
        lines.Add(note);
        Debug.Log("Note added: " + note);
    }
}