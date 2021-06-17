using Discord.Commands;
using System.Threading.Tasks;

namespace CardCollectiveBot.Console.Module
{
    public class Info : ModuleBase<SocketCommandContext>
    {
		[Command("about")]
		[Summary("Describes the Bots purpose.")]
		public Task AboutAsync()
			=> ReplyAsync("I am a collection of various card games for various numbers of players. Type !help for a list of games");

		[Command("help")]
		[Summary("Lists all base commands accompanied with descriptions")]
		public Task HelpAsync()
			=> ReplyAsync("Nothing here yet");
	}
}
