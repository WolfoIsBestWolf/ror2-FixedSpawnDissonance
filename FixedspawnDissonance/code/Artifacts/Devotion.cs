using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Projectile;
using RoR2.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.Items;
 
using UnityEngine.Networking;

namespace VanillaArtifactsPlus
{
   /* public class DevotionEquipmentHolder
    {
        public EquipmentIndex equipmentIndex = EquipmentIndex.None;
    }*/
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
            if (WConfig.DevotionFlags.Value)
            {
                bodyL.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
                bodyE.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
                bodyL.bodyFlags |= CharacterBody.BodyFlags.ImmuneToLava;
                bodyE.bodyFlags |= CharacterBody.BodyFlags.ImmuneToLava;
                /*
                bodyL.bodyFlags |= CharacterBody.BodyFlags.ImmuneToVoidDeath;
                bodyE.bodyFlags |= CharacterBody.BodyFlags.ImmuneToVoidDeath;
                bodyL.bodyFlags |= CharacterBody.BodyFlags.ImmuneToExecutes;
                bodyE.bodyFlags |= CharacterBody.BodyFlags.ImmuneToExecutes;
                */
                IL.RoR2.FogDamageController.MyFixedUpdate += NoFogLemurianDamage;
            }
          
            bodyL.lavaCooldown = 2;
            bodyE.lavaCooldown = 2;


            //DevotedLemurian.GetComponent<CharacterBody>().bodyFlags |= CharacterBody.BodyFlags.Devotion; //They don't have this by default ???
            //DevotedLemurianElder.GetComponent<CharacterBody>().bodyFlags |= CharacterBody.BodyFlags.Devotion; //This somehow makes them get DroneWeapons ????
            #endregion
 


            GameObject DevotedMaster = Addressables.LoadAssetAsync<GameObject>(key: "90b219aa7b48e824384c5abd65c30f70").WaitForCompletion();
            var AI = DevotedMaster.GetComponents<AISkillDriver>();
            //0 DevotedSecondarySkill
            //1 StrafeAndShoot
            //2 StopAndShoot
            //3 ReturnToLeaderDefault
            //4 WaitNearLeaderDefault
            //5 StrafeNearbyEnemies
            //6 ReturnToOwnerLeash
            //7 Chase



            AI[0].minTargetHealthFraction = 0;
            AI[1].minTargetHealthFraction = 0;
            AI[2].minTargetHealthFraction = 0;
            AI[5].minTargetHealthFraction = 0;
            //AI[4].minDistance = 10; 
            AI[7].minTargetHealthFraction = 0;
            AI[7].minDistance = 30;
            AI[7].minUserHealthFraction = 40; //Dont chase at low health
            /*
            AISkillDriver returnToLeader = AI[3];
            AISkillDriver returnToLeaderMakeLast = DevotedMaster.AddComponent<AISkillDriver>();
            //returnToLeaderMakeLast.maxTargetHealthFraction = returnToLeader;
            returnToLeaderMakeLast.driverUpdateTimerOverride = 2;
            returnToLeaderMakeLast.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            returnToLeaderMakeLast.movementType = AISkillDriver.MovementType.FleeMoveTarget;
            returnToLeaderMakeLast.minDistance = float.NegativeInfinity;
            returnToLeaderMakeLast.maxDistance = 35;
            returnToLeaderMakeLast.resetCurrentEnemyOnNextDriverSelection = true;
            returnToLeaderMakeLast.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            returnToLeaderMakeLast.customName = "BackOffFromDead";



             returnToLeaderMakeLast.maxTargetHealthFraction = 0;
            returnToLeaderMakeLast.driverUpdateTimerOverride = 2;
            returnToLeaderMakeLast.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            returnToLeaderMakeLast.movementType = AISkillDriver.MovementType.FleeMoveTarget;
            returnToLeaderMakeLast.minDistance = float.NegativeInfinity;
            returnToLeaderMakeLast.maxDistance = 35;
            returnToLeaderMakeLast.resetCurrentEnemyOnNextDriverSelection = true;
            returnToLeaderMakeLast.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            returnToLeaderMakeLast.customName = "BackOffFromDead";*/

            On.DevotedLemurianController.OnDevotedBodyDead += RemoveVoidDiosLikeRegular;
 
            if (WConfig.DevotionVoidInfestor.Value)
            {
                IL.RoR2.HealthComponent.UpdateLastHitTime += ExpellVoidInfestorsAtLow;
            }
            
        }

        private static void ExpellVoidInfestorsAtLow(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.Before,
             x => x.MatchLdflda("RoR2.HealthComponent", "itemCounts"),
              x => x.MatchLdfld("RoR2.HealthComponent/ItemCounts", "fragileDamageBonus")
            ))
            {
                
                c.EmitDelegate<System.Func<HealthComponent, HealthComponent>>(self =>
                {
                    if (self.body.teamComponent.teamIndex == TeamIndex.Void)
                    {
                        if (self.combinedHealthFraction < 0.05f)
                        {   
                            if (self.body.master && self.body.master.TryGetComponent<DevotedLemurianController>(out var a))
                            {
                                if (a.DevotedEvolutionLevel == 1)
                                {
                                    a._devotionInventoryController.GenerateEliteBuff(self.body, a, true);
                                }
                                else if (a.DevotedEvolutionLevel >= 3)
                                {
                                    a._devotionInventoryController.GenerateEliteBuff(self.body, a, true);
                                }
                                else
                                {
                                    self.body.inventory.SetEquipmentIndex(EquipmentIndex.None);
                                }
                                BaseAI component = self.body.master.GetComponent<BaseAI>();
                                if (component)
                                {
                                    component.currentEnemy.Reset();
                                }
                                self.Networkhealth = self.fullCombinedHealth * 0.05f;
                                self.body.master.teamIndex = TeamIndex.Player;
                                self.body.teamComponent.teamIndex = TeamIndex.Player;
                                EffectManager.SimpleImpactEffect(EntityStates.VoidInfestor.Infest.successfulInfestEffectPrefab, self.transform.position, Vector3.up, false);

                                Vector3 position3 = self.body.corePosition;
                                GameObject gameObject5 = UnityEngine.Object.Instantiate<GameObject>(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/EliteVoid/VoidInfestorMaster.prefab").WaitForCompletion(), position3, Quaternion.identity);
                                CharacterMaster component5 = gameObject5.GetComponent<CharacterMaster>();
                                if (component5)
                                {
                                    component5.teamIndex = TeamIndex.Void;
                                    NetworkServer.Spawn(gameObject5);
                                    component5.SpawnBodyHere();
                                }
                            }
                        }

                    }
                    return self;
                });
            }
            else
            {
                Debug.LogWarning("IL Failed : IL.RoR2.FogDamageController.FixedUpdate");
            }
        }

 

        private static void RemoveVoidDiosLikeRegular(On.DevotedLemurianController.orig_OnDevotedBodyDead orig, DevotedLemurianController self)
        {
            if (self._devotionInventoryController.HasItem(DLC1Content.Items.ExtraLifeVoid))
            {
                self._devotionInventoryController.RemoveItem(DLC1Content.Items.ExtraLifeVoid.itemIndex, 1);
                /*foreach (ContagiousItemManager.TransformationInfo transformationInfo in ContagiousItemManager.transformationInfos)
                {
                    ContagiousItemManager.TryForceReplacement(self._devotionInventoryController._devotionMinionInventory, transformationInfo.originalItem);
                }*/
                return;
            }
            orig(self);
            
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
                        //This would be funny
                        //But not really like Server-Side because they're untiered in vanilla
                        //pickupIndex = PickupCatalog.FindPickupIndex("ItemIndex.ScrapWhiteSuppressed");
                        break;
                    case ItemTier.VoidTier2:
                        pickupIndex = PickupCatalog.FindPickupIndex("ItemIndex.ScrapGreen");
                        //pickupIndex = PickupCatalog.FindPickupIndex("ItemIndex.ScrapGreenSuppressed");
                        break;
                    case ItemTier.VoidTier3:
                        pickupIndex = PickupCatalog.FindPickupIndex("ItemIndex.ScrapRed");
                        //pickupIndex = PickupCatalog.FindPickupIndex("ItemIndex.ScrapRedSuppressed");
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
            self._leashDistSq = 18000f;
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