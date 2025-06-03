using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using DSharpPlus;

string GetEnv(string key) {
    return Environment.GetEnvironmentVariable(key) ?? throw new Exception($"Missing environment variable {key}");
}

var format = @"can you believe it guys? {0}! just tomorrow away! {0} is in tomorrow! woohoo! i am so happy about this information! {0} just tomorrow away, oh wow! can you believe it? tomorrow! just in tomorrow! it got here so fast! tomorrow! just t";
var date = DateTime.ParseExact(GetEnv("DATE"), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
var regex = new Regex(GetEnv("REGEX"), RegexOptions.IgnoreCase);
var webfishing = GetEnv("WEBFISHING");

var discord = new DiscordClient(new DiscordConfiguration() {
    Token = GetEnv("DISCORD_TOKEN"),
    TokenType = TokenType.Bot,
    Intents = DiscordIntents.GuildMessages | DiscordIntents.DirectMessages | DiscordIntents.MessageContents,
});

discord.MessageCreated += async (_, eventArgs) => {
    Console.WriteLine(eventArgs.Message.Content);
    if (!eventArgs.Author.IsCurrent && regex.IsMatch(eventArgs.Message.Content)) {
        var timeToWebfishing = date - DateTime.UtcNow;

        await eventArgs.Message.RespondAsync(string.Format(format, webfishing));
    }
};

await discord.ConnectAsync();

await Task.Delay(-1);
