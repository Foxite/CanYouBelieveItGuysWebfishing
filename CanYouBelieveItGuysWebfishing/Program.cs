using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using DSharpPlus;

string GetEnv(string key) {
	return Environment.GetEnvironmentVariable(key) ?? throw new Exception($"Missing environment variable {key}");
}

var format = @"can you believe it guys? {0}! just {1} {2} away! {0} is in {1} {2}! woohoo! i am so happy about this information! {0} just {1} {2} away, oh wow! can you believe it? {0}! just in {1} {2}! it got here so fast! {0}! just {1} {3}";
var date = DateTime.ParseExact(GetEnv("DATE"), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
var regex = new Regex(GetEnv("REGEX"), RegexOptions.IgnoreCase);
var webfishing = GetEnv("WEBFISHING");
var probability = double.Parse(Environment.GetEnvironmentVariable("PROBABILITY") ?? "1");
var cooldown = TimeSpan.FromSeconds(int.Parse(Environment.GetEnvironmentVariable("COOLDOWN_SECONDS") ?? "0"));
ulong[] ignoreChannels = (Environment.GetEnvironmentVariable("IGNORE_CHANNELS") ?? "").Split(";").Select(ulong.Parse).ToArray();

// by channel id
var lastResponses = new Dictionary<ulong, DateTime>();
var random = new Random();
var discord = new DiscordClient(new DiscordConfiguration() {
	Token = GetEnv("DISCORD_TOKEN"),
	TokenType = TokenType.Bot,
	Intents = DiscordIntents.GuildMessages | DiscordIntents.DirectMessages | DiscordIntents.MessageContents,
});

discord.MessageCreated += async (_, eventArgs) => {
	TimeSpan timeToWebfishing = date - DateTime.UtcNow;
	if (timeToWebfishing.TotalSeconds < 0) {
		return;
	}

	if (ignoreChannels.Contains(eventArgs.Channel.Id)) {
		return;
	}

	if (eventArgs.Author.IsCurrent || !regex.IsMatch(eventArgs.Message.Content)) {
		return;
	}

	if (random.NextDouble() > probability) {
		return;
	}

	if (lastResponses.TryGetValue(eventArgs.Channel.Id, out DateTime lastResponse) && (DateTime.UtcNow - lastResponse) < cooldown) {
		return;
	}

	lastResponses[eventArgs.Channel.Id] = DateTime.UtcNow;
	
	int timeQuantity;
	string? timeQuantityString = null;
	string timeUnit;
	if (timeToWebfishing.TotalHours < 24) {
		timeQuantity = (int) timeToWebfishing.TotalHours;
		timeUnit = "hour";

		if (timeQuantity == 1) {
			timeQuantityString = "an";
		}
	} else if (timeToWebfishing.TotalDays < 6) { // on purpose, say "a week" if it's 6 days
		timeQuantity = (int) timeToWebfishing.TotalDays;
		timeUnit = "day";

		if (timeQuantity == 1) {
			timeQuantityString = "a";
		}
	} else {
		timeQuantity = (int) Math.Round(timeToWebfishing.TotalDays / 7);
		if (timeQuantity == 0) {
			timeQuantity = 1;
		}
		timeUnit = "week";

		if (timeQuantity == 1) {
			timeQuantityString = "a";
		}
	}

	if (timeQuantity > 1) {
		timeUnit += "s";
	}

	string timeUnitCutOff = timeUnit[0].ToString();

	timeQuantityString ??= timeQuantity.ToString();

	await eventArgs.Message.RespondAsync(string.Format(format, webfishing, timeQuantityString, timeUnit, timeUnitCutOff));
};

await discord.ConnectAsync();

await Task.Delay(-1);
