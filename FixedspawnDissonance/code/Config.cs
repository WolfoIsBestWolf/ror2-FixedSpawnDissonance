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

        public static ConfigEntry<bool> DisableNewContent;

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
        public static ConfigEntry<bool> Honor_EliteWormsAlways;
        public static ConfigEntry<bool> Honor_PerfectMithrix;
        public static ConfigEntry<bool> Honor_EliteMinions;
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



        public static void InitConfig()
        {
            ConfigFile = new ConfigFile(Paths.ConfigPath + "\\Wolfo.Vanilla_Artifacts_Plus.cfg", true);

            DisableNewContent = ConfigFile.Bind(
                ": Main :",
                "Disable New Content",
                false,
                "Disable new items/monsters added by this mod for Host mod only situations.\nMeaning Enigma Fragments and Greater Soul Wisps will not exist.\nNot sure how well other stuff works."
            );

            CommandChanges = ConfigFile.Bind(
                ": Main :",
                "Enable Artifact of Command changes",
                true,
                "Command giving choices for Elite Equipment. Void Command Essences have Particles now."
            );

            DissonanceChanges = ConfigFile.Bind(
                ": Main :",
                "Enable Artifact of Dissonance changes",
                true,
                "Adds Sots enemies and Hermit Crabs. Enemies spawn as Perfected in Commencement. Random skins for enemies that have skins such as Titans"
            );

            EnigmaChanges = ConfigFile.Bind(
                ": Main :",
                "Enable Artifact of Enigma changes",
                true,
                "Equipment Drops replaced with Enigma Fragment that lower cooldown. Removes equipment like Recycler from Enigma allowed equipment list."
            );

            DevotionChanges = ConfigFile.Bind(
                ": Main :",
                "Enable Artifact of Devotion changes",
                true,
                "Adds Devoted Lemurian inventory to TAB. Allow Voids to be given. Adds Sots Elite types. Various fixes to various issues with Devotion."
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
                "Rebirth store random item",
                true,
                "Get a random item from your last Rebirth run if you failed to be reborn in Prime Meridian."
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
                "When Honor is active, new Minions will be given a Tier 1 Elite Equipment"
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
            Honor_EliteWorms.SettingChanged += Honor_EliteWormsAlways_SettingChanged;
            Honor_EliteWormsAlways = ConfigFile.Bind(
                "Honor",
                "Elite Worms Always",
                false,
                "Allow Elite Worms to spawn always."
            );
            Honor_EliteWormsAlways.SettingChanged += Honor_EliteWormsAlways_SettingChanged;
            EvoMoreItems = ConfigFile.Bind(
                "Evolution",
                "Give more items",
                true,
                "Turn the whole module on or off."
            );
            EvoMoreAfterLoop = ConfigFile.Bind(
                "Evolution",
                "Evo more items only after Loop",
                true,
                "For when you think 2/3 Whites at the start is too much but want enemies to still get a powerboost later."
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
                "Sacrifice more monsters at Stage Start",
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





            RiskConfig();
        }

        private static void Honor_EliteWormsAlways_SettingChanged(object sender, System.EventArgs e)
        {
            if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.EliteOnly))
            {
                Honor.Worm_EliteStuff(true);
            }
            else
            {
                Honor.Worm_EliteStuff(false);
            }
        }

        public static void RiskConfig()
        {
            Texture2D texture = Assets.Bundle.LoadAsset<Texture2D>("Assets/ArtifactsVanilla/icon_old.png");
            Sprite modIconS = Sprite.Create(texture, new Rect(0, 0, 256, 256), new Vector2(0.5f, 0.5f));
            ModSettingsManager.SetModIcon(modIconS);
            ModSettingsManager.SetModDescription("Additions to various vanilla Artifacts.");


            ModSettingsManager.AddOption(new CheckBoxOption(DevotionInventory, false));
            ModSettingsManager.AddOption(new CheckBoxOption(DevotionShowAllInventory, false));
            ModSettingsManager.AddOption(new CheckBoxOption(DevotionAllowVoids, false));
            ModSettingsManager.AddOption(new CheckBoxOption(DevotionAllowLunars, false));
            CheckBoxConfig overwriteName = new CheckBoxConfig
            {
                category = "General",
                restartRequired = false,
            };
            ModSettingsManager.AddOption(new CheckBoxOption(RebirthStoreAlways, overwriteName));   

            ModSettingsManager.AddOption(new CheckBoxOption(VenganceHealthRebalance, overwriteName));
            ModSettingsManager.AddOption(new CheckBoxOption(SacrificeMoreEnemySpawns, overwriteName));
            ModSettingsManager.AddOption(new CheckBoxOption(EvoMoreItems, overwriteName));
            overwriteName.restartRequired = true;
            ModSettingsManager.AddOption(new CheckBoxOption(KinYellowsForEnemies, overwriteName));

            ModSettingsManager.AddOption(new CheckBoxOption(Honor_EliteWorms, false));
            ModSettingsManager.AddOption(new CheckBoxOption(Honor_EliteWormsAlways, false));
            ModSettingsManager.AddOption(new CheckBoxOption(Honor_PerfectMithrix, false));
            ModSettingsManager.AddOption(new CheckBoxOption(Honor_EliteMinions, false));
            ModSettingsManager.AddOption(new CheckBoxOption(Honor_RedoneElites, false));


            ModSettingsManager.AddOption(new CheckBoxOption(DisableNewContent, true));

            ModSettingsManager.AddOption(new CheckBoxOption(SacrificeChanges, true));
            ModSettingsManager.AddOption(new CheckBoxOption(RebirthChanges, true));
            ModSettingsManager.AddOption(new CheckBoxOption(HonorChanges, true));
            ModSettingsManager.AddOption(new CheckBoxOption(VenganceChanges, true));
            ModSettingsManager.AddOption(new CheckBoxOption(EnigmaChanges, true));
            ModSettingsManager.AddOption(new CheckBoxOption(SoulChanges, true));
            ModSettingsManager.AddOption(new CheckBoxOption(EvolutionChanges, true));
            ModSettingsManager.AddOption(new CheckBoxOption(DevotionChanges, true));
            ModSettingsManager.AddOption(new CheckBoxOption(DissonanceChanges, true));
            ModSettingsManager.AddOption(new CheckBoxOption(SpiteChanges, true));
        }
    }
}
