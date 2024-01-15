using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace FixedspawnDissonance
{
    public class Honor
    {
        public static List<EquipmentDef> EliteEquipmentDefs = new List<EquipmentDef>();

        //Honor Stupid Elite
        public static GivePickupsOnStart HonorAffixGiverVoidling0 = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/VoidRaidCrab/MiniVoidRaidCrabMasterBase.prefab").WaitForCompletion().AddComponent<GivePickupsOnStart>();
        public static GivePickupsOnStart HonorAffixGiverVoidling1 = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/VoidRaidCrab/MiniVoidRaidCrabMasterPhase1.prefab").WaitForCompletion().AddComponent<GivePickupsOnStart>();
        public static GivePickupsOnStart HonorAffixGiverVoidling2 = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/VoidRaidCrab/MiniVoidRaidCrabMasterPhase2.prefab").WaitForCompletion().AddComponent<GivePickupsOnStart>();
        public static GivePickupsOnStart HonorAffixGiverVoidling3 = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/VoidRaidCrab/MiniVoidRaidCrabMasterPhase3.prefab").WaitForCompletion().AddComponent<GivePickupsOnStart>();

        public static GivePickupsOnStart BrotherLunarAffixGiver = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/BrotherMaster").AddComponent<GivePickupsOnStart>();
        public static GivePickupsOnStart BrotherHurtLunarAffixGiver = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/BrotherHurtMaster").AddComponent<GivePickupsOnStart>();
        public static GivePickupsOnStart BrotherHauntLunarAffixGiver = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/BrotherHauntMaster").AddComponent<GivePickupsOnStart>();


        public static void Start()
        {
            if (WConfig.HonorPerfectedLunarBosses.Value == true)
            {
                LunarAffixHonorInit();
            }
            LunarAffixDisable();
            if (WConfig.HonorStartingEliteEquip.Value)
            {
                On.RoR2.Run.Start += Honor.HonorGiveEliteEquipOnStart;
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

        public static void LunarAffixHonorInit()
        {
            BrotherLunarAffixGiver.equipmentString = "EliteLunarEquipment";
            BrotherHurtLunarAffixGiver.equipmentString = "EliteLunarEquipment";
            BrotherHauntLunarAffixGiver.equipmentString = "EliteLunarEquipment";

            HonorAffixGiverVoidling0.equipmentString = "EliteVoidEquipment";
            HonorAffixGiverVoidling1.equipmentString = "EliteVoidEquipment";
            HonorAffixGiverVoidling2.equipmentString = "EliteVoidEquipment";
            HonorAffixGiverVoidling3.equipmentString = "EliteVoidEquipment";
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
                    int index = Main.Random.Next(EliteEquipmentDefs.Count);

                    inventory.SetEquipment(new EquipmentState(EliteEquipmentDefs[index].equipmentIndex, Run.FixedTimeStamp.negativeInfinity, 0), 0);
                    //inventory.SetEquipmentIndex(EliteEquipmentHonorDefs[index].equipmentIndex);
                    inventory.GiveItem(RoR2Content.Items.BoostDamage, 5);
                    inventory.GiveItem(RoR2Content.Items.BoostHp, 15);
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

        public static void LunarAffixDisable()
        {
            BrotherLunarAffixGiver.enabled = false;
            BrotherHurtLunarAffixGiver.enabled = false;
            BrotherHauntLunarAffixGiver.enabled = false;

            HonorAffixGiverVoidling0.enabled = false;
            HonorAffixGiverVoidling1.enabled = false;
            HonorAffixGiverVoidling2.enabled = false;
            HonorAffixGiverVoidling3.enabled = false;
        }
        public static void LunarAffixEnable()
        {
            BrotherLunarAffixGiver.enabled = true;
            BrotherHurtLunarAffixGiver.enabled = true;
            BrotherHauntLunarAffixGiver.enabled = true;

            HonorAffixGiverVoidling0.enabled = true;
            HonorAffixGiverVoidling1.enabled = true;
            HonorAffixGiverVoidling2.enabled = true;
            HonorAffixGiverVoidling3.enabled = true;
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