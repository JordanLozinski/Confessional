using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;

// TODO: Presumably we want to be able to offer a choice of what to say at certain parts of a dialog. 

// State for our dialog box. 
// Empty = no text displayed, Rolling = currently rolling out the text,
// Rolled = text is fully rolled out
// Choics = text is fully rolled out, and we are displaying some choice buttons to the player.
enum DialogState
{
	Empty = 0,
	Rolling = 1,
	Rolled = 2,
	Choices = 3
}

// Roadmap:
// Yarn dialog at all working
// Yarn choices working
// Yarn text speed tags
// Portraits
// Yarn text effect tags
// Voice synthesis

public class Dialog : Yarn.Unity.DialogueUIBehaviour {

	// If we want text effects, seems like TextMeshPro might be a place to start
	public Text textDisplay;

	// Option buttons
	public List<Button> optionButtons;

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

	public GameObject dialogueContainer;

	// Optional game object: Some kind of chevron (>>) at the bottom of the text display that indicates when the text is completely rolled out
	public GameObject continuePrompt;



	void Awake() {
		if (dialogueContainer != null)
			dialogueContainer.SetActive(false);

		foreach (var button in optionButtons) {
			button.gameObject.SetActive (false);
		}
		
		// TODO: May have to set text object and button objects to inactive as well
	}


	// Call this when you want to begin displaying the first paragraph.
	public override void DialogueStarted()
	{
		// TODO: Have this object own all the UI assets associated with this dialog
		dialogueContainer.SetActive(true);
	}

	public override void DialogueComplete()
	{
		if (dialogueContainer != null)
			dialogueContainer.SetActive(false);
	}


	public override Dialogue.HandlerExecutionType RunLine (Yarn.Line line, ILineLocalisationProvider localisationProvider, System.Action onLineComplete)
	{
		// Start displaying the line; it will call onComplete later
		// which will tell the dialogue to continue
		StartCoroutine(DoRunLine(line, localisationProvider, onLineComplete));
		return Dialogue.HandlerExecutionType.PauseExecution;
	}

	public override Dialogue.HandlerExecutionType RunCommand (Yarn.Command command, System.Action onCommandComplete) {
		// Dispatch this command via the 'On Command' handler.
		// onCommand?.Invoke(command.Text);

		// Signal to the DialogueRunner that it should continue
		// executing. (This implementation of RunCommand always signals
		// that execution should continue, and never calls
		// onCommandComplete.)
		return Dialogue.HandlerExecutionType.ContinueExecution;
	}

	private IEnumerator DoRunLine(Yarn.Line line, ILineLocalisationProvider localisationProvider, System.Action onComplete)
	{
		Debug.Log("Beta");
		dialogueContainer.SetActive(true);
		ds = DialogState.Rolling;
		string text = localisationProvider.GetLocalisedTextForLine(line);
		textDisplay.text = "";
		foreach (char letter in text)
		{
			
			textDisplay.text += letter;
			if (!Input.GetKeyDown(advanceDialogueKey))
			{
				if (letter != ' ') yield return new WaitForSeconds(typingSpeed);
			}
			else
			{
				textDisplay.text = text;
				break;
			}
		}
		ds = DialogState.Rolled;

		// Show the continue prompt, if we have one
		// TODO: continue prompt

		while (Input.GetKeyDown(advanceDialogueKey) == false)
			yield return null;

		yield return new WaitForEndOfFrame();

		// Remove the continue prompt if we have one


		onComplete();
	}

	
	public override void RunOptions (Yarn.OptionSet optionSet, ILineLocalisationProvider localisationProvider, System.Action<int> onOptionSelected) {
		StartCoroutine(DoRunOptions(optionSet, localisationProvider, onOptionSelected));
	}

    private IEnumerator DoRunOptions (Yarn.OptionSet optionsCollection, ILineLocalisationProvider localisationProvider, System.Action<int> selectOption)
	{
		if (optionsCollection.Options.Length > optionButtons.Count)
		{
			Debug.LogWarning("There arent enough buttons for the amount of choices we need to display. This will cause problems");
		}

		int i = 0;
		
		ds = DialogState.Choices;

		foreach (var optionString in optionsCollection.Options)
		{
			optionButtons[i].gameObject.SetActive(true);

			optionButtons[i].onClick.RemoveAllListeners();
			optionButtons[i].onClick.AddListener(() => SelectOption(optionString.ID));

			var optionText = localisationProvider.GetLocalisedTextForLine(optionString.Line);

			if (optionText == null)
			{
				Debug.LogWarning($"Option {optionString.Line.ID} doesn't have any localised text");
                optionText = optionString.Line.ID;
			}

			var unityText = optionButtons [i].GetComponentInChildren<Text> ();
			if (unityText != null) {
				unityText.text = optionText;
			}
			i++;
		}

		while (ds == DialogState.Choices)
		{
			yield return null;
		}
		Debug.Log("Alpha");

		// Hide all the buttons
		foreach (var button in optionButtons)
		{
			button.gameObject.SetActive(false);
		}
	}

	public void SelectOption(int optionID)
	{
		if (ds != DialogState.Choices)
		{
			Debug.LogWarning("An option was selected, but the dialogue UI was not expecting it.");
            return;
		}
		ds = DialogState.Rolled;
	}
}