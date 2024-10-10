using System.Text.RegularExpressions;
using DSharpPlus;

var format = @"can you believe it guys? webfishing! just {0} hours away!
webfishing is in {0} hours! woohoo! i am so happy about this information!
webfishing just 2 days away, oh wow! can you believe it? webfishing!
just in {0} hours! it got here so fast! webfishing! just {0} h";

var regex = new Regex("web ?fishing", RegexOptions.IgnoreCase);

var discord = new DiscordClient(new DiscordConfiguration() {
    Token = Environment.GetEnvironmentVariable("DISCORD_TOKEN"),
    TokenType = TokenType.Bot,
    Intents = DiscordIntents.GuildMessages | DiscordIntents.DirectMessages | DiscordIntents.MessageContents,
});

discord.MessageCreated += async (_, eventArgs) => {
    if (!eventArgs.Author.IsCurrent && regex.IsMatch(eventArgs.Message.Content)) {
        var webfishing = new DateTime(2024, 10, 12, 1, 0, 0, DateTimeKind.Utc);
        var timeToWebfishing = webfishing - DateTime.UtcNow;

        await eventArgs.Message.RespondAsync(string.Format(format, (int) timeToWebfishing.TotalHours));
    }
};

await discord.ConnectAsync();

await Task.Delay(-1);
