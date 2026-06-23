using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alexander.Models;
using Alexander.Repositories;

namespace Alexander.Services
{
    public class BettingService : IBettingService
    {
        private readonly IBettingRepository _bettingRepo;
        private readonly IEconomyRepository _economyRepo;

        public BettingService(IBettingRepository bettingRepo, IEconomyRepository economyRepo)
        {
            _bettingRepo = bettingRepo;
            _economyRepo = economyRepo;
        }

        public async Task<int> CreateEventAsync(string description, ulong creatorId)
        {
            return await _bettingRepo.CreateEventAsync(description, creatorId);
        }

        public async Task<(bool Success, string Message)> LockEventAsync(int eventId, ulong userId, bool isAdmin)
        {
            var activeEvent = await _bettingRepo.GetActiveEventByIdAsync(eventId);
            if (activeEvent == null) return (false, $"No se encontró la apuesta con el ID {eventId}.");
            if (activeEvent.IsLocked) return (false, "Esta apuesta ya estaba bloqueada.");

            if (activeEvent.CreatorId != userId && !isAdmin)
            {
                return (false, "Solo el creador de la apuesta o un administrador pueden bloquearla.");
            }

            activeEvent.IsLocked = true;
            await _bettingRepo.SaveChangesAsync();
            return (true, string.Empty);
        }

        public async Task<List<BetEvent>> GetActiveEventsAsync()
        {
            return await _bettingRepo.GetActiveEventsAsync();
        }

        public async Task<(bool Success, string Message, string EventDesc, int EstimatedPayout)> PlaceBetAsync(ulong userId, int eventId, int amount, bool inFavor)
        {
            if (amount <= 0)
                return (false, "La cantidad a apostar debe ser mayor a cero.", string.Empty, 0);

            var activeEvent = await _bettingRepo.GetActiveEventByIdAsync(eventId);
            if (activeEvent == null)
                return (false, $"No se encontró ninguna apuesta activa con el ID {eventId}.", string.Empty, 0);

            if (activeEvent.IsLocked)
                return (false, "¡Tiempo agotado! Las apuestas para este evento ya han sido cerradas.", string.Empty, 0);

            if (activeEvent.Tickets.Any(t => t.UserId == userId))
                return (false, "Ya colocaste una apuesta para este evento. ¡Solo se permite un ticket por pana!", string.Empty, 0);

            var profile = await _economyRepo.GetByUserIdAsync(userId);
            if (profile == null)
            {
                profile = new EconomyProfile { UserId = userId, SmashCoins = 1000 };
                await _economyRepo.AddAsync(profile);
            }

            if (profile.SmashCoins < amount)
                return (false, $"Fondos insuficientes. Tu saldo actual es de **{profile.SmashCoins} SmashCoins**.", string.Empty, 0);

            profile.SmashCoins -= amount;
            await _economyRepo.UpdateAsync(profile);

            var ticket = new BetTicket
            {
                BetEventId = activeEvent.Id,
                UserId = userId,
                Amount = amount,
                IsBetAFavor = inFavor
            };
            await _bettingRepo.AddTicketAsync(ticket);

            var updatedEvent = await _bettingRepo.GetActiveEventByIdAsync(eventId);

            int winnersPool = updatedEvent.Tickets.Where(t => t.IsBetAFavor == inFavor).Sum(t => t.Amount);
            int losersPool = updatedEvent.Tickets.Where(t => t.IsBetAFavor != inFavor).Sum(t => t.Amount);

            double winRatio = (double)amount / winnersPool;
            int estimatedExtra = (int)Math.Floor(winRatio * losersPool);
            int estimatedPayout = amount + estimatedExtra;

            return (true, string.Empty, activeEvent.Description, estimatedPayout);
        }

        public async Task<List<string>> ResolveEventAsync(int eventId, bool wonInFavor)
        {
            var report = new List<string>();
            var activeEvent = await _bettingRepo.GetActiveEventByIdAsync(eventId);

            if (activeEvent == null)
            {
                report.Add($"No se encontró la apuesta con el ID {eventId}.");
                return report;
            }

            activeEvent.IsClosed = true;
            activeEvent.IsWinnerAFavor = wonInFavor;

            var tickets = activeEvent.Tickets;
            string resultText = wonInFavor ? "A FAVOR" : "EN CONTRA";

            report.Add($"🏁 **RESULTADO DE LA APUESTA** 🏁\n> **Evento:** {activeEvent.Description}\n> **Resultado Ganador:** {resultText}\n");

            if (!tickets.Any())
            {
                report.Add("Nadie apostó en este evento. No hay dinero que repartir.");
                await _bettingRepo.SaveChangesAsync();
                return report;
            }

            var winners = tickets.Where(t => t.IsBetAFavor == wonInFavor).ToList();
            var losers = tickets.Where(t => t.IsBetAFavor != wonInFavor).ToList();

            if (!winners.Any())
            {
                report.Add("¡Casa llena! Todo el mundo apostó al lado equivocado y perdieron su dinero. Las SmashCoins se queman 🔥.");
                await _bettingRepo.SaveChangesAsync();
                return report;
            }

            int losersPool = losers.Sum(t => t.Amount);
            int winnersPool = winners.Sum(t => t.Amount);

            foreach (var ticket in winners)
            {
                double winRatio = (double)ticket.Amount / winnersPool;
                int extraPrize = (int)Math.Floor(winRatio * losersPool);
                int totalPayout = ticket.Amount + extraPrize;

                var profile = await _economyRepo.GetByUserIdAsync(ticket.UserId);
                if (profile != null)
                {
                    profile.SmashCoins += totalPayout;
                    await _economyRepo.UpdateAsync(profile);
                }

                report.Add($"• <@{ticket.UserId}> apostó {ticket.Amount} y cobró **{totalPayout} SmashCoins** (Ganancia de +{extraPrize} 📈).");
            }

            await _bettingRepo.SaveChangesAsync();
            return report;
        }
        public async Task<(bool Success, string Message)> LockEventAsync(int eventId)
        {
            var activeEvent = await _bettingRepo.GetActiveEventByIdAsync(eventId);
            if (activeEvent == null) return (false, $"No se encontró la apuesta con el ID {eventId}.");
            if (activeEvent.IsLocked) return (false, "Esta apuesta ya estaba bloqueada.");

            activeEvent.IsLocked = true;
            await _bettingRepo.SaveChangesAsync();
            return (true, string.Empty);
        }
    }
}