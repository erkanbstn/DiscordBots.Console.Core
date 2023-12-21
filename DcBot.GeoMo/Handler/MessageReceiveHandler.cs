using DcBot.Common.MessageHandler;
using DcBot.Common.PermissionHandler;
using DcBot.Common.PrefixHandler;
using DcBot.Service.Interfaces;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace DcBot.GeoMo.Handler
{
    public class MessageReceiveHandler : ModuleBase<SocketCommandContext>
    {
        private readonly IMessageControl _messageControl;
        private readonly IPrefixControl _prefixControl;
        private readonly CommandService _commandService;
        private readonly IUserService _userService;
        private readonly ISkillService _skillService;

        public MessageReceiveHandler(IMessageControl messageControl, IPrefixControl prefixControl, CommandService commandService, IUserService userService, ISkillService skillService)
        {
            _messageControl = messageControl;
            _prefixControl = prefixControl;
            _commandService = commandService;
            _userService = userService;
            _skillService = skillService;
        }

        [Command("gmhelp")]
        [Summary("Yardım")]
        [PermissionControl(GuildPermission.SendMessages)]
        public async Task HelpCommand()
        {
            var prefixes = _prefixControl.GeoBotPrefixes();

            string prefixList = string.Join(" | ", prefixes);

            string helpMessage = $"**Prefixler:**\n| {prefixList} |\n\n**Komutlar:**\n";

            var commandGroups = _commandService.Modules
                .Select(module => new
                {
                    ModuleName = module.Name,
                    Commands = module.Commands
                    .Where(command => !command.Attributes.OfType<PermissionControlAttribute>().Any() || command.Attributes.OfType<PermissionControlAttribute>().Any(attr => (Context.User as SocketGuildUser).GuildPermissions.Has(attr.RequiredPermission)))
                    .Select(command => $"`{string.Join("`, `", command.Aliases)}` - {command.Summary ?? "Açıklama Yok"}")
                });

            foreach (var group in commandGroups)
            {
                helpMessage += $"**`{group.ModuleName}`**\n{string.Join("\n", group.Commands)}\n\n";
            }

            await _messageControl.EmbedAsync(Context, Color.Purple, "white check mark", helpMessage);
        }

        [Command("gupg")]
        [Summary("Yetenek Geliştir")]
        [PermissionControl(GuildPermission.SendMessages)]
        public async Task EarnXp(int xpAmount)
        {
            var user = await _userService.EnsureUserExistsAsync(Context.User.Id.ToString(), Context.User.Username);
            var skill = await _skillService.FirstOrDefaultAsync(x => x.Name == "Earn" && x.UserId == user.Id);

            skill.Xp += xpAmount;
            user.Money -= xpAmount;

            while (skill.Xp >= skill.XpRequired)
            {
                skill.Xp -= skill.XpRequired;
                skill.Level++;

                Random random = new Random();

                skill.XpRequired += random.Next((int)skill.XpRequired, (int)(skill.Level * skill.XpRequired));

                skill.DailyGgCount++;
            }

            await _skillService.UpdateAsync(skill);
            await _userService.UpdateAsync(user);
            await ReplyAsync($"Mevcut `Xp` Miktarınız: `{skill.Xp}/{skill.XpRequired}!` Seviyeniz: `{skill.Level}`, Günlük `GG` Kullanma Hakkınız: `{skill.DailyGgCount}`");
        }

        [Command("gg")]
        [Summary("Para Kazan")]
        [PermissionControl(GuildPermission.SendMessages)]
        public async Task EarnMoney()
        {
            var user = await _userService.EnsureUserExistsAsync(Context.User.Id.ToString(), Context.User.Username);
            var skill = await _skillService.FirstOrDefaultAsync(x => x.Name == "Earn" && x.UserId == user.Id);

            if (skill.DailyGgCount <= 0)
            {
                await ReplyAsync("Günlük `GG` Kullanma Hakkınız Kalmadı.");
                return;
            }

            int earnedMoney = new Random().Next(10, 101);
            user.Money += earnedMoney;
            skill.DailyGgCount--;

            await _skillService.UpdateAsync(skill);
            await _userService.UpdateAsync(user);
            await ReplyAsync($"GeoMo 'dan `{earnedMoney}` Para Kazandınız! Günlük GG Kullanma Hakkınız: `{skill.DailyGgCount}`");
        }

        [Command("gk")]
        [Summary("Şans Oyunu Oyna")]
        [PermissionControl(GuildPermission.SendMessages)]
        public async Task CoinFlip(string choice, int betAmount)
        {
            bool isWin = false;
            if (choice.ToLower() != "y" && choice.ToLower() != "t")
            {
                await ReplyAsync("Lütfen `'T'` veya `'Y'` Seçeneğini Belirtin.");
                return;
            }

            var user = await _userService.EnsureUserExistsAsync(Context.User.Id.ToString(), Context.User.Username);

            if (user.Money < betAmount)
            {
                await ReplyAsync("Şans Oyunu İçin Yeterli Bakiyeniz Bulunmamaktadır.");
                return;
            }

            string result = new Random().Next(0, 2) == 0 ? "t" : "y";
            isWin = choice.ToLower() == /*result*/"t";

            user.Money -= betAmount;
            user.Money += isWin ? betAmount * 2 : -betAmount;
            await _userService.UpdateAsync(user);
            string resultMessage = isWin ? "Kazandınız!" : "Kaybettiniz.";
            await ReplyAsync($"Yazı Tura Sonucu: `{result.ToUpper()}`. `{resultMessage}` Toplam Paranız: `{user.Money}`");
        }

        [Command("gcash")]
        [Summary("Kasa Göster")]
        [PermissionControl(GuildPermission.SendMessages)]
        public async Task CheckBalance()
        {
            var user = await _userService.EnsureUserExistsAsync(Context.User.Id.ToString(), Context.User.Username);
            await ReplyAsync($"Mevcut Para Miktarınız: `{user.Money}`");
        }

        [Command("gxp")]
        [Summary("Yetenek Xp Göster")]
        [PermissionControl(GuildPermission.SendMessages)]
        public async Task CheckSkillXp()
        {
            var user = await _userService.EnsureUserExistsAsync(Context.User.Id.ToString(), Context.User.Username);
            var skill = await _skillService.FirstOrDefaultAsync(x => x.Name == "Earn" && x.UserId == user.Id);

            await ReplyAsync($"`GG` İçin Mevcut Skill Xp Miktarınız: `{skill.Xp}/{skill.XpRequired}`");
        }

        [Command("gsend")]
        [Summary("Para Gönder")]
        [PermissionControl(GuildPermission.SendMessages)]
        public async Task CheckSkillXp(SocketGuildUser receiverUser, int amount)
        {
            var sender = await _userService.EnsureUserExistsAsync(Context.User.Id.ToString(), Context.User.Username);
            var receiver = await _userService.EnsureUserExistsAsync(receiverUser.Id.ToString(), receiverUser.Username);
            sender.Money -= amount;
            receiver.Money += amount;
            await _userService.UpdateAsync(sender);
            await _userService.UpdateAsync(receiver);
            await ReplyAsync($"{receiverUser.Mention}! `{sender.UserName}` Sana `{amount}` Kadar `GeoMo Cash` Gönderdi!");
        }
    }
}