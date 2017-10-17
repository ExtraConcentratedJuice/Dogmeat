﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Dogmeat.Utilities;

namespace Dogmeat.Commands
{ 
    public class Purge : ModuleBase
    {
        [Command("purge"), Alias("prune"), Summary("Purges messages on executed channel.")]
        public async Task PurgeAsync([Summary("Number of messages to purge")] int count, [Summary("User to prune")] IGuildUser User = null)
        {
            if (!await Utilities.Commands.CommandMasterAsync(Context.Guild, Context.User, Context.Channel)) return;

            if (User == null)
            {
                Context.Channel.DeleteMessagesAsync(await Context.Channel.GetMessagesAsync(count).Flatten());

                IEnumerable<IMessage> DeleteMe =
                    new List<IMessage> {await Context.Channel.SendMessageAsync($"Purged {count} messages.")};

                Thread.Sleep(5000);

                Context.Channel.DeleteMessagesAsync(DeleteMe);
            }
            else
            {
                IEnumerable<IMessage> Messages = await Context.Channel.GetMessagesAsync(count).Flatten();
                IEnumerable<IMessage> UserMessages = Messages.Where(Message => Message.Author == User);

                Context.Channel.DeleteMessagesAsync(UserMessages);

                IEnumerable<IMessage> DeleteMe = new List<IMessage>
                {
                    await Context.Channel.SendMessageAsync($"Purged {count} messages from {User.Mention}")
                };

                Thread.Sleep(5000);
                
                Context.Channel.DeleteMessagesAsync(DeleteMe);
            }
        }
    }
}
