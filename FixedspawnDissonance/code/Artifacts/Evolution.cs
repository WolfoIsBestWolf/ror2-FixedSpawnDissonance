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


            ItemTag[] TagsMonsterTeamGain = { ItemTag.AIBlacklist, ItemTag.OnKillEffect, ItemTag.EquipmentRelated, ItemTag.SprintRelated, ItemTag.PriorityScrap, ItemTag.InteractableRelated, ItemTag.HoldoutZoneRelated, ItemTag.Count };

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



            if (WConfig.EvoMoreItems.Value == true)
            {
                //Debug.Log("More Evolution Items");
                //On.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.GrantMonsterTeamItem += EvolutionMoreItems;

                //Shouldn't relly on methods that add/remove hooks
                /*On.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.GrantMonsterTeamItem += (orig) =>
                {
                    if (InProcessOfMoreEvoItems == false)
                    {
                        On.RoR2.Inventory.GiveItem_ItemIndex_int += EvolutionGiveMoreItemsOld;
                    }
                   orig();
                };*/

                On.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.GrantMonsterTeamItem += EvolutionGiveMoreItems;
            }


            //Void Team getting Void replacements of normal items
            On.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.OnServerCardSpawnedGlobal += (orig, spawnResult) =>
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
            };
        }

        private static void EvolutionGiveMoreItems(On.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.orig_GrantMonsterTeamItem orig)
        {
            bool GiveMoreEvoItems = false;
            if (WConfig.EvoMoreAfterLoop.Value == false)
            {
                GiveMoreEvoItems = true;
            }
            else if (WConfig.EvoMoreAfterLoop.Value == true && RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.currentItemIterator >= 5)
            {
                GiveMoreEvoItems = true;
            }
            if (GiveMoreEvoItems == true)
            {
                PickupDropTable pickupDropTable = MonsterTeamGainsItemsArtifactManager.dropPattern[MonsterTeamGainsItemsArtifactManager.currentItemIterator++ % MonsterTeamGainsItemsArtifactManager.dropPattern.Length];
                if (!pickupDropTable)
                {
                    return;
                }
                PickupIndex pickupIndex = pickupDropTable.GenerateDrop(MonsterTeamGainsItemsArtifactManager.treasureRng);
                if (pickupIndex != PickupIndex.none)
                {
                    PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
                    if (pickupDef != null)
                    {
                        int iterator = (RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.currentItemIterator - 1) % RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.dropPattern.Length;
                        //Debug.LogWarning(iterator);
                        switch (iterator)
                        {
                            case 0:
                            case 1:
                                MonsterTeamGainsItemsArtifactManager.monsterTeamInventory.GiveItem(pickupDef.itemIndex, WConfig.EvoMoreWhite.Value);
                                return;
                            case 2:
                            case 3:
                                MonsterTeamGainsItemsArtifactManager.monsterTeamInventory.GiveItem(pickupDef.itemIndex, WConfig.EvoMoreGreen.Value);
                                return;
                            case 4:
                                MonsterTeamGainsItemsArtifactManager.monsterTeamInventory.GiveItem(pickupDef.itemIndex, WConfig.EvoMoreRed.Value);
                                return;
                        }
                    }
                }
            }
            else
            {
                orig();
            }
        }

        public static void EvolutionGiveMoreItemsOld(On.RoR2.Inventory.orig_GiveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)
        {
            //Debug.LogWarning(self);
            //Inventory MonsterTeamGainItemRandom = GameObject.Find("MonsterTeamGainsItemsArtifactInventory(Clone)").GetComponent<RoR2.Inventory>();
            if (self == MonsterTeamGainsItemsArtifactManager.monsterTeamInventory)
            {
                //InProcessOfMoreEvoItems = true;
                bool GiveMoreEvoItems = false;
                if (WConfig.EvoMoreAfterLoop.Value == false)
                {
                    GiveMoreEvoItems = true;
                }
                else if (WConfig.EvoMoreAfterLoop.Value == true && RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.currentItemIterator >= 5)
                {
                    GiveMoreEvoItems = true;
                }
                //Debug.LogWarning(GiveMoreEvoItems);

                //Debug.LogWarning(iterator-1);

                if (ItemCatalog.GetItemDef(itemIndex).tier == ItemTier.Lunar)
                {
                    itemIndex = dtMonsterTeamLunarItem.GenerateDrop(RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.treasureRng).itemIndex;
                    //Debug.LogWarning(ItemCatalog.GetItemDef(itemIndex));
                }
                if (GiveMoreEvoItems == true)
                {
                    int iterator = (RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.currentItemIterator - 1) % RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.dropPattern.Length;
                    //Debug.LogWarning(iterator);
                    switch (iterator)
                    {
                        case 0:
                        case 1:
                            orig(self, itemIndex, WConfig.EvoMoreWhite.Value);
                            return;
                        case 2:
                        case 3:
                            orig(self, itemIndex, WConfig.EvoMoreGreen.Value);
                            return;
                        case 4:
                            orig(self, itemIndex, WConfig.EvoMoreRed.Value);
                            return;
                    }
                };

            }
            else
            {
                //InProcessOfMoreEvoItems = false;
                //On.RoR2.Inventory.GiveItem_ItemIndex_int -= EvolutionGiveMoreItemsOld;
            }

            orig(self, itemIndex, count);
        }


        public static void Tagchanger()
        {
            DLC1Content.Items.MoveSpeedOnKill.tags = DLC1Content.Items.MoveSpeedOnKill.tags.Add(ItemTag.OnKillEffect);


            RoR2Content.Items.MonstersOnShrineUse.tags = RoR2Content.Items.MonstersOnShrineUse.tags.Add(ItemTag.InteractableRelated);
            RoR2Content.Items.GoldOnHit.tags = RoR2Content.Items.GoldOnHit.tags.Add(ItemTag.AIBlacklist);
            RoR2Content.Items.LunarTrinket.tags = RoR2Content.Items.LunarTrinket.tags.Add(ItemTag.AIBlacklist);
            DLC1Content.Items.LunarSun.tags = DLC1Content.Items.LunarSun.tags.Add(ItemTag.AIBlacklist);

            RoR2Content.Items.LunarPrimaryReplacement.tags = RoR2Content.Items.LunarPrimaryReplacement.tags.Remove(ItemTag.AIBlacklist);
            RoR2Content.Items.LunarSecondaryReplacement.tags = RoR2Content.Items.LunarSecondaryReplacement.tags.Remove(ItemTag.AIBlacklist);
            RoR2Content.Items.LunarUtilityReplacement.tags = RoR2Content.Items.LunarUtilityReplacement.tags.Remove(ItemTag.AIBlacklist);
            RoR2Content.Items.LunarSpecialReplacement.tags = RoR2Content.Items.LunarSpecialReplacement.tags.Remove(ItemTag.AIBlacklist);

            RoR2Content.Items.NovaOnHeal.tags = RoR2Content.Items.NovaOnHeal.tags.Add(ItemTag.AIBlacklist);
            RoR2Content.Items.ShockNearby.tags = RoR2Content.Items.ShockNearby.tags.Add(ItemTag.Count);
            DLC1Content.Items.DroneWeapons.tags = DLC1Content.Items.DroneWeapons.tags.Add(ItemTag.AIBlacklist);
            RoR2Content.Items.BarrierOnOverHeal.tags = RoR2Content.Items.BarrierOnOverHeal.tags.Add(ItemTag.Count);
            DLC1Content.Items.CritDamage.tags = DLC1Content.Items.CritDamage.tags.Add(ItemTag.AIBlacklist);
            DLC1Content.Items.RegeneratingScrap.tags = DLC1Content.Items.RegeneratingScrap.tags.Add(ItemTag.AIBlacklist);




            DLC2Content.Items.IncreaseDamageOnMultiKill.tags = DLC2Content.Items.IncreaseDamageOnMultiKill.tags.Add(ItemTag.AIBlacklist); //Blacklisted because it can bug things.
            DLC2Content.Items.DelayedDamage.tags = DLC2Content.Items.DelayedDamage.tags.Add(ItemTag.AIBlacklist); //Blacklisted because it can bug things.

            DLC2Content.Items.ExtraStatsOnLevelUp.tags = DLC2Content.Items.ExtraStatsOnLevelUp.tags.Add(ItemTag.AIBlacklist);
            DLC2Content.Items.LowerPricedChests.tags = DLC2Content.Items.LowerPricedChests.tags.Add(ItemTag.AIBlacklist);
            DLC2Content.Items.ExtraShrineItem.tags = DLC2Content.Items.ExtraShrineItem.tags.Add(ItemTag.AIBlacklist);

            DLC2Content.Items.TeleportOnLowHealth.tags = DLC2Content.Items.TeleportOnLowHealth.tags.Add(ItemTag.Count);
           
            DLC2Content.Items.GoldOnStageStart.tags = DLC2Content.Items.GoldOnStageStart.tags.Add(ItemTag.AIBlacklist);
            DLC2Content.Items.ResetChests.tags = DLC2Content.Items.ResetChests.tags.Add(ItemTag.AIBlacklist);
            DLC2Content.Items.BoostAllStats.tags = DLC2Content.Items.BoostAllStats.tags.Add(ItemTag.AIBlacklist);

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