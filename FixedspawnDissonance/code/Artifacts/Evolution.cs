using MonoMod.Cil;
using RoR2;
using RoR2.Artifacts;
using RoR2.Items;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace VanillaArtifactsPlus
{
    public class Evolution
    {
        public static BasicPickupDropTable dtMonsterTeamLunarItem = ScriptableObject.CreateInstance<BasicPickupDropTable>();
        //public static bool InProcessOfMoreEvoItems = false;
        public static ItemTag evoBlacklist = (ItemTag)94;
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
                x => x.MatchCallvirt("RoR2.Inventory", "GiveItemPermanent")))
            {
                c.EmitDelegate<System.Func<ItemIndex, ItemIndex>>((item) =>
                {
                    ItemDef def = ItemCatalog.GetItemDef(item);
                    if (def.tier == ItemTier.Lunar)
                    {
                        UniquePickup drop = dtMonsterTeamLunarItem.GeneratePickup(MonsterTeamGainsItemsArtifactManager.treasureRng);
                        if (drop.pickupIndex != PickupIndex.none)
                        {
                            PickupDef pickupDef = PickupCatalog.GetPickupDef(drop.pickupIndex);
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
                            int iterator = (MonsterTeamGainsItemsArtifactManager.currentItemIterator - 1) % MonsterTeamGainsItemsArtifactManager.dropPattern.Length;
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






    }
}