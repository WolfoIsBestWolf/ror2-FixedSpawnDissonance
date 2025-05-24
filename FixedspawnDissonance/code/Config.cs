using BepInEx;
using BepInEx.Configuration;
using RiskOfOptions.Options;
using RiskOfOptions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RiskOfOptions.OptionConfigs;
using RoR2;

namespace FixedspawnDissonance
{

    public class WConfig
    {
        public static ConfigFile ConfigFile;
 
        public static ConfigEntry<bool> CommandChanges;
        public static ConfigEntry<bool> DissonanceChanges;
        public static ConfigEntry<bool> DevotionChanges;
        public static ConfigEntry<bool> EnigmaChanges;
        public static ConfigEntry<bool> EvolutionChanges;
        public static ConfigEntry<bool> HonorChanges;
        public static ConfigEntry<bool> KinChanges;
        public static ConfigEntry<bool> SacrificeChanges;
        public static ConfigEntry<bool> SoulChanges;
 
        public static ConfigEntry<bool> VenganceChanges;
        public static ConfigEntry<bool> SpiteChanges;
        public static ConfigEntry<bool> RebirthChanges;

        public static ConfigEntry<bool> RebirthStoreAlways;


        public static ConfigEntry<bool> KinYellowsForEnemies;

        public static ConfigEntry<bool> Honor_EliteWorms;
        public static ConfigEntry<bool> cfgEliteWormsAlways;
        public static ConfigEntry<bool> Honor_PerfectMithrix;
        public static ConfigEntry<bool> Honor_EliteMinions;
        public static ConfigEntry<bool> Honor_EliteMinionsNoGilded;
        public static ConfigEntry<bool> Honor_RedoneElites;

        public static ConfigEntry<bool> VengenceEquipment;
        public static ConfigEntry<bool> VenganceHealthRebalance;
        public static ConfigEntry<bool> VengenceBlacklist;
        public static ConfigEntry<bool> VengenceGoodDrop;

        public static ConfigEntry<float> EnigmaCooldownReduction;
        public static ConfigEntry<bool> EnigmaInterrupt;
        public static ConfigEntry<bool> EnigmaMovement;
 
        public static ConfigEntry<bool> SacrificeMoreEnemySpawns;

        public static ConfigEntry<bool> EvoBetterBlacklist;
        public static ConfigEntry<bool> EvoMoreAfterLoop;
        public static ConfigEntry<bool> EvoMoreItems;
        public static ConfigEntry<int> EvoMoreWhite;
        public static ConfigEntry<int> EvoMoreGreen;
        public static ConfigEntry<int> EvoMoreRed;
        public static ConfigEntry<bool> EvoVoidTeam;

        public static ConfigEntry<bool> DevotionInventory;
        public static ConfigEntry<bool> DevotionShowAllInventory;
        public static ConfigEntry<bool> DevotionAllowVoids;
        public static ConfigEntry<bool> DevotionAllowLunars;

        public static ConfigEntry<bool> cfgContent;

        public enum HonorWorms
        {
            Off,
            Honor,
            Always,
        }
        public enum HonorMinions
        {
            Off,
            NoDrones,
            All,
        }

        public static void InitConfig()
        {
            ConfigFile = new ConfigFile(Paths.ConfigPath + "\\Wolfo.Vanilla_Artifacts_Plus.cfg", true);

        
            CommandChanges = ConfigFile.Bind(
                ": Main :",
                "Enable Artifact of Command changes",
                true,
                "Command giving choices for Elite Equipment."
            );

            DissonanceChanges = ConfigFile.Bind(
                ": Main :",
                "Enable Artifact of Dissonance changes",
                true,
                "Adds Hermit Crabs.\n Enemies spawn as Perfected in Commencement.\n Random skins for enemies that have skins such as Titans"
            );

            EnigmaChanges = ConfigFile.Bind(
                ": Main :",
                "Enable Artifact of Enigma changes",
                true,
                "Equipment Drops replaced with Enigma Fragment that lower cooldown.\nRemoves equipment like Recycler from Enigma allowed equipment list."
            );

            DevotionChanges = ConfigFile.Bind(
                ": Main :",
                "Enable Artifact of Devotion changes",
                true,
                "Adds Devoted Lemurian inventory to TAB.\nAllow Voids to be given.\nAdds Sots Elite types. Various fixes to various issues with Devotion."
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
                "Elite Worms. Minions get Elite Equips. Perfected Mithrix, Voidtouched Voidling"
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
                "Bosses drop large chest items. Void Team drops Void Items. Slight nerf to green amounts from regular enemies. Stages start with 50% more enemies."
            );

            SoulChanges = ConfigFile.Bind(
                ": Main :",
                "Enable Artifact of Soul changes",
                true,
                "Big enemies spawn Greater Soul Wisps. Soul Wisps inherit elites. General Soul stat adjustments"
            );
 

            VenganceChanges = ConfigFile.Bind(
                ": Main :",
                "Enable Artifact of Vengance changes",
                true,
                "Umbras can use Equipment. Umbras drop better Items. General balancing, like less healing, innate defense, get less items."
            );

            SpiteChanges = ConfigFile.Bind(
                ": Main :",
                "Enable Artifact of Spite changes",
                true,
                "More chaotic Spite Bombs. Spite Bombs hurt enemies too."
            );
            RebirthChanges = ConfigFile.Bind(
                            ": Main :",
                            "Enable Artifact of Rebirth changes",
                            true,
                            "Allow Void and Lunars to be given. Get a random item from your last Rebirth run if you failed to be reborn in Prime Meridian."
                        );
            RebirthStoreAlways = ConfigFile.Bind(
                "Rebirth",
                "Rebirth | Random item on death",
                true,
                "Store a random item from your last Rebirth run if you failed to be reborn in Prime Meridian."
            );

            Honor_PerfectMithrix = ConfigFile.Bind(
                "Honor",
                "Final Bosses specific elite type",
                true,
                "Perfected Mithrix, Voidtouched Voidling, Gilded False Son."
            );
            Honor_EliteMinions = ConfigFile.Bind(
                "Honor",
                "Force Minions to be Elite",
                true,
                "When Honor is active, new Minions will be given a Tier 1 Elite Equipment. They will not recieve stat boosts."
            );
            Honor_EliteMinionsNoGilded = ConfigFile.Bind(
                "Honor",
                "Minions no Gilded",
                true,
                "Remove Gilded from the allowed pool of Minion elites. For the previous config."
            );
            Honor_RedoneElites = ConfigFile.Bind(
                "Honor",
                "Honor affect all elite types",
                true,
                "Make all elite types cheaper and weaker, not just tier 1. Allows for more modded elite types to show up."
            );
            Honor_EliteWorms = ConfigFile.Bind(
                "Honor",
                "Elite Worms",
                true,
                "Allow Elite Worms to spawn during Honor. Vanilla removes Worms entirely."
            );
            Honor_EliteWorms.SettingChanged += cfgEliteWorms_Changed;
            cfgEliteWormsAlways = ConfigFile.Bind(
                "General",
                "Allow Elite Worms always",
                false,
                "Allow Elite Worms to spawn even without Honor.\n\nElite Worms will function."
            );
            cfgEliteWormsAlways.SettingChanged += cfgEliteWorms_Changed;
            EvoMoreItems = ConfigFile.Bind(
                "Evolution",
                "Evolution | More Items",
                true,
                "Should Evolution give more items.\n\nDefault : After Looping\n3 White\n2 Green\n1 Red"
            );
            EvoMoreAfterLoop = ConfigFile.Bind(
                "Evolution",
                "Evolution | After Looping",
                true,
                "Start giving more items after or before looping."
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
                "Umbra | Health Rebalance",
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
                "Sacrifice | More monsters on Stage Start",
                true,
                "Stages spawn 50% more enemies to replace the chests. Enemies during the stage unaffected."
            );

            EnigmaInterrupt = ConfigFile.Bind(
               "Enigma",
               "Enable Interrupting Equipment",
               false,
               "Should Recycler and Tricorn be an option for Enigma"
            );
 
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

            DevotionShowAllInventory = ConfigFile.Bind(
                "Devotion",
                "Show every players Devotion Inventory",
                true,
                "Should every players, lemurians, inventory be shown, or only your lemurians inventory."
            );
            DevotionAllowVoids = ConfigFile.Bind(
                 "Devotion",
                 "Devotion Void Items",
                 true,
                 "Allow Void Items to be given to Lemurians"
             );
            DevotionAllowLunars = ConfigFile.Bind(
                 "Devotion",
                 "Devotion Lunar Items",
                 false,
                 "Allow Lunar Items to be given to Lemurians"
             );
            DevotionInventory = ConfigFile.Bind(
                "Devotion",
                "Show Devoted Lemurian Inventory",
                true,
                "Show the oldest Lemurians inventory on the scoreboard. Every players is shown alternating player, lem. See other config."
            );
            cfgContent = ConfigFile.Bind(
              ": Main :",
              "RequiredByAll Content",
              true,
              "Option to disable certain content and allows mod to be run Server-Side with people who don't share the mod.\n\nWill disable Artifact of Soul and Enigma tweaks."
          );

        }

        public static void cfgEliteWorms_Changed(object sender, System.EventArgs e)
        {
            bool SetTo = WConfig.cfgEliteWormsAlways.Value;
            if (!SetTo)
            {
                SetTo = WConfig.Honor_EliteWorms.Value && RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.EliteOnly);
            }
            CharacterSpawnCard cscMagmaWorm = LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscMagmaWorm");
            cscMagmaWorm.noElites = SetTo;
            cscMagmaWorm.eliteRules = SpawnCard.EliteRules.Default;
            CharacterSpawnCard cscElectricWorm = LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscElectricWorm");
            cscElectricWorm.noElites = SetTo;
            cscElectricWorm.eliteRules = SpawnCard.EliteRules.Default;
        }

        public static void RiskConfig()
        {
            Texture2D texture = Assets.Bundle.LoadAsset<Texture2D>("Assets/ArtifactsVanilla/icon_old.png");
            Sprite modIconS = Sprite.Create(texture, new Rect(0, 0, 256, 256), new Vector2(0.5f, 0.5f));
            ModSettingsManager.SetModIcon(modIconS);
            ModSettingsManager.SetModDescription("Additions to various vanilla Artifacts.");

            CheckBoxConfig overwriteName = new CheckBoxConfig
            {
                category = "General",
                restartRequired = true,
            };
            ModSettingsManager.AddOption(new CheckBoxOption(cfgContent, overwriteName));

            overwriteName = new CheckBoxConfig
            {
                category = "General",
                restartRequired = false,
            };
            ModSettingsManager.AddOption(new CheckBoxOption(DevotionInventory, false));
            ModSettingsManager.AddOption(new CheckBoxOption(DevotionShowAllInventory, false));
            ModSettingsManager.AddOption(new CheckBoxOption(DevotionAllowVoids, false));
            ModSettingsManager.AddOption(new CheckBoxOption(DevotionAllowLunars, false));
         
            ModSettingsManager.AddOption(new CheckBoxOption(RebirthStoreAlways, overwriteName));   

            ModSettingsManager.AddOption(new CheckBoxOption(VenganceHealthRebalance, overwriteName));
            ModSettingsManager.AddOption(new CheckBoxOption(SacrificeMoreEnemySpawns, overwriteName));
            ModSettingsManager.AddOption(new CheckBoxOption(EvoMoreItems, overwriteName));
            overwriteName.restartRequired = true;
            ModSettingsManager.AddOption(new CheckBoxOption(KinYellowsForEnemies, overwriteName));

            ModSettingsManager.AddOption(new CheckBoxOption(Honor_EliteWorms, false));
            ModSettingsManager.AddOption(new CheckBoxOption(cfgEliteWormsAlways, false));
            ModSettingsManager.AddOption(new CheckBoxOption(Honor_PerfectMithrix, false));
            ModSettingsManager.AddOption(new CheckBoxOption(Honor_EliteMinions, false));
            ModSettingsManager.AddOption(new CheckBoxOption(Honor_EliteMinionsNoGilded, false));
            ModSettingsManager.AddOption(new CheckBoxOption(Honor_RedoneElites, false));


            overwriteName = new CheckBoxConfig
            {
                category = "Artifact",
                restartRequired = true,
            };
            ModSettingsManager.AddOption(new CheckBoxOption(SacrificeChanges, overwriteName));
            ModSettingsManager.AddOption(new CheckBoxOption(RebirthChanges, overwriteName));
            ModSettingsManager.AddOption(new CheckBoxOption(HonorChanges, overwriteName));
            ModSettingsManager.AddOption(new CheckBoxOption(VenganceChanges, overwriteName));
            ModSettingsManager.AddOption(new CheckBoxOption(EnigmaChanges, overwriteName));
            ModSettingsManager.AddOption(new CheckBoxOption(SoulChanges, overwriteName));
            ModSettingsManager.AddOption(new CheckBoxOption(EvolutionChanges, overwriteName));
            ModSettingsManager.AddOption(new CheckBoxOption(DevotionChanges, overwriteName));
            ModSettingsManager.AddOption(new CheckBoxOption(DissonanceChanges, overwriteName));
            ModSettingsManager.AddOption(new CheckBoxOption(SpiteChanges, overwriteName));
        }
    }
}
