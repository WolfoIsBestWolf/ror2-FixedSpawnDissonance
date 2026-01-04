using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace VanillaArtifactsPlus
{
    public class Rebirth
    {
        public static void Start()
        {
            if (WConfig.RebirthChanges.Value)
            {
                //Rebirth should work in more ways, just How?
                //On.RoR2.Run.BeginGameOver += StoreRebirthAlways;
                On.RoR2.PickupPickerController.SetOptionsFromInteractor += MoreRebirth;
            }

        }

        private static void StoreRebirthAlways(On.RoR2.Run.orig_BeginGameOver orig, Run self, GameEndingDef gameEndingDef)
        {
            orig(self, gameEndingDef);
            if (NetworkServer.active && RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(DLC2Content.Artifacts.Rebirth))
            {
                if (WConfig.RebirthStoreAlways.Value)
                {
                    foreach (PlayerCharacterMasterController player in PlayerCharacterMasterController.instances)
                    {
                        if (player.networkUser.rebirthItem == ItemIndex.None)
                        {
                            List<ItemIndex> itemsFiltered = new List<ItemIndex>();
                            var items = player.master.inventory.itemAcquisitionOrder;
                            for (int i = 0; i < items.Count; i++)
                            {
                                ItemDef def = ItemCatalog.GetItemDef(items[i]);
                                if (def.tier != ItemTier.NoTier && def.tier != ItemTier.Lunar)
                                {
                                    if (!def.ContainsTag(ItemTag.RebirthBlacklist) && !def.ContainsTag(ItemTag.Scrap))
                                    {
                                        itemsFiltered.Add(items[i]);
                                    }
                                }
                            }
                            if (itemsFiltered.Count > 0)
                            {
                                int co = self.treasureRng.RangeInt(0, itemsFiltered.Count);
                                player.networkUser.CallCmdStoreRebirthItems(itemsFiltered[co]);
                                Debug.Log("Random Rebirth store : " + ItemCatalog.GetItemDef(itemsFiltered[co]));
                            }

                        }
                    }
                }

            }

        }

        private static void MoreRebirth(On.RoR2.PickupPickerController.orig_SetOptionsFromInteractor orig, PickupPickerController self, Interactor activator)
        {
            if (self.isRebirthChoice)
            {
                ItemTierCatalog.GetItemTierDef(ItemTier.Lunar).canScrap = true;
                ItemTierCatalog.GetItemTierDef(ItemTier.VoidTier1).canScrap = true;
                ItemTierCatalog.GetItemTierDef(ItemTier.VoidTier2).canScrap = true;
                ItemTierCatalog.GetItemTierDef(ItemTier.VoidTier3).canScrap = true;
                ItemTierCatalog.GetItemTierDef(ItemTier.VoidBoss).canScrap = true;
                ItemTierCatalog.GetItemTierDef(ItemTier.FoodTier).canScrap = true;
            }
            orig(self, activator);
            if (self.isRebirthChoice)
            {
                ItemTierCatalog.GetItemTierDef(ItemTier.Lunar).canScrap = false;
                ItemTierCatalog.GetItemTierDef(ItemTier.VoidTier1).canScrap = false;
                ItemTierCatalog.GetItemTierDef(ItemTier.VoidTier2).canScrap = false;
                ItemTierCatalog.GetItemTierDef(ItemTier.VoidTier3).canScrap = false;
                ItemTierCatalog.GetItemTierDef(ItemTier.VoidBoss).canScrap = false;
                ItemTierCatalog.GetItemTierDef(ItemTier.FoodTier).canScrap = false;
            }
        }
    }
}