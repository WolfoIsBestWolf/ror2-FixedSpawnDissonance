using BepInEx;
using R2API.Utils;
using RoR2;
using RoR2.Artifacts;
using System.Linq;
using UnityEngine;

namespace VanillaArtifactsPlus

{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("Wolfo.VanillaArtifactsPlus", "VanillaArtifactsPlus", "3.5.1")]
    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync, VersionStrictness.DifferentModVersionsAreOk)]


    public class VanillaArtifactsMain : BaseUnityPlugin
    {
        public static readonly System.Random Random = new System.Random();
        public static bool EnableNewContent = false;

        public void Awake()
        {
            Assets.Init(Info);
            WConfig.InitConfig();

            GameModeCatalog.availability.CallWhenAvailable(CallLate_ModSupport);
            //RoR2.ContentManagement.ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;

            On.RoR2.SceneDirector.Start += DSArtifactCheckerOnStageAwake;

            //On.RoR2.Stage.Start += StageStartMethodFSD;
            RunArtifactManager.onArtifactEnabledGlobal += RunArtifactManager_onArtifactEnabledGlobal;
            RunArtifactManager.onArtifactDisabledGlobal += RunArtifactManager_onArtifactDisabledGlobal;

            if (WConfig.DevotionChanges.Value == true)
            {
                Devotion.Start();
            }
            if (WConfig.CommandChanges.Value == true)
            {
                Command.Start();
            }
            if (WConfig.EnigmaChanges.Value == true)
            {
                Enigma.Start();
            }
            if (WConfig.EvolutionChanges.Value == true)
            {
                Evolution.Start();
            }
            if (WConfig.HonorChanges.Value == true)
            {
                Honor.Start();
            }
            if (WConfig.KinChanges.Value == true)
            {
                Kin.Start();
            }
            if (WConfig.SacrificeChanges.Value == true)
            {
                Sacrifice.Start();
            }
            if (WConfig.SoulChanges.Value == true)
            {
                Soul.Start();
            }
            if (WConfig.VenganceChanges.Value == true)
            {
                Vengence.Start();
            }
            Rebirth.Start();

        }

        private void Start() //Called at the first frame of the game.
        {

            EnableNewContent = WConfig.cfgContent.Value == WConfig.Content.Enabled;
            if (WConfig.cfgContent.Value == WConfig.Content.AutoDetect)
            {
                EnableNewContent = NetworkModCompatibilityHelper._networkModList.Length != 0;
            }
            Debug.Log("VanillaArtifactTweaks | Content? " + EnableNewContent);
            if (EnableNewContent)
            {
                string mod = Info.Metadata.GUID + ";" + Info.Metadata.Version;
                NetworkModCompatibilityHelper.networkModList = NetworkModCompatibilityHelper.networkModList.Append(mod);

                Soul.MakeNewSoulWisp();
                Enigma.MakeEnigmaFragment();
            }
            //Needs to be called late for reasons
            if (WConfig.DissonanceChanges.Value == true)
            {
                Dissonance.Start();
            }
            if (WConfig.KinYellowsForEnemies.Value == true)
            {
                KinBossDropsForEnemies.Start();
            }
            WConfig.cfgEliteWorms_Changed(null, null);
        }


        internal static void CallLate_ModSupport()
        {

            Soul.CallLate();

            if (WConfig.EnigmaChanges.Value == true)
            {
                Enigma.CallLate();
            }
            if (WConfig.KinChanges.Value == true && WConfig.KinYellowsForEnemies.Value == true)
            {
                KinBossDropsForEnemies.ModBossDropChanger();
            }
            if (WConfig.SpiteChanges.Value == true)
            {
                Spite.SpiteChangesCalledLate();
            }
            if (WConfig.VenganceChanges.Value == true && WConfig.VengenceEquipment.Value == true)
            {
                Vengence.EnableEquipmentForVengence();
            }


            WConfig.RiskConfig();

            DLC2Content.Items.KnockBackHitEnemies.tags = DLC2Content.Items.KnockBackHitEnemies.tags.Remove(ItemTag.DevotionBlacklist);
            DLC2Content.Items.IncreasePrimaryDamage.tags = DLC2Content.Items.IncreasePrimaryDamage.tags.Remove(ItemTag.DevotionBlacklist);

        }




        public void RunArtifactManager_onArtifactEnabledGlobal(RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
        {
            if (EnableNewContent)
            {
                if (artifactDef == RoR2Content.Artifacts.WispOnDeath && WConfig.SoulChanges.Value == true)
                {
                    if (Soul.unlockable)
                    {
                        Run.instance.unlockablesAlreadyFullyObtained.Add(Soul.unlockable);
                    }
                    On.RoR2.MasterSummon.Perform += Soul.SoulSpawnGreaterUniversal;
                }
                else if (artifactDef == RoR2Content.Artifacts.Enigma && WConfig.EnigmaChanges.Value == true)
                {
                    On.RoR2.GenericPickupController.AttemptGrant += Enigma.EnigmaEquipmentGranter;
                    On.RoR2.GenericPickupController.CreatePickup += Enigma.EnigmaFragmentMaker;
                }
            }
            if (artifactDef == RoR2Content.Artifacts.EliteOnly && WConfig.HonorChanges.Value == true)
            {
                Honor.OnArtifactEnable();
            }
            else if (artifactDef == RoR2Content.Artifacts.SingleMonsterType)
            {
                RoR2.UI.EnemyInfoPanel.MarkDirty();
                LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscHermitCrab").requiredFlags = 0;
                LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscVulture").requiredFlags = 0;
            }
            else if (artifactDef == RoR2Content.Artifacts.MixEnemy)
            {
                //LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscHermitCrab").requiredFlags = 0;
                LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscVulture").requiredFlags = 0;
            }
            else if (artifactDef == RoR2Content.Artifacts.Swarms)
            {
                //On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += Swarms.SwarmsDeployableLimitChanger;
            }
            else if (artifactDef == RoR2Content.Artifacts.Sacrifice && WConfig.SacrificeChanges.Value == true)
            {
                On.RoR2.Artifacts.SacrificeArtifactManager.OnServerCharacterDeath += Sacrifice.SacrificeArtifactManager_OnServerCharacterDeath;
            }
            else if (artifactDef == RoR2Content.Artifacts.ShadowClone && WConfig.VenganceChanges.Value == true)
            {
                Vengence.OnArtifactEnable();
            }
            else if (artifactDef == RoR2Content.Artifacts.MonsterTeamGainsItems)
            {
                MonsterTeamGainsItemsArtifactManager.EnsureMonsterItemCountMatchesStageCount();
            }
            else if (artifactDef == CU8Content.Artifacts.Devotion)
            {
                RoR2Content.Items.BoostDamage.hidden = true;
                RoR2Content.Items.BoostHp.hidden = true;
            }
        }

        public void RunArtifactManager_onArtifactDisabledGlobal(RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
        {
            if (EnableNewContent)
            {
                if (artifactDef == RoR2Content.Artifacts.WispOnDeath && WConfig.SoulChanges.Value == true)
                {
                    On.RoR2.MasterSummon.Perform -= Soul.SoulSpawnGreaterUniversal;
                }
                else if (artifactDef == RoR2Content.Artifacts.Enigma && WConfig.EnigmaChanges.Value == true)
                {
                    On.RoR2.GenericPickupController.AttemptGrant -= Enigma.EnigmaEquipmentGranter;
                    On.RoR2.GenericPickupController.CreatePickup -= Enigma.EnigmaFragmentMaker;
                }
            }
            if (artifactDef == RoR2Content.Artifacts.EliteOnly && WConfig.HonorChanges.Value == true)
            {
                Honor.OnArtifactDisable();
            }
            else if (artifactDef == RoR2Content.Artifacts.SingleMonsterType)
            {
                if (Stage.instance)
                {
                    Stage.instance.Network_singleMonsterTypeBodyIndex = -1;
                    EnemyInfoPanelInventoryProvider.isDirty = true;
                }
                LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscHermitCrab").requiredFlags = RoR2.Navigation.NodeFlags.NoCeiling;
                LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscVulture").requiredFlags = RoR2.Navigation.NodeFlags.NoCeiling;
            }
            else if (artifactDef == RoR2Content.Artifacts.MixEnemy)
            {
                //LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscHermitCrab").requiredFlags = RoR2.Navigation.NodeFlags.NoCeiling;
                LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscVulture").requiredFlags = RoR2.Navigation.NodeFlags.NoCeiling;
            }
            else if (artifactDef == RoR2Content.Artifacts.Swarms)
            {
                //On.RoR2.CharacterMaster.GetDeployableSameSlotLimit -= Swarms.SwarmsDeployableLimitChanger;
            }
            else if (artifactDef == RoR2Content.Artifacts.Sacrifice && WConfig.SacrificeChanges.Value == true)
            {
                On.RoR2.Artifacts.SacrificeArtifactManager.OnServerCharacterDeath -= Sacrifice.SacrificeArtifactManager_OnServerCharacterDeath;
            }
            else if (artifactDef == RoR2Content.Artifacts.ShadowClone && WConfig.VenganceChanges.Value == true)
            {
                Vengence.OnArtifactDisable();
            }
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
                            masterPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/ShopkeeperMaster"),
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
                if (WConfig.SpiteChanges.Value == true)
                {
                    BombArtifactManager.bombDamageCoefficient = 1.875f + 0.125f * (Run.instance.stageClearCount); //Stage clear count starts at 0
                }
            }

        }

    }
}

