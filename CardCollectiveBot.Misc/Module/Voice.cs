using Discord;
using Discord.Audio;
using Discord.Audio.Streams;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CardCollectiveBot.Misc
{
    public class Voice : ModuleBase<SocketCommandContext>
    {
        [Command("canigeta", RunMode = RunMode.Async)]
        public async Task JoinChannel(IVoiceChannel channel = null)
        {
            channel = await AssignChannel(channel);

            var audioClient = await channel.ConnectAsync();

            await SendAsync(audioClient, "C:\\Users\\njack\\Desktop\\audioCues\\uhyeah2.ogg");

            await channel.DisconnectAsync();
        }

        [Command("shush", RunMode = RunMode.Async)]
        public async Task StartShush(IVoiceChannel channel = null)
        {
            channel = await AssignChannel(channel);

            var audioClient = await channel.ConnectAsync();

            var discord = await SendAsync(audioClient, "C:\\Users\\njack\\Desktop\\audioCues\\activation.ogg");

            var x = channel.GetUsersAsync();

            var y = (await x.FlattenAsync()).Where(e => !e.IsBot).Select(e => (e as SocketGuildUser)).ToList();

            y.ForEach(async e => await ListenUserAsync((InputStream)e.AudioStream, discord, audioClient));
        }

        [Command("stop", RunMode = RunMode.Async)]
        public async Task Stop(IVoiceChannel channel = null)
        {
            channel = await AssignChannel(channel);

            await channel.DisconnectAsync();
        }

        private async Task ListenUserAsync(InputStream userAudioStream, AudioOutStream discord, IAudioClient client)
        {
            while (true)
            {
                if (userAudioStream.AvailableFrames > 0)
                {
                    var task = SendAsync(client, "C:\\Users\\njack\\Desktop\\audioCues\\shush.ogg", discord);

                    Thread.Sleep(500);

                    await task;
                }

                for (; userAudioStream.AvailableFrames > 0;)
                {
                    await userAudioStream.ReadFrameAsync(CancellationToken.None);
                }
            }
        }

        private async Task<IVoiceChannel> AssignChannel(IVoiceChannel channel)
        {
            channel ??= (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null) { await Context.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); }
            return channel;
        }

        private Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }

        private async Task<AudioOutStream> SendAsync(IAudioClient client, string path, AudioOutStream discord = null)
        {
            using var ffmpeg = CreateStream(path);
            using var output = ffmpeg.StandardOutput.BaseStream;
            discord ??= client.CreatePCMStream(AudioApplication.Mixed);
            try
            {
                await output.CopyToAsync(discord);
            }
            finally { await discord.FlushAsync(); }

            return discord;
        }
    }
}
