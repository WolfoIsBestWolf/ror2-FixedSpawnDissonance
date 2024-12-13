using MonoMod.Cil;
using RoR2;
using RoR2.Artifacts;
using RoR2.Items;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FixedspawnDissonance
{
    public class Evolution
    {
        public static BasicPickupDropTable dtMonsterTeamLunarItem = ScriptableObject.CreateInstance<BasicPickupDropTable>();
        //public static bool InProcessOfMoreEvoItems = false;

        public static void Start()
        {

            BasicPickupDropTable dtMonsterTeamTier1Item = Addressables.LoadAssetAsync<BasicPickupDropTable>(key: "RoR2/Base/MonsterTeamGainsItems/dtMonsterTeamTier1Item.asset").WaitForCompletion();
            BasicPickupDropTable dtMonsterTeamTier2Item = Addressables.LoadAssetAsync<BasicPickupDropTable>(key: "RoR2/Base/MonsterTeamGainsItems/dtMonsterTeamTier2Item.asset").WaitForCompletion();
            BasicPickupDropTable dtMonsterTeamTier3Item = Addressables.LoadAssetAsync<BasicPickupDropTable>(key: "RoR2/Base/MonsterTeamGainsItems/dtMonsterTeamTier3Item.asset").WaitForCompletion();


            ItemTag[] TagsMonsterTeamGain = { ItemTag.AIBlacklist, ItemTag.OnKillEffect, ItemTag.EquipmentRelated, ItemTag.SprintRelated, ItemTag.InteractableRelated, ItemTag.HoldoutZoneRelated, ItemTag.Count };

            dtMonsterTeamTier1Item.bannedItemTags = TagsMonsterTeamGain;
            dtMonsterTeamTier2Item.bannedItemTags = TagsMonsterTeamGain;
            dtMonsterTeamTier3Item.bannedItemTags = TagsMonsterTeamGain;
            dtMonsterTeamLunarItem.bannedItemTags = TagsMonsterTeamGain;

            dtMonsterTeamLunarItem.tier1Weight = 0;
            dtMonsterTeamLunarItem.tier2Weight = 0;
            dtMonsterTeamLunarItem.tier3Weight = 0;
            dtMonsterTeamLunarItem.lunarItemWeight = 1;
            dtMonsterTeamLunarItem.canDropBeReplaced = false;
            dtMonsterTeamLunarItem.name = "dtMonsterTeamLunarItem";



           
            IL.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.GrantMonsterTeamItem += Evolution_MoreItems;
            On.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.OnServerCardSpawnedGlobal += Evo_VoidTeamCorrupted;

        }

        private static void Evo_VoidTeamCorrupted(On.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.orig_OnServerCardSpawnedGlobal orig, SpawnCard.SpawnResult spawnResult)
        {
            orig(spawnResult);
            CharacterMaster characterMaster = spawnResult.spawnedInstance ? spawnResult.spawnedInstance.GetComponent<CharacterMaster>() : null;
            if (characterMaster && characterMaster.teamIndex == TeamIndex.Void)
            {
                characterMaster.inventory.AddItemsFrom(MonsterTeamGainsItemsArtifactManager.monsterTeamInventory);
                foreach (ContagiousItemManager.TransformationInfo transformationInfo in ContagiousItemManager.transformationInfos)
                {
                    ContagiousItemManager.TryForceReplacement(characterMaster.inventory, transformationInfo.originalItem);
                }
            }
        }

        private static void Evolution_MoreItems(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallvirt("RoR2.Inventory", "GiveItem")))
            {
                c.Index--;
                c.EmitDelegate<System.Func<ItemIndex, ItemIndex>>((item) =>
                {
                    ItemDef def = ItemCatalog.GetItemDef(item);
                    if (def.tier == ItemTier.Lunar)
                    {
                        PickupIndex pickupIndex = dtMonsterTeamLunarItem.GenerateDrop(MonsterTeamGainsItemsArtifactManager.treasureRng);
                        if (pickupIndex != PickupIndex.none)
                        {
                            PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
                            if (pickupDef != null)
                            {
                                return pickupDef.itemIndex;
                            }
                        }
                    }
                    return item;
                });
                c.Index++;

                c.EmitDelegate<System.Func<int, int>>((amount) =>
                {
                    if (WConfig.EvoMoreItems.Value == true)
                    {
                        bool GiveMoreEvoItems = false;
                        if (WConfig.EvoMoreAfterLoop.Value == true && MonsterTeamGainsItemsArtifactManager.currentItemIterator > 5)
                        {
                            GiveMoreEvoItems = true;
                        }
                        else if (WConfig.EvoMoreAfterLoop.Value == false)
                        {
                            GiveMoreEvoItems = true;
                        }
                        if (GiveMoreEvoItems)
                        {
                            int iterator = (MonsterTeamGainsItemsArtifactManager.currentItemIterator-1) % MonsterTeamGainsItemsArtifactManager.dropPattern.Length;
                            switch (iterator)
                            {
                                case 0:
                                case 1:
                                    return WConfig.EvoMoreWhite.Value;
                                case 2:
                                case 3:
                                    return WConfig.EvoMoreGreen.Value;
                                case 4:
                                    return WConfig.EvoMoreRed.Value;
                            }
                        }
                    }
                    return amount;
                });
            }
            else
            {
                Debug.LogWarning("IL Failed: Evolution_MoreItems");
            }
        }

         

        public static void Tagchanger()
        {
            #region White



            #endregion
            #region Green
            DLC1Content.Items.PrimarySkillShuriken.tags = DLC1Content.Items.PrimarySkillShuriken.tags.Add(ItemTag.AIBlacklist);

            DLC1Content.Items.MoveSpeedOnKill.tags = DLC1Content.Items.MoveSpeedOnKill.tags.Add(ItemTag.OnKillEffect);
            DLC1Content.Items.RegeneratingScrap.tags = DLC1Content.Items.RegeneratingScrap.tags.Add(ItemTag.AIBlacklist);
            DLC2Content.Items.LowerPricedChests.tags = DLC2Content.Items.LowerPricedChests.tags.Add(ItemTag.AIBlacklist);
            DLC2Content.Items.ExtraShrineItem.tags = DLC2Content.Items.ExtraShrineItem.tags.Add(ItemTag.AIBlacklist);
            DLC2Content.Items.IncreasePrimaryDamage.tags = DLC2Content.Items.IncreasePrimaryDamage.tags.Add(ItemTag.AIBlacklist);
            DLC2Content.Items.ExtraStatsOnLevelUp.tags = DLC2Content.Items.ExtraStatsOnLevelUp.tags.Add(ItemTag.AIBlacklist);

            RoR2Content.Items.BonusGoldPackOnKill.tags = RoR2Content.Items.BonusGoldPackOnKill.tags.Add(ItemTag.AIBlacklist);
            RoR2Content.Items.Infusion.tags = RoR2Content.Items.Infusion.tags.Add(ItemTag.AIBlacklist);
            #endregion
            #region Red
            RoR2Content.Items.NovaOnHeal.tags = RoR2Content.Items.NovaOnHeal.tags.Add(ItemTag.AIBlacklist);
            DLC2Content.Items.GoldOnStageStart.tags = DLC2Content.Items.GoldOnStageStart.tags.Add(ItemTag.AIBlacklist);

            RoR2Content.Items.BarrierOnOverHeal.tags = RoR2Content.Items.BarrierOnOverHeal.tags.Add(ItemTag.Count);
            DLC1Content.Items.MoreMissile.tags = DLC1Content.Items.MoreMissile.tags.Add(ItemTag.Count);
            #endregion
            #region Boss
            RoR2Content.Items.TitanGoldDuringTP.tags = RoR2Content.Items.TitanGoldDuringTP.tags.Add(ItemTag.HoldoutZoneRelated);
            RoR2Content.Items.TitanGoldDuringTP.tags = RoR2Content.Items.TitanGoldDuringTP.tags.Add(ItemTag.AIBlacklist);
            RoR2Content.Items.SprintWisp.tags = RoR2Content.Items.SprintWisp.tags.Add(ItemTag.AIBlacklist);
            RoR2Content.Items.SiphonOnLowHealth.tags = RoR2Content.Items.SiphonOnLowHealth.tags.Add(ItemTag.AIBlacklist);
            DLC1Content.Items.MinorConstructOnKill.tags = DLC1Content.Items.MinorConstructOnKill.tags.Add(ItemTag.AIBlacklist);

            #endregion
            #region Lunar
            RoR2Content.Items.MonstersOnShrineUse.tags = RoR2Content.Items.MonstersOnShrineUse.tags.Add(ItemTag.InteractableRelated);
            RoR2Content.Items.GoldOnHit.tags = RoR2Content.Items.GoldOnHit.tags.Add(ItemTag.AIBlacklist);
            RoR2Content.Items.LunarTrinket.tags = RoR2Content.Items.LunarTrinket.tags.Add(ItemTag.AIBlacklist);
            RoR2Content.Items.FocusConvergence.tags = RoR2Content.Items.FocusConvergence.tags.Add(ItemTag.AIBlacklist);
            DLC1Content.Items.LunarSun.tags = DLC1Content.Items.LunarSun.tags.Add(ItemTag.AIBlacklist);
            DLC1Content.Items.RandomlyLunar.tags = DLC1Content.Items.RandomlyLunar.tags.Add(ItemTag.AIBlacklist);
            DLC2Content.Items.OnLevelUpFreeUnlock.tags = DLC2Content.Items.OnLevelUpFreeUnlock.tags.Add(ItemTag.AIBlacklist);


            #endregion
            #region Void
            DLC1Content.Items.ElementalRingVoid.tags = DLC1Content.Items.ElementalRingVoid.tags.Add(ItemTag.AIBlacklist);
            DLC1Content.Items.ExplodeOnDeathVoid.tags = DLC1Content.Items.ExplodeOnDeathVoid.tags.Add(ItemTag.AIBlacklist);

            DLC1Content.Items.MushroomVoid.tags = DLC1Content.Items.MushroomVoid.tags.Add(ItemTag.SprintRelated);
            DLC1Content.Items.MushroomVoid.tags = DLC1Content.Items.MushroomVoid.tags.Add(ItemTag.AIBlacklist);
            DLC1Content.Items.TreasureCacheVoid.tags = DLC1Content.Items.TreasureCacheVoid.tags.Add(ItemTag.AIBlacklist);
            #endregion

            #region Modded
            //SS2 Missing some tags
            ItemDef tempDef = ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex("WatchMetronome"));
            if (tempDef != null)
            {
                tempDef.tags = tempDef.tags.Add(ItemTag.SprintRelated);
            }
            tempDef = ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex("PortableReactor"));
            if (tempDef != null)
            {
                tempDef.tags = tempDef.tags.Add(ItemTag.AIBlacklist);
            }
            tempDef = ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex("HuntersSigil"));
            if (tempDef != null)
            {
                tempDef.tags = tempDef.tags.Add(ItemTag.AIBlacklist);
            }
            tempDef = ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex("VV_ITEM_EHANCE_VIALS_ITEM"));
            if (tempDef != null)
            {
                tempDef.tags = tempDef.tags.Add(ItemTag.AIBlacklist);
            }
            #endregion


            #region Category stuff

            RoR2Content.Items.ParentEgg.tags[0] = ItemTag.Healing;
            RoR2Content.Items.ShieldOnly.tags[0] = ItemTag.Healing;
            RoR2Content.Items.LunarUtilityReplacement.tags[0] = ItemTag.Healing;
            RoR2Content.Items.RandomDamageZone.tags[0] = ItemTag.Damage;
            DLC1Content.Items.HalfSpeedDoubleHealth.tags[0] = ItemTag.Healing;
            DLC1Content.Items.LunarSun.tags[0] = ItemTag.Damage;

            DLC1Content.Items.MinorConstructOnKill.tags = DLC1Content.Items.MinorConstructOnKill.tags.Add(ItemTag.Utility);
            RoR2Content.Items.Knurl.tags = RoR2Content.Items.Knurl.tags.Remove(ItemTag.Utility);
            RoR2Content.Items.Pearl.tags = RoR2Content.Items.Pearl.tags.Remove(ItemTag.Utility);
            RoR2Content.Items.Pearl.tags = RoR2Content.Items.Pearl.tags.Add(ItemTag.Healing);

            RoR2Content.Items.Infusion.tags = RoR2Content.Items.Infusion.tags.Remove(ItemTag.Utility);
            RoR2Content.Items.GhostOnKill.tags = RoR2Content.Items.GhostOnKill.tags.Remove(ItemTag.Damage);
            RoR2Content.Items.HeadHunter.tags = RoR2Content.Items.HeadHunter.tags.Remove(ItemTag.Utility);
            RoR2Content.Items.BarrierOnKill.tags = RoR2Content.Items.BarrierOnKill.tags.Remove(ItemTag.Utility);
            RoR2Content.Items.BarrierOnOverHeal.tags = RoR2Content.Items.BarrierOnOverHeal.tags.Remove(ItemTag.Utility);
            RoR2Content.Items.FallBoots.tags = RoR2Content.Items.FallBoots.tags.Remove(ItemTag.Damage);

            RoR2Content.Items.NovaOnHeal.tags = RoR2Content.Items.NovaOnHeal.tags.Remove(ItemTag.Damage);
            RoR2Content.Items.NovaOnHeal.tags = RoR2Content.Items.NovaOnHeal.tags.Add(ItemTag.Healing);

            //RoR2Content.Items.PersonalShield.tags = RoR2Content.Items.PersonalShield.tags.Add(ItemTag.Healing);
            DLC1Content.Items.ImmuneToDebuff.tags = DLC1Content.Items.ImmuneToDebuff.tags.Add(ItemTag.Healing);
            DLC1Content.Items.ElementalRingVoid.tags = DLC1Content.Items.ElementalRingVoid.tags.Remove(ItemTag.Utility);

            #endregion

            DLC2Content.Items.KnockBackHitEnemies.tags = DLC2Content.Items.KnockBackHitEnemies.tags.Remove(ItemTag.DevotionBlacklist);
            DLC2Content.Items.IncreasePrimaryDamage.tags = DLC2Content.Items.IncreasePrimaryDamage.tags.Remove(ItemTag.DevotionBlacklist);
        }


        /*
        //Some sort of old worse execution on the more items thing I guess
        private static void EvolutionMoreItems(On.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.orig_GrantMonsterTeamItem orig)
        {
            orig();
            if (NetworkServer.active)
            {
                if (MoreEvoAfterLoop.Value == false || MoreEvoAfterLoop.Value == true && Run.instance.stageClearCount >= 5)
                {
                    MonsterTeamGainItemRandom = GameObject.Find("MonsterTeamGainsItemsArtifactInventory(Clone)").GetComponent<RoR2.Inventory>();
                    int[] invoutput = new int[ItemCatalog.itemCount];
                    MonsterTeamGainItemRandom.WriteItemStacks(invoutput);

                    if (LoopEvoMultiplierDone == false)
                    {
                        LoopEvoMultiplierDone = true;
                        for (var i = 0; i < invoutput.Length; i++)
                        {
                            if (invoutput[i] > 0)
                            {

                                if (ItemCatalog.GetItemDef((ItemIndex)i).tier == ItemTier.Tier1)
                                {
                                    int WhiteAmount = MoreEvoWhite.Value;
                                    int WhiteGiveCount = ((invoutput[i] * WhiteAmount) - invoutput[i]);
                                    MonsterTeamGainItemRandom.GiveItem((ItemIndex)i, WhiteGiveCount);
                                }
                                else if (ItemCatalog.GetItemDef((ItemIndex)i).tier == ItemTier.Tier2)
                                {
                                    int GreenAmount = MoreEvoGreen.Value;
                                    int GreenGiveCount = ((invoutput[i] * GreenAmount) - invoutput[i]);
                                    MonsterTeamGainItemRandom.GiveItem((ItemIndex)i, GreenGiveCount);
                                }
                                else if (ItemCatalog.GetItemDef((ItemIndex)i).tier == ItemTier.Tier3)
                                {
                                    int RedAmount = MoreEvoRed.Value;
                                    int RedGiveCount = ((invoutput[i] * RedAmount) - invoutput[i]);
                                    MonsterTeamGainItemRandom.GiveItem((ItemIndex)i, RedGiveCount);
                                }
                            }
                        }
                    }

                    MonsterTeamGainItemRandom = GameObject.Find("MonsterTeamGainsItemsArtifactInventory(Clone)").GetComponent<RoR2.Inventory>();
                    MonsterTeamGainItemRandom.WriteItemStacks(invoutput);

                    for (var i = 0; i < invoutput.Length; i++)
                    {

                        if (invoutput[i] > 0)
                        {

                            if (ItemCatalog.GetItemDef((ItemIndex)i).tier == ItemTier.Tier1)
                            {
                                int WhiteAmount = MoreEvoWhite.Value;
                                int WhiteCount = 0;
                                int WhiteDivCount = 0;
                                int WhiteGiveCount = 0;
                                WhiteCount = WhiteCount + invoutput[i];
                                while (WhiteCount >= WhiteAmount) { WhiteCount = WhiteCount - WhiteAmount; WhiteDivCount++; };
                                if (WhiteCount < WhiteAmount) { WhiteGiveCount = (WhiteCount + WhiteDivCount) * WhiteAmount; }
                                MonsterTeamGainItemRandom.RemoveItem((ItemIndex)i, invoutput[i]);
                                MonsterTeamGainItemRandom.GiveItem((ItemIndex)i, WhiteGiveCount);
                            }
                            else if (ItemCatalog.GetItemDef((ItemIndex)i).tier == ItemTier.Tier2)
                            {
                                int GreenAmount = MoreEvoGreen.Value;
                                int GreenCount = 0;
                                int GreenDivCount = 0;
                                int GreenGiveCount = 0;
                                GreenCount = GreenCount + invoutput[i];
                                while (GreenCount >= GreenAmount) { GreenCount = GreenCount - GreenAmount; GreenDivCount++; };
                                if (GreenCount < GreenAmount) { GreenGiveCount = (GreenCount + GreenDivCount) * GreenAmount; }
                                MonsterTeamGainItemRandom.RemoveItem((ItemIndex)i, invoutput[i]);
                                MonsterTeamGainItemRandom.GiveItem((ItemIndex)i, GreenGiveCount);
                            }
                            else if (ItemCatalog.GetItemDef((ItemIndex)i).tier == ItemTier.Tier3)
                            {
                                int RedAmount = MoreEvoRed.Value;
                                int RedCount = 0;
                                int RedDivCount = 0;
                                int RedGiveCount = 0;
                                RedCount = RedCount + invoutput[i];
                                while (RedCount >= RedAmount) { RedCount = RedCount - RedAmount; RedDivCount++; };
                                if (RedCount < RedAmount) { RedGiveCount = (RedCount + RedDivCount) * RedAmount; }
                                MonsterTeamGainItemRandom.RemoveItem((ItemIndex)i, invoutput[i]);
                                MonsterTeamGainItemRandom.GiveItem((ItemIndex)i, RedGiveCount);
                            }
                        }
                    }

                    //Debug.Log($"More Evo items");
                }
            }

        }
        */

    }
}