using RoR2;
using System.Collections.Generic;
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
            On.RoR2.ScriptedCombatEncounter.BeginEncounter += Honor_ForceSpecialEliteType;
            On.RoR2.InfiniteTowerExplicitSpawnWaveController.Initialize += Honor_SimuForceSpecialEliteType;

            //Probably don't remove this based on Honor
            On.RoR2.CharacterBody.UpdateItemAvailability += RemoveFireTrailFromWorm;

            Addressables.LoadAssetAsync<EliteDef>(key: "RoR2/DLC1/EliteEarth/edEarthHonor.asset").WaitForCompletion().healthBoostCoefficient = 2;
        }

        public static void OnArtifactEnable()
        {
            if (NetworkServer.active)
            {
                WConfig.cfgEliteWorms_Changed(null, null);
                Honor.Honor_EliteTiers(true);
                if (WConfig.Honor_PerfectMithrix.Value == true)
                {
                    On.RoR2.CharacterBody.OnOutOfDangerChanged += Honor.PreventPerfectedMithrixFromRegenningShield;
                }
                /*if (WConfig.Honor_EliteMinions.Value)
                {
                    On.RoR2.MinionOwnership.MinionGroup.AddMinion += Honor.MinionsInheritHonor;
                }*/
            }
        }

        public static void OnArtifactDisable()
        {
            WConfig.cfgEliteWorms_Changed(null, null);
            Honor.Honor_EliteTiers(false);
            if (WConfig.Honor_PerfectMithrix.Value == true)
            {
                On.RoR2.CharacterBody.OnOutOfDangerChanged -= Honor.PreventPerfectedMithrixFromRegenningShield;
            }
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
                //On.RoR2.CombatDirector.IsEliteOnlyArtifactActive += DisableHonorEliteTier;
                On.RoR2.CombatDirector.NotEliteOnlyArtifactActive += AllowNormalTiersHonor;
                CombatDirector.eliteTiers[0].isAvailable = (SpawnCard.EliteRules rules) => false;
                value = 0.5f;
            }
            else
            {
                //On.RoR2.CombatDirector.IsEliteOnlyArtifactActive -= DisableHonorEliteTier;
                On.RoR2.CombatDirector.NotEliteOnlyArtifactActive -= AllowNormalTiersHonor;
                //CombatDirector.eliteTiers[0].isAvailable = (SpawnCard.EliteRules rules) => true;
                CombatDirector.eliteTiers[0].isAvailable = ((SpawnCard.EliteRules rules) => CombatDirector.NotEliteOnlyArtifactActive());
            }
            //Ideally keep Lunars being allowed to spawn as Tier1, if anything add Tier2
            //How do we force ignore spawn card rules ig ig

            foreach (EliteDef eliteDef in EliteCatalog.eliteDefs)
            {
                if (!eliteDef.name.EndsWith("Honor"))
                {
                    eliteDef.healthBoostCoefficient = Mathf.LerpUnclamped(1f, eliteDef.healthBoostCoefficient, value);
                    eliteDef.damageBoostCoefficient = Mathf.LerpUnclamped(1f, eliteDef.damageBoostCoefficient, value);
                    //Debug.Log(eliteDef + " hp:" + eliteDef.healthBoostCoefficient+ " dmg:"+ eliteDef.damageBoostCoefficient); 
                }
            }

            /*for (int i = 0; i < CombatDirector.eliteTiers.Length; i++)
            {
                //Debug.Log("EliteTier " + i);
                if (CombatDirector.eliteTiers[i].eliteTypes.Length > 0)
                {
                    if (CombatDirector.eliteTiers[i].eliteTypes[0] == RoR2Content.Elites.LightningHonor)
                    {
                        //Should help with Adaptive Elite spam, I think??
                        var Temp = CombatDirector.eliteTiers[0];
                        CombatDirector.eliteTiers[0] = CombatDirector.eliteTiers[i];
                        CombatDirector.eliteTiers[i] = Temp;
                        continue;
                    }
                }
                CombatDirector.eliteTiers[i].costMultiplier = Mathf.LerpUnclamped(1f, CombatDirector.eliteTiers[i].costMultiplier, value);
            }*/
        }

        private static bool AllowNormalTiersHonor(On.RoR2.CombatDirector.orig_NotEliteOnlyArtifactActive orig)
        {
            return true;
        }
        public static bool DisableHonorEliteTier(On.RoR2.CombatDirector.orig_IsEliteOnlyArtifactActive orig)
        {
            //Disable original Honor
            return false;
        }

        private static void Honor_SimuForceSpecialEliteType(On.RoR2.InfiniteTowerExplicitSpawnWaveController.orig_Initialize orig, InfiniteTowerExplicitSpawnWaveController self, int waveIndex, Inventory enemyInventory, GameObject spawnTargetObject)
        {
            if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.EliteOnly))
            {
                if (WConfig.Honor_PerfectMithrix.Value)
                {
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

        private static void Honor_ForceSpecialEliteType(On.RoR2.ScriptedCombatEncounter.orig_BeginEncounter orig, ScriptedCombatEncounter self)
        {
            orig(self);
            if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.EliteOnly))
            {
                if (self.rng.nextNormalizedFloat < 0.70f)
                {
                    return;
                }

                if (!WConfig.Honor_PerfectMithrix.Value)
                {
                    return;
                }
                string name = self.spawns[0].spawnCard.name;
                bool doIt = false;
                EquipmentDef elite = null;

                if (name.StartsWith("cscBrother") || name.StartsWith("cscScavLunar"))
                {
                    doIt = true;
                    elite = RoR2Content.Equipment.AffixLunar;            
                }
                else if (name.StartsWith("cscMiniVoidR") || name.StartsWith("cscVoidInfe"))
                {
                    doIt = true;
                    elite = DLC1Content.Equipment.EliteVoidEquipment;
                }
                else if (name.StartsWith("cscFalseSon") || name.StartsWith("cscTitanGold"))
                {
                    elite = DLC2Content.Equipment.EliteAurelioniteEquipment;
                    if (Run.instance.IsExpansionEnabled(elite.requiredExpansion))
                    {
                        doIt = true;
                    }                 
                }
                if (doIt)
                {
                    for (int i = 0; i < self.combatSquad.memberCount; i++)
                    {
                        self.combatSquad.membersList[i].inventory.SetEquipmentIndex(elite.equipmentIndex);
                    }
                }
            }
        }

        private static void RemoveFireTrailFromWorm(On.RoR2.CharacterBody.orig_UpdateItemAvailability orig, CharacterBody self)
        {
            orig(self);
            if (self && self.GetComponent<WormBodyPositions2>())
            {
                self.itemAvailability.hasFireTrail = false;
            }
        }

        public static void PreventPerfectedMithrixFromRegenningShield(On.RoR2.CharacterBody.orig_OnOutOfDangerChanged orig, CharacterBody self)
        {
            orig(self);
            if (self.name.StartsWith("Bro"))
            {
                self.outOfDangerStopwatch = -1000;
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