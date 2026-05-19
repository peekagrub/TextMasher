using System.Reflection;
using Modding;
using UnityEngine;

namespace TextMasher;

internal class FixedUpdateDialogueBox : MonoBehaviour {
    private DialogueBox dialogue;

    private FieldInfo normalRevealSpeedField;
    private FieldInfo typingField;

    private MethodInfo typewriteCurrentPageMethod;

    private void Start() {
        dialogue = gameObject.GetComponent<DialogueBox>();
        normalRevealSpeedField = typeof(DialogueBox).GetField("normalRevealSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
        typingField = typeof(DialogueBox).GetField("typing", BindingFlags.NonPublic | BindingFlags.Instance);
        typewriteCurrentPageMethod = typeof(DialogueBox).GetMethod("TypewriteCurrentPage", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    private void FixedUpdate() {
        bool typing = (bool)typingField.GetValue(dialogue);

        if (TextMasher.IsActive(dialogue)) {
            if (dialogue.revealSpeed != 146 && typing) {
                dialogue.revealSpeed = 146;
                normalRevealSpeedField.SetValue(dialogue, dialogue.revealSpeed);
                dialogue.StartCoroutine(typewriteCurrentPageMethod.Name, 0);
            }
        } else {
            if (dialogue.revealSpeed != 65 && typing) {
                dialogue.revealSpeed = 65;
                normalRevealSpeedField.SetValue(dialogue, dialogue.revealSpeed);
                dialogue.StartCoroutine(typewriteCurrentPageMethod.Name, 0);
            }
        }
    }

}
