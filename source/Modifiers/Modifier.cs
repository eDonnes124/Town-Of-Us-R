using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Modifiers
{
    public abstract class Modifier
    {
        public static readonly Dictionary<byte, List<Modifier>> ModifierDictionary = new();
        public Func<string> TaskText;
        public virtual string FullModifierTaskText()
        {
            return $"{ColorString}Modifier: {Name}\n{TaskText()}</color>"; ;
        }

        protected Modifier(PlayerControl player)
        {
            Player = player;
            if (ModifierDictionary.ContainsKey(player.PlayerId)) ModifierDictionary[player.PlayerId].Add(this);
            else ModifierDictionary.Add(player.PlayerId, new List<Modifier>() { this });
        }

        public static IEnumerable<Modifier> AllModifiers => ModifierDictionary.Values.SelectMany(x => x).ToList();
        protected internal string Name { get; set; }
        protected internal string SymbolName { get; set; }

        protected internal string GetColoredSymbol()
        {
            if (SymbolName == null) return null;

            return $"{ColorString}{SymbolName}</color>";
        }

        public string PlayerName { get; set; }
        private PlayerControl _player { get; set; }
        public PlayerControl Player
        {
            get => _player;
            set
            {
                if (_player != null) _player.nameText().color = Color.white;

                _player = value;
                PlayerName = value.Data.PlayerName;
            }
        }
        protected internal Color Color { get; set; }
        protected internal ModifierEnum ModifierType { get; set; }
        public string ColorString => "<color=#" + Color.ToHtmlStringRGBA() + ">";

        private bool Equals(Modifier other)
        {
            return Equals(Player, other.Player) && ModifierType == other.ModifierType;
        }

        internal virtual bool ModifierWin(LogicGameFlowNormal __instance)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Modifier)) return false;
            return Equals((Modifier) obj);
        }


        public override int GetHashCode()
        {
            return HashCode.Combine(Player, (int) ModifierType);
        }


        public static bool operator ==(Modifier a, Modifier b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.ModifierType == b.ModifierType && a.Player.PlayerId == b.Player.PlayerId;
        }

        public static bool operator !=(Modifier a, Modifier b)
        {
            return !(a == b);
        }

        public static Modifier[] GetModifiers(PlayerControl player)
        {
            if (player == null || !ModifierDictionary.ContainsKey(player.PlayerId)) return Array.Empty<Modifier>();
            return ModifierDictionary[player.PlayerId].ToArray();
        }

        public static IEnumerable<Modifier> GetModifiers(ModifierEnum modifiertype)
        {
            return AllModifiers.Where(x => x.ModifierType == modifiertype);
        }

        public virtual List<PlayerControl> GetTeammates()
        {
            var team = new List<PlayerControl>();
            return team;
        }

        public static T[] GetModifiers<T>(PlayerControl player) where T : Modifier
        {
            if (player == null || !ModifierDictionary.ContainsKey(player.PlayerId)) return Array.Empty<T>();
            return ModifierDictionary[player.PlayerId].Where(x => x is T).Select(x => (T)x).ToArray();
        }

        public static T GetModifier<T>(PlayerControl player) where T : Modifier
        {
            return GetModifiers<T>(player).FirstOrDefault() as T;
        }

        public static T GetModifier<T>(PlayerVoteArea player) where T : Modifier
        {
            return GetModifiers(player).Where(x => x is T).First() as T;
        }

        public static Modifier[] GetModifiers(PlayerVoteArea area)
        {
            var player = PlayerControl.AllPlayerControls.ToArray()
                .FirstOrDefault(x => x.PlayerId == area.TargetPlayerId);
            return player == null ? null : GetModifiers(player);
        }
    }

    public abstract class Ability
    {
        public static readonly Dictionary<byte, Ability> AbilityDictionary = new Dictionary<byte, Ability>();
        public Func<string> TaskText;

        protected Ability(PlayerControl player)
        {
            Player = player;
            AbilityDictionary.Add(player.PlayerId, this);
        }

        public static IEnumerable<Ability> AllAbilities => AbilityDictionary.Values.ToList();
        protected internal string Name { get; set; }

        public string PlayerName { get; set; }
        private PlayerControl _player { get; set; }
        public PlayerControl Player
        {
            get => _player;
            set
            {
                if (_player != null) _player.nameText().color = Color.white;

                _player = value;
                PlayerName = value.Data.PlayerName;
            }
        }
        protected internal Color Color { get; set; }
        protected internal AbilityEnum AbilityType { get; set; }
        public string ColorString => "<color=#" + Color.ToHtmlStringRGBA() + ">";

        private bool Equals(Ability other)
        {
            return Equals(Player, other.Player) && AbilityType == other.AbilityType;
        }

        internal virtual bool EABBNOODFGL(ShipStatus __instance)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Ability)) return false;
            return Equals((Ability)obj);
        }


        public override int GetHashCode()
        {
            return HashCode.Combine(Player, (int)AbilityType);
        }


        public static bool operator ==(Ability a, Ability b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.AbilityType == b.AbilityType && a.Player.PlayerId == b.Player.PlayerId;
        }

        public static bool operator !=(Ability a, Ability b)
        {
            return !(a == b);
        }

        public static Ability GetAbility(PlayerControl player)
        {
            return (from entry in AbilityDictionary where entry.Key == player.PlayerId select entry.Value)
                .FirstOrDefault();
        }

        public static T GetAbility<T>(PlayerControl player) where T : Ability
        {
            return GetAbility(player) as T;
        }

        public static Ability GetAbility(PlayerVoteArea area)
        {
            var player = PlayerControl.AllPlayerControls.ToArray()
                .FirstOrDefault(x => x.PlayerId == area.TargetPlayerId);
            return player == null ? null : GetAbility(player);
        }

        public static IEnumerable<Ability> GetAbilities(AbilityEnum abilitytype)
        {
            return AllAbilities.Where(x => x.AbilityType == abilitytype);
        }
    }
}