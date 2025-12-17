using RoR2;
using RoR2.Artifacts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace VanillaArtifactsPlus
{
    public class Honor
    {
        public static List<EliteDef> minionEliteDefs;

        public static void Start()
        {
            //Does Simu not have PromoteHonor normally??
 
            //On.RoR2.InfiniteTowerExplicitSpawnWaveController.Initialize += Honor_SimuForceSpecialEliteType;

            //Probably don't remove this based on Honor
            On.RoR2.CharacterBody.UpdateItemAvailability += RemoveFireTrailFromWorm;

            Addressables.LoadAssetAsync<EliteDef>(key: "RoR2/DLC1/EliteEarth/edEarthHonor.asset").WaitForCompletion().healthBoostCoefficient = 2;

            On.RoR2.Artifacts.EliteOnlyArtifactManager.PromoteIfHonor += EliteOnlyArtifactManager_PromoteIfHonor;

            On.EntityStates.BrotherMonster.EnterSkyLeap.OnEnter += EnterSkyLeap_OnEnter;
            On.EntityStates.FalseSonBoss.LunarGazeLeap.OnEnter += LunarGazeLeap_OnEnter;
        }

        private static void LunarGazeLeap_OnEnter(On.EntityStates.FalseSonBoss.LunarGazeLeap.orig_OnEnter orig, EntityStates.FalseSonBoss.LunarGazeLeap self)
        {
            orig(self);
            self.characterBody.outOfDangerStopwatch = -99999f;
        }

        private static void EliteOnlyArtifactManager_PromoteIfHonor(On.RoR2.Artifacts.EliteOnlyArtifactManager.orig_PromoteIfHonor orig, CharacterMaster instanceMaster, Xoroshiro128Plus rng, EliteDef[] eliteDefs)
        {
            if (WConfig.Honor_PerfectMithrix.Value)
            {
                if (eliteDefs == null)
                {
                    var list = EliteOnlyArtifactManager.eliteDefs.ToList();
                    list.Add(DLC2Content.Elites.Aurelionite);
                    if (Run.instance.stageClearCount > 6)
                    {
                        foreach (EliteDef eliteDef in EliteCatalog.eliteDefs)
                        {
                            if (eliteDef.devotionLevel == EliteDef.DevotionEvolutionLevel.High)
                            {
                                list.Add(eliteDef);
                            }
                        }
                        list.Remove(RoR2Content.Elites.Lunar);
                    }
                    if (MoonBatteryMissionController.instance)
                    {
                        list.Add(RoR2Content.Elites.Lunar); list.Add(RoR2Content.Elites.Lunar);
                    }
                    if (VoidRaidGauntletController.instance)
                    {
                        list.Add(DLC1Content.Elites.Void); list.Add(DLC1Content.Elites.Void);
                    }
                    orig(instanceMaster, Run.instance.bossRewardRng, list.ToArray());
                    return;
                }
            }

            orig(instanceMaster, Run.instance.bossRewardRng, eliteDefs);
        }

        public static void OnArtifactEnable()
        {
            if (NetworkServer.active)
            {
                WConfig.cfgEliteWorms_Changed(null, null);
                Honor.Honor_EliteTiers(true);
                if (WConfig.Honor_PerfectMithrix.Value == true)
                {
                   
                }
                /*if (WConfig.Honor_EliteMinions.Value)
                {
                    On.RoR2.MinionOwnership.MinionGroup.AddMinion += Honor.MinionsInheritHonor;
                }*/
            }
        }

        private static void EnterSkyLeap_OnEnter(On.EntityStates.BrotherMonster.EnterSkyLeap.orig_OnEnter orig, EntityStates.BrotherMonster.EnterSkyLeap self)
        {
            orig(self);
            self.characterBody.outOfDangerStopwatch = -99999f;
        }

        public static void OnArtifactDisable()
        {
            WConfig.cfgEliteWorms_Changed(null, null);
            Honor.Honor_EliteTiers(false);
           /* if (WConfig.Honor_EliteMinions.Value)
            {
                On.RoR2.MinionOwnership.MinionGroup.AddMinion -= Honor.MinionsInheritHonor;
            }*/
        }

   


        public static void Honor_EliteTiers(bool activate)
        {
            if (activate)
            {
                minionEliteDefs = new List<EliteDef>();
                foreach (EliteDef eliteDef in EliteCatalog.eliteDefs)
                {
                    if (eliteDef.name.EndsWith("Honor"))
                    {
                        if (eliteDef.IsAvailable())
                        {
                            minionEliteDefs.Add(eliteDef);
                        }
                    }
                }
                /*if (WConfig.Honor_EliteMinionsNoGilded.Value)
                {
                    minionEliteDefs.Remove(DLC2Content.Elites.AurelioniteHonor);
                }*/
            }
            else
            {
                minionEliteDefs = null;
            }

            if (!WConfig.Honor_RedoneElites.Value)
            {
                return;
            }
            //List<EliteDef> changedList = new List<EliteDef>();

            float value = 2f;
            if (activate)
            {
                value = 0.5f;
            }
            else
            {
            }
            //Ideally keep Lunars being allowed to spawn as Tier1, if anything add Tier2
            //How do we force ignore spawn card rules ig ig

            foreach (EliteDef eliteDef in EliteCatalog.eliteDefs)
            {
                if (eliteDef.devotionLevel == EliteDef.DevotionEvolutionLevel.High)
                {
                    eliteDef.healthBoostCoefficient = Mathf.LerpUnclamped(1f, eliteDef.healthBoostCoefficient, value);
                    eliteDef.damageBoostCoefficient = Mathf.LerpUnclamped(1f, eliteDef.damageBoostCoefficient, value);
                }
            }

            for (int i = 0; i < CombatDirector.eliteTiers.Length; i++)
            {
                if (CombatDirector.eliteTiers[i].eliteTypes.Length > 0)
                {
                    if (CombatDirector.eliteTiers[i].eliteTypes[0] == RoR2Content.Elites.Poison)
                    {
                        CombatDirector.eliteTiers[i].costMultiplier = Mathf.LerpUnclamped(1f, CombatDirector.eliteTiers[i].costMultiplier, value);
                    }
                    else if (CombatDirector.eliteTiers[i].eliteTypes[0] == RoR2Content.Elites.Lunar)
                    {
                        CombatDirector.eliteTiers[i].costMultiplier = Mathf.LerpUnclamped(1f, CombatDirector.eliteTiers[i].costMultiplier, value);
                    }
                }
            } 
        }

         
        private static void Honor_SimuForceSpecialEliteType(On.RoR2.InfiniteTowerExplicitSpawnWaveController.orig_Initialize orig, InfiniteTowerExplicitSpawnWaveController self, int waveIndex, Inventory enemyInventory, GameObject spawnTargetObject)
        {
            if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.EliteOnly))
            {
                if (WConfig.Honor_PerfectMithrix.Value)
                {
                    if (self.rng.nextNormalizedFloat > 0.3f)
                    {
                        orig(self, waveIndex, enemyInventory, spawnTargetObject);
                        return;
                    }
                    string name = self.spawnList[0].spawnCard.name;
                    if (name.StartsWith("cscBrother"))
                    {
                        self.spawnList[0].eliteDef = RoR2Content.Elites.Lunar;
                    }
                    else if (name.StartsWith("cscScavLunar"))
                    {
                        self.spawnList[0].eliteDef = RoR2Content.Elites.Lunar;
                    }
                    else if (name.StartsWith("cscMiniVoidR"))
                    {
                        self.spawnList[0].eliteDef = DLC1Content.Elites.Void;
                    }
                    else if (name.StartsWith("cscITVoidMe"))
                    {
                        self.spawnList[0].eliteDef = DLC1Content.Elites.Void;
                    }
                    else if (name.StartsWith("cscVoidInfestor"))
                    {
                        self.spawnList[0].eliteDef = DLC1Content.Elites.Void;
                    }
                    else if (name.StartsWith("cscFalseSon"))
                    {
                        self.spawnList[0].eliteDef = DLC2Content.Elites.Aurelionite;
                    }
                    else if (name.StartsWith("cscTitanGold"))
                    {
                        if (Run.instance.IsExpansionEnabled(DLC2Content.Elites.Aurelionite.eliteEquipmentDef.requiredExpansion))
                        {
                            self.spawnList[0].eliteDef = DLC2Content.Elites.Aurelionite;
                        }
                    }
                }   
            }
            orig(self, waveIndex, enemyInventory, spawnTargetObject);
        }

      

        private static void RemoveFireTrailFromWorm(On.RoR2.CharacterBody.orig_UpdateItemAvailability orig, CharacterBody self)
        {
            orig(self);
            if (self && self.GetComponent<WormBodyPositions2>())
            {
                self.itemAvailability.hasFireTrail = false;
            }
        }
 

        public static void MinionsInheritHonor(On.RoR2.MinionOwnership.MinionGroup.orig_AddMinion orig, NetworkInstanceId ownerId, global::RoR2.MinionOwnership minion)
        {
            orig(ownerId, minion);
            if (NetworkServer.active)
            {
                Inventory inventory = minion.gameObject.GetComponent<Inventory>();
                if (inventory && inventory.currentEquipmentIndex == EquipmentIndex.None)
                {
                    if (minionEliteDefs.Count > 0)
                    {
                        int index = VanillaArtifactsMain.Random.Next(minionEliteDefs.Count);
                        inventory.SetEquipmentIndex(minionEliteDefs[index].eliteEquipmentDef.equipmentIndex);
                        //inventory.GiveItem(RoR2Content.Items.BoostHp, (int)(minionEliteDefs[index].healthBoostCoefficient - 1) * 10);
                        //inventory.GiveItem(RoR2Content.Items.BoostDamage, (int)(minionEliteDefs[index].damageBoostCoefficient - 1) * 10);
                    }
                }

            }
        }


 

    }
}