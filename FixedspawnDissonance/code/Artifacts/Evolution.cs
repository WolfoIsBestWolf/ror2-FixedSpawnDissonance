using HG;
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


            ItemTag[] TagsMonsterTeamGain = { ItemTag.AIBlacklist, ItemTag.OnKillEffect, ItemTag.EquipmentRelated, ItemTag.SprintRelated, ItemTag.InteractableRelated, ItemTag.HoldoutZoneRelated, evoBlacklist };

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


            bool evo_config_mod = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.EvolutionConfig");

            if (!evo_config_mod)
            {
                IL.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.GrantMonsterTeamItem += Evolution_MoreItems;

            }
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
                x => x.MatchLdcI4(1),
                x => x.MatchCallvirt("RoR2.Inventory", "GiveItem")))
            {
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
                c.TryGotoNext(MoveType.After,
                x => x.MatchLdcI4(1));
                
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
                                    return 3;
                                case 2:
                                case 3:
                                    return 2;
                                case 4:
                                    return 1;
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


        public static ItemTag evoBlacklist = (ItemTag)94;

        public static void Tagchanger()
        {
            #region White



            #endregion
            #region Green
            ArrayUtils.ArrayAppend(ref RoR2Content.Items.BonusGoldPackOnKill.tags, ItemTag.AIBlacklist); //Useless
            ArrayUtils.ArrayAppend(ref RoR2Content.Items.Infusion.tags, ItemTag.AIBlacklist); //Useless
            ArrayUtils.ArrayAppend(ref DLC1Content.Items.PrimarySkillShuriken.tags, evoBlacklist); //Borderline Overpowered
            ArrayUtils.ArrayAppend(ref DLC1Content.Items.MoveSpeedOnKill.tags, ItemTag.OnKillEffect); //Missed Tag

            #endregion
            #region Red
            ArrayUtils.ArrayAppend(ref RoR2Content.Items.NovaOnHeal.tags, ItemTag.AIBlacklist); //Overpowered

            ArrayUtils.ArrayAppend(ref RoR2Content.Items.BarrierOnOverHeal.tags, evoBlacklist); //Useless
            ArrayUtils.ArrayAppend(ref DLC1Content.Items.MoreMissile.tags, evoBlacklist); //Useless

            #endregion
            #region Boss
            ArrayUtils.ArrayAppend(ref RoR2Content.Items.TitanGoldDuringTP.tags, ItemTag.AIBlacklist);
            ArrayUtils.ArrayAppend(ref RoR2Content.Items.SprintWisp.tags, ItemTag.AIBlacklist);
            ArrayUtils.ArrayAppend(ref RoR2Content.Items.SiphonOnLowHealth.tags, ItemTag.AIBlacklist);
            ArrayUtils.ArrayAppend(ref DLC1Content.Items.MinorConstructOnKill.tags, ItemTag.AIBlacklist);
 
            #endregion
            #region Lunar
            ArrayUtils.ArrayAppend(ref RoR2Content.Items.MonstersOnShrineUse.tags, ItemTag.AIBlacklist);
            ArrayUtils.ArrayAppend(ref RoR2Content.Items.GoldOnHit.tags, ItemTag.AIBlacklist);
            ArrayUtils.ArrayAppend(ref RoR2Content.Items.LunarTrinket.tags, ItemTag.AIBlacklist);
            ArrayUtils.ArrayAppend(ref RoR2Content.Items.FocusConvergence.tags, ItemTag.AIBlacklist);
            ArrayUtils.ArrayAppend(ref DLC1Content.Items.LunarSun.tags, ItemTag.AIBlacklist);
            ArrayUtils.ArrayAppend(ref DLC1Content.Items.RandomlyLunar.tags, ItemTag.AIBlacklist);
            ArrayUtils.ArrayAppend(ref DLC2Content.Items.OnLevelUpFreeUnlock.tags, ItemTag.AIBlacklist);

            #endregion
            #region Void
            ArrayUtils.ArrayAppend(ref DLC1Content.Items.ElementalRingVoid.tags, ItemTag.AIBlacklist); //Unfun
            ArrayUtils.ArrayAppend(ref DLC1Content.Items.ExplodeOnDeathVoid.tags, ItemTag.AIBlacklist); //Op
            ArrayUtils.ArrayAppend(ref DLC1Content.Items.MushroomVoid.tags, ItemTag.SprintRelated);
            ArrayUtils.ArrayAppend(ref DLC1Content.Items.MushroomVoid.tags, ItemTag.AIBlacklist); //Sprint is blacklisted
            ArrayUtils.ArrayAppend(ref DLC1Content.Items.TreasureCacheVoid.tags, ItemTag.AIBlacklist);
 
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

         

    }
}