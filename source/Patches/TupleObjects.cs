using System;

namespace TownOfUs
{
    // records are a nice 1 liner that are ok if values arent modified after creation. 
    // if modification needed, use class.
    
    public record RoleData(Type Role, int Chance, bool Unique);

    
    public record ModifierData(Type Modifier, int Chance);


    public record AbilityData(Type Ability, CustomRPC Rpc, int Chance);


    public record ElevatorData(bool AtTopFloor, object Player);

    
    public class InteractionData
    {
        public bool FullCooldownReset { get; set; }
        
        public bool GaReset { get; set; }
        
        public bool SurvReset { get; set; }
        
        public bool ZeroSecReset { get; set; }
        
        public bool AbilityUsed { get; set; }

        public InteractionData(bool fullCooldownReset, bool gaReset, bool survReset, bool zeroSecReset, bool abilityUsed)
        {
            FullCooldownReset = fullCooldownReset;
            GaReset = gaReset;
            SurvReset = survReset;
            ZeroSecReset = zeroSecReset;
            AbilityUsed = abilityUsed;
        }
    }


    public class ConversionData 
    {
        public PlayerControl Player { get; set; } 
        
        public int UnconvertableChance { get; set; }
        
        public ConversionData(PlayerControl player, int unconvertableChance)
        {
            Player = player;
            UnconvertableChance = unconvertableChance;
        }
    }
}