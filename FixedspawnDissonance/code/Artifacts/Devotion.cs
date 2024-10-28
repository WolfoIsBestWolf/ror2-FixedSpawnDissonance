using RoR2;
using RoR2.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FixedspawnDissonance
{
    public class Devotion
    {

        public static void Start()
        {
            On.RoR2.DevotionInventoryController.OnDevotionArtifactEnabled += FixMissingEliteTypes;
            //IL failed to work so I'll do this stupider version
            On.RoR2.DevotionInventoryController.OnDevotionArtifactDisabled += DevotionInventoryController_OnDevotionArtifactDisabled;
           
            On.RoR2.CharacterAI.LemurianEggController.CreateItemTakenOrb += LemurianEggController_CreateItemTakenOrb; 

            ////Issues with Nuxlars
            //When first ever Lem dies, doesn't start showing next Lem.
            //When first ever Lem dies, won't ever add new Lemurian Inventory
            //Ported from Nuxlar with Permission
            On.RoR2.UI.ScoreboardController.Rebuild += AddLemurianInventory;
            //On.DevotedLemurianController.Start += GetLemInventory;
        }

        private static void DevotionInventoryController_OnDevotionArtifactDisabled(On.RoR2.DevotionInventoryController.orig_OnDevotionArtifactDisabled orig, RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
        {
            if (artifactDef == CU8Content.Artifacts.Devotion)
            {
                List<EquipmentIndex> lowLevelEliteBuffs = new List<EquipmentIndex>(DevotionInventoryController.lowLevelEliteBuffs);
                List<EquipmentIndex> highLevelEliteBuffs = new List<EquipmentIndex>(DevotionInventoryController.highLevelEliteBuffs);
                orig(runArtifactManager, artifactDef);
                DevotionInventoryController.lowLevelEliteBuffs = lowLevelEliteBuffs;
                DevotionInventoryController.highLevelEliteBuffs = highLevelEliteBuffs;
            }
            else
            {
                orig(runArtifactManager, artifactDef);
            }
        }

        private static void LemurianEggController_CreateItemTakenOrb(On.RoR2.CharacterAI.LemurianEggController.orig_CreateItemTakenOrb orig, RoR2.CharacterAI.LemurianEggController self, Vector3 effectOrigin, GameObject targetObject, ItemIndex itemIndex)
        {
            //Why the fuck do they null this after the artitact is disabled or before it's ever enabled.
            if (!DevotionInventoryController.s_effectPrefab)
            {
                RoR2Content.Items.BoostDamage.hidden = true;
                RoR2Content.Items.BoostHp.hidden = true;
                DevotionInventoryController.s_effectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OrbEffects/ItemTakenOrbEffect");
            }
            orig(self, effectOrigin, targetObject, itemIndex);
        }


        private static void AddLemurianInventory(On.RoR2.UI.ScoreboardController.orig_Rebuild orig, ScoreboardController self)
        {
            orig(self);
            //Basically overwrites vanilla one entirely hopefully not an issue

            //Better this than checking Artifact
            if (DevotionInventoryController.InstanceList.Count > 0)
            {
                List<CharacterMaster> list = new List<CharacterMaster>();
                foreach (PlayerCharacterMasterController playerCharacterMasterController in PlayerCharacterMasterController.instances)
                {
                    list.Add(playerCharacterMasterController.master);

                    if (WConfig.DevotionOnlyOneInventory.Value)
                    {
                        CharacterMaster summonerMaster = playerCharacterMasterController.master;
                        MinionOwnership.MinionGroup minionGroup = MinionOwnership.MinionGroup.FindGroup(summonerMaster.netId);
                        if (minionGroup != null)
                        {
                            foreach (MinionOwnership minionOwnership in minionGroup.members)
                            {
                                Inventory devotedLemurianController;
                                //if (minionOwnership && minionOwnership.GetComponent<DevotedLemurianController>())
                                if (minionOwnership && minionOwnership.TryGetComponent<Inventory>(out devotedLemurianController))
                                {
                                    if (devotedLemurianController.GetItemCount(CU8Content.Items.LemurianHarness) > 0)
                                    {
                                        list.Add(minionOwnership.GetComponent<CharacterMaster>());
                                    }                                   
                                    break;
                                }
                            }
                        }
                    }
                }
                //If every player has their own Lem inventory why would we only display the local one
                if (!WConfig.DevotionOnlyOneInventory.Value)
                {
                    CharacterMaster summonerMaster = LocalUserManager.readOnlyLocalUsersList.First<LocalUser>().cachedMasterController.master;
                    MinionOwnership.MinionGroup minionGroup = MinionOwnership.MinionGroup.FindGroup(summonerMaster.netId);
                    if (minionGroup != null)
                    {
                        foreach (MinionOwnership minionOwnership in minionGroup.members)
                        {
                            DevotedLemurianController devotedLemurianController;
                            if (minionOwnership && minionOwnership.GetComponent<CharacterMaster>().TryGetComponent<DevotedLemurianController>(out devotedLemurianController))
                            {
                                list.Add(minionOwnership.GetComponent<CharacterMaster>());
                                break;
                            }
                        }
                    }
                }

                self.SetStripCount(list.Count);
                for (int i = 0; i < list.Count; i++)
                {
                    self.stripAllocator.elements[i].SetMaster(list[i]);
                }
            }
        }


        private static void FixMissingEliteTypes(On.RoR2.DevotionInventoryController.orig_OnDevotionArtifactEnabled orig, RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
        {
            orig(runArtifactManager, artifactDef);
            if (artifactDef != CU8Content.Artifacts.Devotion)
            {
                return;
            }
            RoR2Content.Items.BoostDamage.hidden = true;
            RoR2Content.Items.BoostHp.hidden = true;
            if (!DevotionInventoryController.lowLevelEliteBuffs.Contains(DLC2Content.Elites.Aurelionite.eliteEquipmentDef.equipmentIndex))
            {
                if (DLC2Content.Elites.Aurelionite.IsAvailable())
                {
                    DevotionInventoryController.lowLevelEliteBuffs.Add(DLC2Content.Elites.Aurelionite.eliteEquipmentDef.equipmentIndex);
                    DevotionInventoryController.highLevelEliteBuffs.Add(DLC2Content.Elites.Aurelionite.eliteEquipmentDef.equipmentIndex);
                    DevotionInventoryController.highLevelEliteBuffs.Add(DLC2Content.Elites.Bead.eliteEquipmentDef.equipmentIndex);
                }
            }
        }
    }
}