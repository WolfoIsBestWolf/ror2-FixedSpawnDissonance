using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace FixedspawnDissonance
{
    public class Vengence
    {
        public static void Start()
        {
            On.RoR2.Artifacts.DoppelgangerInvasionManager.CreateDoppelganger += DoppelgangerInvasionManager_CreateDoppelganger;


            //
            On.RoR2.MasterCopySpawnCard.FromMaster += (orig, srcCharacterMaster, copyItems, copyEquipment, onPreSpawnSetup) =>
            {
                MasterCopySpawnCard temp = orig(srcCharacterMaster, copyItems, copyEquipment, onPreSpawnSetup);
                if (srcCharacterMaster && srcCharacterMaster.inventory && srcCharacterMaster.inventory.GetItemCount(RoR2Content.Items.InvadingDoppelganger) > 0)
                {
                    temp.GiveItem(RoR2Content.Items.InvadingDoppelganger);
                }
                return temp;
            };




            //Vengence + Metamorphosis for random Foe
            On.RoR2.Artifacts.DoppelgangerSpawnCard.FromMaster += (orig, srcCharacterMaster) =>
            {
                RoR2.Artifacts.DoppelgangerSpawnCard tempspawncard = orig(srcCharacterMaster);

                if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.RandomSurvivorOnRespawn))
                {
                    //tempspawncard.prefab;
                    BodyIndex bodyIndex = BodyIndex.None;
                    SurvivorDef survivorDef = null;
                    do
                    {
                        int randomint = Main.Random.Next(SurvivorCatalog.survivorCount);
                        bodyIndex = SurvivorCatalog.GetBodyIndexFromSurvivorIndex((SurvivorIndex)randomint);
                        survivorDef = SurvivorCatalog.GetSurvivorDef((SurvivorIndex)randomint);
                        //Debug.LogWarning(survivorDef);
                    }
                    while (survivorDef.hidden == true);
                    //Debug.LogWarning(SurvivorCatalog.survivorCount);
                    //Debug.LogWarning(survivorDef);

                    tempspawncard.prefab = MasterCatalog.GetMasterPrefab(MasterCatalog.FindAiMasterIndexForBody(bodyIndex));
                }

                return tempspawncard;
            };


            if (WConfig.VengenceGoodDrop.Value == true)
            {
                RoR2.DoppelgangerDropTable dtShadowClone = Addressables.LoadAssetAsync<RoR2.DoppelgangerDropTable>(key: "RoR2/Base/ShadowClone/dtDoppelganger.asset").WaitForCompletion();

                dtShadowClone.canDropBeReplaced = false;
                dtShadowClone.tier1Weight = 0.1f;
                dtShadowClone.tier2Weight = 60;
                dtShadowClone.tier3Weight = 30;
                dtShadowClone.bossWeight = 50;
                dtShadowClone.lunarItemWeight = 0.1f;
                dtShadowClone.voidTier1Weight = 0.1f;
                dtShadowClone.voidTier2Weight = 40;
                dtShadowClone.voidTier3Weight = 20;
                dtShadowClone.voidBossWeight = 15;

                //Allows Dios to drop, I suppose same could maybe done for Elixirs 
                On.RoR2.DoppelgangerDropTable.GenerateWeightedSelection += (orig, self) =>
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
                };


            }

        }

        public static float UmbraHealHalf(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask procChainMask, bool nonRegen)
        {
            if (self.itemCounts.invadingDoppelganger > 0)
            {
                amount /= 2;
            }
            return orig(self, amount, procChainMask, nonRegen);
        }

        private static void VenganceItemFilter(Inventory inventory)
        {
            //Remove all OnKill, Minion items
            for (int i = 0; i < inventory.itemAcquisitionOrder.Count; i++)
            {
                ItemDef tempDef = ItemCatalog.GetItemDef(inventory.itemAcquisitionOrder[i]);
                //Debug.Log("Filter : "+tempDef.name);
                if (tempDef.ContainsTag(ItemTag.CannotCopy) || tempDef.ContainsTag(ItemTag.OnKillEffect) || tempDef.ContainsTag(ItemTag.AIBlacklist))
                {
                    //Debug.Log("Remove : " + tempDef.name);
                    inventory.RemoveItem(tempDef, inventory.GetItemCount(tempDef));
                }

            }
        }

        public static void DoppelgangerInvasionManager_CreateDoppelganger(On.RoR2.Artifacts.DoppelgangerInvasionManager.orig_CreateDoppelganger orig, CharacterMaster srcCharacterMaster, Xoroshiro128Plus rng)
        {
            orig(srcCharacterMaster, rng);

            if (NetworkServer.active)
            {
                CombatSquad[] bossgrouplist2 = UnityEngine.Object.FindObjectsOfType(typeof(CombatSquad)) as CombatSquad[];

                //Inventory MonsterTeamGainItemRandom = RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.monsterTeamInventory;

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

                                List<ItemIndex> temporder = new List<ItemIndex>();
                                temporder.AddRange(srcCharacterMaster.inventory.itemAcquisitionOrder);
                                clonelist[j].inventory.itemAcquisitionOrder = temporder;
                                //This for the most part works but I have no idea why it's so fucky sometimes
                                VenganceItemFilter(clonelist[j].inventory);
                                clonelist[j].inventory.GiveItem(RoR2Content.Items.UseAmbientLevel);


                                //In case they get it from somewhere for some reason
                                clonelist[j].inventory.RemoveItem(RoR2Content.Items.BoostDamage, clonelist[j].inventory.GetItemCount(RoR2Content.Items.BoostDamage));
                                clonelist[j].inventory.RemoveItem(RoR2Content.Items.BoostHp, clonelist[j].inventory.GetItemCount(RoR2Content.Items.BoostHp));

                                //RemoveAll
                                clonelist[j].inventory.RemoveItem(RoR2Content.Items.NovaOnHeal, clonelist[j].inventory.GetItemCount(RoR2Content.Items.NovaOnHeal));
                                clonelist[j].inventory.RemoveItem(RoR2Content.Items.Feather, clonelist[j].inventory.GetItemCount(RoR2Content.Items.Feather));

                                clonelist[j].inventory.RemoveItem(RoR2Content.Items.ExtraLife, clonelist[j].inventory.GetItemCount(RoR2Content.Items.ExtraLife));
                                clonelist[j].inventory.RemoveItem(DLC1Content.Items.ExtraLifeVoid, clonelist[j].inventory.GetItemCount(DLC1Content.Items.ExtraLifeVoid));
                                clonelist[j].inventory.RemoveItem(DLC1Content.Items.HealingPotion, clonelist[j].inventory.GetItemCount(DLC1Content.Items.HealingPotion));

                                clonelist[j].inventory.RemoveItem(RoR2Content.Items.CaptainDefenseMatrix, clonelist[j].inventory.GetItemCount(RoR2Content.Items.CaptainDefenseMatrix));

                                //Bears
                                clonelist[j].inventory.RemoveItem(RoR2Content.Items.Bear, clonelist[j].inventory.GetItemCount(RoR2Content.Items.Bear));
                                clonelist[j].inventory.RemoveItem(DLC1Content.Items.BearVoid, clonelist[j].inventory.GetItemCount(DLC1Content.Items.BearVoid));


                                int loops = Run.instance.loopClearCount;
                                int scalingitemlimit = Run.instance.loopClearCount * 1 + 1;

                                //Offensive items 2
                                int CritGlasses = clonelist[j].inventory.GetItemCount(RoR2Content.Items.CritGlasses);
                                if (CritGlasses > 1) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.CritGlasses, CritGlasses - 1); }

                                int BleedOnHit = clonelist[j].inventory.GetItemCount(RoR2Content.Items.BleedOnHit);
                                if (BleedOnHit > 1) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.BleedOnHit, BleedOnHit - 1); }





                                //Defensive items
                                int ArmorPlate = clonelist[j].inventory.GetItemCount(RoR2Content.Items.ArmorPlate);
                                int SprintArmor = clonelist[j].inventory.GetItemCount(RoR2Content.Items.SprintArmor);
                                int ShieldOnly = clonelist[j].inventory.GetItemCount(RoR2Content.Items.ShieldOnly);
                                int IncreaseHealing = clonelist[j].inventory.GetItemCount(RoR2Content.Items.IncreaseHealing);
                                int PersonalShield = clonelist[j].inventory.GetItemCount(RoR2Content.Items.PersonalShield);
                                int Pearl = clonelist[j].inventory.GetItemCount(RoR2Content.Items.Pearl);
                                int ShinyPearl = clonelist[j].inventory.GetItemCount(RoR2Content.Items.ShinyPearl);
                                int Medkit = clonelist[j].inventory.GetItemCount(RoR2Content.Items.Medkit);
                                int Mushroom = clonelist[j].inventory.GetItemCount(RoR2Content.Items.Mushroom);
                                int ParentEgg = clonelist[j].inventory.GetItemCount(RoR2Content.Items.ParentEgg);
                                int RepeatHeal = clonelist[j].inventory.GetItemCount(RoR2Content.Items.RepeatHeal);
                                int BarrierOnOverHeal = clonelist[j].inventory.GetItemCount(RoR2Content.Items.BarrierOnOverHeal);

                                //Speed 
                                int Hoof = clonelist[j].inventory.GetItemCount(RoR2Content.Items.Hoof);
                                int SprintBonus = clonelist[j].inventory.GetItemCount(RoR2Content.Items.SprintBonus);
                                int SprintOutOfCombat = clonelist[j].inventory.GetItemCount(RoR2Content.Items.SprintOutOfCombat);

                                if (Hoof > 4) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.Hoof, Hoof - 6); }
                                if (SprintBonus > 2) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.SprintBonus, SprintBonus - 3); }
                                if (SprintOutOfCombat > 2) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.SprintOutOfCombat, SprintOutOfCombat - 3); }



                                //Offensive Stacking Damge items
                                int FireRing = clonelist[j].inventory.GetItemCount(RoR2Content.Items.FireRing);
                                int IceRing = clonelist[j].inventory.GetItemCount(RoR2Content.Items.IceRing);
                                int Missile = clonelist[j].inventory.GetItemCount(RoR2Content.Items.Missile);
                                int FireballsOnHit = clonelist[j].inventory.GetItemCount(RoR2Content.Items.FireballsOnHit);
                                int LightningStrikeOnHit = clonelist[j].inventory.GetItemCount(RoR2Content.Items.LightningStrikeOnHit);
                                int Clover = clonelist[j].inventory.GetItemCount(RoR2Content.Items.Clover);
                                int NovaOnLowHealth = clonelist[j].inventory.GetItemCount(RoR2Content.Items.NovaOnLowHealth);
                                int NearbyDamageBonus = clonelist[j].inventory.GetItemCount(RoR2Content.Items.NearbyDamageBonus);
                                int StickyBomb = clonelist[j].inventory.GetItemCount(RoR2Content.Items.StickyBomb);
                                int BleedOnHitAndExplode = clonelist[j].inventory.GetItemCount(RoR2Content.Items.BleedOnHitAndExplode);

                                //Auto Play Items
                                int SprintWisp = clonelist[j].inventory.GetItemCount(RoR2Content.Items.SprintWisp);

                                int MushroomVoid = clonelist[j].inventory.GetItemCount(DLC1Content.Items.MushroomVoid);

                                int PrimarySkillShuriken = clonelist[j].inventory.GetItemCount(DLC1Content.Items.PrimarySkillShuriken);
                                int MissileVoid = clonelist[j].inventory.GetItemCount(DLC1Content.Items.MissileVoid);
                                int ChainLightningVoid = clonelist[j].inventory.GetItemCount(DLC1Content.Items.ChainLightningVoid);
                                int ElementalRingVoid = clonelist[j].inventory.GetItemCount(DLC1Content.Items.ElementalRingVoid);

                                int ExplodeOnDeathVoid = clonelist[j].inventory.GetItemCount(DLC1Content.Items.ExplodeOnDeathVoid);

                                int LunarSun = clonelist[j].inventory.GetItemCount(DLC1Content.Items.LunarSun);

                                int bonushp = 0;



                                if (MushroomVoid > 1) { clonelist[j].inventory.RemoveItem(DLC1Content.Items.MushroomVoid, MushroomVoid - 1); }
                                if (Mushroom > 1) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.Mushroom, Mushroom - 1); }

                                if (LunarSun > 2) { clonelist[j].inventory.RemoveItem(DLC1Content.Items.LunarSun, LunarSun / 2); }


                                if (PrimarySkillShuriken > scalingitemlimit) { clonelist[j].inventory.RemoveItem(DLC1Content.Items.PrimarySkillShuriken, (PrimarySkillShuriken - scalingitemlimit - ((PrimarySkillShuriken - scalingitemlimit) / 2))); }

                                //Debug.LogWarning(scalingitemlimit);
                                //Debug.LogWarning(scalingitemlimitbig);

                                if (ArmorPlate > scalingitemlimit) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.ArmorPlate, (ArmorPlate - scalingitemlimit - ((ArmorPlate - scalingitemlimit) / 2))); }
                                if (BarrierOnOverHeal > 1) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.BarrierOnOverHeal, BarrierOnOverHeal - 1); }
                                if (SprintArmor > 1) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.SprintArmor, SprintArmor - 1); }
                                //if (ShieldOnly > 0) { clonelist[j].inventory.GiveItem(RoR2Content.Items.CutHp); clonelist[j].inventory.GiveItem(RoR2Content.Items.BoostHp); }
                                if (ShieldOnly > scalingitemlimit) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.ShieldOnly, (ShieldOnly - scalingitemlimit - ((ShieldOnly - scalingitemlimit) / 2))); }
                                if (IncreaseHealing > loops) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.IncreaseHealing, (IncreaseHealing - loops)); }
                                if (PersonalShield > scalingitemlimit) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.PersonalShield, (PersonalShield - scalingitemlimit - ((PersonalShield - scalingitemlimit) / 2))); }
                                if (Pearl > scalingitemlimit) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.Pearl, (Pearl - scalingitemlimit - ((Pearl - scalingitemlimit) / 2))); }
                                if (ShinyPearl > scalingitemlimit) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.ShinyPearl, (ShinyPearl - scalingitemlimit - ((ShinyPearl - scalingitemlimit) / 2))); }
                                if (Medkit > scalingitemlimit) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.Medkit, (Medkit - scalingitemlimit - ((Medkit - scalingitemlimit) / 2))); }
                                if (ParentEgg > scalingitemlimit) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.ParentEgg, (ParentEgg - scalingitemlimit - ((ParentEgg - scalingitemlimit) / 2))); }
                                if (RepeatHeal > 0) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.RepeatHeal, RepeatHeal); }



                                if (NovaOnLowHealth > 1) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.NovaOnLowHealth, NovaOnLowHealth - 1); }
                                if (FireRing > scalingitemlimit)
                                {
                                    FireRing = VengenceItemReduction(FireRing, scalingitemlimit);
                                    clonelist[j].inventory.RemoveItem(RoR2Content.Items.FireRing, FireRing); bonushp += FireRing;
                                }
                                if (IceRing > scalingitemlimit)
                                {
                                    IceRing = VengenceItemReduction(IceRing, scalingitemlimit);
                                    clonelist[j].inventory.RemoveItem(RoR2Content.Items.IceRing, IceRing); bonushp += IceRing;
                                }
                                if (ElementalRingVoid > scalingitemlimit)
                                {
                                    ElementalRingVoid = VengenceItemReduction(ElementalRingVoid, scalingitemlimit);
                                    clonelist[j].inventory.RemoveItem(DLC1Content.Items.ElementalRingVoid, ElementalRingVoid); bonushp += ElementalRingVoid;
                                }
                                if (Missile > scalingitemlimit)
                                {
                                    Missile = VengenceItemReduction(Missile, scalingitemlimit);
                                    clonelist[j].inventory.RemoveItem(RoR2Content.Items.Missile, Missile); bonushp += Missile;
                                }
                                if (FireballsOnHit > scalingitemlimit)
                                {
                                    FireballsOnHit = VengenceItemReduction(FireballsOnHit, scalingitemlimit);
                                    clonelist[j].inventory.RemoveItem(RoR2Content.Items.FireballsOnHit, FireballsOnHit); bonushp += FireballsOnHit;
                                }
                                if (LightningStrikeOnHit > scalingitemlimit)
                                {
                                    LightningStrikeOnHit = VengenceItemReduction(LightningStrikeOnHit, scalingitemlimit);
                                    clonelist[j].inventory.RemoveItem(RoR2Content.Items.LightningStrikeOnHit, LightningStrikeOnHit); bonushp += LightningStrikeOnHit;
                                }
                                if (MissileVoid > scalingitemlimit)
                                {
                                    MissileVoid = VengenceItemReduction(MissileVoid, scalingitemlimit);
                                    clonelist[j].inventory.RemoveItem(DLC1Content.Items.MissileVoid, MissileVoid); bonushp += MissileVoid;
                                }
                                if (ChainLightningVoid > scalingitemlimit)
                                {
                                    ChainLightningVoid = VengenceItemReduction(ChainLightningVoid, scalingitemlimit);
                                    clonelist[j].inventory.RemoveItem(DLC1Content.Items.ChainLightningVoid, ChainLightningVoid); bonushp += ChainLightningVoid;
                                }
                                if (Clover > scalingitemlimit) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.Clover, (Clover - scalingitemlimit - ((Clover - scalingitemlimit) / 2))); }
                                if (NearbyDamageBonus > scalingitemlimit) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.NearbyDamageBonus, (NearbyDamageBonus - scalingitemlimit - ((NearbyDamageBonus - scalingitemlimit) / 2))); }
                                if (StickyBomb > scalingitemlimit) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.StickyBomb, (StickyBomb - scalingitemlimit - ((StickyBomb - scalingitemlimit) / 2))); }


                                if (ExplodeOnDeathVoid > 1) { clonelist[j].inventory.RemoveItem(DLC1Content.Items.ExplodeOnDeathVoid, ExplodeOnDeathVoid - 1); }
                                if (BleedOnHitAndExplode > 1) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.BleedOnHitAndExplode, BleedOnHitAndExplode - 1); }
                                if (SprintWisp > 1) { clonelist[j].inventory.RemoveItem(RoR2Content.Items.SprintWisp, SprintWisp - 1); }


                                clonelist[j].inventory.GiveItem(RoR2Content.Items.BoostHp, bonushp / 2);

                                //clonelist[j].inventory.AddItemsFrom(RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.monsterTeamInventory);

                                if (clonelist[j].inventory.currentEquipmentIndex == RoR2Content.Equipment.BurnNearby.equipmentIndex)
                                {
                                    //clonelist[j].inventory.SetEquipmentIndex(EquipmentIndex.None);
                                    clonelist[j].inventory.SetEquipment(new EquipmentState(EquipmentIndex.None, Run.FixedTimeStamp.negativeInfinity, 0), 0);
                                }
                            }

                            if (WConfig.VenganceHealthRebalance.Value == true)
                            {
                                CharacterBody tempbody = clonelist[j].GetBody();

                                tempbody.autoCalculateLevelStats = false;
                                if (tempbody.baseMaxHealth >= 200)
                                {
                                    tempbody.baseMaxHealth *= 0.9f;
                                    tempbody.levelMaxHealth *= 0.9f;
                                }
                                else if (tempbody.baseMaxHealth < 110 || clonelist[j].name.Equals("MercMonsterMaster(Clone)") || clonelist[j].name.Equals("VoidSurvivorMonsterMaster(Clone)"))
                                {
                                    tempbody.baseMaxHealth *= 1.1f;
                                    tempbody.levelMaxHealth *= 1.1f;
                                }
                                else
                                {
                                    tempbody.baseMaxHealth *= 1.3f;
                                    tempbody.levelMaxHealth *= 1.3f;
                                }
                                tempbody.OnLevelUp();
                                clonelist[j].inventory.GiveItem(RoR2Content.Items.CutHp, 2);
                                clonelist[j].inventory.GiveItem(RoR2Content.Items.LevelBonus, 1);
                                clonelist[j].inventory.GiveItem(DLC1Content.Items.OutOfCombatArmor, 1);

                                clonelist[j].inventory.GiveItem(RoR2Content.Items.AdaptiveArmor);



                                if (srcCharacterMaster.hasBody)
                                {
                                    float bonuslevel = srcCharacterMaster.GetBody().level;
                                    //clonelist[j].inventory.GiveItem(RoR2Content.Items.LevelBonus, (int)bonuslevel);
                                }


                            }

                            if (WConfig.VengenceGoodDrop.Value == true)
                            {
                                List<ItemIndex> temporder = new List<ItemIndex>();
                                temporder.AddRange(srcCharacterMaster.inventory.itemAcquisitionOrder);

                                temporder.Remove(RoR2Content.Items.ExtraLifeConsumed.itemIndex);
                                clonelist[j].inventory.RemoveItem(RoR2Content.Items.ExtraLifeConsumed, 10000);
                                temporder.Remove(DLC1Content.Items.ExtraLifeVoidConsumed.itemIndex);
                                clonelist[j].inventory.RemoveItem(DLC1Content.Items.ExtraLifeVoidConsumed, 10000);

                                //clonelist[j].inventory.itemAcquisitionOrder.

                                clonelist[j].inventory.itemAcquisitionOrder = temporder;
                            }

                            //Swarms fix, still broken u7.5
                            //Each player gets their own ShadowCloneEncounter object
                            //Some fucking how this also copies it to the next enemy that spawns
                            if (j > 0)
                            {
                                bossgrouplist2[i].membersList[j].inventory.CopyItemsFrom(bossgrouplist2[i].membersList[0].inventory);
                                Debug.Log("Trying to copy items to swarms clone " + bossgrouplist2[i].membersList[j]);
                            }
                        }
                    }
                }
            }
        }


        public static int VengenceItemReduction(int amount, int limit)
        {
            return amount - limit - ((amount - limit) / 2);
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

            /*
            RoR2.CharacterAI.AISkillDriver[] skilllist;

            GameObject CaptainMaster = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/CaptainMonsterMaster");
            skilllist = CaptainMaster.GetComponents<RoR2.CharacterAI.AISkillDriver>();

            for (var i = 0; i < skilllist.Length; i++)
            {
                if (skilllist[i].skillSlot == SkillSlot.Utility || skilllist[i].customName.Equals("FireLongRange") || skilllist[i].customName.Equals("ShortStrafe") || skilllist[i].customName.Equals("BackUpIfClose"))
                {
                    UnityEngine.Object.Destroy(skilllist[i]);
                }
                else if (skilllist[i].customName.Contains("ChaseEnemy"))
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].minDistance = 100;
                }
                else if (skilllist[i].customName.Equals("MarkOrbitalStrike") || skilllist[i].customName.Equals("StartOrbitalStrike"))
                {
                    skilllist[i].aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy;
                    skilllist[i].nextHighPriorityOverride = null;
                }
                else if (skilllist[i].customName.Contains("FireShotgun"))
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].shouldFireEquipment = true;
                    skilllist[i].maxDistance = 80;
                    skilllist[i].nextHighPriorityOverride = null;
                }
                else
                {
                    skilllist[i].shouldSprint = true;
                    skilllist[i].movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget;
                }
            }


            skilllist = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/CommandoMonsterMaster").GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {

                if (skilllist[i].skillSlot != SkillSlot.Primary && skilllist[i].skillSlot != SkillSlot.Special)
                {
                    skilllist[i].shouldSprint = true;
                }
                if (skilllist[i].skillSlot != SkillSlot.Utility && skilllist[i].skillSlot != SkillSlot.None)
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].shouldFireEquipment = true;
                }
            }
            skilllist = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/CrocoMonsterMaster").GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {
                if (skilllist[i].skillSlot != SkillSlot.Primary)
                {
                    skilllist[i].shouldSprint = true;
                }
                if (skilllist[i].customName.Contains("StrafeAndSlashTarget") && skilllist[i].customName.Contains("LeapToEnemy") || skilllist[i].customName.Contains("ChaseAndSlashTarget") || skilllist[i].customName.Contains("FireContagion") || skilllist[i].customName.Contains("FireSecondary"))
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].shouldFireEquipment = true;
                }
            }

            skilllist = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/ToolbotMonsterMaster").GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {

                if (skilllist[i].skillSlot != SkillSlot.Primary && skilllist[i].skillSlot != SkillSlot.Special)
                {
                    skilllist[i].shouldSprint = true;
                }
                if (!skilllist[i].customName.Contains("SwitchOffNailgun") && !skilllist[i].customName.Contains("ChaseEnemy"))
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].shouldFireEquipment = true;
                }
            }

            skilllist = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/LoaderMonsterMaster").GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {

                skilllist[i].shouldSprint = true;
                if (!skilllist[i].customName.Contains("ChaseEnemy"))
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].shouldFireEquipment = true;
                }
            }

            skilllist = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/Bandit2MonsterMaster").GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {
                if (skilllist[i].skillSlot != SkillSlot.Primary && skilllist[i].skillSlot != SkillSlot.Special)
                {
                    skilllist[i].shouldSprint = true;
                }
                if (skilllist[i].skillSlot != SkillSlot.None)
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].shouldFireEquipment = true;
                }
            }

            skilllist = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/TreebotMonsterMaster").GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {
                if (skilllist[i].skillSlot != SkillSlot.Primary)
                {
                    skilllist[i].shouldSprint = true;
                }
                if (!skilllist[i].customName.Contains("FireMortar") && !skilllist[i].customName.Contains("ChaseEnemy"))
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].shouldFireEquipment = true;
                }
            }

            skilllist = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/EngiMonsterMaster").GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {
                if (!skilllist[i].customName.Contains("PaintHarpoons"))
                {
                    skilllist[i].shouldSprint = true;
                }


                if (!skilllist[i].customName.Contains("ReturnToTurrets") && !skilllist[i].customName.Contains("ThrowShield") && !skilllist[i].customName.Contains("CancelPaintingIfNoLoS") && !skilllist[i].customName.Contains("ChaseEnemy"))
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].shouldFireEquipment = true;
                }
            }

            skilllist = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/MageMonsterMaster").GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {
                if (skilllist[i].skillSlot != SkillSlot.Special)
                {
                    skilllist[i].shouldSprint = true;
                }
                if (!skilllist[i].customName.Contains("EscapeWithSuperJump") && !skilllist[i].customName.Contains("ChaseEnemy"))
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].shouldFireEquipment = true;
                }
            }

            skilllist = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/HereticMonsterMaster").GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {
                if (skilllist[i].skillSlot != SkillSlot.Primary)
                {
                    skilllist[i].shouldSprint = true;
                }
                if (skilllist[i].skillSlot != SkillSlot.Utility && skilllist[i].skillSlot != SkillSlot.None)
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].shouldFireEquipment = true;
                }
            }

            skilllist = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/HuntressMonsterMaster").GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {

                skilllist[i].shouldSprint = true;

                if (!skilllist[i].customName.Contains("BlinkAwayFromEnemy") && !skilllist[i].customName.Contains("ChaseEnemy"))
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].shouldFireEquipment = true;
                }
            }

            skilllist = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/MercMonsterMaster").GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {
                if (skilllist[i].movementType == RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget)
                {
                    skilllist[i].shouldSprint = true;
                }
                if (!skilllist[i].customName.Contains("Chase"))
                {
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].shouldFireEquipment = true;
                }
            }


            skilllist = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/Railgunner/RailgunnerMonsterMaster.prefab").WaitForCompletion().GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {
                if (skilllist[i].movementType == RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget)
                {
                    skilllist[i].shouldSprint = true;

                }
                if (skilllist[i].skillSlot != SkillSlot.None && skilllist[i].skillSlot != SkillSlot.Primary && skilllist[i].skillSlot != SkillSlot.Secondary && skilllist[i].movementType != RoR2.CharacterAI.AISkillDriver.MovementType.FleeMoveTarget)
                {
                    skilllist[i].shouldFireEquipment = true;
                }
            }

            skilllist = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/VoidSurvivor/VoidSurvivorMonsterMaster.prefab").WaitForCompletion().GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {

                if (skilllist[i].skillSlot == SkillSlot.Primary)
                {
                    skilllist[i].buttonPressType = RoR2.CharacterAI.AISkillDriver.ButtonPressType.Hold;
                }
                else if (skilllist[i].skillSlot == SkillSlot.Special)
                {
                    skilllist[i].minDistance = 300;
                }
                else if (skilllist[i].movementType == RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget)
                {
                    skilllist[i].shouldSprint = true;
                }


                if (skilllist[i].skillSlot != SkillSlot.None && skilllist[i].movementType != RoR2.CharacterAI.AISkillDriver.MovementType.FleeMoveTarget)
                {
                    skilllist[i].shouldFireEquipment = true;
                }
            }
            */
        }


    }
}