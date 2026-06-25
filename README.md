# 🤖 AlexanderPanasBot

**Discord Economy & Betting System**

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Discord](https://img.shields.io/badge/Discord-Bot-5865F2?logo=discord)](https://discord.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![C#](https://img.shields.io/badge/C%23-100%25-blue?logo=csharp)](https://github.com/sebas-gith/AlexanderPanasBot)

---

## 📋 Índice

- [Visión General](#-visión-general)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Modelos de Datos](#-modelos-de-datos)
- [Comandos Disponibles](#-comandos-disponibles)
- [Instalación](#-instalación)
- [Configuración](#-configuración)
- [Contribución](#-contribución)
- [Licencia](#-licencia)

---

## 🎯 Visión General

**AlexanderPanasBot** es un bot de Discord para sistemas de economía y apuestas, desarrollado en .NET 10.0 con arquitectura basada en servicios.

### Características Principales

- **Sistema de Economía**: Cuentas, transferencias, ranking
- **Sistema de Apuestas**: Creación, participación, resolución
- **APIs Integradas**: Giphy, datos curiosos
- **UI Interactiva**: Botones y modales

---

## 📁 Estructura del Proyecto

```
AlexanderPanasBot/
├── Commands/                 # Comandos del bot
│   ├── BettingCommand.cs     # Sistema de apuestas
│   ├── EconomyCommand.cs     # Comandos económicos
│   ├── FactApiCommand.cs     # Datos curiosos
│   ├── GreetingCommand.cs    # UI interactiva
│   ├── PingCommand.cs        # Health check
│   └── SpamCommand.cs        # Utilidades
│
├── Config/                   # Configuración
│   └── ConfigurationManager.cs
│
├── Data/                     # Base de datos
│   └── BotDbContext.cs
│
├── Events/                   # Eventos de Discord
│   └── DiscordEventHandler.cs
│
├── Models/                   # Entidades de datos
│   ├── AccountResult.cs
│   ├── BetEvent.cs
│   ├── BetTicket.cs
│   └── EconomyProfile.cs
│
├── Repositories/             # Acceso a datos
│   ├── BettingRepository.cs
│   ├── EconomyRepository.cs
│   ├── IBettingRepository.cs
│   └── IEconomyRepository.cs
│
├── Services/                 # Lógica de negocio
│   ├── BettingService.cs
│   ├── EconomyService.cs
│   ├── FactApiService.cs
│   ├── GhipyApiService.cs
│   ├── IBettingService.cs
│   ├── IEconomyService.cs
│   ├── InteractionHandlingService.cs
│   └── LoggingService.cs
│
├── images/                   # Recursos gráficos
│   └── NPCAlexander.png
│
└── Program.cs               # Punto de entrada
```
---

## 🤖 Comandos Disponibles

### Economía
| Comando | Descripción | Permisos |
|---------|-------------|----------|
| `/economy openaccount` | Crear cuenta | Todos |
| `/economy balance` | Ver saldo | Todos |
| `/economy transfer @usuario cantidad` | Transferir monedas | Todos |
| `/economy top cantidad` | Ranking de usuarios | Todos |
| `/economy addcoins @usuario cantidad` | Agregar monedas | Admins |
| `/economy daily` | Reclamar recompensa botin diario | Todos |

### Apuestas
| Comando | Descripción | Permisos |
|---------|-------------|----------|
| `/bet create descripción` | Crear evento de apuesta | Todos |
| `/bet list` | Listar apuestas activas | Todos |
| `/bet resolve id favor/contra` | Resolver apuesta | Admins |

### Utilidades
| Comando | Descripción |
|---------|-------------|
| `/ping` | Health check |
| `/randomfact` | Dato curioso (en inglés) |
| `/saludar` | UI interactiva de prueba |
| `/spamuser @usuario texto cantidad` | Spam controlado (canal específico) |

---

## 🛠️ Instalación

### Requisitos

- .NET 10.0 SDK
- Discord Bot Token ([Developer Portal](https://discord.com/developers/applications))
- Giphy API Key (opcional)

### Pasos

```bash
# 1. Clonar repositorio
git clone https://github.com/sebas-gith/AlexanderPanasBot.git
cd AlexanderPanasBot

# 2. Configurar appsettings.json
# Crear archivo Config/appsettings.json con tu token

# 3. Restaurar dependencias
dotnet restore

# 4. Ejecutar
dotnet run
```

---

## ⚙️ Configuración

### appsettings.json
```json
{
  "Discord": {
    "Token": "YOUR_DISCORD_BOT_TOKEN"
  },
  "Apis": {
    "RandomFact": {
      "BaseUrl": "https://catfact.ninja/fact",
      "TimeoutSeconds": 5
    },
    "Giphy": {
      "BaseUrl": "https://api.giphy.com/v1/gifs/search",
      "ApiKey": "YOUR_GIPHY_API_KEY",
      "TimeoutSeconds": 5
    }
  }
}
```

### Dependency Injection (Program.cs)
```csharp
services.AddSingleton<DiscordSocketClient>()
        .AddSingleton<InteractionService>()
        .AddSingleton<InteractionHandlingService>()
        .AddSingleton<LoggingService>()
        .AddDbContext<BotDbContext>(options =>
            options.UseSqlite("Data Source=economia.db"))
        .AddSingleton<IEconomyRepository, EconomyRepository>()
        .AddSingleton<IEconomyService, EconomyService>()
        .AddTransient<IBettingRepository, BettingRepository>()
        .AddTransient<IBettingService, BettingService>()
        .AddSingleton<FactApiService>()
        .AddSingleton<GhipyApiService>()
        .AddSingleton<ConfigurationManager>();
```

### Base de Datos
```csharp
using (var scope = services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BotDbContext>();
    db.Database.EnsureCreated();
}
```

---

## 🤝 Contribución

### Estándares de Código
- **Nombramiento**: PascalCase (clases/métodos), camelCase (parámetros)
- **Documentación**: XML comments
- **Patrones**: Repository + Service Layer
- **Resultados**: `AccountResult` para respuestas consistentes
---

## 📜 Licencia

MIT License - Consultar archivo [LICENSE](LICENSE)

---

## 📞 Contacto

- **GitHub**: [sebas-gith](https://github.com/sebas-gith)
- **Issues**: [GitHub Issues](https://github.com/sebas-gith/AlexanderPanasBot/issues)

---

*Última actualización: 26 de junio de 2026*