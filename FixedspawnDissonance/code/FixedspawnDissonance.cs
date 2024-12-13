using BepInEx;
using R2API.Utils;
using RoR2;
using RoR2.Artifacts;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace FixedspawnDissonance
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Wolfo.VanillaArtifactsPlus", "VanillaArtifactsPlus", "3.2.0")]
    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync, VersionStrictness.DifferentModVersionsAreOk)]


    public class Main : BaseUnityPlugin
    {
        public static readonly System.Random Random = new System.Random();

        public void Awake()
        {
            Assets.Init(Info);
            WConfig.InitConfig();

            GameModeCatalog.availability.CallWhenAvailable(ModSupport);
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
            Swarms.Start();
            Glass.Start();
            Rebirth.Start();
 
        }

        private void Start() //Called at the first frame of the game.
        {
            //Needs to be called late for reasons
            if (WConfig.DissonanceChanges.Value == true)
            {
                Dissonance.Start();
            }
            if (WConfig.KinYellowsForEnemies.Value == true)
            {
                KinBossDropsForEnemies.Start();
            }
            if (WConfig.HonorChanges.Value == true)
            {
                Honor.Worm_EliteStuff(false);
            }
        }


        internal static void ModSupport()
        {
    
            


            GameObject AffixEarthHealerBody = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/EliteEarth/AffixEarthHealerBody.prefab").WaitForCompletion();
            Soul.IndexAffixHealingCore = AffixEarthHealerBody.GetComponent<CharacterBody>().bodyIndex;
            if (!WConfig.DisableNewContent.Value)
            {
                Soul.SoulGreaterWispIndex = Soul.SoulGreaterWispBody.GetComponent<CharacterBody>().bodyIndex;
                Soul.SoulArchWispIndex = Soul.SoulArchWispBody.GetComponent<CharacterBody>().bodyIndex;
            }

            if (WConfig.EvolutionChanges.Value == true && WConfig.EvoBetterBlacklist.Value == true)
            {
                Evolution.Tagchanger();
            }
            if (WConfig.EnigmaChanges.Value == true)
            {
                R2API.LanguageAPI.Add("ITEM_ENIGMAEQUIPMENTBOOST_DESC", string.Format(Language.GetString("ITEM_ENIGMAEQUIPMENTBOOST_DESC"), WConfig.EnigmaCooldownReduction.Value, WConfig.EnigmaCooldownReduction.Value));
                Enigma.EnigmaCallLate();
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
        }




        public void RunArtifactManager_onArtifactEnabledGlobal(RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
        {
            if (artifactDef == RoR2Content.Artifacts.wispOnDeath && WConfig.SoulChanges.Value == true)
            {
                if (NetworkServer.active)
                {
                    if (!WConfig.DisableNewContent.Value)
                    {
                        On.RoR2.MasterSummon.Perform += Soul.SoulSpawnGreaterUniversal;
                    }
                }
            }
            else if (artifactDef == RoR2Content.Artifacts.eliteOnlyArtifactDef && WConfig.HonorChanges.Value == true)
            {
                if (NetworkServer.active)
                {
                    Honor.Worm_EliteStuff(true);
                    Honor.Honor_EliteTiers(true);
                    if (WConfig.Honor_PerfectMithrix.Value == true)
                    {
                        On.RoR2.CharacterBody.OnOutOfDangerChanged += Honor.PreventPerfectedMithrixFromRegenningShield;
                    }
                    if (WConfig.Honor_EliteMinions.Value)
                    {
                        On.RoR2.MinionOwnership.MinionGroup.AddMinion += Honor.MinionsInheritHonor;
                    }
                }
            }
            else if (artifactDef == RoR2Content.Artifacts.enigmaArtifactDef && WConfig.EnigmaChanges.Value == true)
            {
                if (NetworkServer.active)
                {
                    if (!WConfig.DisableNewContent.Value)
                    {
                        On.RoR2.GenericPickupController.AttemptGrant += Enigma.EnigmaEquipmentGranter;
                        On.RoR2.GenericPickupController.CreatePickup += Enigma.EnigmaFragmentMaker;
                    }
                }
            }
            else if (artifactDef == RoR2Content.Artifacts.singleMonsterTypeArtifactDef && WConfig.KinChanges.Value == true)
            {
                RoR2.UI.EnemyInfoPanel.MarkDirty();
                LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscHermitCrab").requiredFlags = 0;
                LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscVulture").requiredFlags = 0;
            }
            else if (artifactDef == RoR2Content.Artifacts.swarmsArtifactDef)
            {
                On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += Swarms.SwarmsDeployableLimitChanger;
            }
            else if (artifactDef == RoR2Content.Artifacts.sacrificeArtifactDef && WConfig.SacrificeChanges.Value == true)
            {
                On.RoR2.Artifacts.SacrificeArtifactManager.OnServerCharacterDeath += Sacrifice.SacrificeArtifactManager_OnServerCharacterDeath;
            }
            else if (artifactDef == RoR2Content.Artifacts.shadowCloneArtifactDef && WConfig.VenganceChanges.Value == true)
            {
                Vengence.OnArtifactEnable();
            }
        }

        public void RunArtifactManager_onArtifactDisabledGlobal(RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
        {
            if (artifactDef == RoR2Content.Artifacts.wispOnDeath && WConfig.SoulChanges.Value == true)
            {
                if (!WConfig.DisableNewContent.Value)
                {
                    On.RoR2.MasterSummon.Perform -= Soul.SoulSpawnGreaterUniversal;
                }
            }
            else if (artifactDef == RoR2Content.Artifacts.eliteOnlyArtifactDef && WConfig.HonorChanges.Value == true)
            {
                Honor.Worm_EliteStuff(false);
                Honor.Honor_EliteTiers(false);
                if (WConfig.Honor_PerfectMithrix.Value == true)
                {
                    On.RoR2.CharacterBody.OnOutOfDangerChanged -= Honor.PreventPerfectedMithrixFromRegenningShield;
                }
                if (WConfig.Honor_EliteMinions.Value)
                {
                    On.RoR2.MinionOwnership.MinionGroup.AddMinion -= Honor.MinionsInheritHonor;
                }
            }
            else if (artifactDef == RoR2Content.Artifacts.enigmaArtifactDef && WConfig.EnigmaChanges.Value == true)
            {
                if (!WConfig.DisableNewContent.Value)
                {
                    On.RoR2.GenericPickupController.AttemptGrant -= Enigma.EnigmaEquipmentGranter;
                    On.RoR2.GenericPickupController.CreatePickup -= Enigma.EnigmaFragmentMaker;
                }
            }
            else if (artifactDef == RoR2Content.Artifacts.singleMonsterTypeArtifactDef && WConfig.KinChanges.Value == true)
            {
                if (Stage.instance)
                {
                    Stage.instance.Network_singleMonsterTypeBodyIndex = -1;
                    EnemyInfoPanelInventoryProvider.isDirty = true;
                }
                LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscHermitCrab").requiredFlags = RoR2.Navigation.NodeFlags.NoCeiling;
                LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscVulture").requiredFlags = RoR2.Navigation.NodeFlags.NoCeiling;
            }
            else if (artifactDef == RoR2Content.Artifacts.swarmsArtifactDef)
            {
                On.RoR2.CharacterMaster.GetDeployableSameSlotLimit -= Swarms.SwarmsDeployableLimitChanger;
            }
            else if (artifactDef == RoR2Content.Artifacts.sacrificeArtifactDef && WConfig.SacrificeChanges.Value == true)
            {
                On.RoR2.Artifacts.SacrificeArtifactManager.OnServerCharacterDeath -= Sacrifice.SacrificeArtifactManager_OnServerCharacterDeath;
            }
            else if (artifactDef == RoR2Content.Artifacts.shadowCloneArtifactDef && WConfig.VenganceChanges.Value == true)
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

