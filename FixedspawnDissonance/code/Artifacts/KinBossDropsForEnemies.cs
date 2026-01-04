using RoR2;

namespace VanillaArtifactsPlus
{
    public class KinBossDropsForEnemies
    {
        public static CharacterMaster tempClayMan;

        public static void Start()
        {

            //Nah the Charged Perforator Lemurian thing is kinda dumb even if funny
            //On.RoR2.BossGroup.OnMemberDefeatedServer += BossDropChanger;

            BodyCatalog.availability.CallWhenAvailable(ChangeLate);
            On.RoR2.BossGroup.DropRewards += BossGroup_DropRewards;
        }

        private static void BossGroup_DropRewards(On.RoR2.BossGroup.orig_DropRewards orig, BossGroup self)
        {
            //the old Bossrewards doesn't do a DLC check
            //So we gotta check if theres any dlc junk, then clear the list
            if (self.bossDrops.Count > 0)
            {
                if (Run.instance.IsItemExpansionLocked(self.bossDrops[0].pickupIndex.pickupDef.itemIndex))
                {
                    self.bossDrops.Clear();
                }
            }
            orig(self);
        }

        public static void ChangeLate()
        {

            //1
            RoR2Content.BodyPrefabs.GolemBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.Knurl" };
            DLC2Content.BodyPrefabs.HalcyoniteBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.Knurl" };
            //1
            RoR2Content.BodyPrefabs.JellyfishBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.NovaOnLowHealth" };
            //1
            RoR2Content.BodyPrefabs.BeetleBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.BeetleGland" };
            RoR2Content.BodyPrefabs.BeetleGuardBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.BeetleGland" };
            //2
            RoR2Content.BodyPrefabs.ClayBruiserBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.SiphonOnLowHealth" };
            DLC1Content.BodyPrefabs.ClayGrenadierBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.SiphonOnLowHealth" };
            //3
            RoR2Content.BodyPrefabs.WispBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.SprintWisp" };
            RoR2Content.BodyPrefabs.GreaterWispBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.SprintWisp" };
            //3
            RoR2Content.BodyPrefabs.LemurianBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.FireballsOnHit" };
            RoR2Content.BodyPrefabs.LemurianBruiserBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.FireballsOnHit" };
            DLC2Content.BodyPrefabs.ScorchlingBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.FireballsOnHit" };
            //3
            RoR2Content.BodyPrefabs.ImpBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.BleedOnHitAndExplode" };
            //4
            RoR2Content.BodyPrefabs.RoboBallMiniBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.RoboBallBuddy" };
            DLC3Content.BodyPrefabs.IronHaulerBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.RoboBallBuddy" };
            DLC3Content.BodyPrefabs.TankerBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.RoboBallBuddy" };
            DLC3Content.BodyPrefabs.WorkerUnitBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.RoboBallBuddy" };
            //5
            RoR2Content.BodyPrefabs.ParentBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.ParentEgg" };
            DLC2Content.BodyPrefabs.ChildBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.ParentEgg" };
            //6
            RoR2Content.BodyPrefabs.LunarExploderBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.Pearl" };
            RoR2Content.BodyPrefabs.LunarGolemBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.Pearl" };
            RoR2Content.BodyPrefabs.LunarWispBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.Pearl" };

            #region DLC1 Items
            //3
            DLC1Content.BodyPrefabs.MinorConstructBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.MinorConstructOnKill" };

            RoR2Content.BodyPrefabs.NullifierBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.VoidMegaCrabItem" };
            DLC1Content.BodyPrefabs.VoidJailerBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.VoidMegaCrabItem" };
            DLC1Content.BodyPrefabs.VoidBarnacleBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.VoidMegaCrabItem" };

            #endregion

            #region DLC3 Items
            //3
            DLC3Content.BodyPrefabs.ExtractorUnitBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.ExtraEquipment" };
            DLC3Content.BodyPrefabs.DefectiveUnitBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.ExtraEquipment" };
            DLC3Content.BodyPrefabs.MinePodBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.ExtraEquipment" };

            RoR2Content.BodyPrefabs.VultureBody.GetComponent<DeathRewards>().bossPickup = new SerializablePickupIndex { pickupName = "ItemIndex.ShockDamageAura" };

            #endregion
        }

        public static void ModBossDropChanger()
        {
            if (tempClayMan != null)
            {
                DeathRewards rewardclay2 = tempClayMan.bodyPrefab.GetComponent<DeathRewards>();
                rewardclay2.bossPickup = new SerializablePickupIndex() { pickupName = "ItemIndex.SiphonOnLowHealth" };
            }
        }




    }

}

