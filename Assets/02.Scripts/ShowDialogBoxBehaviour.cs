using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;

public class ShowDialogBoxBehaviour : PlayableBehaviour
{
    public string name;
    public List<DialogMessage> messages;
    public float duration;

    private bool hasPlayed = false;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
        if (!hasPlayed && Application.isPlaying) {
            hasPlayed = true;

            // SystemMessageManager¿« ShowDialogBox »£√‚
            var messageContents = new List<string>();
            foreach (var message in messages) {
                messageContents.Add($"<size={message.FontSize}>{message.Message}</size>");
            }

            SystemMessageManager.Instance?.ShowDialogBox(name, messageContents, duration).Forget();
        }
    }
}

[System.Serializable]
public class DialogMessage
{
    public string Message;
    public int FontSize;

    public DialogMessage(string message, int fontSize) {
        Message = message;
        FontSize = fontSize;
    }
}
