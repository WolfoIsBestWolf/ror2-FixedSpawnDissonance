using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Artifacts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace FixedspawnDissonance
{

    public class WConfig
    {
        public static ConfigFile ConfigFile;

        public static ConfigEntry<bool> CommandChanges;
        public static ConfigEntry<bool> DissonanceChanges;
        public static ConfigEntry<bool> EnigmaChanges;
        public static ConfigEntry<bool> EvolutionChanges;
        public static ConfigEntry<bool> HonorChanges;
        public static ConfigEntry<bool> KinChanges;
        public static ConfigEntry<bool> SacrificeChanges;
        public static ConfigEntry<bool> SoulChanges;
        public static ConfigEntry<bool> SwarmsChanges;
        public static ConfigEntry<bool> VenganceChanges;
        public static ConfigEntry<bool> SpiteChanges;
        //public static ConfigEntry<bool> FrailtyChanges;
        //public static ConfigEntry<bool> EnableGeneralChanges;




        //public static ConfigEntry<bool> DissonanceScavsAllStage;
        //public static ConfigEntry<bool> DissonancePerfectedForAll;

        //public static ConfigEntry<bool> KinNoRepeatConfig;
        public static ConfigEntry<bool> KinYellowsForEnemies;

        public static ConfigEntry<string> HonorEliteWormRules;
        public static ConfigEntry<bool> HonorPerfectedLunarBosses;
        public static ConfigEntry<bool> HonorStartingEliteEquip;
        public static ConfigEntry<bool> HonorMinionAlwaysElite;

        public static ConfigEntry<bool> VengenceEquipment;
        public static ConfigEntry<bool> VenganceHealthRebalance;
        public static ConfigEntry<bool> VengenceBlacklist;
        public static ConfigEntry<bool> VengenceGoodDrop;

        public static ConfigEntry<float> EnigmaCooldownReduction;
        public static ConfigEntry<bool> EnigmaInterrupt;
        //public static ConfigEntry<bool> EnigmaDestructive;
        //public static ConfigEntry<bool> EnigmaDirectional;
        public static ConfigEntry<bool> EnigmaMovement;
        //public static ConfigEntry<bool> EnigmaTonic;

        //public static ConfigEntry<bool> SacrificeVoids;
        public static ConfigEntry<bool> SacrificeMoreEnemySpawns;

        public static ConfigEntry<bool> EvoBetterBlacklist;
        public static ConfigEntry<bool> EvoMoreAfterLoop;
        public static ConfigEntry<bool> EvoMoreItems;
        public static ConfigEntry<int> EvoMoreWhite;
        public static ConfigEntry<int> EvoMoreGreen;
        public static ConfigEntry<int> EvoMoreRed;
        public static ConfigEntry<int> EvoVoidTeam;


        //public static ConfigEntry<bool> RarerEnemies;
        //public static ConfigEntry<bool> BullshitScavs;
        //public static ConfigEntry<bool> DebugPrintDissonanceSpawns;
        //public static ConfigEntry<int> ScavWithYellowItem;
        //public static ConfigEntry<bool> VengenceAmbient;
        //public static ConfigEntry<bool> VengencePlayerLevelMulti;
        //public static ConfigEntry<bool> CommandAffixChoice;
        //public static ConfigEntry<bool> ScavEquipmentBlacklist;
        //public static ConfigEntry<bool> VengenceHelfire;

        //public static ConfigEntry<bool> MoreEvoLimboThing;
        //public static ConfigEntry<bool> MoreEvoMoon2Thing;
        //public static ConfigEntry<int> EnigmaCooldown;
        //public static ConfigEntry<bool> EnigmaUpgrade;
        //public static ConfigEntry<bool> EnigmaChatMessage;

        //public static ConfigEntry<bool> SoulImmortal;
        //public static ConfigEntry<bool> SoulStats;
        //public static ConfigEntry<bool> SoulSpeed;
        //public static ConfigEntry<bool> SoulLesserDecay;
        //public static ConfigEntry<bool> SoulGreaterDecay;


        public static void InitConfig()
        {
            ConfigFile = new ConfigFile(Paths.ConfigPath + "\\Wolfo.Vanilla_Artifacts_Plus.cfg", true);

            CommandChanges = ConfigFile.Bind(
                ": Main :",
                "Enable Artifact of Command changes",
                true,
                "Command Essences with 1 choices automatically pick the item. Command giving choices for Elite Equipment"
            );

            DissonanceChanges = ConfigFile.Bind(
                ": Main :",
                "Enable Artifact of Dissonance changes",
                true,
                "General balance. Enemies spawning as Perfected in Commencement. Random skins for enemies that have skins such as Titans"
            );

            EnigmaChanges = ConfigFile.Bind(
                ": Main :",
                "Enable Artifact of Enigma changes",
                true,
                "Replacing Equipment Drops with Enigma Fragment. Enabling/Disabling various Equipment for Enigma Compatible"
            );

            EvolutionChanges = ConfigFile.Bind(
                ": Main :",
                "Enable Artifact of Evolution changes",
                true,
                "Evo giving more items after looping. Better Item Blacklist. Void Team gets void items."
            );


            HonorChanges = ConfigFile.Bind(
                ": Main :",
                "Enable Artifact of Honor changes",
                true,
                "Elite Worms. Starting with Elite Equip. Minions get Elite Equips. Perfected Mithrix, Voidtouched Voidling"
            );

            KinChanges = ConfigFile.Bind(
                ": Main :",
                "Enable Artifact of Kin changes",
                true,
                "Teleporter can still drop yellow items based on the enemy. No repeat enemies"
            );

            SacrificeChanges = ConfigFile.Bind(
                ": Main :",
                "Enable Artifact of Sacrifice changes",
                true,
                "Void Team drops Void Items. Stages start with 50% more enemies on them as they replace chests"
            );

            SoulChanges = ConfigFile.Bind(
                ": Main :",
                "Enable Artifact of Soul changes",
                true,
                "Big enemies spawn Greater Soul Wisps. Soul Wisps inherit elites. General Soul stat adjustments"
            );

            SwarmsChanges = ConfigFile.Bind(
                ": Main :",
                "Enable Artifact of Swarm changes",
                true,
                "Get like 4 empathy cores instead of 2 like how you get 2 beetle guards instead of 1."
            );

            VenganceChanges = ConfigFile.Bind(
                ": Main :",
                "Enable Artifact of Vengance changes",
                true,
                "Umbras can use Equipment. General Umbra balance hopefully for the better. Umbras drop better Items."
            );

            SpiteChanges = ConfigFile.Bind(
                ": Main :",
                "Enable Artifact of Spite changes",
                true,
                "Spite, but like, more. Damage scales with stages beaten"
            );

            /*FrailtyChanges = ConfigFile.Bind(
                ": Main :",
                "Enable Artifact of Frailty changes",
                false,
                "Remove all fall damage immunity"
            );*/







            HonorPerfectedLunarBosses = ConfigFile.Bind(
                "Honor",
                "Honor making Perfected Lunar Bosses and Voidtouched Void bosses",
                true,
                "Affects all Mithrix phases, Twisted Scavengers and Voidling. If turned off they will go back to being their guaranteed specific tier 1 elite."
            );
            HonorStartingEliteEquip = ConfigFile.Bind(
                "Honor",
                "Elite Starting Equip",
                false,
                "When starting a run with Artifact of Honor, recieve a random Tier 1 Elite Equipment"
            );
            HonorMinionAlwaysElite = ConfigFile.Bind(
                "Honor",
                "Force Minions to be Elite",
                true,
                "When Honor is active, new Minions will be given a Tier 1 Elite Equipment"
            );
            HonorEliteWormRules = ConfigFile.Bind<string>(
                "Honor",
                "Elite Worm Rules",
                "HonorOnly",
                "Enable/Disable Elite Magma & Overloading Worms. Elites with an effect around the character might look weird as the Worm character can often be far away from the Worm\nValid Options:\nAlways:        Worms can always spawn as elites\nHonorOnly:  Worms will be Elites when Artifact of Honor is enabled\nNever:          Worms will never be Elite (Vanilla)"
            );

            EvoMoreItems = ConfigFile.Bind(
                "Evolution",
                "Give more items",
                true,
                "Turn the whole module on or off."
            );
            EvoMoreAfterLoop = ConfigFile.Bind(
                "Evolution",
                "Multiply items only after Loop",
                true,
                "For when you think 3/5 Whites at the start is too much but want enemies to still get a powerboost later."
            );

            EvoMoreWhite = ConfigFile.Bind(
                "Evolution",
                "Amount of Whites",
                3,
                "If a white item is given, how many. (do Not set to 0)"
            );
            EvoMoreGreen = ConfigFile.Bind(
                "Evolution",
                "Amount of Green",
                2,
                "If a green item is given, how many. (do Not set to 0)"
            );
            EvoMoreRed = ConfigFile.Bind(
                "Evolution",
                "Amount of Red",
                1,
                "If a red item is given, how many. (do Not set to 0)"
            );



            VengenceEquipment = ConfigFile.Bind(
                "Vengence",
                "Make Umbras use Equipment",
                true,
                "Makes them use their equipment at mostly appropiate times and semi close ranges but their AI can be strange.\nThey are able to use Equipment by default with Gesture but this will make them use it while attacking."
            );
            VengenceGoodDrop = ConfigFile.Bind(
                "Vengence",
                "Umbras only drop high tier items.",
                true,
                "Umbras only drop Green/Red/Yellow (if one is present)"
            );

            VenganceHealthRebalance = ConfigFile.Bind(
                "Vengence",
                "Umbra Health Rebalance",
                true,
                "Umbras get less health but start with an Opal and Adaptive Armor."
            );

            VengenceBlacklist = ConfigFile.Bind(
                "Vengence",
                "PreSet Item Blacklist for Umbras",
                true,
                "Make Umbras not spawn with items like Tougher Timers or Nkuhanas Opinion.\nThis is to prevent unfair situations or unavoidable autoplay on the Umbras part making them not fun to fight."
            );
            SacrificeMoreEnemySpawns = ConfigFile.Bind(
                "Sacrifice",
                "More Stage enemy spawns",
                true,
                "Stages spawn with more enemy on them"
            );

            EnigmaInterrupt = ConfigFile.Bind(
               "Enigma",
               "Enable Interrupting Equipment",
               false,
               "Should Recycler and Tricorn be an option for Enigma"
            );
            /*EnigmaDestructive = ConfigFile.Bind(
               "Enigma",
               "Enable Descructive Equipment",
               true,
               "Should Helfire Tincture, Glowing Meteorite, Effigy of Grief be an option for Enigma"
           );*/
            /*EnigmaTonic = ConfigFile.Bind(
                "Enigma",
                "Enable Spinel Tonic",
                false,
                 "Should Spinel Tonic be an option for Enigma"
                       );*/
            EnigmaMovement = ConfigFile.Bind(
                           "Enigma",
                           "Enable Movement affecting Equipment",
                           true,
                           "Should Volcanic Egg and Milky Chrysalis be an option for Enigma"
                       );


            EnigmaCooldownReduction = ConfigFile.Bind(
                           "Enigma",
                           "Enigma Fragment Equipment Cooldown reduction",
                           12f,
                           ""
                       );


            KinYellowsForEnemies = ConfigFile.Bind(
                "Kin",
                "Yellow Items Drops for Hordes of Many",
                true,
                "Enable/Disable Hordes of Many dropping Yellow items depending on the Enemy\nPrimarily intended with Kin but helps in general"
            );

            EvoBetterBlacklist = ConfigFile.Bind(
                "Evolution",
                "More AI Blacklist items",
                true,
                "Prevents Scavs and Enemies from spawning with; Nkuhanas Opinion, Aegis, Happiest Mask, Ghors Tome, Death Mark, Infusion\n Prevents Artifact of Evolution giving; Tesla Coil, Razorwire"
            );


            /*
            SoulImmortal = ConfigFile.Bind(
                           "1h - Soul",
                           "Greater Soul Wisps",
                           true,
                           "Should tougher enemies spawn Greater Soul Wisps"
                       );
            */



            /*SacrificeVoids = ConfigFile.Bind(
                "1k - Sacrifice",
                "Void Enemies drop Void Items",
                true,
                "While they have added ways to get void items from Sacrifice, it still feels fitting."
            );*/

            /*KinNoRepeatConfig = ConfigFile.Bind(
    "1d - Kin",
    "No Kin repeats",
    true,
    "Tries to pick a different enemy than the one you had last stage when using Artifact of Kin."
);*/
            /*DissonanceScavsAllStage = ConfigFile.Bind(
    "Dissonance",
    "Scavengers on every stage while Dissonant",
    false,
    "While using Artifact of Dissonance\nIf disabled Scavengers will have a 25% chance to be possible spawn on the current stage like every other boss\nIf enabled Scavengers will be a possible spawn on every stage"
);
DissonancePerfectedForAll = ConfigFile.Bind(
    "Dissonance",
    "All enemies can be Perfected on Commencement",
    true,
    "Sets the EliteRules for all enemies selected by Dissonance on Commencement to the Lunar rule.\nMeaning only Perfected elites normally and Perfected + Tier 1 Elites with Honor"
);*/

 
        }

    }
}
