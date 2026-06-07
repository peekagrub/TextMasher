using System.Reflection;
using Modding;

namespace TextMasher;

public class TextMasher : Mod
{
    new public string GetName() => "TextMasher";
    public override string GetVersion() => "1.0.0.0";

    private static FieldInfo hiddenField;
    private static FieldInfo proxyFSMField;

    private FixedUpdateDialogueBox fixedUpdate;

    public override void Initialize () {
        hiddenField = typeof(DialogueBox).GetField("hidden", BindingFlags.NonPublic | BindingFlags.Instance);
        proxyFSMField = typeof(DialogueBox).GetField("proxyFSM", BindingFlags.NonPublic | BindingFlags.Instance);

        On.DialogueBox.ShowPage += OnShowPage;
        On.DialogueBox.Start += OnDialogueBoxStart;
        On.DialogueBox.SendEndEvent += OnSendEndEvent;
    }

    internal static bool IsActive(DialogueBox self, bool log = false) {
        HeroActions actions = GameManager.instance.inputHandler.inputActions;
        bool hidden = (bool)hiddenField.GetValue(self);
        bool actionsEval = (actions.attack.IsPressed || actions.jump.IsPressed || actions.cast.IsPressed);
        return !hidden && actionsEval;
    }

    private void OnDialogueBoxStart(On.DialogueBox.orig_Start orig, DialogueBox self) {
        orig(self);
        fixedUpdate = self.gameObject.AddComponent<FixedUpdateDialogueBox>();
    }

    private void OnShowPage(On.DialogueBox.orig_ShowPage orig, DialogueBox self, int pageNum) {
        orig(self, pageNum);

        if (IsActive(self))
        {
            self.Invoke("SpeedupTypewriter", 1f / 30);
        }
    }

    private void OnSendEndEvent(On.DialogueBox.orig_SendEndEvent orig, DialogueBox self) {
        orig(self);

        if (IsActive(self))
        {
            fixedUpdate.fsm = (PlayMakerFSM)proxyFSMField.GetValue(self);
            fixedUpdate.Invoke("ClosePage", 1f / 30);
        }
    }
}
