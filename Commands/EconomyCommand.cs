using Discord;
using Discord.Interactions;
using System.Threading.Tasks;
using Alexander.Services;
using System.Text.RegularExpressions;
namespace Alexander.Commands
{
    [Group("economy", "Comandos de economía del servidor")]
    public class EconomyCommand : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IEconomyService _economyService;

        public EconomyCommand(IEconomyService economyService)
        {
            _economyService = economyService;
        }

        [SlashCommand("openaccount", "Abre una cuenta en el server")]
        public async Task OpenAccount()
        {
            await DeferAsync(ephemeral: true);

            try
            {
                var userId = Context.User.Id;
                var username = Context.User.Username;

                bool exists = await _economyService.AccountExistsAsync(userId);
                if (exists)
                {
                    var embed = new EmbedBuilder()
                        .WithTitle("❌ Cuenta ya existente")
                        .WithDescription($"**{username}**, ya tienes una cuenta activa en el servidor.")
                        .WithColor(Color.Red)
                        .WithFooter($"ID: {userId}")
                        .WithCurrentTimestamp()
                        .Build();

                    await FollowupAsync(embed: embed);
                    return;
                }

                var result = await _economyService.OpenAccountAsync(userId, 1000);

                if (result.Success)
                {
                    var embed = new EmbedBuilder()
                        .WithTitle("✅ Cuenta creada exitosamente")
                        .WithDescription($"**{username}**, tu cuenta ha sido creada con éxito.")
                        .AddField("💰 Monedas iniciales", $"{result.Profile.SmashCoins:N0} 🪙", true)
                        .AddField("🆔 ID de usuario", $"`{userId}`", true)
                        .AddField("📅 Fecha de creación", $"<t:{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}:F>", true)
                        .WithColor(Color.Green)
                        .WithFooter($"Servidor: {Context.Guild?.Name ?? "DM"}")
                        .WithCurrentTimestamp()
                        .Build();

                    await FollowupAsync(embed: embed);
                }
                else
                {
                    var embed = new EmbedBuilder()
                        .WithTitle("❌ Error al crear la cuenta")
                        .WithDescription($"**{username}**, no se pudo crear tu cuenta.")
                        .AddField("Motivo", result.Message)
                        .WithColor(Color.Red)
                        .WithCurrentTimestamp()
                        .Build();

                    await FollowupAsync(embed: embed);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en OpenAccount: {ex.Message}");
                Console.WriteLine(ex.StackTrace);

                var embed = new EmbedBuilder()
                    .WithTitle("❌ Error inesperado")
                    .WithDescription("Ocurrió un error al crear tu cuenta. Por favor, intenta nuevamente más tarde.")
                    .WithColor(Color.Red)
                    .WithCurrentTimestamp()
                    .Build();

                await FollowupAsync(embed: embed);
            }
        }

        [SlashCommand("balance", "Muestra tu balance actual")]
        public async Task Balance()
        {
            await DeferAsync(ephemeral: true);

            try
            {
                var userId = Context.User.Id;
                var username = Context.User.Username;

                bool exists = await _economyService.AccountExistsAsync(userId);
                if (!exists)
                {
                    var embeds = new EmbedBuilder()
                        .WithTitle("❌ Sin cuenta")
                        .WithDescription($"**{username}**, no tienes una cuenta activa. Usa `/openaccount` para crear una.")
                        .WithColor(Color.Red)
                        .WithCurrentTimestamp()
                        .Build();

                    await FollowupAsync(embed: embeds);
                    return;
                }

                var balance = await _economyService.GetBalanceAsync(userId);

                var embed = new EmbedBuilder()
                    .WithTitle($"💰 Balance de {username}")
                    .WithDescription($"Tienes **{balance:N0}** monedas 🪙")
                    .AddField("🆔 ID", $"`{userId}`", true)
                    .AddField("📊 Estado", "Cuenta activa", true)
                    .WithColor(Color.Gold)
                    .WithFooter($"Solicitado por {username}")
                    .WithCurrentTimestamp()
                    .Build();

                await FollowupAsync(embed: embed);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Balance: {ex.Message}");
                await FollowupAsync("❌ Ocurrió un error al obtener tu balance.", ephemeral: true);
            }
        }

        [SlashCommand("addcoins", "Agrega monedas a un usuario (Solo admins)")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddCoins(
            [Summary("usuario", "Usuario al que agregar monedas")] IUser user,
            [Summary("cantidad", "Cantidad de monedas a agregar")] int amount)
        {
            await DeferAsync(ephemeral: true);

            try
            {
                if (amount <= 0)
                {
                    await FollowupAsync("❌ La cantidad debe ser mayor a 0.", ephemeral: true);
                    return;
                }

                bool exists = await _economyService.AccountExistsAsync(user.Id);
                if (!exists)
                {
                    await FollowupAsync($"❌ El usuario **{user.Username}** no tiene una cuenta activa.", ephemeral: true);
                    return;
                }
                if(Context.User.Id == 642330520868487218)
                {
                    await FollowupAsync($"❌ El usuario **{user.Username}** no tiene permisos suficientes. Por palomo : <", ephemeral: true);
                    return;
                }

                bool success = await _economyService.AddCoinsAsync(user.Id, amount);
                if (success)
                {
                    var newBalance = await _economyService.GetBalanceAsync(user.Id);

                    var embed = new EmbedBuilder()
                        .WithTitle("✅ Monedas agregadas")
                        .WithDescription($"Se agregaron **{amount:N0}** monedas a **{user.Username}**")
                        .AddField("💰 Nuevo balance", $"{newBalance:N0} 🪙", true)
                        .AddField("👤 Agregado por", Context.User.Mention, true)
                        .WithColor(Color.Green)
                        .WithCurrentTimestamp()
                        .Build();

                    await FollowupAsync(embed: embed);
                }
                else
                {
                    await FollowupAsync($"❌ No se pudieron agregar las monedas a **{user.Username}**.", ephemeral: true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AddCoins: {ex.Message}");
                await FollowupAsync("❌ Ocurrió un error al agregar monedas.", ephemeral: true);
            }
        }

        [SlashCommand("transfer", "Transfiere monedas a otro usuario")]
        public async Task Transfer(
            [Summary("usuario", "Usuario al que transferir")] IUser user,
            [Summary("cantidad", "Cantidad de monedas a transferir")] int amount)
        {
            await DeferAsync(ephemeral: true);

            try
            {
                var fromUserId = Context.User.Id;
                var toUserId = user.Id;
                var username = Context.User.Username;

                if (fromUserId == toUserId)
                {
                    await FollowupAsync("❌ No puedes transferirte monedas a ti mismo.", ephemeral: true);
                    return;
                }

                if (amount <= 0)
                {
                    await FollowupAsync("❌ La cantidad debe ser mayor a 0.", ephemeral: true);
                    return;
                }

                bool fromExists = await _economyService.AccountExistsAsync(fromUserId);
                bool toExists = await _economyService.AccountExistsAsync(toUserId);

                if (!fromExists)
                {
                    await FollowupAsync($"❌ **{username}**, no tienes una cuenta activa. Usa `/openaccount` para crear una.", ephemeral: true);
                    return;
                }

                if (!toExists)
                {
                    await FollowupAsync($"❌ El usuario **{user.Username}** no tiene una cuenta activa.", ephemeral: true);
                    return;
                }

                bool success = await _economyService.TransferCoinsAsync(fromUserId, toUserId, amount);

                if (success)
                {
                    var newBalanceFrom = await _economyService.GetBalanceAsync(fromUserId);
                    var newBalanceTo = await _economyService.GetBalanceAsync(toUserId);

                    var embed = new EmbedBuilder()
                        .WithTitle("💸 Transferencia realizada")
                        .WithDescription($"Se transfirieron **{amount:N0}** monedas a **{user.Username}**")
                        .AddField("📤 Tu nuevo balance", $"{newBalanceFrom:N0} 🪙", true)
                        .AddField($"📥 Balance de {user.Username}", $"{newBalanceTo:N0} 🪙", true)
                        .WithColor(Color.Blue)
                        .WithCurrentTimestamp()
                        .Build();

                    await FollowupAsync(embed: embed);
                }
                else
                {
                    await FollowupAsync($"❌ No se pudo completar la transferencia. Verifica que tienes suficientes monedas.", ephemeral: true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Transfer: {ex.Message}");
                await FollowupAsync("❌ Ocurrió un error al realizar la transferencia.", ephemeral: true);
            }
        }

        [SlashCommand("top", "Muestra los usuarios con más monedas")]
        public async Task Top([Summary("cantidad", "Cantidad de usuarios a mostrar (1-10)")] int count = 5)
        {
            await DeferAsync(ephemeral: false);

            try
            {
                count = Math.Clamp(count, 1, 10);

                var allBalances = await _economyService.GetAllBalancesAsync();

                if (allBalances.Count == 0)
                {
                    await FollowupAsync("📊 No hay usuarios registrados aún.");
                    return;
                }

                var topUsers = allBalances
                    .OrderByDescending(kvp => kvp.Value)
                    .Take(count)
                    .ToList();

                var embed = new EmbedBuilder()
                    .WithTitle($"🏆 Top {count} usuarios con más monedas")
                    .WithColor(Color.Gold)
                    .WithCurrentTimestamp();

                int position = 1;
                foreach (var kvp in topUsers)
                {
                    string username;
                    try
                    {
                        var user = await Context.Client.GetUserAsync(kvp.Key);
                        username = user?.Username ?? $"Usuario {kvp.Key}";
                    }
                    catch
                    {
                        username = $"Usuario {kvp.Key}";
                    }

                    string emoji = position switch
                    {
                        1 => "🥇",
                        2 => "🥈",
                        3 => "🥉",
                        _ => $"#{position}"
                    };

                    embed.AddField(
                        $"{emoji} {username}",
                        $"{kvp.Value:N0} 🪙",
                        false
                    );

                    position++;
                }

                embed.WithFooter($"Total de usuarios: {allBalances.Count}");

                await FollowupAsync(embed: embed.Build());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Top: {ex.Message}");
                await FollowupAsync("❌ Ocurrió un error al obtener el top.", ephemeral: true);
            }
        }
    }
}