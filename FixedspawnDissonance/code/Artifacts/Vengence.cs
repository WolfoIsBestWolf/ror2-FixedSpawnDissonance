
using RoR2;
using RoR2.Artifacts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace VanillaArtifactsPlus
{
    public class Vengence
    {
        public static List<ItemDef> vengenceBlacklist;
        //public static List<ItemDef> limitTheseItemsDmg;
        //public static List<ItemDef> limitTheseItemsHpSpeed;

        public static void OnArtifactEnable()
        {
            On.RoR2.HealthComponent.Heal += Vengence.Umbra_HalfHealing;
            vengenceBlacklist = new List<ItemDef> 
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
            /*limitTheseItemsDmg = new List<ItemDef>
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
            };*/
            
        }
        public static void OnArtifactDisable()
        {
            On.RoR2.HealthComponent.Heal -= Vengence.Umbra_HalfHealing;
            vengenceBlacklist = null;
            //limitTheseItemsDmg = null;
            //limitTheseItemsHpSpeed = null;
        }

        public static void Start()
        {
    
            On.RoR2.Artifacts.DoppelgangerSpawnCard.FromMaster += VengenceMetamorphosisSynergy;

            if (WConfig.VengenceGoodDrop.Value == true)
            {
                DoppelgangerDropTable dtShadowClone = Addressables.LoadAssetAsync<DoppelgangerDropTable>(key: "RoR2/Base/ShadowClone/dtDoppelganger.asset").WaitForCompletion();

                dtShadowClone.canDropBeReplaced = false;
                dtShadowClone.tier1Weight = 0.1f;
                dtShadowClone.tier2Weight = 80;
                dtShadowClone.tier3Weight = 20;
                dtShadowClone.bossWeight = 20;
                dtShadowClone.lunarItemWeight = 0.1f;
                dtShadowClone.voidTier1Weight = 10f;
                dtShadowClone.voidTier2Weight = 30;
                dtShadowClone.voidTier3Weight = 10;
                dtShadowClone.voidBossWeight = 10;

                //Allows Dios to drop, I suppose same could maybe done for Elixirs 
                On.RoR2.DoppelgangerDropTable.GenerateWeightedSelection += DoppelGanger_AllowDiosDrop;
            }

        }
 
        //WHY DOES THIS NTO RUN??
        private static CharacterMaster srcCharacterMasterTEMP;
        public static void OnPreSpawnSetup(CharacterMaster master)
        {
            Debug.Log("Umbra of " + master.name);
            VenganceItemFilter(master.inventory);
         

            if (WConfig.VenganceHealthRebalance.Value == true)
            {
                master.onBodyStart += OnDoppelgangerBody;
                master.inventory.RemoveItemPermanent(RoR2Content.Items.UseAmbientLevel, master.inventory.GetItemCountPermanent(RoR2Content.Items.UseAmbientLevel));
                master.inventory.GiveItemPermanent(RoR2Content.Items.LevelBonus, (int)TeamManager.instance.GetTeamLevel(TeamIndex.Player));
                master.inventory.GiveItemPermanent(RoR2Content.Items.AdaptiveArmor);
            }

            if (WConfig.VengenceGoodDrop.Value == true)
            {
                List<ItemIndex> temporder = new List<ItemIndex>();
                temporder.AddRange(srcCharacterMasterTEMP.inventory.itemAcquisitionOrder);
                master.inventory.itemAcquisitionOrder = temporder;
                temporder.Remove(RoR2Content.Items.ExtraLifeConsumed.itemIndex);
                master.inventory.RemoveItemPermanent(RoR2Content.Items.ExtraLifeConsumed, 10000);
                temporder.Remove(DLC1Content.Items.ExtraLifeVoidConsumed.itemIndex);
                master.inventory.RemoveItemPermanent(DLC1Content.Items.ExtraLifeVoidConsumed, 10000);
                master.inventory.itemAcquisitionOrder = temporder;
            }
        }

        public static void OnDoppelgangerBody(CharacterBody body)
        {
            body.autoCalculateLevelStats = false;
            body.baseMaxHealth = (body.baseMaxHealth + 110) / 2; //Less Tanky Loader and MulT
            body.levelMaxHealth = (body.levelMaxHealth + 33) / 2;
            body.baseDamage = 12f;
            body.levelDamage = 2.4f;
            body.MarkAllStatsDirty();
        }

        private static void DoppelGanger_AllowDiosDrop(On.RoR2.DoppelgangerDropTable.orig_GenerateWeightedSelection orig, DoppelgangerDropTable self)
        {
            orig(self);
            if (self.doppelgangerInventory)
            {
                foreach (ItemIndex itemIndex in self.doppelgangerInventory.itemAcquisitionOrder)
                {
                    ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
                    if (itemDef)
                    {
                        if (itemDef == RoR2Content.Items.ExtraLifeConsumed)
                        {
                            PickupIndex pickupIndex = PickupCatalog.FindPickupIndex(RoR2Content.Items.ExtraLife.itemIndex);
                            self.selector.AddChoice(new UniquePickup(pickupIndex), self.tier3Weight);
                        }
                        else if (itemDef == DLC1Content.Items.ExtraLifeVoidConsumed)
                        {
                            PickupIndex pickupIndex = PickupCatalog.FindPickupIndex(DLC1Content.Items.ExtraLifeVoid.itemIndex);
                            self.selector.AddChoice(new UniquePickup(pickupIndex), self.voidTier3Weight);
                        }
                        else if (itemDef == DLC2Content.Items.LowerPricedChestsConsumed)
                        {
                            PickupIndex pickupIndex = PickupCatalog.FindPickupIndex(DLC2Content.Items.LowerPricedChests.itemIndex);
                            self.selector.AddChoice(new UniquePickup(pickupIndex), self.tier3Weight);
                        }
                        else if (itemDef == DLC2Content.Items.TeleportOnLowHealthConsumed)
                        {
                            PickupIndex pickupIndex = PickupCatalog.FindPickupIndex(DLC2Content.Items.TeleportOnLowHealth.itemIndex);
                            self.selector.AddChoice(new UniquePickup(pickupIndex), self.voidTier3Weight);
                        }
                    }          
                }
            }
        }

        private static DoppelgangerSpawnCard VengenceMetamorphosisSynergy(On.RoR2.Artifacts.DoppelgangerSpawnCard.orig_FromMaster orig, CharacterMaster srcCharacterMaster)
        {
            DoppelgangerSpawnCard tempspawncard = orig(srcCharacterMaster);
            try
            {
                if (WConfig.VengenceAlwaysRandom.Value || RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.RandomSurvivorOnRespawn))
                {
                    BodyIndex bodyIndex = BodyIndex.None;
                    SurvivorDef survivorDef = null;
                    do
                    {
                        int randomint = VanillaArtifactsMain.Random.Next(SurvivorCatalog.survivorCount);
                        bodyIndex = SurvivorCatalog.GetBodyIndexFromSurvivorIndex((SurvivorIndex)randomint);
                        survivorDef = SurvivorCatalog.GetSurvivorDef((SurvivorIndex)randomint);
                    }
                    while (survivorDef.hidden == true);
                    tempspawncard.prefab = MasterCatalog.GetMasterPrefab(MasterCatalog.FindAiMasterIndexForBody(bodyIndex));
                }
                tempspawncard.onPreSpawnSetup += OnPreSpawnSetup;
                srcCharacterMasterTEMP = srcCharacterMaster;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e); 
                return tempspawncard;
            }
            return tempspawncard;
        }

        public static float Umbra_HalfHealing(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask procChainMask, bool nonRegen)
        {
            if (self.itemCounts.invadingDoppelganger > 0)
            {
                amount /= 3;
            }
            return orig(self, amount, procChainMask, nonRegen);
        }

        private static void VenganceItemFilter(Inventory inventory)
        {
            try
            {
                for (int i = 0; i < inventory.itemAcquisitionOrder.Count; i++)
                {
                    ItemDef tempDef = ItemCatalog.GetItemDef(inventory.itemAcquisitionOrder[i]);
                    if (tempDef.ContainsTag(ItemTag.CannotCopy) || tempDef.ContainsTag(ItemTag.OnKillEffect) || tempDef.ContainsTag(ItemTag.AIBlacklist) || tempDef.ContainsTag(ItemTag.BrotherBlacklist))
                    {
                        inventory.RemoveItemPermanent(tempDef, inventory.GetItemCountPermanent(tempDef));
                    }
                }
                if (WConfig.VengenceBlacklist.Value == false)
                {
                    return;
                }
                //int itemLimitDefense = (1+Run.instance.loopClearCount)*2;
                foreach (ItemDef itemDef in vengenceBlacklist)
                {
                    inventory.RemoveItemPermanent(itemDef, inventory.GetItemCountPermanent(itemDef));
                }
                for (int i = 0; i < inventory.itemAcquisitionOrder.Count; i++)
                {
                    int itemCount = inventory.GetItemCountPermanent(inventory.itemAcquisitionOrder[i]);
                    if (itemCount > 1)
                    {
                        inventory.RemoveItemPermanent(inventory.itemAcquisitionOrder[i], itemCount / 2);
                    }
                }

                if (inventory.currentEquipmentIndex == RoR2Content.Equipment.BurnNearby.equipmentIndex)
                {
                    inventory.SetEquipmentIndex(EquipmentIndex.None, true);
                }
                else if (inventory.currentEquipmentIndex == DLC2Content.Equipment.HealAndRevive.equipmentIndex)
                {
                    inventory.SetEquipmentIndex(EquipmentIndex.None, true);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e);
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