using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using RoR2.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace VanillaArtifactsPlus
{
    public class Devotion
    {

        public static void Start()
        {
            On.RoR2.UI.ScoreboardController.Rebuild += AddLemurianInventory;

            
            On.RoR2.PickupPickerController.SetOptionsFromInteractor += VoidForLemurians;
            On.RoR2.DevotionInventoryController.DropScrapOnDeath += ScrapForVoids;
            On.DevotedLemurianController.Start += TeleportMoreOften;

            #region Lum & Breaching fix
            GameObject Fireball = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/Base/Lemurian/Fireball.prefab").WaitForCompletion();
            GameObject LemurianBigFireball = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/Base/LemurianBruiser/LemurianBigFireball.prefab").WaitForCompletion();
            Fireball.GetComponent<ProjectileDamage>().damageType.damageSource = DamageSource.Primary;
            LemurianBigFireball.GetComponent<ProjectileDamage>().damageType.damageSource = DamageSource.Primary;
            #endregion

            #region Body Tags
            GameObject DevotedLemurian = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/CU8/DevotedLemurianBody.prefab").WaitForCompletion();
            GameObject DevotedLemurianElder = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/CU8/DevotedLemurianBruiserBody.prefab").WaitForCompletion();
            CharacterBody bodyL = DevotedLemurian.GetComponent<CharacterBody>();
            CharacterBody bodyE = DevotedLemurianElder.GetComponent<CharacterBody>();
            bodyL.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            bodyE.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            bodyL.bodyFlags |= CharacterBody.BodyFlags.ImmuneToLava;
            bodyE.bodyFlags |= CharacterBody.BodyFlags.ImmuneToLava;
            bodyL.bodyFlags |= CharacterBody.BodyFlags.ImmuneToExecutes;
            bodyE.bodyFlags |= CharacterBody.BodyFlags.ImmuneToExecutes;

            //DevotedLemurian.GetComponent<CharacterBody>().bodyFlags |= CharacterBody.BodyFlags.Devotion; //They don't have this by default ???
            //DevotedLemurianElder.GetComponent<CharacterBody>().bodyFlags |= CharacterBody.BodyFlags.Devotion; //This somehow makes them get DroneWeapons ????
            #endregion
            IL.RoR2.FogDamageController.MyFixedUpdate += NoFogLemurianDamage;
  
        }

  
        private static void NoFogLemurianDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.Before,
             x => x.MatchLdcR4(0.5f),
             x => x.MatchMul()
            ))
            {
                c.Next.Operand = 0.00f;
            }
            else
            {
                Debug.LogWarning("IL Failed : IL.RoR2.FogDamageController.FixedUpdate");
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

        private static void TeleportMoreOften(On.DevotedLemurianController.orig_Start orig, DevotedLemurianController self)
        {
            orig(self);
            self._leashDistSq = 16000f;
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
   
        private static void AddLemurianInventory(On.RoR2.UI.ScoreboardController.orig_Rebuild orig, ScoreboardController self)
        {
            orig(self);
            //Basically overwrites vanilla one entirely hopefully not an issue
            if (WConfig.DevotionInventory.Value)
            {
                //Better this than checking Artifact
                //Because you might have Lems after the Artifact is disabled.
                if (DevotionInventoryController.InstanceList.Count > 0)
                {
                    List<CharacterMaster> list = new List<CharacterMaster>();
                    foreach (PlayerCharacterMasterController playerCharacterMasterController in PlayerCharacterMasterController.instances)
                    {
                        if (playerCharacterMasterController.isConnected)
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


         
    }
}