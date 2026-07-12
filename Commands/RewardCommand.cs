using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Alexander.Services;

namespace Alexander.Commands
{
    [Group("reward", "Gestión de la tienda de recompensas")]
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    public class RewardCommand : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly RewardService _rewardService;

        public RewardCommand(RewardService rewardService)
        {
            _rewardService = rewardService;
        }

        [SlashCommand("add_role", "Crea un artículo y lo publica en este canal de inmediato")]
        [DefaultMemberPermissions(GuildPermission.Administrator)]
        public async Task AddRoleReward(string nombre, float precio, IRole rolADar, string descripcion = "Te otorga un rol especial")
        {
            var recompensa = await _rewardService.CreateRoleRewardAsync(nombre, precio, rolADar.Id, descripcion);

            var embed = new EmbedBuilder()
                .WithTitle($"🛒 {nombre}")
                .WithDescription(descripcion)
                .AddField("Precio", $"{precio:N0} 🪙", true)
                .AddField("Recompensa", rolADar.Mention, true)
                .WithColor(Color.Purple)
                .WithFooter($"Producto ID: {recompensa.Id} | Alexander Economy System")
                .Build();

            var componentes = new ComponentBuilder()
                .WithButton(
                    label: $"Comprar ({precio:N0} 🪙)",
                    customId: $"buy_item_{recompensa.Id}",
                    style: ButtonStyle.Success,
                    emote: new Emoji("🛒")
                );

            await RespondAsync(embed: embed, components: componentes.Build());
        }

        [ComponentInteraction("buy_item_*", true)]
        public async Task HandleBuyButton(string idString)
        {
            int rewardId = int.Parse(idString);
            var usuario = Context.User as SocketGuildUser;

            var resultado = await _rewardService.BuyRewardAsync(rewardId, usuario);

            await RespondAsync(resultado.Message, ephemeral: true);
        }
    }
}