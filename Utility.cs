using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;


namespace DamageReport
{
  public partial class DamageReport
  {
    private void PrintToAllChat(string message, bool prefix = true)
    {
      string prefixText = prefix ? $"\u200B{ChatColors.Orange}{ChatColors.Magenta}DRINK-TEAM{ChatColors.Default} | " : "";
      Server.PrintToChatAll($"{prefixText}{message}");
    }

    private void PrintToPlayerChat(CCSPlayerController player, string message, bool prefix = true)
    {
      string prefixText = prefix ? $"\u200B{ChatColors.Orange}{ChatColors.Magenta}DRINK-TEAM{ChatColors.Default} | " : "";
      player.PrintToChat($"{prefixText}{message}");
    }

    private void Log(string message)
    {
      Console.WriteLine($"[{ModuleName}] " + message);
    }
  }
}