using System.Reflection;
using Modding;

namespace TextMasher;

public class TextMasher : Mod
{
    new public string GetName() => "TextMasher";
    public override string GetVersion() => "1.0.0.0";

    private static FieldInfo hiddenField;
    private FieldInfo proxyFSMField;

    private FixedUpdateDialogueBox fixedUpdate;

    public override void Initialize () {
        hiddenField = typeof(DialogueBox).GetField("hidden", BindingFlags.NonPublic | BindingFlags.Instance);
        proxyFSMField = typeof(DialogueBox).GetField("proxyFSM", BindingFlags.NonPublic | BindingFlags.Instance);

        On.DialogueBox.Start += OnDialogueBoxStart;
        On.DialogueBox.SendEndEvent += OnSendEndEvent;
        On.DialogueBox.SpeedupTypewriter += OnSpeedupTypewriter;
    }

    internal static bool IsActive(DialogueBox self, bool log = false) {
        HeroActions actions = GameManager.instance.inputHandler.inputActions;
        bool hidden = (bool)hiddenField.GetValue(self);
        bool actionsEval = (actions.attack.IsPressed || actions.jump.IsPressed || actions.cast.IsPressed);
        return !hidden && actionsEval;
    }

    private void OnDialogueBoxStart (On.DialogueBox.orig_Start orig, DialogueBox self) {
        orig(self);
        fixedUpdate = self.gameObject.AddComponent<FixedUpdateDialogueBox>();
    }

    private void OnSendEndEvent(On.DialogueBox.orig_SendEndEvent orig, DialogueBox self) {
        orig(self);
        if (IsActive(self)) {
            PlayMakerFSM proxyFSFM = (PlayMakerFSM)proxyFSMField.GetValue(self);
            proxyFSFM.SendEvent("NEXT");
        }
    }
    
    private void OnSpeedupTypewriter(On.DialogueBox.orig_SpeedupTypewriter orig, DialogueBox self) {
        if (!IsActive(self)) {
            orig(self);
        }
    }
}
