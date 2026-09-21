using Flurl.Http;
using global::Fallout.Common.Tooling;

namespace Plisky.Fallout.Fusion;

[Serializable]
public class DiscordSettings : ToolOptions {
    public required string WebHookUrl { get; set; }
}


public class DiscordTasks : ToolTasks {

    public static void SendNotification(DiscordSettings settings, string message) {

        var payload = new { content = message };

        settings.WebHookUrl.PostJsonAsync(payload);
    }
}
