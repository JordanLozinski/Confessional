using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;
using TMPro;
public class Notebook : MonoBehaviour 
{
    public static Notebook instance;
    public List<string> lines;
    public GameObject notebookOverlay;

    public int leaveNotebookFrame = 0;
    Button btn;
    Image image;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        btn = GetComponent<Button>();
        image = GetComponent<Image>();
    }

    [YarnCommand("takenote")]
    public void TakeNote(string note)
    {
        // If button isnt showing, show it
        if (!image.enabled)
        {
            btn.interactable = true;
            image.enabled = true;
        }
        lines.Add(note);
        Debug.Log("Note added: " + note);
    }

    public void ShowNotebook()
    {
        StartCoroutine(DoShowNotebook());
    }

    public IEnumerator DoShowNotebook()
    {
        btn.interactable = false;
        image.enabled = false;
        
        notebookOverlay.SetActive(true);
        TextMeshProUGUI text = notebookOverlay.GetComponentInChildren<TextMeshProUGUI>();
        foreach (string line in lines)
        {
            text.text += line;
            text.text += "\n";
        }

        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }

        leaveNotebookFrame = Time.frameCount;
        text.text = "";
        notebookOverlay.SetActive(false);
        btn.interactable = true;
        image.enabled = true;
    }
}