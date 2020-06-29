using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;
using TMPro;

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
// Yarn dialog at all working: DONE
// Yarn choices working: DONE
// Yarn text speed tags: DONE
// Portraits: DONE
// Voice synthesis
// Yarn text effect tags

public class Dialog : Yarn.Unity.DialogueUIBehaviour {

	// If we want text effects, seems like TextMeshPro might be a place to start
	public TextMeshProUGUI textDisplay;

	// Option buttons
	public List<Button> optionButtons;

	const float TEXT_ADVANCE_LOCKOUT = 0.3f;
	const float DEFAULT_TYPING_SPEED = 0.03f;
	// Seconds per character 
	public float typingSpeed = DEFAULT_TYPING_SPEED;
	private DialogState ds = DialogState.Empty;
	private float timeSinceSkipRolling = -4.5f;

	Dictionary<string, CharacterData> characterDatas;

	// TODO: Seems like it would be nice if we could also click through dialog
	public string advanceDialogueKey = "x"; 

	public GameObject dialogueContainer;

	// Optional game object: Some kind of chevron (>>) at the bottom of the text display that indicates when the text is completely rolled out
	public GameObject continuePrompt;

	// Populate w/ name when non-Player talks
	public GameObject nameplate;

	CharacterData speaker = null;
	bool skipRolling = false;

	void Awake() {
		CharacterData[] cds = GameObject.Find("CharacterData").GetComponents<CharacterData>();
		characterDatas = new Dictionary<string, CharacterData>();
		foreach (CharacterData cd in cds)
		{
			characterDatas[cd.characterName] = cd;
		}

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
		Debug.Log("Dialogue started");
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

	private void tagHelper(string tag)
	{
		string[] tmp = tag.Split('=');
		if (tmp.Length != 2)
		{
			Debug.LogWarning("This shouldnt happen lmao");
		}
		switch (tmp[0])
		{
			case "typingSpeed":
				typingSpeed = float.Parse(tmp[1]);
				break;
			default:
				Debug.LogWarning("This shouldn't happen either");
				break;
		}
	}

	void Update() {
		if (Input.GetKeyDown(advanceDialogueKey) && skipRolling == false && ds == DialogState.Rolling)
		{
			skipRolling = true;
			timeSinceSkipRolling = Time.time;
		}
	}

	private IEnumerator DoRunLine(Yarn.Line line, ILineLocalisationProvider localisationProvider, System.Action onComplete)
	{
		Debug.Log("Beta");
		dialogueContainer.SetActive(true);
		ds = DialogState.Rolling;
		string text = localisationProvider.GetLocalisedTextForLine(line);
		// Yarn Spinner doesnt let you escape certain characters so we need to do workarounds like this :/
		text = Regex.Replace(text, "&", "#");

		textDisplay.text = "";
		int i = -1;
		int tagStartIndex = -1;
		int styleTagStartIndex = -1;
		var match = Regex.Match(text, "([A-z ]+): ");

		// Disable previous speaker's portrait.
		if (speaker != null)
			speaker.portraitUI.SetActive(false);

		// Get portrait with the captured group
		if (characterDatas.TryGetValue(match.Groups[1].Value, out speaker))
		{
			speaker.portraitUI.SetActive(true);
		}
		else
		{
			Debug.LogWarning("Couldnt find portrait for character named " + match.Groups[1].Value);
		}

		// Chop off the front of text
		text = text.Substring(match.Length);

		if (match.Groups[1].Value == "Notebook")
		{
			Notebook.instance.TakeNote(text);
		}
		typingSpeed = DEFAULT_TYPING_SPEED;
		foreach (char letter in text)
		{
			i++;
			if (letter == '<')
			{
				styleTagStartIndex = i;
			}
			if (letter == '>')
			{
				styleTagStartIndex = -1;
			}
			if (letter == '~') // We're looking at 
			{
				tagStartIndex = i;
				
			}
			if (tagStartIndex != -1)
			{
				if (letter == '`')
				{ 
					tagHelper(text.Substring(tagStartIndex+1, i-tagStartIndex-1));
					tagStartIndex = -1;
				}
				continue;
			}
			textDisplay.text += letter;
			if ("aeiouyAEIOUY".IndexOf(letter) == -1)
			{
				//play a voice synthesis sound
			}
			if (!skipRolling)
			{
				if (letter != ' ' && styleTagStartIndex == -1) yield return new WaitForSeconds(typingSpeed);
			}
		}
		skipRolling = false;
		ds = DialogState.Rolled;

		// Show the continue prompt, if we have one
		// TODO: continue prompt

		Debug.Log(Time.time - timeSinceSkipRolling);
		while (!Input.GetKeyDown(advanceDialogueKey) || (Time.time - timeSinceSkipRolling < TEXT_ADVANCE_LOCKOUT))
			yield return null;

		yield return new WaitForEndOfFrame();

		speaker.portraitUI.SetActive(false);
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

		speaker.portraitUI.SetActive(true);
		foreach (var optionString in optionsCollection.Options)
		{
			optionButtons[i].gameObject.SetActive(true);

			optionButtons[i].onClick.RemoveAllListeners();
			optionButtons[i].onClick.AddListener(() => SelectOption(optionString.ID, selectOption));

			var optionText = localisationProvider.GetLocalisedTextForLine(optionString.Line);

			if (optionText == null)
			{
				Debug.LogWarning($"Option {optionString.Line.ID} doesn't have any localised text");
                optionText = optionString.Line.ID;
			}

			var unityText = optionButtons [i].GetComponentInChildren<TextMeshProUGUI> ();
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

		// TODO: THIS SEEMS KINDA SKETCH
		// speaker.portraitUI.SetActive(false);

		// Hide all the buttons
		foreach (var button in optionButtons)
		{
			button.gameObject.SetActive(false);
		}
	}

	public void SelectOption(int optionID, System.Action<int> optionSelectHandler)
	{
		if (ds != DialogState.Choices)
		{
			Debug.LogWarning("An option was selected, but the dialogue UI was not expecting it.");
            return;
		}
		ds = DialogState.Rolled;
		optionSelectHandler?.Invoke(optionID);
	}
}