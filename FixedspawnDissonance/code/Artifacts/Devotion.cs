using RoR2.Projectile;
using RoR2;
using RoR2.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Collections;

namespace FixedspawnDissonance
{
    public class Devotion
    {

        public static void Start()
        {
            On.RoR2.DevotionInventoryController.OnDevotionArtifactEnabled += FixMissingEliteTypes;
            //IL failed to work so I'll do this stupider version
            On.RoR2.DevotionInventoryController.OnDevotionArtifactDisabled += Fix_EliteListBeingBlank;

            On.RoR2.CharacterAI.LemurianEggController.CreateItemTakenOrb += Fix_NullrefWhenOrb;

            ////Issues with Nuxlars
            //When first ever Lem dies, doesn't start showing next Lem.
            //When first ever Lem dies, won't ever add new Lemurian Inventory
            //Ported from Nuxlar with Permission
            On.RoR2.UI.ScoreboardController.Rebuild += AddLemurianInventory;
            //On.DevotedLemurianController.Start += GetLemInventory;
            On.RoR2.DevotionInventoryController.UpdateAllMinions += FixEvoltionBeing1Behind;
            IL.RoR2.DevotionInventoryController.UpdateMinionInventory += FixOneBehindRemoveItemHere;
            //On.RoR2.DevotionInventoryController.UpdateMinionInventory += FixBodyComponentsSometimesJustNotBeingThere;

            
            On.RoR2.PickupPickerController.SetOptionsFromInteractor += VoidForLemurians;
            On.RoR2.DevotionInventoryController.DropScrapOnDeath += ScrapForVoids;


            GameObject Fireball = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/Base/Lemurian/Fireball.prefab").WaitForCompletion();
            GameObject LemurianBigFireball = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/Base/LemurianBruiser/LemurianBigFireball.prefab").WaitForCompletion();
            Fireball.GetComponent<ProjectileDamage>().damageType.damageSource = DamageSource.Primary;
            LemurianBigFireball.GetComponent<ProjectileDamage>().damageType.damageSource = DamageSource.Primary;


            On.RoR2.Util.HealthComponentToTransform += FixTwisteds_NotWorkingOnPlayers;

            On.DevotedLemurianController.Start += DevotedLemurianController_Start;
        }

        private static void FixOneBehindRemoveItemHere(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallvirt("RoR2.Inventory", "GiveItem")))
            {
                c.Prev.OpCode = OpCodes.Ldc_I4_0;
            }
            else
            {
                Debug.LogWarning("IL Failed: FixOneBehindRemoveItemHerel");
            }
        }

        private static void ScrapForVoids(On.RoR2.DevotionInventoryController.orig_DropScrapOnDeath orig, DevotionInventoryController self, ItemIndex devotionItem, CharacterBody minionBody)
        {
            orig(self, devotionItem, minionBody);
            PickupIndex pickupIndex = PickupIndex.none;
            ItemDef itemDef = ItemCatalog.GetItemDef(devotionItem);
            if (itemDef != null)
            {
                switch (itemDef.tier)
                {
                    case ItemTier.Lunar:
                        pickupIndex = PickupCatalog.FindPickupIndex(RoR2Content.MiscPickups.LunarCoin.miscPickupIndex);
                        break;
                    case ItemTier.VoidTier1:
                        pickupIndex = PickupCatalog.FindPickupIndex("ItemIndex.ScrapWhite");
                        break;
                    case ItemTier.VoidTier2:
                        pickupIndex = PickupCatalog.FindPickupIndex("ItemIndex.ScrapGreen");
                        break;
                    case ItemTier.VoidTier3:
                        pickupIndex = PickupCatalog.FindPickupIndex("ItemIndex.ScrapRed");
                        break;
                    case ItemTier.VoidBoss:
                        pickupIndex = PickupCatalog.FindPickupIndex("ItemIndex.ScrapYellow");
                        break;
                }
            }
            if (pickupIndex != PickupIndex.none)
            {
                PickupDropletController.CreatePickupDroplet(pickupIndex, minionBody.corePosition, Vector3.down * 15f);
            }
        }

        private static void DevotedLemurianController_Start(On.DevotedLemurianController.orig_Start orig, DevotedLemurianController self)
        {
            orig(self);
            self._leashDistSq = 12000f;
        }

        private static Transform FixTwisteds_NotWorkingOnPlayers(On.RoR2.Util.orig_HealthComponentToTransform orig, HealthComponent healthComponent)
        {
            if (healthComponent.body && healthComponent.body.mainHurtBox)
            {
                return healthComponent.body.mainHurtBox.transform;
            }
            return orig(healthComponent);
        }

      

        private static void FixBodyComponentsSometimesJustNotBeingThere(On.RoR2.DevotionInventoryController.orig_UpdateMinionInventory orig, DevotionInventoryController self, DevotedLemurianController devotedLemurianController, bool shouldEvolve)
        {
            orig(self,devotedLemurianController,shouldEvolve);
            if (devotedLemurianController.LemurianBody)
            {
                if (devotedLemurianController.LemurianBody.inventory)
                {
                    devotedLemurianController.LemurianBody.OnInventoryChanged();
                }
            }
        }

        private static void VoidForLemurians(On.RoR2.PickupPickerController.orig_SetOptionsFromInteractor orig, PickupPickerController self, Interactor activator)
        {
            bool egg = self.GetComponent<RoR2.CharacterAI.LemurianEggController>();
            if (egg)
            {
                if (WConfig.DevotionAllowVoids.Value)
                {
                    ItemTierCatalog.GetItemTierDef(ItemTier.VoidTier1).canScrap = true;
                    ItemTierCatalog.GetItemTierDef(ItemTier.VoidTier2).canScrap = true;
                    ItemTierCatalog.GetItemTierDef(ItemTier.VoidTier3).canScrap = true;
                }
                if (WConfig.DevotionAllowLunars.Value)
                {
                    ItemTierCatalog.GetItemTierDef(ItemTier.Lunar).canScrap = true;
                }
            }
            orig(self,activator);
            if (egg)
            {
                if (WConfig.DevotionAllowVoids.Value)
                {
                    ItemTierCatalog.GetItemTierDef(ItemTier.VoidTier1).canScrap = false;
                    ItemTierCatalog.GetItemTierDef(ItemTier.VoidTier2).canScrap = false;
                    ItemTierCatalog.GetItemTierDef(ItemTier.VoidTier3).canScrap = false;
                }
                if (WConfig.DevotionAllowLunars.Value)
                {
                    ItemTierCatalog.GetItemTierDef(ItemTier.Lunar).canScrap = false;
                }
            }
        }

        private static void FixEvoltionBeing1Behind(On.RoR2.DevotionInventoryController.orig_UpdateAllMinions orig, DevotionInventoryController self, bool shouldEvolve)
        {
            //Update the item count of all lemurians first, then copy it. Instead of copying lower counts.
            if (shouldEvolve)
            {
                if (self._summonerMaster)
                {
                    MinionOwnership.MinionGroup minionGroup = MinionOwnership.MinionGroup.FindGroup(self._summonerMaster.netId);
                    if (minionGroup != null)
                    {
                        foreach (MinionOwnership minionOwnership in minionGroup.members)
                        {
                            DevotedLemurianController devotedLemurianController;
                            if (minionOwnership && minionOwnership.GetComponent<CharacterMaster>().TryGetComponent<DevotedLemurianController>(out devotedLemurianController))
                            {
                                self._devotionMinionInventory.GiveItem(devotedLemurianController.DevotionItem, 1);
                            }
                        }
                    }
                }
            }          
            orig(self,shouldEvolve);
            self.StartCoroutine(FixItemComponentsBeingDeleted(self));
        }

        public static IEnumerator FixItemComponentsBeingDeleted(DevotionInventoryController self)
        {
            yield return new WaitForSeconds(1);
            if (self._summonerMaster)
            {
                MinionOwnership.MinionGroup minionGroup = MinionOwnership.MinionGroup.FindGroup(self._summonerMaster.netId);
                if (minionGroup != null)
                {
                    foreach (MinionOwnership minionOwnership in minionGroup.members)
                    {
                        DevotedLemurianController devotedLemurianController;
                        if (minionOwnership && minionOwnership.GetComponent<CharacterMaster>().TryGetComponent<DevotedLemurianController>(out devotedLemurianController))
                        {
                            if (devotedLemurianController.LemurianBody)
                            {
                                if (devotedLemurianController.LemurianBody.inventory)
                                {
                                    devotedLemurianController.LemurianBody.OnInventoryChanged();
                                }
                            }
                        }
                    }
                }
            }
        }


        private static void Fix_EliteListBeingBlank(On.RoR2.DevotionInventoryController.orig_OnDevotionArtifactDisabled orig, RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
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

        private static void Fix_NullrefWhenOrb(On.RoR2.CharacterAI.LemurianEggController.orig_CreateItemTakenOrb orig, RoR2.CharacterAI.LemurianEggController self, Vector3 effectOrigin, GameObject targetObject, ItemIndex itemIndex)
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
            if (WConfig.DevotionInventory.Value)
            {
                //Better this than checking Artifact
                if (DevotionInventoryController.InstanceList.Count > 0)
                {
                    List<CharacterMaster> list = new List<CharacterMaster>();
                    foreach (PlayerCharacterMasterController playerCharacterMasterController in PlayerCharacterMasterController.instances)
                    {
                        list.Add(playerCharacterMasterController.master);

                        if (WConfig.DevotionShowAllInventory.Value)
                        {
                            CharacterMaster summonerMaster = playerCharacterMasterController.master;
                            MinionOwnership.MinionGroup minionGroup = MinionOwnership.MinionGroup.FindGroup(summonerMaster.netId);
                            if (minionGroup != null)
                            {
                                foreach (MinionOwnership minionOwnership in minionGroup.members)
                                {
                                    Inventory devotedLemurianController;
                                    if (minionOwnership && minionOwnership.TryGetComponent<Inventory>(out devotedLemurianController))
                                    {
                                        if (devotedLemurianController.GetItemCount(CU8Content.Items.LemurianHarness) > 0)
                                        {
                                            list.Add(minionOwnership.GetComponent<CharacterMaster>());
                                            break;
                                        }                                        
                                    }
                                }
                            }
                        }
                    }
                    //If every player has their own Lem inventory why would we only display the local one
                    if (!WConfig.DevotionShowAllInventory.Value)
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