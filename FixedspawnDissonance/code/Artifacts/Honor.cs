using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace FixedspawnDissonance
{
    public class Honor
    {
        public static List<EquipmentDef> EliteEquipmentDefs = new List<EquipmentDef>();

        public static void Start()
        {
            if (WConfig.HonorPerfectedLunarBosses.Value == true)
            {
                On.RoR2.ScriptedCombatEncounter.BeginEncounter += ScriptedCombatEncounter_BeginEncounter;
                On.RoR2.InfiniteTowerExplicitSpawnWaveController.Initialize += InfiniteTowerExplicitSpawnWaveController_Initialize;
            }

            //Probably don't remove this based on Honor
            On.RoR2.CharacterBody.UpdateItemAvailability += RemoveFireTrailFromWorm;

            if (WConfig.HonorStartingEliteEquip.Value)
            {
                On.RoR2.Run.Start += Honor.HonorGiveEliteEquipOnStart;
            }
        }

        private static void InfiniteTowerExplicitSpawnWaveController_Initialize(On.RoR2.InfiniteTowerExplicitSpawnWaveController.orig_Initialize orig, InfiniteTowerExplicitSpawnWaveController self, int waveIndex, Inventory enemyInventory, GameObject spawnTargetObject)
        {
            if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.EliteOnly))
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
                else if (name.StartsWith("cscFalseSon"))
                {
                    self.spawnList[0].eliteDef = DLC2Content.Elites.Aurelionite;
                }
            }


            orig(self, waveIndex, enemyInventory, spawnTargetObject);
        }

        private static void ScriptedCombatEncounter_BeginEncounter(On.RoR2.ScriptedCombatEncounter.orig_BeginEncounter orig, ScriptedCombatEncounter self)
        {
            orig(self);
            if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.EliteOnly))
            {
                string name = self.spawns[0].spawnCard.name;
                if (name.StartsWith("cscBrother"))
                {
                    for (int i = 0; i < self.combatSquad.memberCount; i++)
                    {
                        self.combatSquad.membersList[i].inventory.SetEquipmentIndex(RoR2Content.Equipment.AffixLunar.equipmentIndex);
                    }
                }
                else if (name.StartsWith("cscMiniVoidR"))
                {
                    for (int i = 0; i < self.combatSquad.memberCount; i++)
                    {
                        self.combatSquad.membersList[i].inventory.SetEquipmentIndex(DLC1Content.Equipment.EliteVoidEquipment.equipmentIndex);
                    }
                }
                else if (name.StartsWith("cscFalseSon"))
                {
                    for (int i = 0; i < self.combatSquad.memberCount; i++)
                    {
                        self.combatSquad.membersList[i].inventory.SetEquipmentIndex(DLC2Content.Equipment.EliteAurelioniteEquipment.equipmentIndex);
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
                //Maybe just check if eq drone
                Inventory inventory = minion.gameObject.GetComponent<Inventory>();
                if (inventory && inventory.currentEquipmentIndex == EquipmentIndex.None)
                {
                    if (EliteEquipmentDefs.Count > 0)
                    {
                        int index = Main.Random.Next(EliteEquipmentDefs.Count);

                        inventory.SetEquipment(new EquipmentState(EliteEquipmentDefs[index].equipmentIndex, Run.FixedTimeStamp.negativeInfinity, 0), 0);
                        //inventory.SetEquipmentIndex(EliteEquipmentHonorDefs[index].equipmentIndex);
                        inventory.GiveItem(RoR2Content.Items.BoostDamage, 5);
                        inventory.GiveItem(RoR2Content.Items.BoostHp, 15);
                    }
                }

            }
        }

        /*public static void HonorSpecialBossEliteFix()
        {
            //Does this really need to run each stage
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
                int affix1 = Main.Random.Next(EliteEquipmentHonorDefs.Count);
                int affix2 = Main.Random.Next(EliteEquipmentHonorDefs.Count);

                GoldTitanAllyHonorGiver.equipmentString = EliteEquipmentHonorDefs[affix1].name;
                HonorAffixGiverGoldTitan.equipmentString = EliteEquipmentHonorDefs[affix1].name;
                HonorAffixGiverSuperRoboBall.equipmentString = EliteEquipmentHonorDefs[affix2].name;
            }
        }*/

        public static void HonorGiveEliteEquipOnStart(On.RoR2.Run.orig_Start orig, Run self)
        {
            orig(self);
            if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.eliteOnlyArtifactDef))
            {
                foreach (var playerController in PlayerCharacterMasterController.instances)
                {
                    int index = Main.Random.Next(Honor.EliteEquipmentDefs.Count);
                    playerController.master.inventory.GiveEquipmentString(Honor.EliteEquipmentDefs[index].name);
                }
            }
        }


        public static void WormStart()
        {
            if (WConfig.HonorEliteWormRules.Value == "Never")
            {
                CharacterSpawnCard MagmaWormEliteHonor = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscMagmaWorm");
                MagmaWormEliteHonor.noElites = true;
                MagmaWormEliteHonor.eliteRules = SpawnCard.EliteRules.Default;

                CharacterSpawnCard ElectricWormEliteHonor = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscElectricWorm");
                ElectricWormEliteHonor.noElites = true;
                ElectricWormEliteHonor.eliteRules = SpawnCard.EliteRules.Default;
                //Logger.LogMessage($"Worms will not be Elites");
            }
            else if (WConfig.HonorEliteWormRules.Value == "HonorOnly")
            {
                //Logger.LogMessage($"Worms will be Elites with Artifact of Honor");
            }
            else if (WConfig.HonorEliteWormRules.Value == "Always")
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
                Debug.LogWarning("Invalid String for Worm Elite Rules");
            }
        }

    }
}