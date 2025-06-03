using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using DSharpPlus;

string GetEnv(string key) {
    return Environment.GetEnvironmentVariable(key) ?? throw new Exception($"Missing environment variable {key}");
}

var format = @"can you believe it guys? {2}! just {0} hours away! {2} is in {0} hours! woohoo! i am so happy about this information! {2} just {1} days away, oh wow! can you believe it? {2}! just in {0} hours! it got here so fast! {2}! just {0} h";
var date = DateTime.ParseExact(GetEnv("DATE"), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
var regex = new Regex(GetEnv("REGEX"), RegexOptions.IgnoreCase);
var webfishing = GetEnv("WEBFISHING");

var discord = new DiscordClient(new DiscordConfiguration() {
    Token = GetEnv("DISCORD_TOKEN"),
    TokenType = TokenType.Bot,
    Intents = DiscordIntents.GuildMessages | DiscordIntents.DirectMessages | DiscordIntents.MessageContents,
});

discord.MessageCreated += async (_, eventArgs) => {
    if (!eventArgs.Author.IsCurrent && regex.IsMatch(eventArgs.Message.Content)) {
        var timeToWebfishing = date - DateTime.UtcNow;

        await eventArgs.Message.RespondAsync(string.Format(format, (int) timeToWebfishing.TotalHours, Environment.GetEnvironmentVariable("JUSTAWEEKAWAY") ?? $"just {Math.Ceiling(timeToWebfishing.TotalDays)} hours away", webfishing));
    }
};

await discord.ConnectAsync();

await Task.Delay(-1);
