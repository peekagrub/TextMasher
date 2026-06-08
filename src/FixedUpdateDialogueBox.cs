using System.Reflection;
using Modding;
using UnityEngine;

namespace TextMasher;

internal class FixedUpdateDialogueBox : MonoBehaviour {
    private DialogueBox dialogue;

    public PlayMakerFSM fsm;

    private FieldInfo fastTypingField;

    private void Start() {
        dialogue = gameObject.GetComponent<DialogueBox>();
        fastTypingField = typeof(DialogueBox).GetField("fastTyping", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    private void FixedUpdate() {
        bool fastTyping = (bool)fastTypingField.GetValue(dialogue);

        if (!fastTyping && TextMasher.IsActive(dialogue)) {
            dialogue.Invoke("SpeedupTypewriter", 1f / 30);
        }
    }

    public void ClosePage()
    {
        fsm.SendEvent("NEXT");
    }
}
