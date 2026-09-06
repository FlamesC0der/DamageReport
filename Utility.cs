using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;


namespace DamageReport
{
  public partial class DamageReport
  {
    private void PrintToAllChat(string message)
    {
      Server.PrintToChatAll($"[Drink-team] {message}");
    }

    private void PrintToPlayerChat(CCSPlayerController player, string message)
    {
      player.PrintToChat($"[Drink-team] {message}");
    }

    private void Log(string message)
    {
      Console.WriteLine("[DamageReport] " + message);
    }
  }
}