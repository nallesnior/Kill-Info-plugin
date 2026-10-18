using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Exiled.Events.EventArgs.Player;
using System;

namespace KillInfoPlugin
{
    public class KillInfoPlugin : Plugin<Config>
    {
        private readonly Dictionary<string, int> _killCounts = new Dictionary<string, int>();

        public override string Name => "KillInfoPlugin";
        public override string Author => "Naleśnior";
        public override Version Version => new Version(3, 0, 0);
        public override string Prefix => "killinfo";

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Dying += OnPlayerDying;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Dying -= OnPlayerDying;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            base.OnDisabled();
        }

        private void OnRoundStarted()
        {
            _killCounts.Clear();
        }

        private void OnPlayerDying(DyingEventArgs ev)
        {
            Player victim = ev.Player;
            Player killer = ev.Attacker;

            if (killer == null || killer == victim)
            {
                victim.ShowHint(Config.SucideMessageFormat, this.Config.MessageDuration);
                return;
            }

            if (!_killCounts.ContainsKey(killer.UserId))
                _killCounts[killer.UserId] = 0;

            _killCounts[killer.UserId]++;
            int totalKills = _killCounts[killer.UserId];

            string victimName = victim.Nickname;
            string victimRole = victim.Role.ToString();

            string message = Config.KillMessageFormat
                .Replace("{killCount}", totalKills.ToString())
                .Replace("{victim}", victimName)
                .Replace("{victimRole}", victimRole);

            killer.ShowHint(message, this.Config.MessageDuration);
        }
    }
}

