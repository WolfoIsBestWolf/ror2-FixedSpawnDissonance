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
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Wolfo.VanillaArtifactsPlus", "VanillaArtifactsPlus", "3.0.0")]
    //[R2APISubmoduleDependency(nameof(ItemAPI), nameof(PrefabAPI), nameof(LanguageAPI))]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]


    public class Main : BaseUnityPlugin
    {
        public static readonly System.Random Random = new System.Random();

        //public static bool runstartonetimeonly = false;

        public static GameObject WarbannerObject = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/networkedobjects/WarbannerWard");
        public static GameObject WarbannerObjectEnemy = R2API.PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/networkedobjects/WarbannerWard"), "WarbannerWardEnemy", true);

        public static CharacterSpawnCard MagmaWormEliteHonor = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscMagmaWorm");
        public static CharacterSpawnCard ElectricWormEliteHonor = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscElectricWorm");

        public void Awake()
        {
            WConfig.InitConfig();

            GameModeCatalog.availability.CallWhenAvailable(ModSupport);
            //RoR2.ContentManagement.ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;

            On.RoR2.SceneDirector.Start += DSArtifactCheckerOnStageAwake;

            //On.RoR2.Stage.Start += StageStartMethodFSD;
            RunArtifactManager.onArtifactEnabledGlobal += RunArtifactManager_onArtifactEnabledGlobal;
            RunArtifactManager.onArtifactDisabledGlobal += RunArtifactManager_onArtifactDisabledGlobal;


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
            //Frailty.Start(); //Its like 1 function
            //Spite.Start(); //Also like 1 block of code that isn't very big
            //Swarms.Start();
           


            //Enemy Warbanner Support for Warbanner on Vengance
            Material WarbannerFlagNormal = WarbannerObject.transform.GetChild(1).GetChild(1).GetComponent<SkinnedMeshRenderer>().material;
            Material WarbannerFlagEnemy = Instantiate(WarbannerFlagNormal);
            WarbannerFlagEnemy.color = new Color(0.35f, 0.35f, 0f, 1f);
            Material WarbannerSphereNormal = WarbannerObject.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material;
            Material WarbannerSphereEnemy = Instantiate(WarbannerSphereNormal);
            WarbannerSphereEnemy.SetColor("_TintColor", new Color(1f, 0.1f, 0.1f, 1f));

            WarbannerObjectEnemy.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = WarbannerSphereEnemy;
            WarbannerObjectEnemy.transform.GetChild(1).GetChild(1).GetComponent<SkinnedMeshRenderer>().material = WarbannerFlagEnemy;
            //ContentAddition.AddNetworkedObject(WarbannerObjectEnemy);
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
                Honor.WormStart();
            }
        }




        public void EnemyWarbannerForVenganceEvolution(On.RoR2.Items.WardOnLevelManager.orig_OnCharacterLevelUp orig, CharacterBody characterBody)
        {
            if (characterBody.teamComponent.teamIndex == TeamIndex.Monster && characterBody.inventory && characterBody.inventory.GetItemCount(RoR2Content.Items.WardOnLevel) > 0)
            {
                RoR2.Items.WardOnLevelManager.wardPrefab = WarbannerObjectEnemy;
                orig(characterBody);
                RoR2.Items.WardOnLevelManager.wardPrefab = WarbannerObject;
                return;
            }
            orig(characterBody);
        }




        internal static void ModSupport()
        {
            if (WConfig.EvolutionChanges.Value == true && WConfig.EvoBetterBlacklist.Value == true)
            {
                Evolution.Tagchanger();
            }
            if (WConfig.EnigmaChanges.Value == true)
            {
                Enigma.EnigmaCallLate();
            }
            if (WConfig.KinChanges.Value == true && WConfig.KinYellowsForEnemies.Value == true)
            {
                KinBossDropsForEnemies.ModBossDropChanger();
            }
            if (WConfig.VenganceChanges.Value == true && WConfig.VengenceEquipment.Value == true)
            {
                Vengence.EnableEquipmentForVengence();
            }
            
            if (WConfig.DissonanceChanges.Value == true)
            {
                //Most mods already add it themselves so always double check
                Dissonance.ModdedEnemiesSupport();
            }

            Command.MakeEliteLists();

            if (WConfig.SpiteChanges.Value == true)
            {
                Spite.SpiteChangesCalledLate();
            }

            //Something about Mending cores spawning Soul Wisps I think which yeah is kinda dumb
            GameObject AffixEarthHealerBody = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/EliteEarth/AffixEarthHealerBody.prefab").WaitForCompletion();
            Soul.IndexAffixHealingCore = AffixEarthHealerBody.GetComponent<CharacterBody>().bodyIndex;
            Soul.SoulGreaterWispIndex = Soul.SoulGreaterWispBody.GetComponent<CharacterBody>().bodyIndex;
            Soul.SoulArchWispIndex = Soul.SoulArchWispBody.GetComponent<CharacterBody>().bodyIndex;

            /*
            Texture dummy = Soul.SoulGreaterWispBody.GetComponent<CharacterBody>().portraitIcon;

            for (int i = 0; i < BodyCatalog.bodyPrefabs.Length; i++)
            {
                CharacterBody characterBody = BodyCatalog.bodyPrefabBodyComponents[i];
                if(characterBody.bodyFlags.HasFlag(CharacterBody.BodyFlags.Masterless))
                {
                    characterBody.portraitIcon = dummy;
                }
            }

            BodyCatalog.FindBodyPrefab("GravekeeperTrackingFireball").GetComponent<CharacterBody>().portraitIcon = dummy;
            BodyCatalog.FindBodyPrefab("LunarWispTrackingBomb").GetComponent<CharacterBody>().portraitIcon = dummy;
            BodyCatalog.FindBodyPrefab("SMInfiniteTowerMaulingRockLarge").GetComponent<CharacterBody>().portraitIcon = dummy;
            BodyCatalog.FindBodyPrefab("SMInfiniteTowerMaulingRockMedium").GetComponent<CharacterBody>().portraitIcon = dummy;
            BodyCatalog.FindBodyPrefab("SMInfiniteTowerMaulingRockSmall").GetComponent<CharacterBody>().portraitIcon = dummy;
            BodyCatalog.FindBodyPrefab("SMMaulingRockLarge").GetComponent<CharacterBody>().portraitIcon = dummy;
            BodyCatalog.FindBodyPrefab("SMMaulingRockMedium").GetComponent<CharacterBody>().portraitIcon = dummy;
            BodyCatalog.FindBodyPrefab("SMMaulingRockSmall").GetComponent<CharacterBody>().portraitIcon = dummy;
            BodyCatalog.FindBodyPrefab("ScavSackProjectile").GetComponent<CharacterBody>().portraitIcon = dummy;
            */
        }









        public void RunArtifactManager_onArtifactEnabledGlobal(RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
        {
            if (artifactDef == RoR2Content.Artifacts.wispOnDeath && WConfig.SoulChanges.Value == true)
            {
                if (NetworkServer.active)
                {
                    On.RoR2.MasterSummon.Perform += Soul.SoulSpawnGreaterUniversal;
                }
            }
            else if (artifactDef == RoR2Content.Artifacts.eliteOnlyArtifactDef && WConfig.HonorChanges.Value == true)
            {
                if (NetworkServer.active)
                {
                    if (WConfig.HonorEliteWormRules.Value == "HonorOnly")
                    {
                        //Debug.Log("Artifact of Honor - Worms will be Elites");
                        MagmaWormEliteHonor.noElites = false;
                        MagmaWormEliteHonor.eliteRules = SpawnCard.EliteRules.Default;
                        ElectricWormEliteHonor.noElites = false;
                        ElectricWormEliteHonor.eliteRules = SpawnCard.EliteRules.Default;
                    }
                    if (WConfig.HonorPerfectedLunarBosses.Value == true)
                    {
                        Honor.LunarAffixEnable();
                        On.RoR2.CharacterBody.OnOutOfDangerChanged += Honor.PreventPerfectedMithrixFromRegenningShield;
                    }
                    if (WConfig.HonorMinionAlwaysElite.Value)
                    {
                        On.RoR2.MinionOwnership.MinionGroup.AddMinion += Honor.MinionsInheritHonor;
                    }               
                }
            }
            else if (artifactDef == RoR2Content.Artifacts.enigmaArtifactDef && WConfig.EnigmaChanges.Value == true)
            {
                if (NetworkServer.active)
                {
                    //BatteryPodPanel.transform.GetChild(0).gameObject.SetActive(false);
                    //BatteryPodPanel.transform.GetChild(1).gameObject.SetActive(false);
                    On.RoR2.GenericPickupController.AttemptGrant += Enigma.EnigmaEquipmentGranter;
                    On.RoR2.GenericPickupController.CreatePickup += Enigma.EnigmaFragmentMaker;
                }
            }
            /*else if (artifactDef == RoR2Content.Artifacts.weakAssKneesArtifactDef && WConfig.FrailtyChanges.Value == true)
            {
                 On.RoR2.GlobalEventManager.OnCharacterHitGroundServer += CharacterHitGround_FrailtyChanges;  
            }*/
            else if (artifactDef == RoR2Content.Artifacts.singleMonsterTypeArtifactDef && WConfig.KinChanges.Value == true)
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
            else if (artifactDef == RoR2Content.Artifacts.swarmsArtifactDef && WConfig.SwarmsChanges.Value == true)
            {
                On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += Swarms.SwarmsDeployableLimitChanger;
                //On.EntityStates.Gup.BaseSplitDeath.OnEnter += Swarms.BaseSplitDeath_OnEnter;
                foreach (TeamDef teamDef in TeamCatalog.teamDefs)
                {
                    teamDef.softCharacterLimit = (int)(teamDef.softCharacterLimit * 1.5f);
                }
            }
            else if (artifactDef == RoR2Content.Artifacts.sacrificeArtifactDef && WConfig.SacrificeChanges.Value == true)
            {
                 On.RoR2.Artifacts.SacrificeArtifactManager.OnServerCharacterDeath += Sacrifice.SacrificeArtifactManager_OnServerCharacterDeath;   
            }
            else if (artifactDef == RoR2Content.Artifacts.shadowCloneArtifactDef && WConfig.VenganceChanges.Value == true)
            {
                On.RoR2.HealthComponent.Heal += Vengence.UmbraHealHalf;
                On.RoR2.Items.WardOnLevelManager.OnCharacterLevelUp += EnemyWarbannerForVenganceEvolution;
            }
            if (WConfig.SpiteChanges.Value == true)
            {
                BombArtifactManager.bombDamageCoefficient = 1.5f + 0.1f * (Run.instance.stageClearCount);
            }
        }

        public void RunArtifactManager_onArtifactDisabledGlobal(RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
        {
            if (artifactDef == RoR2Content.Artifacts.wispOnDeath && WConfig.SoulChanges.Value == true)
            {
                On.RoR2.MasterSummon.Perform -= Soul.SoulSpawnGreaterUniversal;
            }
            else if (artifactDef == RoR2Content.Artifacts.eliteOnlyArtifactDef && WConfig.HonorChanges.Value == true)
            {
                if (WConfig.HonorEliteWormRules.Value == "HonorOnly")
                {
                    MagmaWormEliteHonor.noElites = true;
                    MagmaWormEliteHonor.eliteRules = SpawnCard.EliteRules.ArtifactOnly;
                    ElectricWormEliteHonor.noElites = true;
                    ElectricWormEliteHonor.eliteRules = SpawnCard.EliteRules.ArtifactOnly;
                }
                if (WConfig.HonorPerfectedLunarBosses.Value == true)
                {
                    Honor.LunarAffixDisable();
                    On.RoR2.CharacterBody.OnOutOfDangerChanged -= Honor.PreventPerfectedMithrixFromRegenningShield;
                }   
                if (WConfig.HonorMinionAlwaysElite.Value)
                {
                    On.RoR2.MinionOwnership.MinionGroup.AddMinion -= Honor.MinionsInheritHonor;
                }
            }
            else if (artifactDef == RoR2Content.Artifacts.enigmaArtifactDef && WConfig.EnigmaChanges.Value == true)
            {
                On.RoR2.GenericPickupController.AttemptGrant -= Enigma.EnigmaEquipmentGranter;
                On.RoR2.GenericPickupController.CreatePickup -= Enigma.EnigmaFragmentMaker;
            }
            /*else if (artifactDef == RoR2Content.Artifacts.weakAssKneesArtifactDef && WConfig.FrailtyChanges.Value == true)
            {
                if (WConfig.FrailtyChanges.Value == true)
                {
                    On.RoR2.GlobalEventManager.OnCharacterHitGroundServer -= CharacterHitGround_FrailtyChanges;
                }
            }*/
            else if (artifactDef == RoR2Content.Artifacts.singleMonsterTypeArtifactDef && WConfig.KinChanges.Value == true)
            {
                if (Stage.instance)
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
                }
                RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscHermitCrab").requiredFlags = RoR2.Navigation.NodeFlags.NoCeiling;
                RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscVulture").requiredFlags = RoR2.Navigation.NodeFlags.NoCeiling;
            }
            else if (artifactDef == RoR2Content.Artifacts.swarmsArtifactDef && WConfig.SwarmsChanges.Value == true)
            {
                On.RoR2.CharacterMaster.GetDeployableSameSlotLimit -= Swarms.SwarmsDeployableLimitChanger;
                //On.EntityStates.Gup.BaseSplitDeath.OnEnter -= Swarms.BaseSplitDeath_OnEnter;
                foreach (TeamDef teamDef in TeamCatalog.teamDefs)
                {
                    teamDef.softCharacterLimit = (int)(teamDef.softCharacterLimit / 1.5f);
                }
            }
            else if (artifactDef == RoR2Content.Artifacts.sacrificeArtifactDef && WConfig.SacrificeChanges.Value == true)
            {
                On.RoR2.Artifacts.SacrificeArtifactManager.OnServerCharacterDeath -= Sacrifice.SacrificeArtifactManager_OnServerCharacterDeath;
            }
            else if (artifactDef == RoR2Content.Artifacts.shadowCloneArtifactDef && WConfig.VenganceChanges.Value == true)
            {
                On.RoR2.HealthComponent.Heal -= Vengence.UmbraHealHalf;
                On.RoR2.Items.WardOnLevelManager.OnCharacterLevelUp -= EnemyWarbannerForVenganceEvolution;
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
                if (WConfig.SpiteChanges.Value == true)
                {
                    BombArtifactManager.bombDamageCoefficient = 1.5f + 0.1f * (Run.instance.stageClearCount); //Stage clear count starts at 0
                }
            }

        }







        /*private void CharacterHitGround_FrailtyChanges(On.RoR2.GlobalEventManager.orig_OnCharacterHitGroundServer orig, GlobalEventManager self, CharacterBody characterBody, Vector3 impactVelocity)
        {
            characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            orig(self, characterBody, impactVelocity);
        }*/


        /*
        private void ContentManager_collectContentPackProviders(RoR2.ContentManagement.ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(new Content());
        }
        */
    }
}

