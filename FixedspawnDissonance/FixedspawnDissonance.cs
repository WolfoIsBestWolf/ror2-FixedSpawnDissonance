using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Artifacts;
using RoR2.Items;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace FixedspawnDissonance
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Wolfo.FixedSpawnDissonance", "FixedSpawnDissonance", "2.0.2")]
    [R2APISubmoduleDependency(nameof(ItemAPI), nameof(PrefabAPI), nameof(LanguageAPI))]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]


    public class FixedspawnDissonance : BaseUnityPlugin
    {
        public static GameObject PickupDisplayVoidParticleSystem;
        public static System.Random Random = new System.Random();

        public bool InProcessOfMoreEvoItems = false;
        public bool cardsadded = false;
        public static bool eliteequipshonorforstage = false;
        public static bool runstartonetimeonly = false;

        public static Inventory MonsterTeamGainItemRandom = null;
        //public static bool MonsterTeamGainExtraItemGiverAdded = false;

        public static ItemDef EnigmaFragmentPurple = null;
        public static float EnigmaFragmentCooldownReduction = 0.90f;

        public static Inventory SoulInventoryToCopy = null;
        public static EquipmentIndex SoulEliteEquipmentIndex = EquipmentIndex.None;
        public static int SoulGreaterDecider = 0;

        private static Rect rec128 = new Rect(0, 0, 128, 128);
        private static Vector2 half = new Vector2(0.5f, 0.5f);

        public static GameObject WarbannerObject = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/networkedobjects/WarbannerWard");
        //public static GameObject WarbannerObjectEnemy = R2API.PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/networkedobjects/WarbannerWard"), "WarbannerWardEnemy", true);

        public static Material WarbannerFlagNormal = WarbannerObject.transform.GetChild(1).GetChild(1).GetComponent<SkinnedMeshRenderer>().material;
        public static Material WarbannerFlagEnemy = null;
        public static Material WarbannerSphereNormal = WarbannerObject.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material;
        public static Material WarbannerSphereEnemy = null;


        public static GameObject SoulGreaterWispBody = R2API.PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/GreaterWispBody"), "GreaterWispSoulBody", true);
        public static GameObject SoulGreaterWispMaster = R2API.PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/GreaterWispMaster"), "GreaterWispSoulMaster", true);
        public static GameObject SoulLesserWispBody = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/WispSoulBody");
        public static GameObject SoulLesserWispMaster = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/WispSoulMaster");
        //public static GameObject EnigmaDisplayPurple = null;
        public static MasterCatalog.MasterIndex SoulGreaterWispMasterIndex = MasterCatalog.MasterIndex.none;
        public static MasterCatalog.MasterIndex MasterIndexAffixHealingCore = MasterCatalog.MasterIndex.none;


        public static BasicPickupDropTable dtMonsterTeamTier1Item = Addressables.LoadAssetAsync<BasicPickupDropTable>(key: "RoR2/Base/MonsterTeamGainsItems/dtMonsterTeamTier1Item.asset").WaitForCompletion();
        public static BasicPickupDropTable dtMonsterTeamTier2Item = Addressables.LoadAssetAsync<BasicPickupDropTable>(key: "RoR2/Base/MonsterTeamGainsItems/dtMonsterTeamTier2Item.asset").WaitForCompletion();
        public static BasicPickupDropTable dtMonsterTeamTier3Item = Addressables.LoadAssetAsync<BasicPickupDropTable>(key: "RoR2/Base/MonsterTeamGainsItems/dtMonsterTeamTier3Item.asset").WaitForCompletion();
        public static BasicPickupDropTable dtMonsterTeamLunarItem = ScriptableObject.CreateInstance<BasicPickupDropTable>();

        public static BasicPickupDropTable dtSacrificeArtifact = LegacyResourcesAPI.Load<BasicPickupDropTable>("DropTables/dtSacrificeArtifact");
        public static BasicPickupDropTable dtSacrificeArtifactVoid = ScriptableObject.CreateInstance<BasicPickupDropTable>();


        public static readonly ArtifactDef DissonanceArtifactVariable = RoR2.LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/MixEnemy");
        public static readonly ArtifactDef MonsterTeamGain = RoR2.LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/MonsterTeamGainsItems");

        //public static ConfigEntry<bool> RarerEnemies;
        public static ConfigEntry<bool> ScavsAllStage;
        public static ConfigEntry<bool> PerectedForAll;
        public static ConfigEntry<bool> YellowItemDropsForEnemies;
        public static ConfigEntry<string> EliteWormRules;
        public static ConfigEntry<bool> ExtraEvoBlacklist;
        //public static ConfigEntry<bool> BullshitScavs;
        //public static ConfigEntry<bool> DebugPrintDissonanceSpawns;

        public static ConfigEntry<bool> PerfectedLunarBosses;
        public static ConfigEntry<bool> HonorStartingEliteEquip;

        public static ConfigEntry<bool> HonorMinionAlwaysElite;
        public static ConfigEntry<bool> KinNoRepeatConfig;
        //public static ConfigEntry<int> ScavWithYellowItem;
        //public static ConfigEntry<bool> VengenceAmbient;
        public static ConfigEntry<bool> VengenceEquipment;
        public static ConfigEntry<bool> VengenceHalfHealth;
        public static ConfigEntry<bool> VengencePlayerLevel;
        //public static ConfigEntry<bool> VengencePlayerLevelMulti;
        //public static ConfigEntry<bool> CommandAffixChoice;
        public static ConfigEntry<bool> VengenceBlacklist;
        public static ConfigEntry<bool> ScavEquipmentBlacklist;
        //public static ConfigEntry<bool> VengenceHelfire;
        public static ConfigEntry<bool> VengenceGoodDrop;

        //public static ConfigEntry<bool> EnigmaUpgrade;
        public static ConfigEntry<bool> EnigmaInterrupt;
        public static ConfigEntry<bool> EnigmaDestructive;
        public static ConfigEntry<bool> EnigmaDirectional;
        public static ConfigEntry<bool> EnigmaMovement;
        public static ConfigEntry<bool> EnigmaTonic;
        //public static ConfigEntry<int> EnigmaCooldown;
        public static ConfigEntry<float> EnigmaCooldownReduction;
        //public static ConfigEntry<bool> EnigmaChatMessage;

        //public static ConfigEntry<bool> SoulImmortal;
        //public static ConfigEntry<bool> SoulStats;
        //public static ConfigEntry<bool> SoulSpeed;
        //public static ConfigEntry<bool> SoulLesserDecay;
        //public static ConfigEntry<bool> SoulGreaterDecay;

        public static ConfigEntry<bool> FrailtyChanges;
        public static ConfigEntry<bool> SacrificeVoids;

        public static ConfigEntry<bool> MoreEvoAfterLoop;
        public static ConfigEntry<bool> MoreEvoLimboThing;
        public static ConfigEntry<bool> MoreEvoMoon2Thing;
        public static ConfigEntry<bool> MoreEvoItems;
        public static ConfigEntry<int> MoreEvoWhite;
        public static ConfigEntry<int> MoreEvoGreen;
        public static ConfigEntry<int> MoreEvoRed;


        //public static ConfigEntry<bool> NoForbiddenBoss;

        //public static CharacterSpawnCard DissoTitan = Instantiate<CharacterSpawnCard>(RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/titan/cscTitanBlackBeach"));

        public static DirectorCard DissoBeetle = null;
        public static DirectorCard DissoGolem = null;
        public static DirectorCard DissoTitan = null;
        public static DirectorCard DissoVermin = null;
        public static DirectorCard DissoVerminFlying = null;


        public static CharacterSpawnCard SkinnedSpawnVermin1 = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/DLC1/Vermin/cscVermin.asset").WaitForCompletion();
        public static CharacterSpawnCard SkinnedSpawnVermin2 = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/DLC1/Vermin/cscVerminSnowy.asset").WaitForCompletion();

        public static CharacterSpawnCard SkinnedSpawnFlyingVermin1 = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/DLC1/FlyingVermin/cscFlyingVermin.asset").WaitForCompletion();
        public static CharacterSpawnCard SkinnedSpawnFlyingVermin2 = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/DLC1/FlyingVermin/cscFlyingVerminSnowy.asset").WaitForCompletion();

        public static CharacterSpawnCard SkinnedSpawnBeetle1 = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Beetle/cscBeetle.asset").WaitForCompletion();
        public static CharacterSpawnCard SkinnedSpawnBeetleShiny = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Beetle/cscBeetleSulfur.asset").WaitForCompletion();

        public static CharacterSpawnCard SkinnedSpawnsGolem1 = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Golem/cscGolem.asset").WaitForCompletion();
        public static CharacterSpawnCard SkinnedSpawnsGolem2 = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Golem/cscGolemNature.asset").WaitForCompletion();
        public static CharacterSpawnCard SkinnedSpawnsGolem3 = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Golem/cscGolemSandy.asset").WaitForCompletion();
        public static CharacterSpawnCard SkinnedSpawnsGolem4 = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Golem/cscGolemSnowy.asset").WaitForCompletion();

        public static CharacterSpawnCard SkinnedSpawnsTitan1 = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Titan/cscTitanBlackBeach.asset").WaitForCompletion();
        public static CharacterSpawnCard SkinnedSpawnsTitan2 = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Titan/cscTitanDampCave.asset").WaitForCompletion();
        public static CharacterSpawnCard SkinnedSpawnsTitan3 = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Titan/cscTitanGolemPlains.asset").WaitForCompletion();
        public static CharacterSpawnCard SkinnedSpawnsTitan4 = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Titan/cscTitanGooLake.asset").WaitForCompletion();

        //Honor Stupid Elite
        public static GivePickupsOnStart HonorAffixGiverVoidInfestor = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/EliteVoid/VoidInfestorMaster.prefab").WaitForCompletion().AddComponent<GivePickupsOnStart>();
        public static GivePickupsOnStart HonorAffixGiverVoidling0 = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/VoidRaidCrab/MiniVoidRaidCrabMasterBase.prefab").WaitForCompletion().AddComponent<GivePickupsOnStart>();
        public static GivePickupsOnStart HonorAffixGiverVoidling1 = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/VoidRaidCrab/MiniVoidRaidCrabMasterPhase1.prefab").WaitForCompletion().AddComponent<GivePickupsOnStart>();
        public static GivePickupsOnStart HonorAffixGiverVoidling2 = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/VoidRaidCrab/MiniVoidRaidCrabMasterPhase2.prefab").WaitForCompletion().AddComponent<GivePickupsOnStart>();
        public static GivePickupsOnStart HonorAffixGiverVoidling3 = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/VoidRaidCrab/MiniVoidRaidCrabMasterPhase3.prefab").WaitForCompletion().AddComponent<GivePickupsOnStart>();

        public static GivePickupsOnStart ScavLunar1LunarAffixGiver = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/ScavLunar1Master").AddComponent<GivePickupsOnStart>();
        public static GivePickupsOnStart ScavLunar2LunarAffixGiver = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/ScavLunar2Master").AddComponent<GivePickupsOnStart>();
        public static GivePickupsOnStart ScavLunar3LunarAffixGiver = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/ScavLunar3Master").AddComponent<GivePickupsOnStart>();
        public static GivePickupsOnStart ScavLunar4LunarAffixGiver = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/ScavLunar4Master").AddComponent<GivePickupsOnStart>();
        public static GivePickupsOnStart BrotherLunarAffixGiver = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/BrotherMaster").AddComponent<GivePickupsOnStart>();
        public static GivePickupsOnStart BrotherHurtLunarAffixGiver = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/BrotherHurtMaster").AddComponent<GivePickupsOnStart>();
        public static GivePickupsOnStart BrotherHauntLunarAffixGiver = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/BrotherHauntMaster").AddComponent<GivePickupsOnStart>();
        public static GivePickupsOnStart BrotherGlassLunarAffixGiver = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/BrotherGlassMaster").AddComponent<GivePickupsOnStart>();

        public static GivePickupsOnStart GoldTitanAllyHonorGiver = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/TitanGoldAllyMaster").AddComponent<GivePickupsOnStart>();
        public static GivePickupsOnStart HonorAffixGiverSuperRoboBall = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/SuperRoboBallBossMaster").AddComponent<GivePickupsOnStart>();
        public static GivePickupsOnStart HonorAffixGiverGoldTitan = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/TitanGoldMaster").AddComponent<GivePickupsOnStart>();

        //public static GivePickupsOnStart SoulLesserWispGiver = null;
        //public static GivePickupsOnStart SoulGreaterWispGiver = null;

        private static GameObject BatteryPodPanel = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/networkedobjects/QuestVolatileBatteryWorldPickup");

        public static CharacterSpawnCard MagmaWormEliteHonor = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscMagmaWorm");
        public static CharacterSpawnCard ElectricWormEliteHonor = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscElectricWorm");

        public static DirectorCardCategorySelection KinBackup = ScriptableObject.CreateInstance<DirectorCardCategorySelection>();
        public static SpawnCard KinNoRepeat;
        public static CharacterMaster tempClayMan;


        static int TotalItemCount = 132;
        static int ZetDumbItemThing = 1;




        public static List<EliteIndex> EliteDefsTemp = new List<EliteIndex>();
        public static List<EquipmentDef> EliteEquipmentDefs = new List<EquipmentDef>();
        public static List<EquipmentIndex> EliteEquipmentEquipmentIndexCommand = new List<EquipmentIndex>();
        public static PickupPickerController.Option[] EliteEquipmentChoicesForCommand;


        public static List<EquipmentDef> EliteEquipmentHonorDefs = new List<EquipmentDef>();


        static readonly System.Random random = new System.Random();
        //public static EquipmentDef temptempEliteEquip;


        public static WeightedSelection<DirectorCard> LunarifiedList = new WeightedSelection<DirectorCard>();

        public static int LunarDone = 0;
        public static bool LoopEvoMultiplierDone = true;

        //ConfigEntry<T> Bind<T>(string section, string key, T defaultValue, string description);
        //ConfigManager.StatsDisplayStatString = ConfigManager.Bind<string>(ConfigManager.ConfigFileStatsDisplay

        private static void LunarAffixDisable()
        {
            ScavLunar1LunarAffixGiver.enabled = false;
            ScavLunar2LunarAffixGiver.enabled = false;
            ScavLunar3LunarAffixGiver.enabled = false;
            ScavLunar4LunarAffixGiver.enabled = false;
            BrotherLunarAffixGiver.enabled = false;
            BrotherHurtLunarAffixGiver.enabled = false;
            BrotherHauntLunarAffixGiver.enabled = false;
            BrotherGlassLunarAffixGiver.enabled = false;

            HonorAffixGiverVoidling0.enabled = false;
            HonorAffixGiverVoidling1.enabled = false;
            HonorAffixGiverVoidling2.enabled = false;
            HonorAffixGiverVoidling3.enabled = false;

            HonorAffixGiverVoidInfestor.enabled = false;

            HonorAffixGiverGoldTitan.enabled = false;
            HonorAffixGiverSuperRoboBall.enabled = false;
            GoldTitanAllyHonorGiver.enabled = false;
        }
        private static void LunarAffixEnable()
        {
            ScavLunar1LunarAffixGiver.enabled = true;
            ScavLunar2LunarAffixGiver.enabled = true;
            ScavLunar3LunarAffixGiver.enabled = true;
            ScavLunar4LunarAffixGiver.enabled = true;
            BrotherLunarAffixGiver.enabled = true;
            BrotherHurtLunarAffixGiver.enabled = true;
            BrotherHauntLunarAffixGiver.enabled = true;
            BrotherGlassLunarAffixGiver.enabled = true;

            HonorAffixGiverVoidling0.enabled = true;
            HonorAffixGiverVoidling1.enabled = true;
            HonorAffixGiverVoidling2.enabled = true;
            HonorAffixGiverVoidling3.enabled = true;
            HonorAffixGiverVoidInfestor.enabled = false;

            HonorAffixGiverGoldTitan.enabled = true;
            HonorAffixGiverSuperRoboBall.enabled = true;
            GoldTitanAllyHonorGiver.enabled = true;
        }


        private void InitConfig()
        {
            ScavsAllStage = Config.Bind(
                "1a - Dissonance",
                "Scavengers on every stage while Dissonant",
                false,
                "While using Artifact of Dissonance\nIf disabled Scavengers will have a 25% chance to be possible spawn on the current stage like every other boss\nIf enabled Scavengers will be a possible spawn on every stage"
            );
            PerectedForAll = Config.Bind(
                "1a - Dissonance",
                "All enemies can be Perfected on Commencement",
                true,
                "Sets the EliteRules for all enemies selected by Dissonance on Commencement to the Lunar rule.\nMeaning only Perfected elites normally and Perfected + Tier 1 Elites with Honor"
            );
            /*
            RarerEnemies = Config.Bind(
                "1a - Dissonance",
                "Less frequent spawns of usually rare enemies.",
                true,
                "Makes Hermit Crabs/Solus Probes spawn less often (they're still selected at the same rate). Hermit Crabs spawn half as often as other enemies in areas they normally spawn in."
            );
            */


            PerfectedLunarBosses = Config.Bind(
                "1b - Honor",
                "Honor making Perfected Lunar Bosses and Voidtouched Void bosses",
                true,
                "Affects all Mithrix phases, Twisted Scavengers and Voidling. If turned off they will go back to being their guaranteed specific tier 1 elite."
            );
            HonorStartingEliteEquip = Config.Bind(
                "1b - Honor",
                "Elite Starting Equip",
                true,
                "When starting a run with Artifact of Honor, recieve a random Tier 1 Elite Equipment"
            );
            HonorMinionAlwaysElite = Config.Bind(
                "1b - Honor",
                "Force Minions to be Elite",
                true,
                "When Honor is active, new Minions will be given a Tier 1 Elite Equipment"
            );
            EliteWormRules = Config.Bind<string>(
                "1b - Honor",
                "Elite Worm Rules",
                "HonorOnly",
                "Enable/Disable Elite Magma & Overloading Worms. Elites with an effect around the character might look weird as the Worm character can often be far away from the Worm\nValid Options:\nAlways:        Worms can always spawn as elites\nHonorOnly:  Worms will be Elites when Artifact of Honor is enabled\nNever:          Worms will never be Elite (Vanilla)"
            );

            MoreEvoItems = Config.Bind(
                "1c - Evolution",
                "Give more items",
                true,
                "Turn the whole module on or off."
            );
            MoreEvoAfterLoop = Config.Bind(
                "1c - Evolution",
                "Multiply items only after Loop",
                true,
                "For when you think 3/5 Whites at the start is too much but want enemies to still get a powerboost later."
            );

            MoreEvoWhite = Config.Bind(
                "1c - Evolution",
                "Amount of Whites",
                3,
                "If a white item is given, how many. (do Not set to 0)"
            );
            MoreEvoGreen = Config.Bind(
                "1c - Evolution",
                "Amount of Green",
                2,
                "If a green item is given, how many. (do Not set to 0)"
            );
            MoreEvoRed = Config.Bind(
                "1c - Evolution",
                "Amount of Red",
                1,
                "If a red item is given, how many. (do Not set to 0)"
            );

            KinNoRepeatConfig = Config.Bind(
                "1d - Kin",
                "No Kin repeats",
                true,
                "Tries to pick a different enemy than the one you had last stage when using Artifact of Kin."
            );

            /*
            VengenceAmbient = Config.Bind(
                "1e - Vengence",
                "Make Umbras scale with Ambient Level",
                true,
                "Umbras from Artifact of Vengence are always at level 1 likely to a bug.\nAlso makes them drop high tier items only due to being a greater challenge."
            );
            */
            VengenceEquipment = Config.Bind(
                "1e - Vengence",
                "Make Umbras use Equipment",
                true,
                "Makes them use their equipment at mostly appropiate times and semi close ranges but their AI can be strange.\nThey are able to use Equipment by default with Gesture but this will make them use it while attacking."
            );
            VengenceGoodDrop = Config.Bind(
                "1e - Vengence",
                "Umbras only drop high tier items.",
                true,
                "Umbras only drop Green/Red/Yellow (if one is present)"
            );

            VengencePlayerLevel = Config.Bind(
                "1e - Vengence",
                "Make Umbras scale with Player Level",
                true,
                ""
            );

            VengenceHalfHealth = Config.Bind(
                "1e - Vengence",
                "Make Umbras have half health",
                true,
                "Incase you think they get far too tanky when scaling with ambient level."
            );

            VengenceBlacklist = Config.Bind(
                "1e - Vengence",
                "PreSet Item Blacklist for Umbras",
                true,
                "Make Umbras not spawn with Nkuhanas Opinion.\nThis is to prevent unfair situations or unavoidable autoplay on the Umbras part making them not fun to fight."
            );
            /*
            VengenceHelfire = Config.Bind(
                "1e - Vengence",
                "Umbras cant have Helfire Tincture",
                true,
                "They instantly kill everything including themselves."
            );
            */
            /*
            CommandAffixChoice = Config.Bind(
                "1f - Command",
                "Choice of Elite Aspect",
                true,
                "Allows you to choose which elite Aspect you want when you obtain one."
            );
            */
            EnigmaInterrupt = Config.Bind(
               "1g - Enigma",
               "Enable Interrupting Equipment",
               false,
               "Should Recycler and Tricorn be an option for Enigma"
            );
            EnigmaDestructive = Config.Bind(
               "1g - Enigma",
               "Enable Descructive Equipment",
               true,
               "Should Helfire Tincture and Glowing Meteorite be an option for Enigma"
           );
            EnigmaTonic = Config.Bind(
                "1g - Enigma",
                "Enable Spinel Tonic",
                false,
                 "Should Spinel Tonic be an option for Enigma"
                       );
            EnigmaMovement = Config.Bind(
                           "1g - Enigma",
                           "Enable Movement affecting Equipment",
                           true,
                           "Should Volcanic Egg and Milky Chrysalis be an option for Enigma"
                       );
            EnigmaDirectional = Config.Bind(
                           "1g - Enigma",
                           "Enable Directional Equipment",
                           true,
                           "Should Effigy of Grief be an option for Enigma."
                       );

            /*
                        EnigmaUpgrade = Config.Bind(
                "1g - Enigma",
                "Upgrade Enigma Artifact",
                true,
                "Make Artifact of Enigma work more like how it did in RoR1. You will get a Enigma Box equipment that activates a random equipment and all equipment will be replaced by Equipment Cooldown Reduction"
            );


            EnigmaChatMessage = Config.Bind(
                         "1g - Enigma",
                                       "Enable Enigma Chat Message",
                                       true,
                                       "Your Enigma invokes EquipmentName"
                                   );
            EnigmaCooldown = Config.Bind(
                           "1g - Enigma",
                           "Enigma Equipment Cooldown",
                           60,
                           "Vanilla is 60"
                       );
            */
            EnigmaCooldownReduction = Config.Bind(
                           "1g - Enigma",
                           "Enigma Fragment Equipment Cooldownreduction",
                           8f,
                           ""
                       );

            /*
            SoulImmortal = Config.Bind(
                           "1h - Soul",
                           "Greater Soul Wisps",
                           true,
                           "Should tougher enemies spawn Greater Soul Wisps"
                       );
            */


            FrailtyChanges = Config.Bind(
                                       "1i - Frailty",
                                       "Frailty remove Falldamage Immunity",
                                       true,
                                       "For Loader and many bosses"
                                   );
            SacrificeVoids = Config.Bind(
                "1k - Sacrifice",
                "Void Enemies drop Void Items",
                true,
                "While they have added ways to get void items from Sacrifice, it still feels fitting."
            );




            YellowItemDropsForEnemies = Config.Bind(
                "2a - Config",
                "Yellow Items Drops for Hordes of Many",
                true,
                "Enable/Disable Hordes of Many dropping Yellow items depending on the Enemy\nPrimarily intended with Kin but helps in general"
            );

            /*
            ScavWithYellowItem = Config.Bind(
                "2a - Config",
                "Scav Bosses spawn with Boss Items",
                1,
                "When you encounter Scavengers as the Boss, they will all spawn with this amount of the same boss item and have a chance to drop that yellow item on TP clear.\n0 will turn it off."
            );
            */
            ExtraEvoBlacklist = Config.Bind(
                "2b - Enemy Item Blacklist",
                "More AI Blacklist items",
                true,
                "Prevents Scavs and Enemies from spawning with; Nkuhanas Opinion, Aegis, Happiest Mask, Ghors Tome, Death Mark, Infusion\n Prevents Artifact of Evolution giving; Tesla Coil, Razorwire"
            );
            ScavEquipmentBlacklist = Config.Bind(
                "2b - Enemy Item Blacklist",
                "Scav Equipment Blacklist",
                true,
                "Prevents Scavs from getting Eccentric Vase, Crowdfunder, Recycler because they generally can not be used by them."
            );
            /*
            BullshitScavs = Config.Bind(
                "2b - Enemy Item Blacklist",
                "Bullshit Scavs",
                false,
                "Do you want Scavengers to be able to spawn with Bustling Fungus, Razorwire and Ceremonial Dagger\nRazorwire also applies to Artifact of Evolution\nUnrecommended\n"
            );
            */



            /*
            NoForbiddenBoss = Config.Bind(
                "2z - Misc",
                "Fix? Forbidden as Boss on Scav/Golem",
                true,
                "For some reason they are tagged as forbiddenAsBoss which is weird considering they still spawn as bosses but potentially this made them spawn less often which is a bit weird with Dissonance\nThis may result in Golem Hordes of Many when you rush the first stage on lower difficulties and Scavengers on Sundered Grove Stage 4 as they can spawn before looping\nIf something seems off and they spawn too often feel free to turn it back off."
            );
            */
            /*
            DebugPrintDissonanceSpawns = Config.Bind(
                "9 - Debug",
                "Print Dissonance spawns in console",
                true,
                "Full list of enemies possible on current stage"
            );
            */
        }





        public static void DSArtifactCheckerOnStageAwake(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {

            orig(self);
            if (RunArtifactManager.instance)
            {

                if (SceneInfo.instance.sceneDef.baseSceneName == "bazaar")
                {
                    GameObject Newt = GameObject.Find("/HOLDER: Store/HOLDER: Store Platforms/ShopkeeperPosition/ShopkeeperMaster");

                    if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.eliteOnlyArtifactDef))
                    {
                        EquipmentState equipmentState = new EquipmentState(RoR2Content.Equipment.AffixLunar.equipmentIndex, Run.FixedTimeStamp.negativeInfinity, 0);
                        Newt.GetComponent<Inventory>().SetEquipment(equipmentState, 0);
                        Newt.GetComponent<Inventory>().GiveItem(RoR2Content.Items.BoostDamage, 19990);
                        Newt.GetComponent<Inventory>().GiveItem(RoR2Content.Items.UseAmbientLevel);
                    }

                    if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.swarmsArtifactDef))
                    {
                        Newt.GetComponent<Inventory>().GiveItem(RoR2Content.Items.CutHp, 1);

                        MasterSummon masterSummon = new MasterSummon
                        {
                            masterPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/ShopkeeperMaster"),
                            position = Newt.transform.position,
                            rotation = Newt.transform.rotation,
                            summonerBodyObject = Newt.GetComponent<CharacterMaster>().bodyPrefab,
                            teamIndexOverride = TeamIndex.Neutral,
                            ignoreTeamMemberLimit = true,
                            inventoryToCopy = Newt.GetComponent<Inventory>(),
                        };
                        CharacterMaster characterMaster = masterSummon.Perform();
                    }

                }


            }

         
         
            BombArtifactManager.bombDamageCoefficient = 1.5f * (Run.instance.stageClearCount);
            
            //if (frailtychanged)

        }


        private static void EvolutionMoreItems(On.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.orig_GrantMonsterTeamItem orig)
        {
            orig();
            if (NetworkServer.active)
            {
                if (MoreEvoAfterLoop.Value == false || MoreEvoAfterLoop.Value == true && Run.instance.stageClearCount >= 5)
                {
                    MonsterTeamGainItemRandom = GameObject.Find("MonsterTeamGainsItemsArtifactInventory(Clone)").GetComponent<RoR2.Inventory>();
                    int[] invoutput = new int[ItemCatalog.itemCount];
                    MonsterTeamGainItemRandom.WriteItemStacks(invoutput);

                    if (LoopEvoMultiplierDone == false)
                    {
                        LoopEvoMultiplierDone = true;
                        for (var i = 0; i < invoutput.Length; i++)
                        {
                            if (invoutput[i] > 0)
                            {

                                if (ItemCatalog.GetItemDef((ItemIndex)i).tier == ItemTier.Tier1)
                                {
                                    int WhiteAmount = MoreEvoWhite.Value;
                                    int WhiteGiveCount = ((invoutput[i] * WhiteAmount) - invoutput[i]);
                                    MonsterTeamGainItemRandom.GiveItem((ItemIndex)i, WhiteGiveCount);
                                }
                                else if (ItemCatalog.GetItemDef((ItemIndex)i).tier == ItemTier.Tier2)
                                {
                                    int GreenAmount = MoreEvoGreen.Value;
                                    int GreenGiveCount = ((invoutput[i] * GreenAmount) - invoutput[i]);
                                    MonsterTeamGainItemRandom.GiveItem((ItemIndex)i, GreenGiveCount);
                                }
                                else if (ItemCatalog.GetItemDef((ItemIndex)i).tier == ItemTier.Tier3)
                                {
                                    int RedAmount = MoreEvoRed.Value;
                                    int RedGiveCount = ((invoutput[i] * RedAmount) - invoutput[i]);
                                    MonsterTeamGainItemRandom.GiveItem((ItemIndex)i, RedGiveCount);
                                }
                            }
                        }
                    }

                    MonsterTeamGainItemRandom = GameObject.Find("MonsterTeamGainsItemsArtifactInventory(Clone)").GetComponent<RoR2.Inventory>();
                    MonsterTeamGainItemRandom.WriteItemStacks(invoutput);

                    for (var i = 0; i < invoutput.Length; i++)
                    {

                        if (invoutput[i] > 0)
                        {

                            if (ItemCatalog.GetItemDef((ItemIndex)i).tier == ItemTier.Tier1)
                            {
                                int WhiteAmount = MoreEvoWhite.Value;
                                int WhiteCount = 0;
                                int WhiteDivCount = 0;
                                int WhiteGiveCount = 0;
                                WhiteCount = WhiteCount + invoutput[i];
                                while (WhiteCount >= WhiteAmount) { WhiteCount = WhiteCount - WhiteAmount; WhiteDivCount++; };
                                if (WhiteCount < WhiteAmount) { WhiteGiveCount = (WhiteCount + WhiteDivCount) * WhiteAmount; }
                                MonsterTeamGainItemRandom.RemoveItem((ItemIndex)i, invoutput[i]);
                                MonsterTeamGainItemRandom.GiveItem((ItemIndex)i, WhiteGiveCount);
                            }
                            else if (ItemCatalog.GetItemDef((ItemIndex)i).tier == ItemTier.Tier2)
                            {
                                int GreenAmount = MoreEvoGreen.Value;
                                int GreenCount = 0;
                                int GreenDivCount = 0;
                                int GreenGiveCount = 0;
                                GreenCount = GreenCount + invoutput[i];
                                while (GreenCount >= GreenAmount) { GreenCount = GreenCount - GreenAmount; GreenDivCount++; };
                                if (GreenCount < GreenAmount) { GreenGiveCount = (GreenCount + GreenDivCount) * GreenAmount; }
                                MonsterTeamGainItemRandom.RemoveItem((ItemIndex)i, invoutput[i]);
                                MonsterTeamGainItemRandom.GiveItem((ItemIndex)i, GreenGiveCount);
                            }
                            else if (ItemCatalog.GetItemDef((ItemIndex)i).tier == ItemTier.Tier3)
                            {
                                int RedAmount = MoreEvoRed.Value;
                                int RedCount = 0;
                                int RedDivCount = 0;
                                int RedGiveCount = 0;
                                RedCount = RedCount + invoutput[i];
                                while (RedCount >= RedAmount) { RedCount = RedCount - RedAmount; RedDivCount++; };
                                if (RedCount < RedAmount) { RedGiveCount = (RedCount + RedDivCount) * RedAmount; }
                                MonsterTeamGainItemRandom.RemoveItem((ItemIndex)i, invoutput[i]);
                                MonsterTeamGainItemRandom.GiveItem((ItemIndex)i, RedGiveCount);
                            }
                        }
                    }

                    //Debug.Log($"More Evo items");
                }
            }

        }


        public void LunarAffixHonorInit()
        {
            GivePickupsOnStart.ItemInfo HPBoost = new GivePickupsOnStart.ItemInfo { itemString = ("BoostHp"), count = 10, };
            GivePickupsOnStart.ItemInfo DMGBonus = new GivePickupsOnStart.ItemInfo { itemString = ("BoostDamage"), count = 10, };
            GivePickupsOnStart.ItemInfo[] LunarAffixHPDMG = new GivePickupsOnStart.ItemInfo[0];
            LunarAffixHPDMG = LunarAffixHPDMG.Add(HPBoost, DMGBonus);

            /*
            ScavLunar1LunarAffixGiver.itemInfos = LunarAffixHPDMG;
            ScavLunar2LunarAffixGiver.itemInfos = LunarAffixHPDMG;
            ScavLunar3LunarAffixGiver.itemInfos = LunarAffixHPDMG;
            ScavLunar4LunarAffixGiver.itemInfos = LunarAffixHPDMG;
            BrotherLunarAffixGiver.itemInfos = LunarAffixHPDMG;
            BrotherHurtLunarAffixGiver.itemInfos = LunarAffixHPDMG;
            BrotherHauntLunarAffixGiver.itemInfos = LunarAffixHPDMG;
            BrotherGlassLunarAffixGiver.itemInfos = LunarAffixHPDMG;
            */
            BrotherLunarAffixGiver.overwriteEquipment = false;
            BrotherHurtLunarAffixGiver.overwriteEquipment = false;
            BrotherHauntLunarAffixGiver.overwriteEquipment = false;
            BrotherHauntLunarAffixGiver.overwriteEquipment = false;

            ScavLunar1LunarAffixGiver.equipmentString = "EliteEarthEquipment";
            ScavLunar2LunarAffixGiver.equipmentString = "EliteFireEquipment";
            ScavLunar3LunarAffixGiver.equipmentString = "EliteIceEquipment";
            ScavLunar4LunarAffixGiver.equipmentString = "EliteLightningEquipment";
            BrotherLunarAffixGiver.equipmentString = "EliteLunarEquipment";
            BrotherHurtLunarAffixGiver.equipmentString = "EliteLunarEquipment";
            BrotherHauntLunarAffixGiver.equipmentString = "EliteLunarEquipment";
            BrotherHauntLunarAffixGiver.equipmentString = "EliteLunarEquipment";

            HonorAffixGiverVoidling0.equipmentString = "EliteVoidEquipment";
            HonorAffixGiverVoidling1.equipmentString = "EliteVoidEquipment";
            HonorAffixGiverVoidling2.equipmentString = "EliteVoidEquipment";
            HonorAffixGiverVoidling3.equipmentString = "EliteVoidEquipment";
            //HonorAffixGiverVoidInfestor.equipmentString = "EliteVoidEquipment";


        }

        public void HonorGoldDudeInit()
        {
            GivePickupsOnStart.ItemInfo HPBoost = new GivePickupsOnStart.ItemInfo { itemString = ("BoostHp"), count = 5, };
            GivePickupsOnStart.ItemInfo DMGBonus = new GivePickupsOnStart.ItemInfo { itemString = ("BoostDamage"), count = 15, };
            GivePickupsOnStart.ItemInfo[] LunarAffixHPDMG = new GivePickupsOnStart.ItemInfo[0];
            LunarAffixHPDMG = LunarAffixHPDMG.Add(HPBoost, DMGBonus);

            GoldTitanAllyHonorGiver.itemInfos = LunarAffixHPDMG;
        }



        internal static void AffixesIn()
        {
            RoR2.LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/AffixLunar").isLunar = false;
            List<EquipmentIndex> FullEquipmentList = EquipmentCatalog.equipmentList;
            int[] invoutput = new int[EquipmentCatalog.equipmentCount];

            for (var i = 0; i < invoutput.Length; i++)
            {

                string tempname = EquipmentCatalog.GetEquipmentDef((EquipmentIndex)i).name;


                if (tempname.Contains("Affix") || tempname.Contains("Void"))
                {
                    if (tempname.Contains("Gold") || tempname.Contains("SecretSpeed") || tempname.Contains("Echo") || tempname.Contains("Yellow"))
                    {
                    }
                    else
                    {
                        EquipmentCatalog.GetEquipmentDef((EquipmentIndex)i).canDrop = true;
                        EquipmentCatalog.GetEquipmentDef((EquipmentIndex)i).isBoss = true;
                    }
                }


            }
        }




        internal static void ModSupport()
        {
            Tagchanger();





            if (BombArtifactManager.maxBombCount == 30)
            {
                BombArtifactManager.maxBombCount = 200;
            }
            /*
            if (BombArtifactManager.extraBombPerRadius == 4)
            {
                BombArtifactManager.extraBombPerRadius = 4;
            }
            */
            if (BombArtifactManager.bombSpawnBaseRadius == 3)
            {
                BombArtifactManager.bombSpawnBaseRadius = 7;
            }
            if (BombArtifactManager.bombBlastRadius == 7)
            {
                BombArtifactManager.bombBlastRadius = 8;
            }
            if (BombArtifactManager.maxBombStepUpDistance == 8)
            {
                BombArtifactManager.maxBombStepUpDistance = 16;
            }
            if (BombArtifactManager.maxBombFallDistance == 60)
            {
                BombArtifactManager.maxBombFallDistance = 240;
            }



            if (VengenceEquipment.Value == true)
            {
                EnableEquipmentForVengence();
            }

            SoulGreaterWispMasterIndex = SoulGreaterWispMaster.GetComponent<CharacterMaster>().masterIndex;

            GameObject AffixEarthHealerMaster = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/EliteEarth/AffixEarthHealerMaster.prefab").WaitForCompletion();
            MasterIndexAffixHealingCore = AffixEarthHealerMaster.GetComponent<CharacterMaster>().masterIndex;

            AffixEarthHealerMaster.AddComponent<GivePickupsOnStart>().equipmentString = "none";



            //GameObject WhiteOrb = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex("ItemIndex.Bear")).dropletDisplayPrefab;
            GameObject OrangeOrb = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex("EquipmentIndex.Fruit")).dropletDisplayPrefab;


            Texture2D texItemEnigmaP = new Texture2D(128, 128, TextureFormat.DXT5, false);
            texItemEnigmaP.LoadImage(Properties.Resources.texItemEnigmaP, false);
            texItemEnigmaP.filterMode = FilterMode.Bilinear;
            texItemEnigmaP.wrapMode = TextureWrapMode.Clamp;
            Sprite texItemEnigmaPS = Sprite.Create(texItemEnigmaP, rec128, half);






            //EnigmaFragmentPurple.pickupModelPrefab = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex("ArtifactIndex.Enigma")).displayPrefab;;
            EnigmaFragmentPurple.pickupIconSprite = texItemEnigmaPS;

            //PickupDef boostequipdef = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex("ItemIndex.BoostEquipmentRecharge"));

            PickupDef boostequipdef = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex("ItemIndex.EnigmaFragment_ArtifactHelper"));

            boostequipdef.baseColor = new Color32(140, 114, 219, 255);
            boostequipdef.darkColor = new Color32(140, 114, 219, 255);



            boostequipdef.displayPrefab = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex("ArtifactIndex.Enigma")).displayPrefab;
            boostequipdef.dropletDisplayPrefab = OrangeOrb;
            boostequipdef.iconSprite = texItemEnigmaPS;



            CharacterSpawnCard[] CSCList = FindObjectsOfType(typeof(CharacterSpawnCard)) as CharacterSpawnCard[];
            for (var i = 0; i < CSCList.Length; i++)
            {
                //Debug.LogWarning(CSCList[i]);
                switch (CSCList[i].name)
                {
                    case "cscArchWisp":
                        DirectorCard DC_ArchWisp = new DirectorCard
                        {
                            spawnCard = CSCList[i],
                            selectionWeight = 3,

                            preventOverhead = true,
                            minimumStageCompletions = 0,
                            spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
                        };
                        RoR2Content.mixEnemyMonsterCards.AddCard(1, DC_ArchWisp);  //
                        break;
                    case "cscClayMan":
                        DirectorCard DC_ClayMan = new DirectorCard
                        {
                            spawnCard = CSCList[i],
                            selectionWeight = 3,

                            preventOverhead = false,
                            minimumStageCompletions = 0,
                            spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
                        };
                        RoR2Content.mixEnemyMonsterCards.AddCard(2, DC_ClayMan);  //30
                        tempClayMan = CSCList[i].prefab.GetComponent<CharacterMaster>();
                        break;
                    case "cscAncientWisp":
                        DirectorCard DC_AncientWisp = new DirectorCard
                        {
                            spawnCard = CSCList[i],
                            selectionWeight = 3,

                            preventOverhead = true,
                            minimumStageCompletions = 0,
                            spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
                        };
                        RoR2Content.mixEnemyMonsterCards.AddCard(0, DC_AncientWisp);  //30
                        break;


                }
            }


            BossPickupEdit.ModBossDropChanger();
        }




        public static void EnigmaEquipmentGranter(On.RoR2.GenericPickupController.orig_AttemptGrant orig, global::RoR2.GenericPickupController self, global::RoR2.CharacterBody body)
        {
            bool replace = body.inventory.currentEquipmentIndex != EquipmentIndex.None;
            orig(self, body);
            //Debug.LogWarning(replace);


            if (replace && PickupCatalog.GetPickupDef(self.pickupIndex).equipmentIndex != EquipmentIndex.None)
            {
                var temp = new GenericPickupController.CreatePickupInfo
                {
                    position = self.transform.position,
                    rotation = self.transform.rotation,
                    //pickupIndex = PickupCatalog.FindPickupIndex(RoR2Content.Items.BoostEquipmentRecharge.itemIndex)
                    pickupIndex = PickupCatalog.FindPickupIndex(EnigmaFragmentPurple.itemIndex)
                };

                GenericPickupController newFragmentpickup = RoR2.GenericPickupController.CreatePickup(temp);
                //newFragmentpickup.pickupDisplay.equipmentParticleEffect.SetActive(true);
                newFragmentpickup.pickupDisplay.tier1ParticleEffect.SetActive(true);
                Instantiate(newFragmentpickup.pickupDisplay.tier1ParticleEffect, newFragmentpickup.pickupDisplay.tier1ParticleEffect.transform.parent);


                Destroy(self.gameObject);
                return;
            }


        }






        public static void SoulWispGreater(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, global::RoR2.GlobalEventManager self, global::RoR2.DamageReport damageReport)
        {

            if (!damageReport.victimBody || !damageReport.victimMaster) { orig(self, damageReport); return; }
            if (NetworkServer.active)
            {
                SoulInventoryToCopy = damageReport.victimMaster.inventory;
                //SoulEliteEquipmentIndex = damageReport.victimMaster.inventory.currentEquipmentIndex;
                if (damageReport.victimMaster.masterIndex == SoulGreaterWispMasterIndex || damageReport.victimMaster.masterIndex == MasterIndexAffixHealingCore)
                {
                    SoulGreaterDecider = 2;
                    //On.RoR2.MasterSummon.Perform += SoulSpawnGreaterDenier;
                }
                else if (damageReport.victimBody.baseMaxHealth > 520)
                {
                    //On.RoR2.MasterSummon.Perform += SoulSpawnGreater;
                    SoulGreaterDecider = 1;
                }
            }
            orig(self, damageReport);
        }

        public static RoR2.CharacterMaster SoulSpawnGreaterUniversal(On.RoR2.MasterSummon.orig_Perform orig, global::RoR2.MasterSummon self)
        {
            if (self.masterPrefab == SoulLesserWispMaster)
            {
                if (SoulGreaterDecider == 1)
                {
                    self.masterPrefab = SoulGreaterWispMaster;
                    SoulGreaterDecider = 0;
                }
                else if (SoulGreaterDecider == 2)
                {
                    SoulGreaterDecider = 0;
                    return null;
                }
                self.inventoryToCopy = SoulInventoryToCopy;
                /*
                CharacterMaster temp = orig(self);
                if (temp.inventory)
                {
                    temp.inventory.SetEquipmentIndex(SoulEliteEquipmentIndex);
                }
                return temp;
                */
            }
           


            return orig(self);
        }

        public static RoR2.CharacterMaster SoulSpawnGreater(On.RoR2.MasterSummon.orig_Perform orig, global::RoR2.MasterSummon self)
        {
            if (self.masterPrefab == SoulLesserWispMaster)
            {
                self.masterPrefab = SoulGreaterWispMaster;
                On.RoR2.MasterSummon.Perform -= SoulSpawnGreater;
            }

            return orig(self);
        }

        public static RoR2.CharacterMaster SoulSpawnGreaterDenier(On.RoR2.MasterSummon.orig_Perform orig, global::RoR2.MasterSummon self)
        {
            if (self.masterPrefab == SoulLesserWispMaster)
            {
                On.RoR2.MasterSummon.Perform -= SoulSpawnGreaterDenier;
                return null;
            }

            return orig(self);
        }


        public static void SoulWispCreator()
        {

            Texture2D texRampWispSoulAlt = new Texture2D(256, 16, TextureFormat.DXT5, false);
            texRampWispSoulAlt.LoadImage(Properties.Resources.texRampWispSoulAlt, false);
            texRampWispSoulAlt.filterMode = FilterMode.Point;
            texRampWispSoulAlt.wrapMode = TextureWrapMode.Clamp;



            CharacterModel GreaterSoulCharacterModel = SoulGreaterWispBody.transform.GetChild(0).GetChild(0).GetComponent<CharacterModel>();

            GreaterSoulCharacterModel.baseLightInfos[0].defaultColor = SoulLesserWispBody.transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<Light>().color;
            GreaterSoulCharacterModel.baseRendererInfos[0].defaultMaterial = SoulLesserWispBody.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().material; //Main Mesh
            GreaterSoulCharacterModel.baseRendererInfos[1].defaultMaterial = SoulLesserWispBody.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(3).GetComponent<ParticleSystemRenderer>().material; //Fire
            GreaterSoulCharacterModel.baseRendererInfos[1].defaultMaterial.SetTexture("_RemapTex", texRampWispSoulAlt);
            GreaterSoulCharacterModel.baseRendererInfos[1].defaultMaterial.SetColor("_CutoffScroll", new Color(15, 13, 15, 15));
            GreaterSoulCharacterModel.baseRendererInfos[1].defaultMaterial.SetColor("_TintColor", new Color(1, 1, 1.8f, 0.6f));



            SoulGreaterWispMaster.GetComponent<CharacterMaster>().bodyPrefab = SoulGreaterWispBody;

            CharacterBody SoulWispBody = SoulLesserWispBody.GetComponent<CharacterBody>();
            CharacterBody GreaterSoulWispBody = SoulGreaterWispBody.GetComponent<CharacterBody>();


            SoulGreaterWispBody.GetComponent<DeathRewards>().logUnlockableDef = null;



            LanguageAPI.Add("SOULWISP_BODY_NAME", "Lesser Soul Wisp", "en");
            LanguageAPI.Add("SOULGREATERWISP_BODY_NAME", "Greater Soul Wisp", "en");


            Texture2D TexSoulWisp = new Texture2D(128, 128, TextureFormat.DXT5, false);
            TexSoulWisp.LoadImage(Properties.Resources.texBodyWispSoul, false);
            TexSoulWisp.filterMode = FilterMode.Bilinear;
            TexSoulWisp.wrapMode = TextureWrapMode.Clamp;

            Texture2D TexGreaterSoulWisp = new Texture2D(128, 128, TextureFormat.DXT5, false);
            TexGreaterSoulWisp.LoadImage(Properties.Resources.texBodyGreaterWispSoul, false);
            TexGreaterSoulWisp.filterMode = FilterMode.Bilinear;
            TexGreaterSoulWisp.wrapMode = TextureWrapMode.Clamp;

            SoulWispBody.portraitIcon = TexSoulWisp;
            SoulWispBody.GetComponent<CharacterBody>().baseNameToken = "SOULWISP_BODY_NAME";

            GreaterSoulWispBody.portraitIcon = TexGreaterSoulWisp;
            GreaterSoulWispBody.baseNameToken = "SOULGREATERWISP_BODY_NAME";



            SoulWispBody.baseMaxHealth *= 3f;
            SoulWispBody.levelMaxHealth *= 4.5f;
            SoulWispBody.baseMoveSpeed *= 1.5f;
            SoulWispBody.baseAcceleration *= 1.5f;
            SoulWispBody.baseRegen = 0f;
            SoulWispBody.levelRegen = 0f;
            SoulWispBody.baseDamage *= 0.75f;
            SoulWispBody.levelDamage *= 0.75f;
            SoulWispBody.baseAttackSpeed *= 1.33f;
            SoulWispBody.bodyFlags = CharacterBody.BodyFlags.OverheatImmune | CharacterBody.BodyFlags.ResistantToAOE | CharacterBody.BodyFlags.ImmuneToGoo | CharacterBody.BodyFlags.ImmuneToVoidDeath;
            SoulWispBody.autoCalculateLevelStats = false;

            GreaterSoulWispBody.baseMaxHealth *= 1.5f;
            GreaterSoulWispBody.levelMaxHealth *= 2.75f;
            GreaterSoulWispBody.baseMoveSpeed *= 1.5f;
            GreaterSoulWispBody.baseAcceleration *= 1.5f;
            GreaterSoulWispBody.baseRegen = 0f;
            GreaterSoulWispBody.levelRegen = 0f;
            GreaterSoulWispBody.baseDamage *= 0.75f;
            GreaterSoulWispBody.levelDamage *= 0.75f;
            GreaterSoulWispBody.baseAttackSpeed *= 1.25f;
            GreaterSoulWispBody.bodyFlags = CharacterBody.BodyFlags.OverheatImmune | CharacterBody.BodyFlags.ResistantToAOE | CharacterBody.BodyFlags.ImmuneToGoo | CharacterBody.BodyFlags.ImmuneToVoidDeath;
            GreaterSoulWispBody.autoCalculateLevelStats = false;

            GivePickupsOnStart.ItemInfo AlienHead = new GivePickupsOnStart.ItemInfo { itemString = ("AlienHead"), count = 1, };
            GivePickupsOnStart.ItemInfo DeathMark = new GivePickupsOnStart.ItemInfo { itemString = ("DeathMark"), count = 1, };
            GivePickupsOnStart.ItemInfo StunGrenade = new GivePickupsOnStart.ItemInfo { itemString = ("StunChanceOnHit"), count = 1, };
            GivePickupsOnStart.ItemInfo[] AlienHeadInfos = new GivePickupsOnStart.ItemInfo[0];

            AlienHeadInfos = AlienHeadInfos.Add(AlienHead, DeathMark, StunGrenade);


            SoulLesserWispMaster.AddComponent<GivePickupsOnStart>().itemInfos = AlienHeadInfos;
            SoulGreaterWispMaster.AddComponent<GivePickupsOnStart>().itemInfos = AlienHeadInfos;

            SoulLesserWispMaster.AddComponent<MasterSuicideOnTimer>().lifeTimer = 20f;
            SoulGreaterWispMaster.AddComponent<MasterSuicideOnTimer>().lifeTimer = 40f;

            /*
           if (SoulImmortal.Value == true)
           {

               SoulWispBody.baseMoveSpeed *= 1.75f;
               SoulWispBody.baseAcceleration *= 1.75f;
               SoulWispBody.baseMaxHealth = 40;
               SoulWispBody.levelMaxHealth = 12;
               SoulWispBody.baseRegen = -2.75f;
               SoulWispBody.levelRegen = -0.55f;
               SoulWispBody.baseDamage *= 0.6f;
               SoulWispBody.levelDamage *= 0.7f;
               SoulWispBody.baseAttackSpeed *= 1.25f;

               GreaterSoulWispBody.baseMoveSpeed *= 1.75f;
               GreaterSoulWispBody.baseAcceleration *= 1.75f;
               GreaterSoulWispBody.baseRegen = -27.5f;
               GreaterSoulWispBody.levelRegen = -5.5f;
               GreaterSoulWispBody.baseDamage *= 0.5f;
               GreaterSoulWispBody.levelDamage *= 0.6f;
               GreaterSoulWispBody.baseAttackSpeed *= 1.33f;

               SoulLesserWispBody.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(1).gameObject.SetActive(false);
               SoulGreaterWispBody.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(false);

               GivePickupsOnStart.ItemInfo AlienHead = new GivePickupsOnStart.ItemInfo { itemString = ("AlienHead"), count = 1, };
               GivePickupsOnStart.ItemInfo DeathMark = new GivePickupsOnStart.ItemInfo { itemString = ("DeathMark"), count = 1, };
               GivePickupsOnStart.ItemInfo StunGrenade = new GivePickupsOnStart.ItemInfo { itemString = ("StunChanceOnHit"), count = 1, };
               GivePickupsOnStart.ItemInfo[] AlienHeadInfos = new GivePickupsOnStart.ItemInfo[0];

               AlienHeadInfos = AlienHeadInfos.Add(AlienHead, DeathMark, StunGrenade);


               SoulLesserWispMaster.AddComponent<GivePickupsOnStart>().itemInfos = AlienHeadInfos;
               SoulGreaterWispMaster.AddComponent<GivePickupsOnStart>().itemInfos = AlienHeadInfos;
               

            //SoulLesserWispGiver = SoulLesserWispMaster.GetComponent<GivePickupsOnStart>();
            //SoulGreaterWispGiver = SoulGreaterWispMaster.GetComponent<GivePickupsOnStart>();
        }
            */

        }


        public static int VengenceItemReduction(int amount, int limit)
        {
            return amount - limit - ((amount - limit) / 2);
        }


        public static void StageStartMethodFSD(On.RoR2.Stage.orig_Start orig, Stage self)
        {
            orig(self);


            if (RunArtifactManager.instance)
            {
                if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.eliteOnlyArtifactDef))
                {

                    EliteEquipmentHonorDefs.Clear();
                    CombatDirector.EliteTierDef[] tempelitetierdefs = CombatDirector.eliteTiers;
                    for (int i = 0; i < tempelitetierdefs[1].eliteTypes.Length; i++)
                    {
                        EliteEquipmentHonorDefs.Add(tempelitetierdefs[1].eliteTypes[i].eliteEquipmentDef);
                    }
                    if (Run.instance.stageClearCount > 4)
                    {
                        for (int i = 0; i < tempelitetierdefs[3].eliteTypes.Length; i++)
                        {
                            EliteEquipmentHonorDefs.Add(tempelitetierdefs[3].eliteTypes[i].eliteEquipmentDef);
                        }
                    }

                    if (EliteEquipmentHonorDefs.Count > 0)
                    {
                        int affix1 = random.Next(EliteEquipmentHonorDefs.Count);
                        int affix2 = random.Next(EliteEquipmentHonorDefs.Count);

                        GoldTitanAllyHonorGiver.equipmentString = EliteEquipmentHonorDefs[affix1].name;
                        HonorAffixGiverGoldTitan.equipmentString = EliteEquipmentHonorDefs[affix1].name;
                        HonorAffixGiverSuperRoboBall.equipmentString = EliteEquipmentHonorDefs[affix2].name;
                    }




                }
            }

        }


        public static void EnemyItemChanges()
        {

            /*
On.RoR2.HealthComponent.Heal += (orig, self, amount, procChainMask, nonRegen) =>
{
    if (self.body && self.body.inventory && nonRegen && self.body.inventory.GetItemCount(RoR2Content.Items.InvadingDoppelganger) > 0)
    {
        amount /= 2;
    }
    return orig(self, amount, procChainMask, nonRegen);
};
    */
            /*


            On.RoR2.DotController.AddDot += (orig, self, attackerObject, duration, dotIndex, damageMultiplier) =>
            {
                //Debug.LogWarning(damageMultiplier);


                if (dotIndex == DotController.DotIndex.Helfire && attackerObject.GetComponent<CharacterBody>().teamComponent.teamIndex == TeamIndex.Monster && self.victimObject.GetComponent<CharacterBody>().teamComponent.teamIndex == TeamIndex.Player)
                {
                    orig(self, attackerObject, duration, dotIndex, damageMultiplier / 72);
                    return;
                }
                orig(self, attackerObject, duration, dotIndex, damageMultiplier);

            };


            On.RoR2.DotController.OnDotStackAddedServer += (orig, self, dotstack) =>
            {
                //Debug.LogWarning("OnDotStackAddedServer");


                if (dotstack.GetFieldValue<DotController.DotIndex>("dotIndex") == DotController.DotIndex.Helfire)
                {
                    if (dotstack.GetFieldValue<TeamIndex>("attackerTeam") == TeamIndex.Monster)
                    {
                        dotstack.SetFieldValue<DamageType>("damageType", DamageType.NonLethal | DamageType.Silent);
                    };
                }
                orig(self, dotstack);
            };

            On.RoR2.Orbs.DevilOrb.OnArrival += (orig, self) =>
            {
                if (self.teamIndex == TeamIndex.Monster)
                {
                    self.procCoefficient = 0;

                    if (self.effectType == RoR2.Orbs.DevilOrb.EffectType.Skull && self.attacker)
                    {
                        var tempbod = self.attacker.GetComponent<CharacterBody>();
                        if (tempbod)
                        {
                            float basehpforcalc = tempbod.baseMaxHealth;
                            float level = tempbod.level;
                            int boosthp = tempbod.inventory.GetItemCount(RoR2Content.Items.BoostHp);
                            if (basehpforcalc > 3200) { basehpforcalc = 3200; };
                            if (boosthp > 170) { boosthp = 170; };
                            //basehpforcalc = (basehpforcalc - basehpforcalc*(basehpforcalc/3800/2))*0.25f + boosthp;

                            basehpforcalc = basehpforcalc = (basehpforcalc - basehpforcalc * (basehpforcalc / 3000 / 2)) * 0.25f + boosthp;

                            if (level < 100)
                            {
                                basehpforcalc *= (0.5f + (level + 1) / 200);
                            }


                            //Debug.LogWarning(RoR2.Util.GetBestBodyName(self.attacker) + " : NkuhanaOpinion prev Damage " + self.damageValue + "   NkuhanaOpinion recalculated " + basehpforcalc);

                            self.damageValue = basehpforcalc;
                            //self.damageValue = basehpforcalc = (basehpforcalc - basehpforcalc * (basehpforcalc / 3800 / 2)) * 0.25f + boosthp;


                        }
                    }

                    //self.attacker
                    //Square root of the damage?
                }
                orig(self);
            };

            On.RoR2.Orbs.LightningOrb.OnArrival += (orig, self) =>
            {
                if (self.teamIndex == TeamIndex.Monster)
                {
                    if (self.lightningType == RoR2.Orbs.LightningOrb.LightningType.Tesla && self.attacker)
                    {


                        var tempbod = self.attacker.GetComponent<CharacterBody>();
                        if (tempbod)
                        {

                            if (tempbod.inventory.GetItemCount(RoR2Content.Items.InvadingDoppelganger) == 0)
                            {
                                float damageforcalc = tempbod.damage;
                                //float currentlevel = tempbod.level;
                                float boostdamage = tempbod.inventory.GetItemCount(RoR2Content.Items.BoostDamage) / 10 + 1;
                                if (tempbod.baseDamage > 20)
                                {
                                    //damageforcalc /= (damageforcalc / 20);
                                    damageforcalc = damageforcalc / tempbod.baseDamage * 20;
                                }
                                else if (tempbod.baseDamage > 4)
                                {
                                    self.procCoefficient = 0.6f;
                                    damageforcalc /= 10;
                                }
                                if (boostdamage > 1)
                                {
                                    damageforcalc = (damageforcalc / boostdamage) * (1 + (boostdamage - 1) / 4);
                                }



                                //Debug.LogWarning(RoR2.Util.GetBestBodyName(self.attacker) + " : Tesla prev Damage " + self.damageValue*2 + "   Tesla recalculated " + damageforcalc);

                                self.damageValue = damageforcalc;
                            }
                        }
                    }

                    //self.attacker
                    //Square root of the damage?
                }
                orig(self);
            };
            */
        }


        public static void PickupItemNotification(On.RoR2.UI.GenericNotification.orig_SetItem orig, RoR2.UI.GenericNotification self, global::RoR2.ItemDef itemDef)
        {
            orig(self, itemDef);

            if (itemDef == EnigmaFragmentPurple)
            {
                self.titleTMP.color = RoR2.ColorCatalog.GetColor(ColorCatalog.ColorIndex.Artifact);
            }
        }

        public void Awake()
        {
            InitConfig();
            SoulWispCreator();
            On.RoR2.ClassicStageInfo.RebuildCards += ClassicStageInfo_RebuildCards;
            GameModeCatalog.availability.CallWhenAvailable(ModSupport);
            RoR2.ContentManagement.ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;


            On.RoR2.Run.Start += Run_onRunStartGlobal;
            On.RoR2.SceneDirector.Start += DSArtifactCheckerOnStageAwake;
            On.RoR2.ClassicStageInfo.HandleMixEnemyArtifact += ClassicStageInfo_HandleMixEnemyArtifact;
            On.RoR2.Stage.Start += StageStartMethodFSD;
            RunArtifactManager.onArtifactEnabledGlobal += RunArtifactManager_onArtifactEnabledGlobal;
            RunArtifactManager.onArtifactDisabledGlobal += RunArtifactManager_onArtifactDisabledGlobal;



            //On.RoR2.FreeChestDropTable.GenerateDropPreReplacement += FreeChestDropTable_GenerateDropPreReplacement;

            On.RoR2.Artifacts.CommandArtifactManager.OnDropletHitGroundServer += CommandArtifactManager_OnDropletHitGroundServer;


            On.RoR2.MasterCopySpawnCard.FromMaster += (orig, srcCharacterMaster, copyItems, copyEquipment, onPreSpawnSetup) =>
            {
                MasterCopySpawnCard temp = orig(srcCharacterMaster, copyItems, copyEquipment, onPreSpawnSetup);
                if (srcCharacterMaster && srcCharacterMaster.inventory && srcCharacterMaster.inventory.GetItemCount(RoR2Content.Items.InvadingDoppelganger) > 0)
                {
                    temp.GiveItem(RoR2Content.Items.InvadingDoppelganger);
                }
                return temp;
            };





            On.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.OnServerCardSpawnedGlobal += (orig, spawnResult) =>
            {
                orig(spawnResult);
                CharacterMaster characterMaster = spawnResult.spawnedInstance ? spawnResult.spawnedInstance.GetComponent<CharacterMaster>() : null;
                if (characterMaster && characterMaster.teamIndex == TeamIndex.Void)
                {
                    characterMaster.inventory.AddItemsFrom(MonsterTeamGainsItemsArtifactManager.monsterTeamInventory);
                    foreach (ContagiousItemManager.TransformationInfo transformationInfo in ContagiousItemManager.transformationInfos)
                    {
                        ContagiousItemManager.TryForceReplacement(characterMaster.inventory, transformationInfo.transformedItem);
                    }
                }
            };


            On.RoR2.Artifacts.SacrificeArtifactManager.OnPrePopulateSceneServer += (orig, sceneDirector) =>
            {
                sceneDirector.monsterCredit = (int)(sceneDirector.monsterCredit * 1.5f);
                orig(sceneDirector);
            };
          










            On.EntityStates.BrotherMonster.ExitSkyLeap.OnEnter += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active && self.healthComponent)
                {
                    DamageInfo damageInfo = new DamageInfo();
                    damageInfo.damage = 1;
                    damageInfo.position = self.characterBody.corePosition;
                    damageInfo.force = Vector3.zero;
                    damageInfo.damageColorIndex = DamageColorIndex.Default;
                    damageInfo.crit = false;
                    damageInfo.attacker = null;
                    damageInfo.inflictor = null;
                    damageInfo.damageType = DamageType.NonLethal;
                    damageInfo.procCoefficient = 0f;
                    damageInfo.procChainMask = default(ProcChainMask);
                    self.healthComponent.TakeDamage(damageInfo);
                }

            };




            //EnemyItemChanges();







            ItemTag[] TagsMonsterTeamGain = { ItemTag.AIBlacklist, ItemTag.OnKillEffect, ItemTag.EquipmentRelated, ItemTag.SprintRelated, ItemTag.PriorityScrap, ItemTag.InteractableRelated, ItemTag.HoldoutZoneRelated, ItemTag.Count };

            dtMonsterTeamTier1Item.bannedItemTags = TagsMonsterTeamGain;
            dtMonsterTeamTier2Item.bannedItemTags = TagsMonsterTeamGain;
            dtMonsterTeamTier3Item.bannedItemTags = TagsMonsterTeamGain;
            dtMonsterTeamLunarItem.bannedItemTags = TagsMonsterTeamGain;

            dtMonsterTeamLunarItem.tier1Weight = 0;
            dtMonsterTeamLunarItem.tier2Weight = 0;
            dtMonsterTeamLunarItem.tier3Weight = 0;
            dtMonsterTeamLunarItem.lunarItemWeight = 1;
            dtMonsterTeamLunarItem.canDropBeReplaced = false;
            dtMonsterTeamLunarItem.name = "dtMonsterTeamLunarItem";

            dtSacrificeArtifactVoid.tier1Weight = 0;
            dtSacrificeArtifactVoid.tier2Weight = 0;
            dtSacrificeArtifactVoid.tier3Weight = 0;
            dtSacrificeArtifactVoid.voidTier1Weight = 85;
            dtSacrificeArtifactVoid.voidTier2Weight = 12;
            dtSacrificeArtifactVoid.voidTier3Weight = 3;
            dtSacrificeArtifactVoid.name = "dtSacrificeArtifactVoid";




            PickupDisplayVoidParticleSystem = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/GenericPickup").GetComponent<GenericPickupController>().pickupDisplay.voidParticleEffect;


            //Move to QoL
            On.RoR2.PickupPickerController.SetOptionsFromPickupForCommandArtifact += (orig, self, pickupIndex) =>
            {
                orig(self, pickupIndex);

                if (pickupIndex.itemIndex != ItemIndex.None)
                {
                    ItemDef tempitemdef = ItemCatalog.GetItemDef(pickupIndex.itemIndex);


                    if (tempitemdef.tier == ItemTier.VoidTier1 || tempitemdef.tier == ItemTier.VoidTier2 || tempitemdef.tier == ItemTier.VoidTier3 || tempitemdef.tier == ItemTier.VoidBoss)
                    {
                        self.gameObject.GetComponent<GenericDisplayNameProvider>().displayToken = "Pink Command Essence";
                        GameObject tempobj = Instantiate(PickupDisplayVoidParticleSystem, self.transform.GetChild(0));
                        tempobj.SetActive(true);
                    }
                }
            };



            On.RoR2.PickupPickerController.GetOptionsFromPickupIndex += CommandGiveAffixChoices;



            On.RoR2.UI.GenericNotification.SetItem += PickupItemNotification;


            LanguageAPI.Add("ARTIFACT_COMMAND_CUBE_INTERACTION_PROMPT", "Choose", "en");

            LanguageAPI.Add("ITEM_ENIGMAEQUIPMENTBOOST_NAME", "Enigma Fragment", "en");
            LanguageAPI.Add("ITEM_ENIGMAEQUIPMENTBOOST_PICKUP", "Reduce equipment cooldown.", "en");
            LanguageAPI.Add("ITEM_ENIGMAEQUIPMENTBOOST_DESC", "<style=cIsUtility>Reduce equipment cooldown</style> by <style=cIsUtility>" + EnigmaCooldownReduction.Value + "%</style> <style=cStack>(+" + EnigmaCooldownReduction.Value + "% per stack)</style>.", "en");

            LanguageAPI.Add("EQUIPMENT_ENIGMAEQUIPMENT_NAME", "Enigma Box", "en");
            LanguageAPI.Add("EQUIPMENT_ENIGMAEQUIPMENT_PICKUP", "What could it be?", "en");
            LanguageAPI.Add("EQUIPMENT_ENIGMAEQUIPMENT_DESC", "Activate a random equipment on use.", "en");

            LanguageAPI.Add("EQUIPMENT_ENIGMA_NAME", "Enigma Box", "en");
            LanguageAPI.Add("EQUIPMENT_ENIGMA_PICKUP", "What could it be?", "en");
            LanguageAPI.Add("EQUIPMENT_ENIGMA_DESC", "Activate a random equipment on use.", "en");


            //var EnigmaArtifactDisplay = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex("ArtifactIndex.Enigma")).displayPrefab;

            ArtifactDef EnigmaArtifactDefTemp = RoR2.LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/Enigma");
            ItemDef EnigmaFragment = ScriptableObject.CreateInstance<ItemDef>();

            EnigmaFragment.name = "EnigmaFragment_ArtifactHelper";
            EnigmaFragment.deprecatedTier = ItemTier.NoTier;
            EnigmaFragment.pickupModelPrefab = EnigmaArtifactDefTemp.pickupModelPrefab;
            //EnigmaFragment.pickupIconSprite = ;
            EnigmaFragment.nameToken = "ITEM_ENIGMAEQUIPMENTBOOST_NAME";
            EnigmaFragment.pickupToken = "ITEM_ENIGMAEQUIPMENTBOOST_PICKUP";
            EnigmaFragment.descriptionToken = "ITEM_ENIGMAEQUIPMENTBOOST_DESC";
            EnigmaFragment.loreToken = "";
            EnigmaFragment.hidden = false;
            EnigmaFragment.canRemove = false;
            EnigmaFragment.tags = new ItemTag[]
            {
                ItemTag.WorldUnique
            };
            ItemDisplayRule[] def = new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                }
            };
            CustomItem customItemEnigmaFragment = new CustomItem(EnigmaFragment, new ItemDisplayRule[0]);
            ItemAPI.Add(customItemEnigmaFragment);
            EnigmaFragmentPurple = EnigmaFragment;

            EnigmaFragmentCooldownReduction = 1 - EnigmaCooldownReduction.Value / 100;

            On.RoR2.Inventory.CalculateEquipmentCooldownScale += (orig, self) =>
            {
                int itemCount = self.GetItemCount(EnigmaFragmentPurple);
                if (itemCount > 0)
                {
                    float tempfloat = orig(self);

                    tempfloat *= Mathf.Pow(EnigmaFragmentCooldownReduction, (float)itemCount);

                    return tempfloat;
                }
                return orig(self);
            };


            if (EnigmaInterrupt.Value == false)
            {
                RoR2.LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/BossHunter").enigmaCompatible = false;
                RoR2.LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/BossHunterConsumed").enigmaCompatible = false;
                RoR2.LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/Recycle").enigmaCompatible = false;
            }

            if (EnigmaMovement.Value == false)
            {
                RoR2.LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/FireBallDash").enigmaCompatible = false;
            }
            else
            {
                RoR2.LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/Jetpack").enigmaCompatible = true;
            }
            if (EnigmaTonic.Value == false)
            {
                RoR2.LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/Tonic").enigmaCompatible = false;
            }
            if (EnigmaDirectional.Value == true)
            {
                RoR2.LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/CrippleWard").enigmaCompatible = true;
            }
            if (EnigmaDestructive.Value == false)
            {
                RoR2.LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/Meteor").enigmaCompatible = true;
                RoR2.LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/BurnNearby").enigmaCompatible = true;
            }








            /*
            On.RoR2.ScavengerItemGranter.Start += (orig, self) =>
            {
                orig(self);

                if (SceneInfo.instance && SceneInfo.instance.sceneDef.baseSceneName == "moon2")
                {
                    ItemIndex itemIndex = ItemIndex.None;
                    PickupIndex pickupIndex = PickupIndex.none;
                    do
                    {


                        pickupIndex = Run.instance.availableLunarDropList[random.Next(Run.instance.availableLunarDropList.Count)];

                        PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
                        Debug.LogWarning(pickupDef.nameToken);
                        if (pickupDef.equipmentIndex != EquipmentIndex.None)
                        {
                            itemIndex = ItemIndex.None;
                        }
                        else
                        {
                            itemIndex = pickupDef.itemIndex;
                        }

                    }
                    while (itemIndex == ItemIndex.None || ItemCatalog.GetItemDef(itemIndex).ContainsTag(ItemTag.AIBlacklist));


                    self.gameObject.GetComponent<Inventory>().GiveItem(itemIndex);
                }
            };
            */


            WarbannerSphereEnemy = Instantiate(WarbannerSphereNormal);
            WarbannerSphereEnemy.SetColor("_TintColor", new Color(1f, 0.1f, 0.1f, 1f));
            WarbannerFlagEnemy = Instantiate(WarbannerFlagNormal);
            WarbannerFlagEnemy.color = new Color(0.35f, 0.35f, 0f, 1f);

            On.RoR2.Items.WardOnLevelManager.OnCharacterLevelUp += (orig, characterBody) =>
            {
                if (characterBody.inventory && characterBody.inventory.GetItemCount(RoR2Content.Items.WardOnLevel) > 0)
                {
                    if (characterBody.teamComponent.teamIndex == TeamIndex.Monster)
                    {
                        //Debug.Log("Enemy Warbanner");
                        WarbannerObject.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = WarbannerSphereEnemy;
                        WarbannerObject.transform.GetChild(1).GetChild(1).GetComponent<SkinnedMeshRenderer>().material = WarbannerFlagEnemy;
                        orig(characterBody);
                        WarbannerObject.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = WarbannerSphereNormal;
                        WarbannerObject.transform.GetChild(1).GetChild(1).GetComponent<SkinnedMeshRenderer>().material = WarbannerFlagNormal;
                        return;
                    }
                }

                orig(characterBody);
            };




            if (VengenceGoodDrop.Value == true)
            {
                /*
                On.RoR2.Artifacts.DoppelgangerInvasionManager.PickItemDrop += (orig, inventory, rng) =>
                {
                    if (inventory.GetTotalItemCountOfTier(ItemTier.Tier2) > 0 || inventory.GetTotalItemCountOfTier(ItemTier.Tier3) > 0 || inventory.GetTotalItemCountOfTier(ItemTier.Boss) > 0)
                    {
                        ItemIndex tempIndex = ItemIndex.None;
                        ItemDef tempdef = null;
                        do
                        {
                            int randomindex = random.Next(inventory.itemAcquisitionOrder.Count);
                            tempIndex = inventory.itemAcquisitionOrder[randomindex];
                            tempdef = ItemCatalog.GetItemDef(tempIndex);
                            if (tempdef == RoR2Content.Items.ExtraLifeConsumed)
                            {
                                tempdef = RoR2Content.Items.ExtraLife;
                                tempIndex = RoR2Content.Items.ExtraLife.itemIndex;
                            }
                        }
                        while (tempdef.tier == ItemTier.Tier1 | tempdef.tier == ItemTier.NoTier | tempdef.tier == ItemTier.Lunar);
                        return (tempIndex);

                    }
                    return orig(inventory, rng);
                };
                */


                RoR2.DoppelgangerDropTable dtShadowClone = Addressables.LoadAssetAsync<RoR2.DoppelgangerDropTable>(key: "RoR2/Base/ShadowClone/dtDoppelganger.asset").WaitForCompletion();

                dtShadowClone.canDropBeReplaced = false;
                dtShadowClone.tier1Weight = 0.1f;
                dtShadowClone.tier2Weight = 60;
                dtShadowClone.tier3Weight = 30;
                dtShadowClone.bossWeight = 60;
                dtShadowClone.lunarItemWeight = 0.1f;
                dtShadowClone.voidTier1Weight = 0.1f;
                dtShadowClone.voidTier2Weight = 40;
                dtShadowClone.voidTier3Weight = 15;
                dtShadowClone.voidBossWeight = 20;





                On.RoR2.DoppelgangerDropTable.GenerateWeightedSelection += (orig, self) =>
                {
                    self.selector.Clear();
                    if (self.doppelgangerInventory)
                    {
                        foreach (ItemIndex itemIndex in self.doppelgangerInventory.itemAcquisitionOrder)
                        {
                            ItemIndex foruseItemindex = itemIndex;
                            ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);

                            if (itemDef == RoR2Content.Items.ExtraLifeConsumed)
                            {
                                itemDef = RoR2Content.Items.ExtraLife;
                                foruseItemindex = RoR2Content.Items.ExtraLife.itemIndex;
                            }
                            if (itemDef == DLC1Content.Items.ExtraLifeVoidConsumed)
                            {
                                itemDef = DLC1Content.Items.ExtraLifeVoid;
                                foruseItemindex = DLC1Content.Items.ExtraLifeVoid.itemIndex;
                            }

                            if (self.CanSelectItem(itemDef))
                            {
                                float num = 0f;

                                if (itemDef.DoesNotContainTag(ItemTag.Scrap))
                                {
                                    switch (itemDef.tier)
                                    {
                                        case ItemTier.Tier1:
                                            num = self.tier1Weight;
                                            break;
                                        case ItemTier.Tier2:
                                            num = self.tier2Weight;
                                            break;
                                        case ItemTier.Tier3:
                                            num = self.tier3Weight;
                                            break;
                                        case ItemTier.Lunar:
                                            num = self.lunarItemWeight;
                                            break;
                                        case ItemTier.Boss:
                                            num = self.bossWeight;
                                            break;
                                        case ItemTier.VoidTier1:
                                            num = self.voidTier1Weight;
                                            break;
                                        case ItemTier.VoidTier2:
                                            num = self.voidTier2Weight;
                                            break;
                                        case ItemTier.VoidTier3:
                                            num = self.voidTier3Weight;
                                            break;
                                        case ItemTier.VoidBoss:
                                            num = self.voidBossWeight;
                                            break;
                                    }
                                    if (num > 0f)
                                    {
                                        PickupIndex pickupIndex = PickupCatalog.FindPickupIndex(foruseItemindex);
                                        self.selector.AddChoice(pickupIndex, num);
                                    }
                                }
                            }
                        }
                    }
                };


            }



            if (PerfectedLunarBosses.Value == true)
            {
                LunarAffixHonorInit();
            }
            LunarAffixDisable();
            /*
            if (CommandAffixChoice.Value == true)
            {
                On.RoR2.UI.LogBook.LogBookController.Init += AffixLogbook;
                EquipmentCatalog.availability.CallWhenAvailable(AffixesIn);

            }*/


            GoldTitanAllyHonorGiver.enabled = false;
            if (HonorMinionAlwaysElite.Value == true)
            {
                HonorGoldDudeInit();
            }




        }

        private PickupIndex FreeChestDropTable_GenerateDropPreReplacement(On.RoR2.FreeChestDropTable.orig_GenerateDropPreReplacement orig, FreeChestDropTable self, Xoroshiro128Plus rng)
        {
            int num = RoR2.Util.GetItemCountForTeam(TeamIndex.Player, DLC1Content.Items.FreeChest.itemIndex, false, true);
            self.selector.Clear();
            self.Add(Run.instance.availableTier1DropList, self.tier1Weight);
            self.Add(Run.instance.availableTier2DropList, self.tier2Weight * (float)num);
            self.Add(Run.instance.availableTier3DropList, self.tier3Weight * Mathf.Pow((float)num, 2f));
            return PickupDropTable.GenerateDropFromWeightedSelection(rng, self.selector);
        }

        private void CommandArtifactManager_OnDropletHitGroundServer(On.RoR2.Artifacts.CommandArtifactManager.orig_OnDropletHitGroundServer orig, ref GenericPickupController.CreatePickupInfo createPickupInfo, ref bool shouldSpawn)
        {
            PickupIndex pickupIndex = createPickupInfo.pickupIndex;
            PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
            if (pickupDef != null)
            {
                PickupPickerController.Option[] options = PickupPickerController.GetOptionsFromPickupIndex(pickupIndex);
                Debug.LogWarning(options.Length);
                if (options.Length <= 1)
                {
                    return;
                }
            }

            orig(ref createPickupInfo, ref shouldSpawn);
        }

        private void SacrificeArtifactManager_OnServerCharacterDeath(On.RoR2.Artifacts.SacrificeArtifactManager.orig_OnServerCharacterDeath orig, DamageReport damageReport)
        {
            if (damageReport.victimTeamIndex == TeamIndex.Void || damageReport.victimBody && damageReport.victimBody.bodyFlags.HasFlag(CharacterBody.BodyFlags.Void))
            {
                RoR2.Artifacts.SacrificeArtifactManager.dropTable = dtSacrificeArtifactVoid;
            }
            else
            {
                RoR2.Artifacts.SacrificeArtifactManager.dropTable = dtSacrificeArtifact;
            }
            orig(damageReport);
        }

        private void BaseSplitDeath_OnEnter(On.EntityStates.Gup.BaseSplitDeath.orig_OnEnter orig, EntityStates.Gup.BaseSplitDeath self)
        {
            self.spawnCount *= 2;
            orig(self);
        }

        private void GlobalEventManager_OnCharacterHitGroundServer(On.RoR2.GlobalEventManager.orig_OnCharacterHitGroundServer orig, GlobalEventManager self, CharacterBody characterBody, Vector3 impactVelocity)
        {
            characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            orig(self,characterBody, impactVelocity);
        }

        private void ClassicStageInfo_RebuildCards(On.RoR2.ClassicStageInfo.orig_RebuildCards orig, ClassicStageInfo self)
        {
            orig(self);


            if (self != null)
            {
                if (PerectedForAll.Value == true)
                {
                    WeightedSelection<DirectorCard> CurrentMonsterList = new WeightedSelection<DirectorCard>();
                    CurrentMonsterList = ClassicStageInfo.instance.monsterSelection;

                    if (LunarDone == 1)
                    {
                        Debug.Log("UnLunarify");
                        for (int k = 0; k < LunarifiedList.Count; k++)
                        {
                            WeightedSelection<DirectorCard>.ChoiceInfo Card3 = LunarifiedList.GetChoice(k);
                            if (!(Card3.value.spawnCard.name == "cscLunarWisp" || Card3.value.spawnCard.name == "cscLunarGolem" || Card3.value.spawnCard.name == "cscLunarExploder"))
                            {
                                //Debug.Log(Card3.value.spawnCard);
                                Card3.value.spawnCard.eliteRules = SpawnCard.EliteRules.Default;
                            }
                        }
                        LunarDone = 0;
                    }

                    if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.mixEnemyArtifactDef))
                    {

                        if (SceneInfo.instance.sceneDef.baseSceneName == "moon2" || SceneInfo.instance.sceneDef.baseSceneName == "itmoon")
                        {
                            LunarDone = 1;
                            LunarifiedList = CurrentMonsterList;

                            Debug.Log("Lunarified");
                            for (int j = 0; j < CurrentMonsterList.Count; j++)
                            {
                                WeightedSelection<DirectorCard>.ChoiceInfo Card2 = CurrentMonsterList.GetChoice(j);
                                Card2.value.spawnCard.eliteRules = SpawnCard.EliteRules.Lunar;
                            }
                        }
                    }



                }



            }

        }

        private void ClassicStageInfo_HandleMixEnemyArtifact(On.RoR2.ClassicStageInfo.orig_HandleMixEnemyArtifact orig, DirectorCardCategorySelection monsterCategories, Xoroshiro128Plus rng)
        {
            int DecideTitan = 0;
            int DecideGolem = 0;
            int DecideVermin = 0;
            int DecideFlyingVermin = 0;
            int DecideBeetle = 0;

            DecideTitan = Random.Next(1, 5);
            DecideGolem = Random.Next(1, 5);
            DecideVermin = Random.Next(1, 3);
            DecideFlyingVermin = Random.Next(1, 3);
            DecideBeetle = Random.Next(1, 101);

            switch (DecideTitan)
            {
                case 1:
                    DissoTitan.spawnCard = SkinnedSpawnsTitan1;
                    break;
                case 2:
                    DissoTitan.spawnCard = SkinnedSpawnsTitan2;
                    break;
                case 3:
                    DissoTitan.spawnCard = SkinnedSpawnsTitan3;
                    break;
                case 4:
                    DissoTitan.spawnCard = SkinnedSpawnsTitan4;
                    break;
            }
            switch (DecideGolem)
            {
                case 1:
                    DissoGolem.spawnCard = SkinnedSpawnsGolem1;
                    break;
                case 2:
                    DissoGolem.spawnCard = SkinnedSpawnsGolem2;
                    break;
                case 3:
                    DissoGolem.spawnCard = SkinnedSpawnsGolem3;
                    break;
                case 4:
                    DissoGolem.spawnCard = SkinnedSpawnsGolem4;
                    break;
            }
            switch (DecideVermin)
            {
                case 1:
                    DissoVermin.spawnCard = SkinnedSpawnVermin1;
                    break;
                case 2:
                    DissoVermin.spawnCard = SkinnedSpawnVermin2;
                    break;
            }
            switch (DecideFlyingVermin)
            {
                case 1:
                    DissoVerminFlying.spawnCard = SkinnedSpawnFlyingVermin1;
                    break;
                case 2:
                    DissoVerminFlying.spawnCard = SkinnedSpawnFlyingVermin2;
                    break;
            }
            if (DecideBeetle == 100)
            {
                Debug.LogWarning("Special");
                DissoBeetle.spawnCard = SkinnedSpawnBeetleShiny;
            }
            else
            {
                DissoBeetle.spawnCard = SkinnedSpawnBeetle1;
            }
            orig(monsterCategories, rng);
        }

        private PickupPickerController.Option[] CommandGiveAffixChoices(On.RoR2.PickupPickerController.orig_GetOptionsFromPickupIndex orig, PickupIndex pickupIndex)
        {
            PickupPickerController.Option[] temp = orig(pickupIndex);


            if (pickupIndex.equipmentIndex != EquipmentIndex.None)
            {
                EquipmentDef tempequipdef = EquipmentCatalog.GetEquipmentDef(pickupIndex.equipmentIndex);
                if (tempequipdef.name.StartsWith("Elite") && tempequipdef.dropOnDeathChance > 0)
                {
                    return EliteEquipmentChoicesForCommand;
                }
            }


            return temp;
        }

        private void Inventory_GiveItem_ItemIndex_int(On.RoR2.Inventory.orig_GiveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)
        {


            //Debug.LogWarning(self);
            //Inventory MonsterTeamGainItemRandom = GameObject.Find("MonsterTeamGainsItemsArtifactInventory(Clone)").GetComponent<RoR2.Inventory>();
            if (self == MonsterTeamGainsItemsArtifactManager.monsterTeamInventory)
            {
                InProcessOfMoreEvoItems = true;
                bool GiveMoreEvoItems = false;
                if (MoreEvoAfterLoop.Value == false)
                {
                    GiveMoreEvoItems = true;
                }
                else if (MoreEvoAfterLoop.Value == true && RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.currentItemIterator >= 5)
                {
                    GiveMoreEvoItems = true;
                }
                //Debug.LogWarning(GiveMoreEvoItems);

                //Debug.LogWarning(iterator-1);

                if (ItemCatalog.GetItemDef(itemIndex).tier == ItemTier.Lunar)
                {
                    itemIndex = dtMonsterTeamLunarItem.GenerateDrop(RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.treasureRng).itemIndex;
                    //Debug.LogWarning(ItemCatalog.GetItemDef(itemIndex));
                }
                if (GiveMoreEvoItems == true)
                {
                    int iterator = (RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.currentItemIterator - 1) % RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.dropPattern.Length;
                    //Debug.LogWarning(iterator);
                    switch (iterator)
                    {
                        case 0:
                        case 1:
                            orig(self, itemIndex, MoreEvoWhite.Value);
                            return;
                        case 2:
                        case 3:
                            orig(self, itemIndex, MoreEvoGreen.Value);
                            return;
                        case 4:
                            orig(self, itemIndex, MoreEvoRed.Value);
                            return;
                    }
                };



            }
            else
            {
                InProcessOfMoreEvoItems = false;
                On.RoR2.Inventory.GiveItem_ItemIndex_int -= Inventory_GiveItem_ItemIndex_int;
            }

            orig(self, itemIndex, count);
        }

        private void Start() //Called at the first frame of the game.
        {
            KinBackup.name = "dcccBackupKinHelper";
            //Run.onRunStartGlobal += Run_onRunStartGlobal;

            //On.RoR2.Stage.Start += LunarEliteDisso;




            Mixenemymaker();

            if (KinNoRepeatConfig.Value == true)
            {

                On.RoR2.ClassicStageInfo.HandleSingleMonsterTypeArtifact += (orig2, monsterCategories, rng) =>
                {
                    KinBackup.CopyFrom(monsterCategories);
                    orig2(monsterCategories, rng);

                    int r = 0;
                    SpawnCard spwncard = monsterCategories.categories[0].cards[0].spawnCard;
                    if (spwncard == KinNoRepeat)
                    {
                        //Debug.Log("Repeat Spawn");
                        do
                        {
                            r++;
                            monsterCategories.CopyFrom(KinBackup);
                            orig2(monsterCategories, rng);
                            spwncard = monsterCategories.categories[0].cards[0].spawnCard;
                            //Debug.LogWarning(monsterCategories.categories[0].cards[0].spawnCard);
                        } while (r < 15 && spwncard == KinNoRepeat);
                    }
                    if (spwncard != KinNoRepeat)
                    {
                        Debug.Log(r + " Cycles until non repeat");
                        KinNoRepeat = spwncard;
                    }
                    else if (r == 15)
                    {
                        Debug.Log(r + " Cycles, stop looking for non repeat");
                    }
                    KinBackup.Clear();
                };
            }


            if (MoreEvoItems.Value == true)
            {
                //Debug.Log("More Evolution Items");
                //On.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.GrantMonsterTeamItem += EvolutionMoreItems;


                On.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.GrantMonsterTeamItem += (orig) =>
                {

                    if (InProcessOfMoreEvoItems == false)
                    {
                        On.RoR2.Inventory.GiveItem_ItemIndex_int += Inventory_GiveItem_ItemIndex_int;
                    }
         
                    //Debug.LogWarning(RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.currentItemIterator);
                    //Debug.LogWarning(RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.currentItemIterator % RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.dropPattern.Length);
                    orig();
                };


            }




            if (YellowItemDropsForEnemies.Value == true)
            {
                BossPickupEdit.Start();
                //Logger.LogMessage($"Boss Drops for Hordes of Many enabled.");
            }






            On.RoR2.Artifacts.DoppelgangerSpawnCard.FromMaster += (orig, srcCharacterMaster) =>
            {
                RoR2.Artifacts.DoppelgangerSpawnCard tempspawncard = orig(srcCharacterMaster);

                if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.RandomSurvivorOnRespawn))
                {
                    //tempspawncard.prefab;




                    BodyIndex bodyIndex = BodyIndex.None;
                    SurvivorDef survivorDef = null;
                    do
                    {
                        int randomint = random.Next(SurvivorCatalog.survivorCount);
                        bodyIndex = SurvivorCatalog.GetBodyIndexFromSurvivorIndex((SurvivorIndex)randomint);
                        survivorDef = SurvivorCatalog.GetSurvivorDef((SurvivorIndex)randomint);
                        //Debug.LogWarning(survivorDef);
                    }
                    while (survivorDef.hidden == true);
                    //Debug.LogWarning(SurvivorCatalog.survivorCount);
                    //Debug.LogWarning(survivorDef);


                    tempspawncard.prefab = MasterCatalog.GetMasterPrefab(MasterCatalog.FindAiMasterIndexForBody(bodyIndex));
                }


                return tempspawncard;
            };




            On.RoR2.Artifacts.DoppelgangerInvasionManager.CreateDoppelganger += (orig, srcCharacterMaster, rng) =>
            {
                orig(srcCharacterMaster, rng);

                if (NetworkServer.active)
                {
                    CombatSquad[] bossgrouplist2 = FindObjectsOfType(typeof(CombatSquad)) as CombatSquad[];

                    Inventory MonsterTeamGainItemRandom = new Inventory();

                    if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.MonsterTeamGainsItems))
                    {
                        MonsterTeamGainItemRandom = GameObject.Find("MonsterTeamGainsItemsArtifactInventory(Clone)").GetComponent<RoR2.Inventory>();
                    }

                    for (var i = 0; i < bossgrouplist2.Length; i++)
                    {
                        //Debug.LogWarning(bossgrouplist2[i]);
                        if (bossgrouplist2[i].name.Equals("ShadowCloneEncounter(Clone)"))
                        {
                            //Debug.Log("Shadow Encounter");
                            bossgrouplist2[i].name = "ShadowCloneEncounterAltered";
                            ReadOnlyCollection<CharacterMaster> clonelist = bossgrouplist2[i].readOnlyMembersList;

                            for (var j = 0; j < clonelist.Count; j++)
                            {
                                Debug.Log("Umbra of " + clonelist[j].name);
                                if (VengenceHalfHealth.Value == true)
                                {
                                    CharacterBody tempbody = clonelist[j].GetBody();

                                    DeathRewards tempdeathreward = tempbody.GetComponent<DeathRewards>();
                                    //RoR2.PickupDropTable tempdroptable = Run.instance.GetComponent<RoR2.Artifacts.DoppelgangerInvasionManager>().dropTable;
                                    /*RoR2.PickupDropTable tempdroptable = LegacyResourcesAPI.Load<PickupDropTable>("DropTables/dtDoppelganger");
                                    if (!tempdeathreward)
                                    {
                                        tempbody.gameObject.AddComponent<DeathRewards>().bossDropTable = tempdroptable;
                                    }
                                    else
                                    {
                                        tempdeathreward.bossDropTable = tempdroptable;
                                    }
                                    */

                                    tempbody.autoCalculateLevelStats = false;
                                    if (tempbody.baseMaxHealth >= 200)
                                    {
                                        tempbody.baseMaxHealth = 180;
                                        tempbody.levelMaxHealth = 54;
                                    }
                                    else if (tempbody.baseMaxHealth < 110 || clonelist[j].name.Equals("MercMonsterMaster(Clone)") || clonelist[j].name.Equals("VoidSurvivorMonsterMaster(Clone)"))
                                    {
                                        tempbody.baseMaxHealth = 120;
                                        tempbody.levelMaxHealth = 36;
                                    }
                                    else
                                    {
                                        tempbody.baseMaxHealth = 140;
                                        tempbody.levelMaxHealth = 42;
                                    }
                                    tempbody.OnLevelUp();
                                    clonelist[j].inventory.GiveItem(RoR2Content.Items.CutHp);
                                    clonelist[j].inventory.GiveItem(RoR2Content.Items.LevelBonus, 1);

                                    int bonusamount = ((Run.instance.NetworkstageClearCount + 1) - 5) * 3;

                                    if (bonusamount > 0)
                                    {
                                        clonelist[j].inventory.GiveItem(RoR2Content.Items.BoostHp, bonusamount);
                                    }


                                }
                                /*
                                if (VengenceAmbient.Value == true)
                                {
                                    //RoR2.Run.instance.ambientLevel
                                    int Amount = (int)RoR2.Run.instance.ambientLevel;
                                    if (Amount > 99)
                                    {
                                        int amount2 = Amount - 100;
                                        Amount = 100 + amount2 / 100;
                                    }
                                    clonelist[j].inventory.GiveItem(RoR2Content.Items.LevelBonus, Amount);
                                    //clonelist[j].inventory.GiveItem(RoR2Content.Items.UseAmbientLevel);
                                }
                                */

                                if (VengencePlayerLevel.Value == true)
                                {
                                    if (srcCharacterMaster.hasBody)
                                    {
                                        float bonuslevel = srcCharacterMaster.GetBody().level;
                                        clonelist[j].inventory.GiveItem(RoR2Content.Items.LevelBonus, (int)bonuslevel);
                                    }

                                }







                                if (VengenceGoodDrop.Value == true)
                                {
                                    List<ItemIndex> temporder = new List<ItemIndex>();
                                    temporder.AddRange(srcCharacterMaster.inventory.itemAcquisitionOrder);

                                    if (MonsterTeamGainItemRandom && RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.monsterTeamGainsItemsArtifactDef))
                                    {
                                        temporder.AddRange(MonsterTeamGainItemRandom.itemAcquisitionOrder);
                                    }
                                    temporder.Remove(RoR2Content.Items.ExtraLifeConsumed.itemIndex);
                                    clonelist[j].inventory.RemoveItem(RoR2Content.Items.ExtraLifeConsumed, 10000);
                                    temporder.Remove(DLC1Content.Items.ExtraLifeVoidConsumed.itemIndex);
                                    clonelist[j].inventory.RemoveItem(DLC1Content.Items.ExtraLifeVoidConsumed, 10000);
                                    clonelist[j].inventory.itemAcquisitionOrder = temporder;
                                }


                                if (VengenceBlacklist.Value == true)
                                {


                                    //Defensive items

                                    int ArmorPlate = clonelist[j].inventory.GetItemCount(RoR2Content.Items.ArmorPlate) - MonsterTeamGainItemRandom.GetItemCount(RoR2Content.Items.ArmorPlate);
                                    int Bear = clonelist[j].inventory.GetItemCount(RoR2Content.Items.Bear);// - MonsterTeamGainItemRandom.GetItemCount(RoR2Content.Items.Bear);
                                    int SprintArmor = clonelist[j].inventory.GetItemCount(RoR2Content.Items.SprintArmor);
                                    int ShieldOnly = clonelist[j].inventory.GetItemCount(RoR2Content.Items.ShieldOnly);
                                    int IncreaseHealing = clonelist[j].inventory.GetItemCount(RoR2Content.Items.IncreaseHealing) - MonsterTeamGainItemRandom.GetItemCount(RoR2Content.Items.IncreaseHealing);
                                    int PersonalShield = clonelist[j].inventory.GetItemCount(RoR2Content.Items.PersonalShield) - MonsterTeamGainItemRandom.GetItemCount(RoR2Content.Items.PersonalShield);
                                    int Pearl = clonelist[j].inventory.GetItemCount(RoR2Content.Items.Pearl);
                                    int ShinyPearl = clonelist[j].inventory.GetItemCount(RoR2Content.Items.ShinyPearl);
                                    int Medkit = clonelist[j].inventory.GetItemCount(RoR2Content.Items.Medkit) - MonsterTeamGainItemRandom.GetItemCount(RoR2Content.Items.Medkit);
                                    int Mushroom = clonelist[j].inventory.GetItemCount(RoR2Content.Items.Mushroom);
                                    int ParentEgg = clonelist[j].inventory.GetItemCount(RoR2Content.Items.ParentEgg);
                                    int RepeatHeal = clonelist[j].inventory.GetItemCount(RoR2Content.Items.RepeatHeal);
                                    int BarrierOnOverHeal = clonelist[j].inventory.GetItemCount(RoR2Content.Items.BarrierOnOverHeal) - MonsterTeamGainItemRandom.GetItemCount(RoR2Content.Items.BarrierOnOverHeal);
                                    int NovaOnHeal = clonelist[j].inventory.GetItemCount(RoR2Content.Items.NovaOnHeal);


                                    int Hoof = clonelist[j].inventory.GetItemCount(RoR2Content.Items.Hoof) - MonsterTeamGainItemRandom.GetItemCount(RoR2Content.Items.Hoof);
                                    int SprintBonus = clonelist[j].inventory.GetItemCount(RoR2Content.Items.SprintBonus);
                                    int SprintOutOfCombat = clonelist[j].inventory.GetItemCount(RoR2Content.Items.SprintOutOfCombat) - MonsterTeamGainItemRandom.GetItemCount(RoR2Content.Items.SprintOutOfCombat);

                                    //Debug.LogWarning(clonelist[j].inventory.GetItemCount(RoR2Content.Items.PersonalShield));
                                    //Debug.LogWarning(MonsterTeamGainItemRandom.GetItemCount(RoR2Content.Items.PersonalShield));

                                    //Offensive Stacking Damge items
                                    int FireRing = clonelist[j].inventory.GetItemCount(RoR2Content.Items.FireRing) - MonsterTeamGainItemRandom.GetItemCount(RoR2Content.Items.FireRing);
                                    int IceRing = clonelist[j].inventory.GetItemCount(RoR2Content.Items.IceRing) - MonsterTeamGainItemRandom.GetItemCount(RoR2Content.Items.IceRing);
                                    int Missile = clonelist[j].inventory.GetItemCount(RoR2Content.Items.Missile) - MonsterTeamGainItemRandom.GetItemCount(RoR2Content.Items.Missile);
                                    int FireballsOnHit = clonelist[j].inventory.GetItemCount(RoR2Content.Items.FireballsOnHit);
                                    int LightningStrikeOnHit = clonelist[j].inventory.GetItemCount(RoR2Content.Items.LightningStrikeOnHit);
                                    int LaserTurbine = clonelist[j].inventory.GetItemCount(RoR2Content.Items.LaserTurbine);
                                    int Clover = clonelist[j].inventory.GetItemCount(RoR2Content.Items.Clover) - MonsterTeamGainItemRandom.GetItemCount(RoR2Content.Items.Clover);
                                    int Crowbar = clonelist[j].inventory.GetItemCount(RoR2Content.Items.Crowbar) - MonsterTeamGainItemRandom.GetItemCount(RoR2Content.Items.Crowbar);
                                    int NovaOnLowHealth = clonelist[j].inventory.GetItemCount(RoR2Content.Items.NovaOnLowHealth);
                                    int NearbyDamageBonus = clonelist[j].inventory.GetItemCount(RoR2Content.Items.NearbyDamageBonus) - MonsterTeamGainItemRandom.GetItemCount(RoR2Content.Items.NearbyDamageBonus);
                                    int StickyBomb = clonelist[j].inventory.GetItemCount(RoR2Content.Items.StickyBomb) - MonsterTeamGainItemRandom.GetItemCount(RoR2Content.Items.StickyBomb);
                                    int BleedOnHitAndExplode = clonelist[j].inventory.GetItemCount(RoR2Content.Items.BleedOnHitAndExplode);

                                    //Auto Play Items
                                    int ExplodeOnDeath = clonelist[j].inventory.GetItemCount(RoR2Content.Items.ExplodeOnDeath);
                                    int Dagger = clonelist[j].inventory.GetItemCount(RoR2Content.Items.Dagger);
                                    int SprintWisp = clonelist[j].inventory.GetItemCount(RoR2Content.Items.SprintWisp);
                                    int Thorns = clonelist[j].inventory.GetItemCount(RoR2Content.Items.Thorns);


                                    int ExtraLife = clonelist[j].inventory.GetItemCount(RoR2Content.Items.ExtraLife);
                                    int ExtraLifeVoid = clonelist[j].inventory.GetItemCount(DLC1Content.Items.ExtraLifeVoid);
                                    int HealingPotion = clonelist[j].inventory.GetItemCount(DLC1Content.Items.HealingPotion);
                                    int BearVoid = clonelist[j].inventory.GetItemCount(DLC1Content.Items.BearVoid);
                                    int MushroomVoid = clonelist[j].inventory.GetItemCount(DLC1Content.Items.MushroomVoid);

                                    int BeetleGland = clonelist[j].inventory.GetItemCount(RoR2Content.Items.BeetleGland);
                                    int RoboBallBuddy = clonelist[j].inventory.GetItemCount(RoR2Content.Items.RoboBallBuddy);
                                    int DroneWeapons = clonelist[j].inventory.GetItemCount(DLC1Content.Items.DroneWeapons);
                                    int VoidMegaCrabItem = clonelist[j].inventory.GetItemCount(DLC1Content.Items.VoidMegaCrabItem);

                                    int PrimarySkillShuriken = clonelist[j].inventory.GetItemCount(DLC1Content.Items.PrimarySkillShuriken);
                                    int MissileVoid = clonelist[j].inventory.GetItemCount(DLC1Content.Items.MissileVoid);
                                    int ChainLightningVoid = clonelist[j].inventory.GetItemCount(DLC1Content.Items.ChainLightningVoid);
                                    int ElementalRingVoid = clonelist[j].inventory.GetItemCount(DLC1Content.Items.ElementalRingVoid);

                                    int ExplodeOnDeathVoid = clonelist[j].inventory.GetItemCount(DLC1Content.Items.ExplodeOnDeathVoid);

                                    int LunarSun = clonelist[j].inventory.GetItemCount(DLC1Content.Items.LunarSun);

                                    int bonushp = 0;
                                    int scalingitemlimit = Run.instance.loopClearCount * 2 + 2;
                                    int scalingitemlimitsmall = Run.instance.loopClearCount * 1 + 1;

                                   


                                    if (ExtraLife > 3) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.ExtraLife, ExtraLife - 3); }
                                    if (ExtraLifeVoid > 3) { clonelist[j].inventory.RemoveItem(DLC1Content.Items.ExtraLifeVoid, ExtraLifeVoid - 3); }
                                    if (HealingPotion > 3) { clonelist[j].inventory.RemoveItem(DLC1Content.Items.HealingPotion, HealingPotion - 3); }
                                    if (MushroomVoid > 1) { clonelist[j].inventory.RemoveItem(DLC1Content.Items.MushroomVoid, MushroomVoid - 1); }
                                    if (Mushroom > 1) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.Mushroom, Mushroom - 1); }
                                    if (BearVoid > scalingitemlimitsmall) { clonelist[j].inventory.RemoveItem(DLC1Content.Items.BearVoid, BearVoid - scalingitemlimitsmall); }

                                    if (LunarSun > 2) { clonelist[j].inventory.RemoveItem(DLC1Content.Items.LunarSun, LunarSun / 2); }


                                    if (PrimarySkillShuriken > scalingitemlimitsmall) { clonelist[j].inventory.RemoveItem(DLC1Content.Items.PrimarySkillShuriken, (PrimarySkillShuriken - scalingitemlimitsmall - ((PrimarySkillShuriken - scalingitemlimitsmall) / 2))); }

                                    //Debug.LogWarning(scalingitemlimit);
                                    //Debug.LogWarning(scalingitemlimitbig);

                                    if (ArmorPlate > scalingitemlimit) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.ArmorPlate, (ArmorPlate - scalingitemlimit - ((ArmorPlate - scalingitemlimit) / 2))); }
                                    if (Bear > scalingitemlimitsmall) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.Bear, Bear - scalingitemlimitsmall); }
                                    if (BarrierOnOverHeal > 1) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.BarrierOnOverHeal, BarrierOnOverHeal - 1); }
                                    if (SprintArmor > 1) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.SprintArmor, SprintArmor - 1); }
                                    //if (ShieldOnly > 0) { clonelist[j].inventory.GiveItem(RoR2Content.Items.CutHp); clonelist[j].inventory.GiveItem(RoR2Content.Items.BoostHp); }
                                    if (ShieldOnly > scalingitemlimitsmall) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.ShieldOnly, (ShieldOnly - scalingitemlimitsmall - ((ShieldOnly - scalingitemlimitsmall) / 2))); }
                                    if (IncreaseHealing > scalingitemlimitsmall) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.IncreaseHealing, (IncreaseHealing - scalingitemlimitsmall - ((IncreaseHealing - scalingitemlimitsmall) / 2))); }
                                    if (PersonalShield > scalingitemlimit) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.PersonalShield, (PersonalShield - scalingitemlimit - ((PersonalShield - scalingitemlimit) / 2))); }
                                    if (Pearl > scalingitemlimit) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.Pearl, (Pearl - scalingitemlimit - ((Pearl - scalingitemlimit) / 2))); }
                                    if (ShinyPearl > scalingitemlimitsmall) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.ShinyPearl, (ShinyPearl - scalingitemlimitsmall - ((ShinyPearl - scalingitemlimitsmall) / 2))); }
                                    if (Medkit > scalingitemlimit) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.Medkit, (Medkit - scalingitemlimit - ((Medkit - scalingitemlimit) / 2))); }
                                    if (ParentEgg > scalingitemlimit) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.ParentEgg, (ParentEgg - scalingitemlimit - ((ParentEgg - scalingitemlimit) / 2))); }
                                    if (RepeatHeal > 0) { clonelist[j].inventory.GiveItem(RoR2Content.Items.RepeatHeal); }


                                    if (Hoof > scalingitemlimit) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.Hoof, Hoof / 2); }
                                    if (SprintBonus > scalingitemlimit) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.SprintBonus, SprintBonus / 2); }
                                    
                                    if (NovaOnLowHealth > scalingitemlimitsmall) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.NovaOnLowHealth, NovaOnLowHealth - scalingitemlimitsmall); }
                                    if (FireRing > scalingitemlimitsmall) {
                                        FireRing = VengenceItemReduction(FireRing, scalingitemlimitsmall);
                                        clonelist[j].inventory.RemoveItem(RoR2Content.Items.FireRing, FireRing); bonushp += FireRing;
                                    }
                                    if (IceRing > scalingitemlimitsmall) {
                                        IceRing = VengenceItemReduction(IceRing, scalingitemlimitsmall);
                                        clonelist[j].inventory.RemoveItem(RoR2Content.Items.IceRing, IceRing); bonushp += IceRing;
                                    }
                                    if (ElementalRingVoid > scalingitemlimitsmall)
                                    {
                                        ElementalRingVoid = VengenceItemReduction(ElementalRingVoid, scalingitemlimitsmall);
                                        clonelist[j].inventory.RemoveItem(DLC1Content.Items.ElementalRingVoid, ElementalRingVoid); bonushp += ElementalRingVoid;
                                    }
                                    if (Missile > scalingitemlimitsmall) {
                                        Missile = VengenceItemReduction(Missile, scalingitemlimitsmall); 
                                        clonelist[j].inventory.RemoveItem(RoR2Content.Items.Missile, Missile); bonushp += Missile;
                                    }
                                    if (FireballsOnHit > scalingitemlimitsmall) {
                                        FireballsOnHit = VengenceItemReduction(FireballsOnHit, scalingitemlimitsmall);
                                        clonelist[j].inventory.RemoveItem(RoR2Content.Items.FireballsOnHit, FireballsOnHit); bonushp += FireballsOnHit;
                                    }
                                    if (LightningStrikeOnHit > scalingitemlimitsmall) {
                                        LightningStrikeOnHit = VengenceItemReduction(LightningStrikeOnHit, scalingitemlimitsmall);
                                        clonelist[j].inventory.RemoveItem(RoR2Content.Items.LightningStrikeOnHit, LightningStrikeOnHit); bonushp += LightningStrikeOnHit;
                                    }
                                    if (MissileVoid > scalingitemlimitsmall)
                                    {
                                        MissileVoid = VengenceItemReduction(MissileVoid, scalingitemlimitsmall);
                                        clonelist[j].inventory.RemoveItem(DLC1Content.Items.MissileVoid, MissileVoid); bonushp += MissileVoid;
                                    }
                                    if (ChainLightningVoid > scalingitemlimitsmall)
                                    {
                                        ChainLightningVoid = VengenceItemReduction(ChainLightningVoid, scalingitemlimitsmall);
                                        clonelist[j].inventory.RemoveItem(DLC1Content.Items.ChainLightningVoid, ChainLightningVoid); bonushp += ChainLightningVoid;
                                    }
                                    if (Clover > scalingitemlimitsmall) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.Clover, (Clover - scalingitemlimitsmall - ((Clover - scalingitemlimitsmall) / 2))); }
                                    if (NearbyDamageBonus > scalingitemlimit) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.NearbyDamageBonus, (NearbyDamageBonus - scalingitemlimit - ((NearbyDamageBonus - scalingitemlimit) / 2))); }
                                    if (StickyBomb > scalingitemlimit) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.StickyBomb, (StickyBomb - scalingitemlimit - ((StickyBomb - scalingitemlimit) / 2))); }


                                    if (ExplodeOnDeath > 1) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.ExplodeOnDeath, ExplodeOnDeath - 1); }
                                    if (ExplodeOnDeathVoid > 1) { clonelist[j].inventory.RemoveItem(DLC1Content.Items.ExplodeOnDeathVoid, ExplodeOnDeathVoid - 1); }
                                    if (Dagger > 1) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.Dagger, Dagger - 1); }
                                    if (BleedOnHitAndExplode > 1) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.BleedOnHitAndExplode, BleedOnHitAndExplode - 1); }
                                    if (SprintWisp > 1) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.SprintWisp, SprintWisp - 1); }
                                    if (Thorns > 1) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.Thorns, Thorns - 1); }


                                    clonelist[j].inventory.RemoveItem(RoR2Content.Items.NovaOnHeal, NovaOnHeal); bonushp += NovaOnHeal;
                                    clonelist[j].inventory.RemoveItem(RoR2Content.Items.RoboBallBuddy, RoboBallBuddy);
                                    clonelist[j].inventory.RemoveItem(RoR2Content.Items.BeetleGland, BeetleGland);
                                    clonelist[j].inventory.RemoveItem(DLC1Content.Items.DroneWeapons, DroneWeapons);
                                    clonelist[j].inventory.RemoveItem(DLC1Content.Items.VoidMegaCrabItem, VoidMegaCrabItem);


                                    clonelist[j].inventory.GiveItem(RoR2Content.Items.BoostHp, bonushp/2);



                                    if (clonelist[j].inventory.currentEquipmentIndex == RoR2Content.Equipment.BurnNearby.equipmentIndex)
                                    {
                                        //clonelist[j].inventory.SetEquipmentIndex(EquipmentIndex.None);
                                        clonelist[j].inventory.SetEquipment(new EquipmentState(EquipmentIndex.None, Run.FixedTimeStamp.negativeInfinity, 0), 0);
                                    }
                                }
                            }


                            if (bossgrouplist2[i].readOnlyMembersList.Count > 1)
                            {
                                for (int member = 1; i < bossgrouplist2[i].readOnlyMembersList.Count; i++)
                                {
                                    bossgrouplist2[i].readOnlyMembersList[member].inventory.AddItemsFrom(bossgrouplist2[i].readOnlyMembersList[0].inventory);
                                }
                            }


                        }
                    }




                }
            };



            if (EliteWormRules.Value == "Never")
            {
                CharacterSpawnCard MagmaWormEliteHonor = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscMagmaWorm");
                MagmaWormEliteHonor.noElites = true;
                MagmaWormEliteHonor.eliteRules = SpawnCard.EliteRules.Default;

                CharacterSpawnCard ElectricWormEliteHonor = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscElectricWorm");
                ElectricWormEliteHonor.noElites = true;
                ElectricWormEliteHonor.eliteRules = SpawnCard.EliteRules.Default;
                //Logger.LogMessage($"Worms will not be Elites");
            }
            else if (EliteWormRules.Value == "HonorOnly")
            {
                //Logger.LogMessage($"Worms will be Elites with Artifact of Honor");
            }
            else if (EliteWormRules.Value == "Always")
            {
                CharacterSpawnCard MagmaWormEliteHonor = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscMagmaWorm");
                MagmaWormEliteHonor.noElites = false;
                MagmaWormEliteHonor.eliteRules = SpawnCard.EliteRules.Default;

                CharacterSpawnCard ElectricWormEliteHonor = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscElectricWorm");
                ElectricWormEliteHonor.noElites = false;
                ElectricWormEliteHonor.eliteRules = SpawnCard.EliteRules.Default;
                //Logger.LogMessage($"Worms can be Elites");
            }
            else
            {
                Debug.LogWarning($"Invalid String for Worm Elite Rules - Resorting to Vanilla");
            }






        }


        private int SwarmsDeployableLimitChanger(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
        {
            int count = orig(self, slot);
            switch (slot)
            {
                case DeployableSlot.RoboBallMini:
                case DeployableSlot.GummyClone:
                case DeployableSlot.VendingMachine:
                case DeployableSlot.VoidMegaCrabItem:
                case DeployableSlot.DroneWeaponsDrone:
                case DeployableSlot.MinorConstructOnKill:
                    count *= 2;
                    break;
            }
            return count;
        }



        public void RunArtifactManager_onArtifactEnabledGlobal(RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
        {
            if (artifactDef == RoR2Content.Artifacts.wispOnDeath)
            {
                if (NetworkServer.active)
                {
                    On.RoR2.GlobalEventManager.OnCharacterDeath += SoulWispGreater;
                    On.RoR2.MasterSummon.Perform += SoulSpawnGreaterUniversal;
                }
            }
            else if (artifactDef == RoR2Content.Artifacts.eliteOnlyArtifactDef)
            {
                if (NetworkServer.active)
                {

                    if (EliteWormRules.Value == "HonorOnly")
                    {
                        Debug.Log($"Artifact of Honor - Worms will be Elites");

                        MagmaWormEliteHonor.noElites = false;
                        MagmaWormEliteHonor.eliteRules = SpawnCard.EliteRules.Default;
                        ElectricWormEliteHonor.noElites = false;
                        ElectricWormEliteHonor.eliteRules = SpawnCard.EliteRules.Default;
                    }

                    if (PerfectedLunarBosses.Value == true)
                    {
                        LunarAffixEnable();
                        //LunarAffixDisable();
                    }


                    EliteEquipmentHonorDefs.Clear();
                    CombatDirector.EliteTierDef[] tempelitetierdefs = CombatDirector.eliteTiers;
                    for (int i = 0; i < tempelitetierdefs[1].eliteTypes.Length; i++)
                    {
                        EliteEquipmentHonorDefs.Add(tempelitetierdefs[1].eliteTypes[i].eliteEquipmentDef);
                    }
                    if (Run.instance.stageClearCount > 4)
                    {
                        for (int i = 0; i < tempelitetierdefs[3].eliteTypes.Length; i++)
                        {
                            EliteEquipmentHonorDefs.Add(tempelitetierdefs[3].eliteTypes[i].eliteEquipmentDef);
                        }
                    }

                    if (EliteEquipmentHonorDefs.Count > 0)
                    {
                        int affix1 = random.Next(EliteEquipmentHonorDefs.Count);
                        int affix2 = random.Next(EliteEquipmentHonorDefs.Count);

                        GoldTitanAllyHonorGiver.equipmentString = EliteEquipmentHonorDefs[affix1].name;
                        HonorAffixGiverGoldTitan.equipmentString = EliteEquipmentHonorDefs[affix1].name;
                        HonorAffixGiverSuperRoboBall.equipmentString = EliteEquipmentHonorDefs[affix2].name;


                    }








                    On.RoR2.MinionOwnership.MinionGroup.AddMinion += MinionsInheritHonor;
                    //On.RoR2.MinionOwnership.MinionGroup.AddMinion -= MinionsInheritNoDrones;
                    //On.RoR2.MinionOwnership.MinionGroup.AddMinion -= MinionsInheritWithDrones;


                }
            }
            else if (artifactDef == RoR2Content.Artifacts.enigmaArtifactDef)
            {

                if (NetworkServer.active)
                {
                    BatteryPodPanel.transform.GetChild(0).gameObject.SetActive(false);
                    BatteryPodPanel.transform.GetChild(1).gameObject.SetActive(false);

                    //On.RoR2.GenericPickupController.CreatePickup += EnigmaPickupChanger;
                    On.RoR2.GenericPickupController.AttemptGrant += EnigmaEquipmentGranter;
                }

            }
            else if (artifactDef == RoR2Content.Artifacts.weakAssKneesArtifactDef)
            {
                if (FrailtyChanges.Value == true)
                {
                    On.RoR2.GlobalEventManager.OnCharacterHitGroundServer += GlobalEventManager_OnCharacterHitGroundServer;
                    if (Run.instance)
                    {
                        Run.instance.ruleBook.ApplyChoice(RuleCatalog.FindChoiceDef("Items.FallBoots.Off"));
                    }
                    /*
                    BodyCatalog.FindBodyPrefab("BeetleQueen2Body").GetComponent<CharacterBody>().bodyFlags++;
                    BodyCatalog.FindBodyPrefab("ClayBossBody").GetComponent<CharacterBody>().bodyFlags++;
                    BodyCatalog.FindBodyPrefab("ElectricWormBody").GetComponent<CharacterBody>().bodyFlags++;
                    BodyCatalog.FindBodyPrefab("ImpBossBody").GetComponent<CharacterBody>().bodyFlags++;
                    BodyCatalog.FindBodyPrefab("LoaderBody").GetComponent<CharacterBody>().bodyFlags++;
                    BodyCatalog.FindBodyPrefab("MagmaWormBody").GetComponent<CharacterBody>().bodyFlags++;
                    BodyCatalog.FindBodyPrefab("ShopkeeperBody").GetComponent<CharacterBody>().bodyFlags++;
                    BodyCatalog.FindBodyPrefab("TitanBody").GetComponent<CharacterBody>().bodyFlags++;
                    BodyCatalog.FindBodyPrefab("TitanGoldBody").GetComponent<CharacterBody>().bodyFlags++;
                    BodyCatalog.FindBodyPrefab("VultureBody").GetComponent<CharacterBody>().bodyFlags++;

                    */
                }

            }
            else if (artifactDef == RoR2Content.Artifacts.singleMonsterTypeArtifactDef)
            {
                Inventory MonsterTeamInventory = RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.monsterTeamInventory;
                if (MonsterTeamInventory)
                {
                    EnemyInfoPanelInventoryProvider panel = MonsterTeamInventory.gameObject.GetComponent<EnemyInfoPanelInventoryProvider>();
                    if (panel)
                    {
                        panel.MarkAsDirty();
                    }
                }

                RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscHermitCrab").requiredFlags = 0;
                RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscVulture").requiredFlags = 0;
            }
            else if (artifactDef == RoR2Content.Artifacts.swarmsArtifactDef)
            {
                On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += SwarmsDeployableLimitChanger;
                On.EntityStates.Gup.BaseSplitDeath.OnEnter += BaseSplitDeath_OnEnter;
                foreach (TeamDef teamDef in TeamCatalog.teamDefs)
                {
                    teamDef.softCharacterLimit = (int)(teamDef.softCharacterLimit*1.5f);
                }
            }
            else if (artifactDef == RoR2Content.Artifacts.sacrificeArtifactDef)
            {
                if (SacrificeVoids.Value == true)
                {
                    On.RoR2.Artifacts.SacrificeArtifactManager.OnServerCharacterDeath += SacrificeArtifactManager_OnServerCharacterDeath;
                }
            }
            BombArtifactManager.bombDamageCoefficient = 1.5f + 0.1f * (Run.instance.stageClearCount);
        }



        public void RunArtifactManager_onArtifactDisabledGlobal(RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
        {
            if (artifactDef == RoR2Content.Artifacts.wispOnDeath)
            {

                On.RoR2.GlobalEventManager.OnCharacterDeath -= SoulWispGreater;
                On.RoR2.MasterSummon.Perform -= SoulSpawnGreaterUniversal;


            }
            else if (artifactDef == RoR2Content.Artifacts.eliteOnlyArtifactDef)
            {


                if (EliteWormRules.Value == "HonorOnly")
                {
                    MagmaWormEliteHonor.noElites = true;
                    MagmaWormEliteHonor.eliteRules = SpawnCard.EliteRules.ArtifactOnly;
                    ElectricWormEliteHonor.noElites = true;
                    ElectricWormEliteHonor.eliteRules = SpawnCard.EliteRules.ArtifactOnly;
                }

                if (PerfectedLunarBosses.Value == true)
                {
                    LunarAffixDisable();
                }

                On.RoR2.MinionOwnership.MinionGroup.AddMinion -= MinionsInheritHonor;



            }
            else if (artifactDef == RoR2Content.Artifacts.enigmaArtifactDef)
            {


                BatteryPodPanel.transform.GetChild(0).gameObject.SetActive(true);
                BatteryPodPanel.transform.GetChild(1).gameObject.SetActive(true);

                //On.RoR2.GenericPickupController.CreatePickup -= EnigmaPickupChanger;
                On.RoR2.GenericPickupController.AttemptGrant -= EnigmaEquipmentGranter;


            }
            else if (artifactDef == RoR2Content.Artifacts.weakAssKneesArtifactDef)
            {
                if (FrailtyChanges.Value == true)
                {
                    On.RoR2.GlobalEventManager.OnCharacterHitGroundServer -= GlobalEventManager_OnCharacterHitGroundServer;
                    if (Run.instance)
                    {
                        Run.instance.ruleBook.ApplyChoice(RuleCatalog.FindChoiceDef("Items.FallBoots.On"));
                    }
                }
            }
            else if (artifactDef == RoR2Content.Artifacts.singleMonsterTypeArtifactDef)
            {
                Stage.instance.Network_singleMonsterTypeBodyIndex = -1;
                Inventory MonsterTeamInventory = RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.monsterTeamInventory;
                if (MonsterTeamInventory)
                {
                    EnemyInfoPanelInventoryProvider panel = MonsterTeamInventory.gameObject.GetComponent<EnemyInfoPanelInventoryProvider>();
                    if (panel)
                    {
                        panel.MarkAsDirty();
                    }
                }
                RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscHermitCrab").requiredFlags = RoR2.Navigation.NodeFlags.NoCeiling;
                RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscVulture").requiredFlags = RoR2.Navigation.NodeFlags.NoCeiling;
            }
            else if (artifactDef == RoR2Content.Artifacts.swarmsArtifactDef)
            {
                On.RoR2.CharacterMaster.GetDeployableSameSlotLimit -= SwarmsDeployableLimitChanger;
                On.EntityStates.Gup.BaseSplitDeath.OnEnter -= BaseSplitDeath_OnEnter;
                foreach (TeamDef teamDef in TeamCatalog.teamDefs)
                {
                    teamDef.softCharacterLimit = (int)(teamDef.softCharacterLimit / 1.5f);
                }
            }
            else if (artifactDef == RoR2Content.Artifacts.sacrificeArtifactDef)
            {
                if (SacrificeVoids.Value == true)
                {
                    On.RoR2.Artifacts.SacrificeArtifactManager.OnServerCharacterDeath -= SacrificeArtifactManager_OnServerCharacterDeath;
                }
            }
        }

        public void Run_onRunStartGlobal(On.RoR2.Run.orig_Start orig, Run self)
        {
            orig(self);


            if (Run.instance)
            {
                MonsterTeamGainItemRandom = RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.monsterTeamInventory;
                
                if (FrailtyChanges.Value == true)
                {
                    if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.weakAssKneesArtifactDef))
                    {
                        Run.instance.ruleBook.ApplyChoice(RuleCatalog.FindChoiceDef("Items.FallBoots.Off"));
                    }
                    else
                    {
                        Run.instance.ruleBook.ApplyChoice(RuleCatalog.FindChoiceDef("Items.FallBoots.On"));
                    }
                }
                
            }


            if (MoreEvoAfterLoop.Value == true)
            {
                LoopEvoMultiplierDone = false;
            }

            if (runstartonetimeonly == false)
            {
                runstartonetimeonly = true;
                BossPickupEdit.ol = Language.GetString("ELITE_MODIFIER_LIGHTNING");
                BossPickupEdit.ol = BossPickupEdit.ol.Replace("{0}", "");

                EliteDefsTemp.AddRange(EliteCatalog.eliteList);
                //Debug.Log(EliteDefsTemp.Count + " Elite Types");
                for (var i = 0; i < EquipmentCatalog.equipmentDefs.Length; i++)
                {
                    EquipmentDef tempEliteEquip = EquipmentCatalog.equipmentDefs[i];
                    //Debug.LogWarning(tempEliteEquip);

                    if (tempEliteEquip != null)
                    {
                        if (tempEliteEquip.dropOnDeathChance == 0 || tempEliteEquip.name.Contains("Gold") || tempEliteEquip.name.Contains("Echo") || tempEliteEquip.name.Contains("Yellow"))
                        {

                        }
                        else
                        {
                            //Debug.LogWarning(tempEliteEquip);
                            EliteEquipmentEquipmentIndexCommand.Add(tempEliteEquip.equipmentIndex);
                            PickupIndex equippickupIndex = PickupCatalog.FindPickupIndex(tempEliteEquip.equipmentIndex);
                            PickupPickerController.Option newoption = new PickupPickerController.Option { pickupIndex = equippickupIndex, available = true };

                            EliteEquipmentChoicesForCommand = EliteEquipmentChoicesForCommand.Add(newoption);

                            if (tempEliteEquip.name.Contains("Poison") || tempEliteEquip.name.Contains("Haunted") || tempEliteEquip.name.Contains("Lunar") || tempEliteEquip.name.Contains("ImpPlane") || tempEliteEquip.name.Contains("Blighted") || tempEliteEquip.name.Contains("SecretSpeed"))
                            {

                            }
                            else
                            {
                                EliteEquipmentDefs.Add(tempEliteEquip);
                            }
                        }

                        
                    }



                }
                //Debug.Log(EliteEquipmentDefs.Count + " Tier1 Elite Equipments");
            }



            if (!RunArtifactManager.instance) { return; }

            if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.eliteOnlyArtifactDef))
            {
                if (EliteWormRules.Value == "HonorOnly")
                {

                    Debug.Log($"RunStart: Honor Located - Worms will be Elites");

                    CharacterSpawnCard MagmaWormEliteHonor = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscMagmaWorm");
                    MagmaWormEliteHonor.noElites = false;
                    MagmaWormEliteHonor.eliteRules = SpawnCard.EliteRules.Default;

                    CharacterSpawnCard ElectricWormEliteHonor = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscElectricWorm");
                    ElectricWormEliteHonor.noElites = false;
                    ElectricWormEliteHonor.eliteRules = SpawnCard.EliteRules.Default;
                }

                foreach (var playerController in PlayerCharacterMasterController.instances)
                {
                    int index = random.Next(EliteEquipmentDefs.Count);
                    playerController.master.inventory.GiveEquipmentString(EliteEquipmentDefs[index].name);
                    if (playerController.master.bodyPrefab.name.Equals("ToolbotBody"))
                    {
                        if (playerController.master.loadout.bodyLoadoutManager.GetSkillVariant(BodyCatalog.FindBodyIndex(playerController.master.bodyPrefab), 4) == 0)
                        {
                            Debug.Log("Double Elite for Retool MULT");
                            playerController.master.inventory.SetActiveEquipmentSlot(1);
                            int index2 = random.Next(EliteEquipmentDefs.Count);
                            playerController.master.inventory.GiveEquipmentString(EliteEquipmentDefs[index2].name);
                        }
                    }
                }
            }


        }


        public void Mixenemymaker()
        {



            DirectorCard DSBeetleQueen = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscBeetleQueen"),

                preventOverhead = false,
                selectionWeight = 3,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard DSVagrant = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscVagrant"),

                preventOverhead = false,
                selectionWeight = 3,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard DSImpBoss = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscImpBoss"),

                preventOverhead = false,
                selectionWeight = 3,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard DSRoboBallBoss = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscRoboBallBoss"),

                preventOverhead = false,
                selectionWeight = 3,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard DSGrovetender = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscGravekeeper"),

                preventOverhead = false,
                selectionWeight = 3,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard DSMagmaWorm = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscMagmaWorm"),

                preventOverhead = false,
                selectionWeight = 3,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Far
            };
            DirectorCard DSElectricWorm = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscElectricWorm"),

                preventOverhead = false,
                selectionWeight = 3,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Far
            };


            DirectorCard DSGolem = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscGolem"),

                preventOverhead = false,
                selectionWeight = 3,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard DSGreaterWisp = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscGreaterWisp"),

                preventOverhead = true,
                selectionWeight = 3,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard DSBell = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscBell"),

                preventOverhead = true,
                selectionWeight = 3,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard DSElderLemurian = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscLemurianBruiser"),

                preventOverhead = false,
                selectionWeight = 3,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard DSBison = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscBison"),

                preventOverhead = false,
                selectionWeight = 3,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard DSClayTemp = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscClayBruiser"),

                preventOverhead = false,
                selectionWeight = 3,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard DSVoidReaver = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscNullifier"),

                preventOverhead = false,
                selectionWeight = 3,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard DSParent = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscParent"),

                preventOverhead = false,
                selectionWeight = 3,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };


            DirectorCard DSLemurian = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscLemurian"),

                preventOverhead = false,
                selectionWeight = 3,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard DSWisp = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscLesserWisp"),

                preventOverhead = true,
                selectionWeight = 3,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard DSBeetle = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscBeetle"),

                preventOverhead = false,
                selectionWeight = 3,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard DSJellyfish = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscJellyfish"),

                preventOverhead = true,
                selectionWeight = 3,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard DSImp = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscImp"),

                preventOverhead = false,
                selectionWeight = 3,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard DSVulture = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscVulture"),

                preventOverhead = false,
                selectionWeight = 3,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard DSRoboBallMini = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscRoboBallMini"),

                preventOverhead = true,
                selectionWeight = 1,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard DSMushroom = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscMiniMushroom"),

                preventOverhead = false,
                selectionWeight = 3,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard DSBeetleGuard = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscBeetleGuard"),

                preventOverhead = false,
                selectionWeight = 3,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };

            DirectorCard DSScav = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscScav"),

                preventOverhead = false,
                selectionWeight = 3,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };


            DirectorCard DSHermitCrab = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscHermitCrab"),
                selectionWeight = 1,

                preventOverhead = false,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Far
            };
            DirectorCard DSClayBoss = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscClayBoss"),
                selectionWeight = 3,

                preventOverhead = false,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard DSGrandparent = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/titan/cscGrandparent"),
                selectionWeight = 3,

                preventOverhead = false,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard DSLunarExploder = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscLunarExploder"),
                selectionWeight = 3,

                preventOverhead = false,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard DSLunarGolem = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscLunarGolem"),
                selectionWeight = 3,

                preventOverhead = false,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard DSLunarWisp = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscLunarWisp"),
                selectionWeight = 3,

                preventOverhead = true,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };


            DirectorCard DSVoidInfestor = new DirectorCard
            {
                spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/DLC1/EliteVoid/cscVoidInfestor.asset").WaitForCompletion(),
                selectionWeight = 1,
                preventOverhead = true,
                minimumStageCompletions = 5,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };



            RoR2Content.mixEnemyMonsterCards.categories[0].selectionWeight = 2;
            RoR2Content.mixEnemyMonsterCards.categories[1].selectionWeight = 2;
            RoR2Content.mixEnemyMonsterCards.categories[2].selectionWeight = 3;

            if (ScavsAllStage.Value == true)
            {
                RoR2Content.mixEnemyMonsterCards.categories[3].selectionWeight = 0 - 8f;

            }
            else
            {
                RoR2Content.mixEnemyMonsterCards.categories[3].selectionWeight = 0.10f;
                RoR2Content.mixEnemyMonsterCards.AddCard(0, DSScav); //2000
            }




            DirectorCard SolusProbeTemp = null;
            DirectorCard LunarWispTemp = null;
            //RoR2Content.mixEnemyMonsterCards.AddCard(3, DSVoidInfestor); //60

            for (int i = RoR2Content.mixEnemyMonsterCards.categories.Length - 1; 0 <= i; i--)
            {
                //Debug.LogWarning(i);
                for (int ii = 0; RoR2Content.mixEnemyMonsterCards.categories[i].cards.Length > ii; ii++)
                {
                    //Debug.LogWarning(RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii].spawnCard);
                    //Debug.LogWarning(ii);

                    if (RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii].spawnCard.name.Equals("cscRoboBallMini"))
                    {
                        SolusProbeTemp = RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii];
                        RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii] = DSHermitCrab;
                    }
                    else if (RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii].spawnCard.name.Equals("cscLunarWisp"))
                    {
                        LunarWispTemp = RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii];
                        RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii] = SolusProbeTemp;

                    }
                    else if (RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii].spawnCard.name.Equals("cscTitanBlackBeach"))
                    {
                        DissoTitan = RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii];
                    }
                    else if (RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii].spawnCard.name.Equals("cscGolem"))
                    {
                        DissoGolem = RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii];
                    }
                    else if (RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii].spawnCard.name.Equals("cscVermin"))
                    {
                        DissoVermin = RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii];
                    }
                    else if (RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii].spawnCard.name.Equals("cscFlyingVermin"))
                    {
                        DissoVerminFlying = RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii];
                    }
                    else if (RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii].spawnCard.name.Equals("cscBeetle"))
                    {
                        DissoBeetle = RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii];
                    }
                }
            }
            RoR2Content.mixEnemyMonsterCards.AddCard(0, LunarWispTemp); //550



            //Logger.LogMessage($"Added Cards.");
            cardsadded = true;



        }


        public static void Tagchanger()
        {



            DLC1Content.Items.MoveSpeedOnKill.tags = DLC1Content.Items.MoveSpeedOnKill.tags.Add(ItemTag.OnKillEffect);

            if (ExtraEvoBlacklist.Value == true)
            {

                RoR2Content.Items.MonstersOnShrineUse.tags = RoR2Content.Items.MonstersOnShrineUse.tags.Add(ItemTag.InteractableRelated);
                RoR2Content.Items.GoldOnHit.tags = RoR2Content.Items.GoldOnHit.tags.Add(ItemTag.AIBlacklist);
                RoR2Content.Items.LunarTrinket.tags = RoR2Content.Items.LunarTrinket.tags.Add(ItemTag.AIBlacklist);
                DLC1Content.Items.LunarSun.tags = DLC1Content.Items.LunarSun.tags.Add(ItemTag.AIBlacklist);

                RoR2Content.Items.LunarPrimaryReplacement.tags = RoR2Content.Items.LunarPrimaryReplacement.tags.Remove(ItemTag.AIBlacklist);
                RoR2Content.Items.LunarSecondaryReplacement.tags = RoR2Content.Items.LunarSecondaryReplacement.tags.Remove(ItemTag.AIBlacklist);
                RoR2Content.Items.LunarUtilityReplacement.tags = RoR2Content.Items.LunarUtilityReplacement.tags.Remove(ItemTag.AIBlacklist);
                RoR2Content.Items.LunarSpecialReplacement.tags = RoR2Content.Items.LunarSpecialReplacement.tags.Remove(ItemTag.AIBlacklist);

                RoR2Content.Items.NovaOnHeal.tags = RoR2Content.Items.NovaOnHeal.tags.Add(ItemTag.AIBlacklist);
                RoR2Content.Items.ShockNearby.tags = RoR2Content.Items.ShockNearby.tags.Add(ItemTag.Count);
                DLC1Content.Items.DroneWeapons.tags = DLC1Content.Items.DroneWeapons.tags.Add(ItemTag.AIBlacklist);
                RoR2Content.Items.BarrierOnOverHeal.tags = RoR2Content.Items.BarrierOnOverHeal.tags.Add(ItemTag.Count);
                DLC1Content.Items.CritDamage.tags = DLC1Content.Items.CritDamage.tags.Add(ItemTag.AIBlacklist);
                DLC1Content.Items.RegeneratingScrap.tags = DLC1Content.Items.RegeneratingScrap.tags.Add(ItemTag.AIBlacklist);
            }
        }

        private static void EnableEquipmentForVengence()
        {


            RoR2.CharacterAI.AISkillDriver[] skilllist;

            GameObject CaptainMaster = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/CaptainMonsterMaster");
            skilllist = CaptainMaster.GetComponents<RoR2.CharacterAI.AISkillDriver>();







            /*
            RoR2.CharacterAI.AISkillDriver DiabloStrikePrimary = CaptainMaster.AddComponent<RoR2.CharacterAI.AISkillDriver>();
            RoR2.CharacterAI.AISkillDriver DiabloStrikeUtility = CaptainMaster.AddComponent<RoR2.CharacterAI.AISkillDriver>();
            RoR2.CharacterAI.AISkillDriver ActivateBeaconsSpecial = CaptainMaster.AddComponent<RoR2.CharacterAI.AISkillDriver>();
            RoR2.CharacterAI.AISkillDriver HealingBeaconPrimary = CaptainMaster.AddComponent<RoR2.CharacterAI.AISkillDriver>();
            RoR2.CharacterAI.AISkillDriver HealingBeaconSecondary = CaptainMaster.AddComponent<RoR2.CharacterAI.AISkillDriver>();
            RoR2.CharacterAI.AISkillDriver ShockBeaconPrimary = CaptainMaster.AddComponent<RoR2.CharacterAI.AISkillDriver>();
            RoR2.CharacterAI.AISkillDriver ShockBeaconSecondary = CaptainMaster.AddComponent<RoR2.CharacterAI.AISkillDriver>();
            RoR2.CharacterAI.AISkillDriver ResupplyBeaconPrimary = CaptainMaster.AddComponent<RoR2.CharacterAI.AISkillDriver>();
            RoR2.CharacterAI.AISkillDriver ResupplyBeaconSecondary = CaptainMaster.AddComponent<RoR2.CharacterAI.AISkillDriver>();
            RoR2.CharacterAI.AISkillDriver HackingBeaconPrimary = CaptainMaster.AddComponent<RoR2.CharacterAI.AISkillDriver>();
            RoR2.CharacterAI.AISkillDriver HackingBeaconSecondary = CaptainMaster.AddComponent<RoR2.CharacterAI.AISkillDriver>();

            DiabloStrikePrimary.requiredSkill = RoR2.LegacyResourcesAPI.Load<RoR2.Skills.SkillDef>("skilldefs/captainbody/CallAirstrikeAlt");
            DiabloStrikeUtility.requiredSkill = RoR2.LegacyResourcesAPI.Load<RoR2.Skills.SkillDef>("skilldefs/captainbody/PrepAirstrikeAlt");
            ActivateBeaconsSpecial.requiredSkill = RoR2.LegacyResourcesAPI.Load<RoR2.Skills.SkillDef>("skilldefs/captainbody/PrepSupplyDrop");
            HealingBeaconPrimary.requiredSkill = RoR2.LegacyResourcesAPI.Load<RoR2.Skills.SkillDef>("skilldefs/captainbody/CallSupplyDropHealing");
            HealingBeaconSecondary.requiredSkill = HealingBeaconPrimary.requiredSkill;
            ShockBeaconPrimary.requiredSkill = RoR2.LegacyResourcesAPI.Load<RoR2.Skills.SkillDef>("skilldefs/captainbody/CallSupplyDropShocking");
            ShockBeaconSecondary.requiredSkill = ShockBeaconPrimary.requiredSkill;
            ResupplyBeaconPrimary.requiredSkill = RoR2.LegacyResourcesAPI.Load<RoR2.Skills.SkillDef>("skilldefs/captainbody/CallSupplyDropEquipmentRestock");
            ResupplyBeaconSecondary.requiredSkill = ResupplyBeaconPrimary.requiredSkill;
            HackingBeaconPrimary.requiredSkill = RoR2.LegacyResourcesAPI.Load<RoR2.Skills.SkillDef>("skilldefs/captainbody/CallSupplyDropHacking");
            HackingBeaconSecondary.requiredSkill = HackingBeaconPrimary.requiredSkill;

            DiabloStrikePrimary.skillSlot = SkillSlot.Primary;
            DiabloStrikeUtility.skillSlot = SkillSlot.Utility;
            ActivateBeaconsSpecial.skillSlot = SkillSlot.Special;
            HealingBeaconPrimary.skillSlot = SkillSlot.Primary;
            HealingBeaconSecondary.skillSlot = SkillSlot.Secondary;
            ShockBeaconPrimary.skillSlot = SkillSlot.Primary;
            ShockBeaconSecondary.skillSlot = SkillSlot.Secondary;
            ResupplyBeaconPrimary.skillSlot = SkillSlot.Primary;
            ResupplyBeaconSecondary.skillSlot = SkillSlot.Secondary;
            HackingBeaconPrimary.skillSlot = SkillSlot.Primary;
            HackingBeaconSecondary.skillSlot = SkillSlot.Secondary;

            DiabloStrikePrimary.customName = "PrepDiablo";
            DiabloStrikeUtility.customName = "MarkDiablo";
            ActivateBeaconsSpecial.customName = "PrepSupplyDrop";
            HealingBeaconPrimary.customName = "BeaconHealingPrimary";
            HealingBeaconSecondary.customName = "BeaconHealingSecondary";
            ShockBeaconPrimary.customName = "BeaconShockPrimary";
            ShockBeaconSecondary.customName = "BeaconShockSecondary";
            ResupplyBeaconPrimary.customName = "BeaconResupplyPrimary";
            ResupplyBeaconSecondary.customName = "BeaconResupplySecondary";
            HackingBeaconPrimary.customName = "BeaconHackingPrimary";
            HackingBeaconSecondary.customName = "BeaconHackingSecondary";

            //ActivateBeaconsSpecial.moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.NearestFriendlyInSkillRange;
            HealingBeaconPrimary.moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.NearestFriendlyInSkillRange;
            HealingBeaconSecondary.moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.NearestFriendlyInSkillRange;
            ResupplyBeaconPrimary.moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.NearestFriendlyInSkillRange;
            ResupplyBeaconSecondary.moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.NearestFriendlyInSkillRange;


            HealingBeaconPrimary.buttonPressType = RoR2.CharacterAI.AISkillDriver.ButtonPressType.TapContinuous;
            HealingBeaconSecondary.buttonPressType = RoR2.CharacterAI.AISkillDriver.ButtonPressType.TapContinuous;
            ShockBeaconPrimary.buttonPressType = RoR2.CharacterAI.AISkillDriver.ButtonPressType.TapContinuous;
            ShockBeaconSecondary.buttonPressType = RoR2.CharacterAI.AISkillDriver.ButtonPressType.TapContinuous;
            ResupplyBeaconPrimary.buttonPressType = RoR2.CharacterAI.AISkillDriver.ButtonPressType.TapContinuous;
            ResupplyBeaconSecondary.buttonPressType = RoR2.CharacterAI.AISkillDriver.ButtonPressType.TapContinuous;
            HackingBeaconPrimary.buttonPressType = RoR2.CharacterAI.AISkillDriver.ButtonPressType.TapContinuous;
            HackingBeaconSecondary.buttonPressType = RoR2.CharacterAI.AISkillDriver.ButtonPressType.TapContinuous;

            DiabloStrikePrimary.aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy;
            HealingBeaconPrimary.aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy;
            HealingBeaconSecondary.aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy;
            ShockBeaconPrimary.aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy;
            ShockBeaconSecondary.aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy;
            ResupplyBeaconPrimary.aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy;
            ResupplyBeaconSecondary.aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy;
            HackingBeaconPrimary.aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy;
            HackingBeaconSecondary.aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy;
            */
            /*
            DiabloStrikePrimary.activationRequiresTargetLoS  = true;
            HealingBeaconPrimary.activationRequiresTargetLoS = true;
            HealingBeaconSecondary.activationRequiresTargetLoS = true;
            ShockBeaconPrimary.activationRequiresTargetLoS = true;
            ShockBeaconSecondary.activationRequiresTargetLoS = true;
            ResupplyBeaconPrimary.activationRequiresTargetLoS = true;
            ResupplyBeaconSecondary.activationRequiresTargetLoS = true;
            HackingBeaconPrimary.activationRequiresTargetLoS = true;
            HackingBeaconSecondary.activationRequiresTargetLoS = true;
            */
            /*
            DiabloStrikePrimary.driverUpdateTimerOverride = 0.2f;
            HealingBeaconPrimary.driverUpdateTimerOverride = 0.2f;
            HealingBeaconSecondary.driverUpdateTimerOverride = 0.2f;
            ShockBeaconPrimary.driverUpdateTimerOverride = 0.2f;
            ShockBeaconSecondary.driverUpdateTimerOverride = 0.2f;
            ResupplyBeaconPrimary.driverUpdateTimerOverride = 0.2f;
            ResupplyBeaconSecondary.driverUpdateTimerOverride = 0.2f;
            HackingBeaconPrimary.driverUpdateTimerOverride = 0.2f;
            HackingBeaconSecondary.driverUpdateTimerOverride = 0.2f;
            */
            //DiabloStrikeUtility.requireSkillReady = true;
            /*
            ActivateBeaconsSpecial.requireSkillReady = true;
            DiabloStrikePrimary.requireSkillReady = true;
            HealingBeaconPrimary.requireSkillReady = true;
            HealingBeaconSecondary.requireSkillReady = true;
            ShockBeaconPrimary.requireSkillReady = true;
            ShockBeaconSecondary.requireSkillReady = true;
            ResupplyBeaconPrimary.requireSkillReady = true;
            ResupplyBeaconSecondary.requireSkillReady = true;
            HackingBeaconPrimary.requireSkillReady = true;
            HackingBeaconSecondary.requireSkillReady = true;
            */


            for (var i = 0; i < skilllist.Length; i++)
            {


                if (skilllist[i].skillSlot == SkillSlot.Utility || skilllist[i].customName.Equals("FireLongRange") || skilllist[i].customName.Equals("ShortStrafe") || skilllist[i].customName.Equals("BackUpIfClose"))
                {
                    Destroy(skilllist[i]);
                }
                else if (skilllist[i].customName.Contains("ChaseEnemy"))
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].minDistance = 100;
                }
                else if (skilllist[i].customName.Equals("MarkOrbitalStrike") || skilllist[i].customName.Equals("StartOrbitalStrike"))
                {
                    skilllist[i].aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy;
                    skilllist[i].nextHighPriorityOverride = null;
                }
                else if (skilllist[i].customName.Contains("FireShotgun"))
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].shouldFireEquipment = true;
                    skilllist[i].maxDistance = 80;
                    skilllist[i].nextHighPriorityOverride = null;
                }
                else
                {
                    skilllist[i].shouldSprint = true;
                    skilllist[i].movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget;
                }








                /*
                if (skilllist[i].customName.Equals("MarkOrbitalStrike"))
                {
                    skilllist[i].shouldFireEquipment = true;
                    skilllist[i].aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy;
                    //skilllist[i].buttonPressType = RoR2.CharacterAI.AISkillDriver.ButtonPressType.TapContinuous;
                }
                else if (skilllist[i].customName.Contains("FireTazer") || skilllist[i].customName.Contains("FireShotgun") || skilllist[i].customName.Contains("BackUpIfClose"))
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].shouldFireEquipment = true;
                }
                */
            }









            skilllist = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/CommandoMonsterMaster").GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {

                if (skilllist[i].skillSlot != SkillSlot.Primary && skilllist[i].skillSlot != SkillSlot.Special)
                {
                    skilllist[i].shouldSprint = true;
                }
                if (skilllist[i].skillSlot != SkillSlot.Utility && skilllist[i].skillSlot != SkillSlot.None)
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].shouldFireEquipment = true;
                }
            }
            skilllist = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/CrocoMonsterMaster").GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {
                if (skilllist[i].skillSlot != SkillSlot.Primary)
                {
                    skilllist[i].shouldSprint = true;
                }
                if (skilllist[i].customName.Contains("StrafeAndSlashTarget") && skilllist[i].customName.Contains("LeapToEnemy") || skilllist[i].customName.Contains("ChaseAndSlashTarget") || skilllist[i].customName.Contains("FireContagion") || skilllist[i].customName.Contains("FireSecondary"))
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].shouldFireEquipment = true;
                }
            }

            skilllist = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/ToolbotMonsterMaster").GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {

                if (skilllist[i].skillSlot != SkillSlot.Primary && skilllist[i].skillSlot != SkillSlot.Special)
                {
                    skilllist[i].shouldSprint = true;
                }
                if (!skilllist[i].customName.Contains("SwitchOffNailgun") && !skilllist[i].customName.Contains("ChaseEnemy"))
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].shouldFireEquipment = true;
                }
            }

            skilllist = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/LoaderMonsterMaster").GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {

                skilllist[i].shouldSprint = true;
                if (!skilllist[i].customName.Contains("ChaseEnemy"))
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].shouldFireEquipment = true;
                }
            }

            skilllist = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/Bandit2MonsterMaster").GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {
                if (skilllist[i].skillSlot != SkillSlot.Primary && skilllist[i].skillSlot != SkillSlot.Special)
                {
                    skilllist[i].shouldSprint = true;
                }
                if (skilllist[i].skillSlot != SkillSlot.None)
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].shouldFireEquipment = true;
                }
            }

            skilllist = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/TreebotMonsterMaster").GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {
                if (skilllist[i].skillSlot != SkillSlot.Primary)
                {
                    skilllist[i].shouldSprint = true;
                }
                if (!skilllist[i].customName.Contains("FireMortar") && !skilllist[i].customName.Contains("ChaseEnemy"))
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].shouldFireEquipment = true;
                }
            }

            skilllist = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/EngiMonsterMaster").GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {
                if (!skilllist[i].customName.Contains("PaintHarpoons"))
                {
                    skilllist[i].shouldSprint = true;
                }


                if (!skilllist[i].customName.Contains("ReturnToTurrets") && !skilllist[i].customName.Contains("ThrowShield") && !skilllist[i].customName.Contains("CancelPaintingIfNoLoS") && !skilllist[i].customName.Contains("ChaseEnemy"))
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].shouldFireEquipment = true;
                }
            }

            skilllist = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/MageMonsterMaster").GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {
                if (skilllist[i].skillSlot != SkillSlot.Special)
                {
                    skilllist[i].shouldSprint = true;
                }
                if (!skilllist[i].customName.Contains("EscapeWithSuperJump") && !skilllist[i].customName.Contains("ChaseEnemy"))
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].shouldFireEquipment = true;
                }
            }

            skilllist = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/HereticMonsterMaster").GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {
                if (skilllist[i].skillSlot != SkillSlot.Primary)
                {
                    skilllist[i].shouldSprint = true;
                }
                if (skilllist[i].skillSlot != SkillSlot.Utility && skilllist[i].skillSlot != SkillSlot.None)
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].shouldFireEquipment = true;
                }
            }

            skilllist = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/HuntressMonsterMaster").GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {

                skilllist[i].shouldSprint = true;

                if (!skilllist[i].customName.Contains("BlinkAwayFromEnemy") && !skilllist[i].customName.Contains("ChaseEnemy"))
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].shouldFireEquipment = true;
                }
            }

            skilllist = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/MercMonsterMaster").GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {
                if (skilllist[i].movementType == RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget)
                {
                    skilllist[i].shouldSprint = true;
                }
                if (!skilllist[i].customName.Contains("Chase"))
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].shouldFireEquipment = true;
                }
            }


            skilllist = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/Railgunner/RailgunnerMonsterMaster.prefab").WaitForCompletion().GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {
                if (skilllist[i].movementType == RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget)
                {
                    skilllist[i].shouldSprint = true;

                }
                if (skilllist[i].skillSlot != SkillSlot.None && skilllist[i].skillSlot != SkillSlot.Primary && skilllist[i].skillSlot != SkillSlot.Secondary && skilllist[i].movementType != RoR2.CharacterAI.AISkillDriver.MovementType.FleeMoveTarget)
                {
                    skilllist[i].shouldFireEquipment = true;
                }
            }

            skilllist = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/VoidSurvivor/VoidSurvivorMonsterMaster.prefab").WaitForCompletion().GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {

                if (skilllist[i].skillSlot == SkillSlot.Primary)
                {
                    skilllist[i].buttonPressType = RoR2.CharacterAI.AISkillDriver.ButtonPressType.Hold;
                }
                else if (skilllist[i].movementType == RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget)
                    {
                        skilllist[i].shouldSprint = true;
                    }
           

                if (skilllist[i].skillSlot != SkillSlot.None && skilllist[i].movementType != RoR2.CharacterAI.AISkillDriver.MovementType.FleeMoveTarget)
                {
                    skilllist[i].shouldFireEquipment = true;
                }
            }

        }

















        public void MinionsInheritHonor(On.RoR2.MinionOwnership.MinionGroup.orig_AddMinion orig, NetworkInstanceId ownerId, global::RoR2.MinionOwnership minion)
        {

            orig(ownerId, minion);

            if (NetworkServer.active)
            {
                Inventory inventory = minion.gameObject.GetComponent<Inventory>();
                if (inventory && inventory.currentEquipmentIndex == EquipmentIndex.None)
                {
                    int index = random.Next(EliteEquipmentHonorDefs.Count);

                    inventory.SetEquipment(new EquipmentState(EliteEquipmentHonorDefs[index].equipmentIndex, Run.FixedTimeStamp.negativeInfinity, 0), 0);
                    //inventory.SetEquipmentIndex(EliteEquipmentHonorDefs[index].equipmentIndex);
                    inventory.GiveItem(RoR2Content.Items.BoostDamage, 5);
                    inventory.GiveItem(RoR2Content.Items.BoostHp, 15);
                }

            }
        }





        private void ContentManager_collectContentPackProviders(RoR2.ContentManagement.ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(new Content());
        }



    }
}

