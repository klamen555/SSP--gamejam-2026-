using UnityEngine;
using UnityEngine.Events;

[System.Serializable] public class StringStringUnityEvent : UnityEvent<string, string> { }

/// <summary>
/// Plays a DialogueSequence. Your dialogue UI hooks into the UnityEvents
/// below (e.g. onLineDisplayed to show text, onChoicePresented to show
/// two buttons). Call SelectOption(0 or 1) from those buttons.
///
/// Other gameplay systems can call Pause()/Resume() to freeze dialogue
/// mid-conversation (e.g. a cutscene interrupts, or the player needs to
/// do something else) without losing their place, or Stop() to end it
/// outright.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Events")]
    public UnityEvent onDialogueStarted;
    public UnityEvent onDialogueEnded;
    public UnityEvent onDialoguePaused;
    public UnityEvent onDialogueResumed;

    [Tooltip("Invoked with (speakerName, text) whenever a Line node is shown.")]
    public StringStringUnityEvent onLineDisplayed;

    [Tooltip("Invoked with (optionAText, optionBText) whenever a Choice node is shown.")]
    public StringStringUnityEvent onChoicePresented;

    private DialogueSequence currentSequence;
    private int currentNodeIndex;

    public bool IsPlaying { get; private set; }
    public bool IsPaused { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>Begin playing a sequence from the start.</summary>
    public void StartDialogue(DialogueSequence sequence)
    {
        if (sequence == null || sequence.nodes.Count == 0)
        {
            Debug.LogWarning("DialogueManager: tried to start an empty or null sequence.");
            return;
        }

        currentSequence = sequence;
        currentNodeIndex = 0;
        IsPlaying = true;
        IsPaused = false;

        onDialogueStarted?.Invoke();
        ShowCurrentNode();
    }

    /// <summary>Call this from your UI's "next line" input (e.g. a click/tap/button).</summary>
    public void Advance()
    {
        if (!IsPlaying || IsPaused) return;

        DialogueNode current = currentSequence.nodes[currentNodeIndex];
        if (current.type == DialogueNodeType.Choice) return; // choices advance via SelectOption

        GoToNode(currentNodeIndex + 1);
    }

    /// <summary>Call this from your choice UI. optionIndex is 0 for A, 1 for B.</summary>
    public void SelectOption(int optionIndex)
    {
        if (!IsPlaying || IsPaused) return;

        DialogueNode current = currentSequence.nodes[currentNodeIndex];
        if (current.type != DialogueNodeType.Choice) return;

        if (optionIndex == 0)
        {
            current.onOptionASelected?.Invoke();
            GoToNode(current.optionANextIndex >= 0 ? current.optionANextIndex : currentNodeIndex + 1);
        }
        else
        {
            current.onOptionBSelected?.Invoke();
            GoToNode(current.optionBNextIndex >= 0 ? current.optionBNextIndex : currentNodeIndex + 1);
        }
    }

    /// <summary>Freeze dialogue in place (e.g. gameplay needs to take over) without losing progress.</summary>
    public void Pause()
    {
        if (!IsPlaying || IsPaused) return;
        IsPaused = true;
        onDialoguePaused?.Invoke();
    }

    /// <summary>Resume exactly where it left off.</summary>
    public void Resume()
    {
        if (!IsPlaying || !IsPaused) return;
        IsPaused = false;
        onDialogueResumed?.Invoke();
        ShowCurrentNode(); // re-fire the current node's event in case UI was hidden while paused
    }

    /// <summary>End the dialogue outright (not resumable — starts fresh next time).</summary>
    public void Stop()
    {
        IsPlaying = false;
        IsPaused = false;
        currentSequence = null;
        currentNodeIndex = 0;
        onDialogueEnded?.Invoke();
    }

    private void GoToNode(int index)
    {
        if (index >= currentSequence.nodes.Count)
        {
            IsPlaying = false;
            onDialogueEnded?.Invoke();
            return;
        }

        currentNodeIndex = index;
        ShowCurrentNode();
    }

    private void ShowCurrentNode()
    {
        DialogueNode node = currentSequence.nodes[currentNodeIndex];

        switch (node.type)
        {
            case DialogueNodeType.Line:
                onLineDisplayed?.Invoke(node.speakerName, node.text);
                break;

            case DialogueNodeType.Choice:
                onChoicePresented?.Invoke(node.optionAText, node.optionBText);
                break;
        }
    }
}
