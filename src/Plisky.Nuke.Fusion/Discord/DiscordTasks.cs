namespace Plisky.Nuke.Fusion;
using System;
using Flurl.Http;
using global::Nuke.Common.Tooling;



[Serializable]
public class DiscordSettings : ToolOptions {
    public string WebHookUrl { get; set; }

}


public class DiscordTasks : ToolTasks {

    public void SendNotification(DiscordSettings settings, string message) {

        var payload = new { content = message };

        settings.WebHookUrl.PostJsonAsync(payload);

    }

}

