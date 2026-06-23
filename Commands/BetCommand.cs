using System.Threading.Tasks;
using System.Linq;
using Discord;
using Discord.Interactions;
using Alexander.Services;

namespace Alexander.Commands
{
    public enum BetSide
    {
        [ChoiceDisplay("A Favor")]
        InFavor = 1,

        [ChoiceDisplay("En Contra")]
        Against = 0
    }

    [Group("bet", "Sistema de apuestas para SmashCoins")]
    public class BettingCommand : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IBettingService _bettingService;

        public BettingCommand(IBettingService bettingService)
        {
            _bettingService = bettingService;
        }

        [SlashCommand("create", "Crea un nuevo evento de apuesta")]
        public async Task CreateEventAsync(
            [Summary("descripcion", "¿A qué le vamos a apostar? (Ej. Juan gana 1v1)")] string description)
        {
            await DeferAsync();
            int eventId = await _bettingService.CreateEventAsync(description, Context.User.Id);

            var embed = new EmbedBuilder()
                .WithTitle("📢 ¡NUEVA APUESTA ABIERTA!")
                .WithDescription($"**Evento:** {description}\n**Creado por:** {Context.User.Mention}")
                .WithColor(Color.Gold)
                .WithFooter($"ID: {eventId} | Presiona un botón abajo para apostar")
                .Build();

            var components = new ComponentBuilder()
                .WithButton("A Favor", $"bet:favor:{eventId}", ButtonStyle.Success)
                .WithButton("En Contra", $"bet:against:{eventId}", ButtonStyle.Danger)
                .WithButton("🔒 Bloquear Apuestas", $"bet:lock:{eventId}", ButtonStyle.Secondary)
                .Build();
            await FollowupAsync(embed: embed, components: components);
        }

        [SlashCommand("list", "Muestra todas las apuestas activas")]
        public async Task ListEventsAsync()
        {
            await DeferAsync();
            var activeEvents = await _bettingService.GetActiveEventsAsync();

            if (!activeEvents.Any())
            {
                await FollowupAsync("No hay ninguna apuesta activa en este momento.");
                return;
            }

            await FollowupAsync("📜 **APUESTAS ACTIVAS:**");

            foreach (var betEvent in activeEvents)
            {
                var embed = new EmbedBuilder()
                    .WithTitle($"Apuesta #{betEvent.Id}")
                    .WithDescription(betEvent.Description)
                    .WithColor(Color.Blue)
                    .WithFooter($"{betEvent.Tickets.Count} panas han apostado en esta.")
                    .Build();

                var components = new ComponentBuilder()
                    .WithButton("A Favor", $"bet:favor:{betEvent.Id}", ButtonStyle.Success)
                    .WithButton("En Contra", $"bet:against:{betEvent.Id}", ButtonStyle.Danger)
                    .Build();

                await Context.Channel.SendMessageAsync(embed: embed, components: components);
            }
        }

        [SlashCommand("resolve", "Cierra una apuesta y le paga a los ganadores")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ResolveEventAsync(
            [Summary("id_evento", "El ID del evento a cerrar")] int eventId,
            [Summary("ganador", "¿Qué lado ganó?")] BetSide winner)
        {
            await DeferAsync();

            bool wonInFavor = winner == BetSide.InFavor;
            var reportLines = await _bettingService.ResolveEventAsync(eventId, wonInFavor);
            string fullReport = string.Join("\n", reportLines);

            await FollowupAsync(fullReport);
        }


        [ComponentInteraction("bet:*:*", ignoreGroupNames: true)]
        public async Task HandleBetButtonAsync(string side, string eventIdStr)
        {
            var modal = new ModalBuilder()
                .WithTitle("Ingresa tu Apuesta")
                .WithCustomId($"modal:{side}:{eventIdStr}")
                .AddTextInput(
                    label: "¿Cuántos SmashCoins vas a apostar?",
                    customId: "amount_input",
                    placeholder: "Ej. 100",
                    minLength: 1,
                    maxLength: 6,
                    required: true)
                .Build();

            await RespondWithModalAsync(modal);
        }
        
        [ComponentInteraction("bet:lock:*", ignoreGroupNames: true)]
        public async Task HandleLockEventAsync(string eventIdStr)
        {
            var guildUser = Context.User as IGuildUser;
            bool isAdmin = guildUser != null && guildUser.GuildPermissions.Administrator;

            if (!int.TryParse(eventIdStr, out int eventId))
            {
                await RespondAsync("❌ Error interno: ID de evento inválido.", ephemeral: true);
                return;
            }

            var result = await _bettingService.LockEventAsync(eventId, Context.User.Id, isAdmin);

            if (!result.Success)
            {
                await RespondAsync($"❌ {result.Message}", ephemeral: true);
                return;
            }

            await RespondAsync($"🔒 **¡APUESTAS CERRADAS!**\nYa no se aceptan más apuestas para el Evento #{eventId}. ¡A disfrutar el espectáculo y esperar el resultado!");
        }

        [ModalInteraction("modal:*:*", ignoreGroupNames: true)]
        public async Task HandleBetModalAsync(string side, string eventIdStr, BetModal modalData)
        {
            await DeferAsync(ephemeral: true);

            if (!int.TryParse(eventIdStr, out int eventId))
            {
                await FollowupAsync("❌ Error interno: ID de evento inválido.", ephemeral: true);
                return;
            }

            if (!int.TryParse(modalData.Amount, out int amount) || amount <= 0)
            {
                await FollowupAsync("❌ Por favor, ingresa una cantidad válida de números enteros.", ephemeral: true);
                return;
            }

            bool inFavor = side == "favor";
            var result = await _bettingService.PlaceBetAsync(Context.User.Id, eventId, amount, inFavor);

            if (!result.Success)
            {
                await FollowupAsync($"❌ {result.Message}", ephemeral: true);
                return;
            }

            string sideText = inFavor ? "A FAVOR" : "EN CONTRA";

            await FollowupAsync($"✅ ¡Apuesta confirmada! Has apostado **{amount} SmashCoins**.", ephemeral: true);

            string anuncio = $"🎰 {Context.User.Mention} ha apostado **{amount}  SmashCoins** **{sideText} a que**:\n" +
                             $"> *\"{result.EventDesc}\"*\n" +
                             $"💸 **Pago estimado actual:** `{result.EstimatedPayout} SmashCoins`";

            await Context.Channel.SendMessageAsync(anuncio);
        }
    }

    public class BetModal : IModal
    {
        public string Title => "Realizar Apuesta";

        [InputLabel("Cantidad de SmashCoins")]
        [ModalTextInput("amount_input")]
        public string Amount { get; set; }
    }
}