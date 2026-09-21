using Flurl.Http.Testing;

namespace Fallout.Fusion.Test;

public class DiscordTasksTests {
    [Fact]
    public void SendNotification_ShouldPostToConfiguredWebhook() {
        using var httpTest = new HttpTest();
        var settings = new DiscordSettings {
            WebHookUrl = "https://discord.example/webhook"
        };

        DiscordTasks.SendNotification(settings, "Build completed.");

        httpTest.ShouldHaveCalled("https://discord.example/webhook")
            .WithVerb(HttpMethod.Post);
    }

    [Fact]
    public void SendNotification_ShouldSendMessageAsContent() {
        using var httpTest = new HttpTest();
        var settings = new DiscordSettings {
            WebHookUrl = "https://discord.example/webhook"
        };

        DiscordTasks.SendNotification(settings, "Build completed: 100%.");

        httpTest.ShouldHaveCalled("https://discord.example/webhook")
            .WithRequestJson(new {
                content = "Build completed: 100%."
            });
    }
}
