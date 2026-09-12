using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;

namespace DamageReport
{
  public partial class DamageReport : BasePlugin
  {
    public override string ModuleName => "DamageReport";
    public override string ModuleVersion => "0.0.1";
    public override string ModuleAuthor => "FlamesCoder https://github.com/FlamesC0der";

    public Dictionary<int, Dictionary<int, DamagePlayerInfo>> playerDamageInfo = [];
    private Dictionary<int, PlayerState> playerStates = [];

    public override void Load(bool hotReload)
    {
      Log("Plugin loaded! Made with ♡ by FlamesCoder");
    }

    [GameEventHandler]
    public HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
    {
      var attacker = @event.Attacker;
      var victim = @event.Userid;

      if (victim == null || !victim.IsValid)
        return HookResult.Continue;

      var victimId = (int)victim.Index;
      var healthAfter = @event.Health;

      // Если хп не было в записях, то создаем новую запись с 100 хп
      if (!playerStates.TryGetValue(victimId, out var state))
      {
        state = new PlayerState
        {
          Health = 100
        };

        playerStates[victimId] = state;
      }

      var healthBefore = state.Health;

      var actualDamage = Math.Max(0, healthBefore - healthAfter);

      state.Health = healthAfter;

      // Если есть человек, нанесший урон, из противоположной команды
      if (actualDamage > 0 && attacker != null && attacker.IsValid && victim.TeamNum != attacker.TeamNum && attacker.TeamNum > 1)
      {
        var attackerId = (int)attacker.Index;

        // Не учитывать урон самому себе
        if (attackerId != victimId)
        {
          if (!playerDamageInfo.TryGetValue(attackerId, out var victims))
          {
            victims = [];
            playerDamageInfo[attackerId] = victims;
          }

          if (!victims.TryGetValue(victimId, out var damageInfo))
          {
            damageInfo = new DamagePlayerInfo();
            victims[victimId] = damageInfo;
          }

          damageInfo.DamageHP += actualDamage;
          damageInfo.Hits++;
        }
      }

      // var attackerName = attacker != null && attacker.IsValid
      //   ? attacker.PlayerName
      //   : "World";

      // PrintToAllChat(
      //   $"{attackerName} -> {victim.PlayerName} | " +
      //   $"{healthBefore} -> {healthAfter} | " +
      //   $"Damage: {actualDamage} | "
      // );

      return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
      playerDamageInfo.Clear();
      playerStates.Clear();
      
      return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
      ShowDamageInfo();

      return HookResult.Continue;
    }

    private void ShowDamageInfo()
    {
      foreach (var player in Utilities.GetPlayers())
      {
        if (!player.IsValid || player.TeamNum < 2)
          continue;
        
        foreach (var opponent in Utilities.GetPlayers())
        {
          if (!opponent.IsValid || opponent.TeamNum < 2)
            continue;

          if (player == opponent)
            continue;

          if (player.TeamNum == opponent.TeamNum)
            continue;
          
          var playerId = (int)player.Index;
          var opponentId = (int)opponent.Index;

          var to = new DamagePlayerInfo();

          if (playerDamageInfo.TryGetValue(playerId, out var toVictims))
          {
            if (toVictims.TryGetValue(opponentId, out var damage))
            {
              to = damage;
            }
          }

          var from = new DamagePlayerInfo();

          if (playerDamageInfo.TryGetValue(opponentId, out var fromVictims))
          {
            if (fromVictims.TryGetValue(playerId, out var damage))
            {
              from = damage;
            }
          }

          var hp = opponent.Pawn.Value?.Health ?? 0;

          var message =
            $"{ChatColors.Green}To: [{to.DamageHP} / {to.Hits}] " +
            $"{ChatColors.Red}From: [{from.DamageHP} / {from.Hits}] " +
            $"{ChatColors.Default}- " +
            $"{ChatColors.ForPlayer(opponent)}{opponent.PlayerName} " +
            $"{ChatColors.Default}({hp} hp)";
          
          PrintToPlayerChat(player, message);
        }
      }
    }

    public class PlayerState
    {
      public int Health { get; set; } = 100;
    }

    public class DamagePlayerInfo
    {
      public int DamageHP { get; set; } = 0;
      public int Hits { get; set; } = 0;
    }
  }
}