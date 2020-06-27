using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Presumably we want to be able to offer a choice of what to say at certain parts of a dialog. 

// State for our dialog box. 
// Empty = no text displayed, Rolling = currently rolling out the text,
// Rolled = text is fully rolled out
enum DialogState
{
	Empty = 0,
	Rolling = 1,
	Rolled = 2
}

// Roadmap:
// Yarn dialog at all working
// Yarn choices working
// Yarn text speed tags
// Yarn text effect tags

public class Dialog : Yarn.Unity.DialogueUIBehaviour {

	// If we want text effects, seems like TextMeshPro might be a place to start
	public Text textDisplay;

	// String for each text box worth of text. 
	public string[] paragraphs;

	// Current index in paragraphs array
	private int index = -1;

	// Seconds per character 
	public float typingSpeed = 0.1f;
	private DialogState ds = DialogState.Empty;

	// TODO: Seems like it would be nice if we could also click through dialog
	public string advanceDialogueKey = "x"; 
	private IEnumerator thread = null;


	// Call this when you want to begin displaying the first paragraph.
	public void BeginDialogue()
	{
		// TODO: Have this object own all the UI assets associated with this dialog
		index = 0;
		thread = Type();
		StartCoroutine(thread);
	}

	public void Update()
	{
		if (Input.GetKeyDown(advanceDialogueKey))
		{
			// TODO Add sounds for going through text
			if (ds == DialogState.Rolling)
			{
				// Instead of rolling out the whole text, we could just make the text roll out faster.
				// If you want that effect instead, replace the code in this block with this line:
				// typingSpeed = 0.3f;


				// The coroutine will break out when it sees ds isn't Rolling
				ds = DialogState.Rolled;
				textDisplay.text = paragraphs[index];
			}
			else if (ds == DialogState.Rolled)
			{
				// Advance paragraph
				index++;
				if (index < paragraphs.length)
				{
					thread = Type();
					StartCoroutine(thread);
				}
				else
				{
					// Dialog is over
				}
			}
		}
	}

	// Roll out the text
	IEnumerator Type()
	{
		ds = DialogState.Rolling;
		textDisplay.text = "";
		foreach(char letter in paragraphs[index].ToCharArray())
		{
			yield return new WaitForSeconds(typingSpeed);	
			// Escape the coroutine if DialogState isn't Rolling		
			if (ds != DialogState.Rolling) yield break;
			textDisplay.text += letter;
		}
		ds = DialogState.Rolled;
	}
}