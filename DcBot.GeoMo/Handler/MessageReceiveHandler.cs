using DcBot.Common.MessageHandler;
using DcBot.Common.PermissionHandler;
using DcBot.Common.PrefixHandler;
using DcBot.Core.Core;
using DcBot.Service.Interfaces;
using DcBot.Service.Services;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DcBot.GeoMo.Handler
{
    public class MessageReceiveHandler : ModuleBase<SocketCommandContext>
    {
        private readonly IMessageControl _messageControl;
        private readonly IPrefixControl _prefixControl;
        private readonly CommandService _commandService;
        private readonly IUserService _userService;
        private readonly ISkillService _skillService;
        private readonly IDcServerService _dcServerService;

        public MessageReceiveHandler(IMessageControl messageControl, IPrefixControl prefixControl, CommandService commandService, IUserService userService, ISkillService skillService, IDcServerService dcServerService)
        {
            _messageControl = messageControl;
            _prefixControl = prefixControl;
            _commandService = commandService;
            _userService = userService;
            _skillService = skillService;
            _dcServerService = dcServerService;
        }

        [Command("mhelp")]
        [Summary("Yardım")]
        [PermissionControl(GuildPermission.SendMessages)]
        public async Task HelpCommand()
        {
            await _prefixControl.GetHelpCommands(Context);
        }

        [Command("gupg")]
        [Summary("Yetenek Geliştir")]
        [PermissionControl(GuildPermission.SendMessages)]
        public async Task EarnXp(string xpName, int xpAmount)
        {
            var user = await _userService.FirstOrDefaultAsync(x => x.DiscordId == Context.User.Id.ToString());

            var skillName = xpName.ToLower() == "gm" ? "GM" : (xpName.ToLower() == "mc" ? "MC" : null);

            if (skillName == null)
            {
                await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "astonished", $"Geliştireceğiniz Yeteneğin Adını Lütfen Doğru Giriniz. Örneğin : `gm` , `mc`"));
                return;
            }

            var skill = await _skillService.FirstOrDefaultAsync(x => x.Name == skillName && x.DiscordId == user.DiscordId);

            skill.Xp += xpAmount;
            user.Money -= xpAmount;

            while (skill.Xp >= skill.XpRequired)
            {
                skill.Xp -= skill.XpRequired;
                skill.Level++;

                Random random = new Random();

                skill.XpRequired += random.Next((int)skill.Xp, (int)(skill.Level * skill.Xp));

                if (skill.Name.ToLower() == "gm")
                {
                    skill.DailyGmCount++;
                    skill.GmCount++;
                }
                else if (skill.Name.ToLower() == "mc")
                {
                    skill.CashAverageMin += random.Next(1, 7);
                    skill.CashAverageMax += random.Next(9, (int)(skill.Level * skill.CashAverageMin));
                }
            }

            await _skillService.UpdateAsync(skill);
            await _userService.UpdateAsync(user);

            if (skill.Name.ToLower() == "gm")
            {
                await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "crystal ball", $"Mevcut `Xp` Miktarınız: `{skill.Xp}/{skill.XpRequired}!` Seviyeniz: `{skill.Level}`, Yeni Günlük `GM` Kullanma Hakkınız: `{skill.GmCount}`"));
            }
            else if (skill.Name.ToLower() == "mc")
            {
                await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "crystal ball", $"Mevcut `Xp` Miktarınız: `{skill.Xp}/{skill.XpRequired}!` Seviyeniz: `{skill.Level}`, Günlük `GM Cash` Kazanma Oranınız: `{skill.CashAverageMin}/{skill.CashAverageMax}!`"));
            }
        }

        [Command("gm")]
        [Summary("Para Kazan")]
        [PermissionControl(GuildPermission.SendMessages)]
        public async Task EarnMoney()
        {
            var user = await _userService.FirstOrDefaultAsync(x => x.DiscordId == Context.User.Id.ToString());
            var skillGM = await _skillService.FirstOrDefaultAsync(x => x.Name == "GM" && x.DiscordId == user.DiscordId);
            var skillMC = await _skillService.FirstOrDefaultAsync(x => x.Name == "MC" && x.DiscordId == user.DiscordId);

            if (skillGM.DailyGmCount <= 0)
            {
                await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "empty nest", $"Günlük `GM` Kullanma Hakkınız Kalmadı."));
                return;
            }

            int earnedMoney = new Random().Next((int)skillMC.CashAverageMin, (int)skillMC.CashAverageMax);
            user.Money += earnedMoney;
            skillGM.DailyGmCount--;

            await _skillService.UpdateAsync(skillGM);
            await _userService.UpdateAsync(user);
            await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "game die", $"GeoMo 'dan `{earnedMoney}` Para Kazandınız! Günlük GM Kullanma Hakkınız: `{skillGM.DailyGmCount}`"));
        }

        [Command("gk")]
        [Summary("Şans Oyunu Oyna")]
        [PermissionControl(GuildPermission.SendMessages)]
        public async Task CoinFlip(int betAmount, string choice = null)
        {
            bool isWin = false;

            var user = await _userService.FirstOrDefaultAsync(x => x.DiscordId == Context.User.Id.ToString());

            if (user.Money < betAmount)
            {
                await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "money with wings", $"Şans Oyunu İçin Yeterli Bakiyeniz Bulunmamaktadır. \n Bakiyeniz : `{user.Money}`"));
                return;
            }

            if (string.IsNullOrEmpty(choice))
            {
                choice = "t";
            }

            string result = new Random().Next(0, 2) == 0 ? "t" : "y";
            isWin = choice.ToLower() == result;

            user.Money += isWin ? betAmount * 2 : -betAmount;
            await _userService.UpdateAsync(user);
            string resultMessage = isWin ? "Kazandınız!" : "Kaybettiniz.";
            await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "game die", $"Yazı Tura Sonucu: `{result.ToUpper()}`. `{resultMessage}` Toplam Paranız: `{user.Money}`"));
        }

        [Command("gcash")]
        [Summary("Kasa Göster")]
        [PermissionControl(GuildPermission.SendMessages)]
        public async Task CheckBalance()
        {
            var user = await _userService.FirstOrDefaultAsync(x => x.DiscordId == Context.User.Id.ToString());

            await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "euro", $"Mevcut Para Miktarınız: `{user.Money}`"));
        }

        [Command("gxp")]
        [Summary("Yetenek Xp Göster")]
        [PermissionControl(GuildPermission.SendMessages)]
        public async Task CheckSkillXp()
        {
            var skillGm = await _skillService.FirstOrDefaultAsync(x => x.Name == "GM" && x.DiscordId == Context.User.Id.ToString());
            var skillMc = await _skillService.FirstOrDefaultAsync(x => x.Name == "MC" && x.DiscordId == Context.User.Id.ToString());

            await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "six pointed star", $"`GM Hak` - Xp : `{skillGm.Xp}/{skillGm.XpRequired}` | Level : `{skillGm.Level}` \n `MC Kazanç` - Xp : `{skillMc.Xp}/{skillMc.XpRequired}` | Level : `{skillMc.Level}`"), 15000);
        }

        [Command("gsend")]
        [Summary("Para Gönder")]
        [PermissionControl(GuildPermission.SendMessages)]
        public async Task CheckSkillXp(SocketGuildUser receiverUser, int amount)
        {
            var sender = await _userService.FirstOrDefaultAsync(x => x.DiscordId == Context.User.Id.ToString());
            var receiver = await _userService.FirstOrDefaultAsync(x => x.DiscordId == receiverUser.Id.ToString());

            if (sender.Money < amount)
            {
                await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "money with wings", $"Bu Miktarı Göndermek İçin Yeterli Bakiyeniz Bulunmamaktadır. \n Bakiyeniz : `{sender.Money}`"));

            }

            sender.Money -= amount;
            receiver.Money += amount;
            await _userService.UpdateAsync(sender);
            await _userService.UpdateAsync(receiver);
            await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "moneybag", $"{receiverUser.Mention}! `{sender.UserName}` Sana `{amount}` Kadar `GeoMo Cash` Gönderdi!"));
        }
        [Command("msync")]
        [Summary("GeoMo Senkronize")]
        [PermissionControl(GuildPermission.Administrator)]
        public async Task SyncCommand()
        {
            var dcServer = await _dcServerService.FirstOrDefaultAsync(x => x.DiscordId == Context.Guild.Id.ToString());

            var skillsData = await _skillService.ToListByFilterAsync(x => x.DcServerId == dcServer.Id);

            foreach (var item in skillsData)
            {
                await _skillService.DeleteAsync(item);
            }

            await Task.Delay(1500);

            var users = await _userService.ToListByFilterAsync(x => x.DcServerId == dcServer.Id);

            foreach (var user in users)
            {
                await _skillService.InsertAsync(new Skill
                {
                    DcServerId = dcServer.Id,
                    DailyGmCount = 5,
                    GmCount = 5,
                    DiscordId = user.DiscordId.ToString(),
                    Name = "GM",
                    Level = 1,
                    Xp = 0,
                    XpRequired = 1400,
                    CashAverageMin = null,
                    CashAverageMax = null
                });

                await _skillService.InsertAsync(new Skill
                {
                    DcServerId = dcServer.Id,
                    DiscordId = user.DiscordId.ToString(),
                    Name = "MC",
                    Level = 1,
                    Xp = 0,
                    XpRequired = 2900,
                    CashAverageMin = 10,
                    CashAverageMax = 101,
                    DailyGmCount = null,
                    GmCount = null,
                });
            }

            await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "canned food", "GeoMo Senkronize Edildi.!"));
        }
    }
}