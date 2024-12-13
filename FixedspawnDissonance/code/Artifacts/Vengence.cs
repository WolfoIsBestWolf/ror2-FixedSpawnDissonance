
using RoR2;
using RoR2.Artifacts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace FixedspawnDissonance
{
    public class Vengence
    {
        public static List<ItemDef> removeAllItems;
        public static List<ItemDef> limitTheseItemsDmg;
        public static List<ItemDef> limitTheseItemsHpSpeed;

        public static void OnArtifactEnable()
        {
            On.RoR2.HealthComponent.Heal += Vengence.Umbra_HalfHealing;
            removeAllItems = new List<ItemDef> 
            {
                RoR2Content.Items.BoostDamage,
                RoR2Content.Items.BoostHp,
                RoR2Content.Items.NovaOnHeal,
                RoR2Content.Items.ExtraLife,
                RoR2Content.Items.CaptainDefenseMatrix,
                RoR2Content.Items.LunarDagger,
                DLC1Content.Items.ExtraLifeVoid,
                DLC1Content.Items.HealingPotion,
            };
            limitTheseItemsDmg = new List<ItemDef>
            {
                RoR2Content.Items.CritGlasses,
                RoR2Content.Items.BleedOnHit,
                RoR2Content.Items.Crowbar,
                RoR2Content.Items.StickyBomb,
                RoR2Content.Items.NearbyDamageBonus,
                RoR2Content.Items.Missile,
                RoR2Content.Items.IceRing,
                RoR2Content.Items.FireRing,
                RoR2Content.Items.Behemoth,
                RoR2Content.Items.Clover,
                RoR2Content.Items.FireballsOnHit,
                RoR2Content.Items.LightningStrikeOnHit,
                RoR2Content.Items.SprintWisp,
                RoR2Content.Items.NovaOnLowHealth,
                DLC1Content.Items.FragileDamageBonus,
                DLC1Content.Items.StrengthenBurn,
                DLC1Content.Items.PrimarySkillShuriken,
                DLC1Content.Items.FragileDamageBonus,
                DLC1Content.Items.CritDamage,
                DLC1Content.Items.MoreMissile,
                DLC1Content.Items.PermanentDebuffOnHit,
                DLC1Content.Items.BleedOnHitVoid,
                DLC1Content.Items.ExplodeOnDeathVoid,
                DLC1Content.Items.ElementalRingVoid,
                DLC1Content.Items.MissileVoid,
                DLC1Content.Items.ChainLightningVoid,
                DLC2Content.Items.MeteorAttackOnHighDamage,
            };
            limitTheseItemsHpSpeed = new List<ItemDef>
            {
                RoR2Content.Items.Hoof,
                RoR2Content.Items.Mushroom,
                RoR2Content.Items.PersonalShield,
                RoR2Content.Items.SprintBonus,
                RoR2Content.Items.ArmorPlate,
                RoR2Content.Items.SprintOutOfCombat,
                RoR2Content.Items.Pearl,
                RoR2Content.Items.ShinyPearl,
                RoR2Content.Items.ShieldOnly,
                RoR2Content.Items.IncreaseHealing,
                RoR2Content.Items.Medkit,
                RoR2Content.Items.SprintArmor,       
                DLC1Content.Items.OutOfCombatArmor,
                DLC1Content.Items.AttackSpeedAndMoveSpeed,
                DLC1Content.Items.ImmuneToDebuff,
                DLC1Content.Items.MushroomVoid,
                DLC1Content.Items.LunarSun,
            };
            
        }
        public static void OnArtifactDisable()
        {
            On.RoR2.HealthComponent.Heal -= Vengence.Umbra_HalfHealing;
        }

        public static void Start()
        {
            On.RoR2.Artifacts.DoppelgangerInvasionManager.CreateDoppelganger += DoppelgangerInvasionManager_CreateDoppelganger;

            On.RoR2.Artifacts.DoppelgangerSpawnCard.FromMaster += VengenceMetamorphosisSynergy;

            if (WConfig.VengenceGoodDrop.Value == true)
            {
                DoppelgangerDropTable dtShadowClone = Addressables.LoadAssetAsync<DoppelgangerDropTable>(key: "RoR2/Base/ShadowClone/dtDoppelganger.asset").WaitForCompletion();

                dtShadowClone.canDropBeReplaced = false;
                dtShadowClone.tier1Weight = 0.1f;
                dtShadowClone.tier2Weight = 60;
                dtShadowClone.tier3Weight = 30;
                dtShadowClone.bossWeight = 30;
                dtShadowClone.lunarItemWeight = 0.1f;
                dtShadowClone.voidTier1Weight = 0.1f;
                dtShadowClone.voidTier2Weight = 40;
                dtShadowClone.voidTier3Weight = 20;
                dtShadowClone.voidBossWeight = 30;

                //Allows Dios to drop, I suppose same could maybe done for Elixirs 
                On.RoR2.DoppelgangerDropTable.GenerateWeightedSelection += DoppelGanger_AllowDiosDrop;
            }

        }

        private static void DoppelGanger_AllowDiosDrop(On.RoR2.DoppelgangerDropTable.orig_GenerateWeightedSelection orig, DoppelgangerDropTable self)
        {
            orig(self);
            if (self.doppelgangerInventory)
            {
                foreach (ItemIndex itemIndex in self.doppelgangerInventory.itemAcquisitionOrder)
                {
                    ItemIndex foruseItemindex = itemIndex;
                    ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);

                    if (itemDef == RoR2Content.Items.ExtraLifeConsumed)
                    {
                        itemDef = RoR2Content.Items.ExtraLife;
                        foruseItemindex = RoR2Content.Items.ExtraLife.itemIndex;

                        PickupIndex pickupIndex = PickupCatalog.FindPickupIndex(foruseItemindex);
                        self.selector.AddChoice(pickupIndex, self.tier3Weight);
                    }
                    else if (itemDef == DLC1Content.Items.ExtraLifeVoidConsumed)
                    {
                        itemDef = DLC1Content.Items.ExtraLifeVoid;
                        foruseItemindex = DLC1Content.Items.ExtraLifeVoid.itemIndex;

                        PickupIndex pickupIndex = PickupCatalog.FindPickupIndex(foruseItemindex);
                        self.selector.AddChoice(pickupIndex, self.voidTier3Weight);
                    }
                }
            }
        }

        private static DoppelgangerSpawnCard VengenceMetamorphosisSynergy(On.RoR2.Artifacts.DoppelgangerSpawnCard.orig_FromMaster orig, CharacterMaster srcCharacterMaster)
        {
            DoppelgangerSpawnCard tempspawncard = orig(srcCharacterMaster);
            if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.RandomSurvivorOnRespawn))
            {
                BodyIndex bodyIndex = BodyIndex.None;
                SurvivorDef survivorDef = null;
                do
                {
                    int randomint = Main.Random.Next(SurvivorCatalog.survivorCount);
                    bodyIndex = SurvivorCatalog.GetBodyIndexFromSurvivorIndex((SurvivorIndex)randomint);
                    survivorDef = SurvivorCatalog.GetSurvivorDef((SurvivorIndex)randomint);
                }
                while (survivorDef.hidden == true);
                tempspawncard.prefab = MasterCatalog.GetMasterPrefab(MasterCatalog.FindAiMasterIndexForBody(bodyIndex));
            }
            return tempspawncard;
        }

        public static float Umbra_HalfHealing(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask procChainMask, bool nonRegen)
        {
            if (self.itemCounts.invadingDoppelganger > 0)
            {
                amount /= 2;
            }
            return orig(self, amount, procChainMask, nonRegen);
        }

        private static void VenganceItemFilter(Inventory inventory)
        {
            int itemLimitDefense = (1+Run.instance.loopClearCount)*2;
            foreach (ItemDef itemDef in removeAllItems)
            {
                inventory.RemoveItem(itemDef, inventory.GetItemCount(itemDef));
            }
            foreach (ItemDef itemDef in limitTheseItemsDmg)
            {
                int itemCount = inventory.GetItemCount(itemDef);
                if (itemCount > 1)
                {
                    inventory.RemoveItem(itemDef, itemCount / 2);
                }
            }
            foreach (ItemDef itemDef in limitTheseItemsHpSpeed)
            {
                int itemCount = inventory.GetItemCount(itemDef);
                if (itemCount > itemLimitDefense)
                {
                    inventory.RemoveItem(itemDef, itemCount - itemLimitDefense);
                }
            }

            for (int i = 0; i < inventory.itemAcquisitionOrder.Count; i++)
            {
                ItemDef tempDef = ItemCatalog.GetItemDef(inventory.itemAcquisitionOrder[i]);
                if (tempDef.ContainsTag(ItemTag.CannotCopy) || tempDef.ContainsTag(ItemTag.OnKillEffect) || tempDef.ContainsTag(ItemTag.AIBlacklist) || tempDef.ContainsTag(ItemTag.BrotherBlacklist))
                {
                    inventory.RemoveItem(tempDef, inventory.GetItemCount(tempDef));
                }
            }


            if (inventory.currentEquipmentIndex == RoR2Content.Equipment.BurnNearby.equipmentIndex)
            {
                inventory.SetEquipmentIndex(EquipmentIndex.None);
            }
            else if (inventory.currentEquipmentIndex == DLC2Content.Equipment.HealAndRevive.equipmentIndex)
            {
                inventory.SetEquipmentIndex(EquipmentIndex.None);
            }
        }

        public static void DoppelgangerInvasionManager_CreateDoppelganger(On.RoR2.Artifacts.DoppelgangerInvasionManager.orig_CreateDoppelganger orig, CharacterMaster srcCharacterMaster, Xoroshiro128Plus rng)
        {
            orig(srcCharacterMaster, rng);      
                CombatSquad[] bossgrouplist2 = Object.FindObjectsOfType(typeof(CombatSquad)) as CombatSquad[];  
                for (var i = 0; i < bossgrouplist2.Length; i++)
                {
                    //Debug.LogWarning(bossgrouplist2[i]);
                    if (bossgrouplist2[i].name.Equals("ShadowCloneEncounter(Clone)"))
                    {
                        //Debug.Log("Shadow Encounter");
                        bossgrouplist2[i].name = "ShadowCloneEncounterAltered";
                        List<CharacterMaster> clonelist = bossgrouplist2[i].membersList;

                        for (var j = 0; j < 1; j++)
                        {
                            Debug.Log("Umbra of " + clonelist[j].name);
                            if (WConfig.VengenceBlacklist.Value == true)
                            {
                                VenganceItemFilter(clonelist[j].inventory);
                                clonelist[j].inventory.GiveItem(RoR2Content.Items.UseAmbientLevel);
                            }

                            if (WConfig.VenganceHealthRebalance.Value == true)
                            {
                                CharacterBody tempbody = clonelist[j].GetBody();

                                tempbody.autoCalculateLevelStats = false;
                                tempbody.baseMaxHealth = 80f;
                                tempbody.levelMaxHealth = 24f;
                                tempbody.baseDamage = 12f;
                                tempbody.levelDamage = 2.4f;
                                tempbody.MarkAllStatsDirty();
                                clonelist[j].inventory.GiveItem(RoR2Content.Items.LevelBonus, 1);
                                clonelist[j].inventory.GiveItem(RoR2Content.Items.AdaptiveArmor);
                            }

                            if (WConfig.VengenceGoodDrop.Value == true)
                            {
                                List<ItemIndex> temporder = new List<ItemIndex>();
                                temporder.AddRange(srcCharacterMaster.inventory.itemAcquisitionOrder);
                                clonelist[j].inventory.itemAcquisitionOrder = temporder;
                                temporder.Remove(RoR2Content.Items.ExtraLifeConsumed.itemIndex);
                                clonelist[j].inventory.RemoveItem(RoR2Content.Items.ExtraLifeConsumed, 10000);
                                temporder.Remove(DLC1Content.Items.ExtraLifeVoidConsumed.itemIndex);
                                clonelist[j].inventory.RemoveItem(DLC1Content.Items.ExtraLifeVoidConsumed, 10000);
                                clonelist[j].inventory.itemAcquisitionOrder = temporder;
                            }

                        }
                    }
                }
           
        }



        public static void EnableEquipmentForVengence()
        {
            for (int i = 0; i < SurvivorCatalog.survivorDefs.Length; i++)
            {
                if (SurvivorCatalog.survivorDefs[i].bodyPrefab)
                {
                    BodyIndex index = SurvivorCatalog.survivorDefs[i].bodyPrefab.GetComponent<CharacterBody>().bodyIndex;
                    GameObject Master = MasterCatalog.GetMasterPrefab(MasterCatalog.FindAiMasterIndexForBody(index));
                    if (Master)
                    {
                        RoR2.CharacterAI.AISkillDriver[] skilllist = Master.GetComponents<RoR2.CharacterAI.AISkillDriver>();
                        //Debug.Log(Master);
                        for (int JJ = 0; JJ < skilllist.Length; JJ++)
                        {
                            if (skilllist[JJ].skillSlot != SkillSlot.None)
                            {
                                skilllist[JJ].shouldFireEquipment = true;
                            }
                        }
                    }

                }
            }
        }


    }
}