using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FixedspawnDissonance
{
    public class KinBossDropsForEnemies
    {
        public static CharacterMaster tempClayMan;

        public static string ol = Language.GetString("ELITE_MODIFIER_LIGHTNING");
        public static void Start()
        {
            SerializablePickupIndex BossItemWisp = new SerializablePickupIndex() { pickupName = "ItemIndex.SprintWisp" };
            //SerializablePickupIndex BossItemVagrant = new SerializablePickupIndex() { pickupName = "ItemIndex.NovaOnLowHealth" };
            //SerializablePickupIndex BossItemTitan = new SerializablePickupIndex() { pickupName = "ItemIndex.Knurl" };
            SerializablePickupIndex BossItemBeetle = new SerializablePickupIndex() { pickupName = "ItemIndex.BeetleGland" };
            //SerializablePickupIndex BossItemImp = new SerializablePickupIndex() { pickupName = "ItemIndex.BleedOnHitAndExplode" };
            SerializablePickupIndex BossItemRoboBall = new SerializablePickupIndex() { pickupName = "ItemIndex.RoboBallBuddy" };
            //SerializablePickupIndex BossItemParent = new SerializablePickupIndex() { pickupName = "ItemIndex.ParentEgg" };
            //SerializablePickupIndex BossItemClay = new SerializablePickupIndex() { pickupName = "ItemIndex.SiphonOnLowHealth" };
            SerializablePickupIndex BossItemMagma = new SerializablePickupIndex() { pickupName = "ItemIndex.FireballsOnHit" };
            //SerializablePickupIndex BossItemElectric = new SerializablePickupIndex() { pickupName = "ItemIndex.LightningStrikeOnHit" };
            SerializablePickupIndex BossItemShinyPearl = new SerializablePickupIndex() { pickupName = "ItemIndex.ShinyPearl" };
            //SerializablePickupIndex BossItemPearl = new SerializablePickupIndex() { pickupName = "ItemIndex.Pearl" };

            //SerializablePickupIndex BossItemLamp = new SerializablePickupIndex() { pickupName = "ItemIndex.MinorConstructOnKill" };
            SerializablePickupIndex BossItemVoid = new SerializablePickupIndex() { pickupName = "ItemIndex.VoidMegaCrabItem" };



            DeathRewards rewardgolem = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/GolemBody").GetComponent<DeathRewards>();
            rewardgolem.bossPickup = new SerializablePickupIndex() { pickupName = "ItemIndex.Knurl" };

            DeathRewards rewardbeetle1 = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/BeetleBody").GetComponent<DeathRewards>();
            rewardbeetle1.bossPickup = BossItemBeetle;
            DeathRewards rewardbeetle2 = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/BeetleGuardBody").GetComponent<DeathRewards>();
            rewardbeetle2.bossPickup = BossItemBeetle;

            DeathRewards rewardjelly = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/JellyfishBody").GetComponent<DeathRewards>();
            rewardjelly.bossPickup = new SerializablePickupIndex() { pickupName = "ItemIndex.NovaOnLowHealth" };

            DeathRewards rewardwisp1 = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/WispBody").GetComponent<DeathRewards>();
            rewardwisp1.bossPickup = BossItemWisp;
            DeathRewards rewardwisp2 = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/GreaterWispBody").GetComponent<DeathRewards>();
            rewardwisp2.bossPickup = BossItemWisp;

            DeathRewards rewardlemurian1 = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/LemurianBody").GetComponent<DeathRewards>();
            rewardlemurian1.bossPickup = BossItemMagma;
            DeathRewards rewardlemurian2 = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/LemurianBruiserBody").GetComponent<DeathRewards>();
            rewardlemurian2.bossPickup = BossItemMagma;

            DeathRewards rewardclay1 = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/ClayBruiserBody").GetComponent<DeathRewards>();
            rewardclay1.bossPickup = new SerializablePickupIndex() { pickupName = "ItemIndex.SiphonOnLowHealth" };
            DeathRewards rewardclay2 = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/ClayGrenadierBody").GetComponent<DeathRewards>();
            rewardclay2.bossPickup = new SerializablePickupIndex() { pickupName = "ItemIndex.SiphonOnLowHealth" };


            DeathRewards rewardimp = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/ImpBody").GetComponent<DeathRewards>();
            rewardimp.bossPickup = new SerializablePickupIndex() { pickupName = "ItemIndex.BleedOnHitAndExplode" };

            DeathRewards rewardparent = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/ParentBody").GetComponent<DeathRewards>();
            rewardparent.bossPickup = new SerializablePickupIndex() { pickupName = "ItemIndex.ParentEgg" };

            DeathRewards rewardroboball1 = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/RoboBallMiniBody").GetComponent<DeathRewards>();
            rewardroboball1.bossPickup = BossItemRoboBall;
            DeathRewards rewardroboball2 = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/VultureBody").GetComponent<DeathRewards>();
            rewardroboball2.bossPickup = BossItemRoboBall;

            DeathRewards rewardlunar1 = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/LunarExploderBody").GetComponent<DeathRewards>();
            rewardlunar1.bossPickup = BossItemShinyPearl;
            DeathRewards rewardlunar2 = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/LunarGolemBody").GetComponent<DeathRewards>();
            rewardlunar2.bossPickup = BossItemShinyPearl;
            DeathRewards rewardlunar3 = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/LunarWispBody").GetComponent<DeathRewards>();
            rewardlunar3.bossPickup = BossItemShinyPearl;


            DeathRewards rewardlamp = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/MinorConstructBody").GetComponent<DeathRewards>();
            rewardlamp.bossPickup = new SerializablePickupIndex() { pickupName = "ItemIndex.MinorConstructOnKill" };



            DeathRewards rewardvoid1 = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/VoidMegaCrabBody").GetComponent<DeathRewards>();
            rewardvoid1.bossPickup = BossItemVoid;
            DeathRewards rewardvoid2 = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/NullifierBody").GetComponent<DeathRewards>();
            rewardvoid2.bossPickup = BossItemVoid;
            DeathRewards rewardvoid3 = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/VoidJailer/VoidJailerBody.prefab").WaitForCompletion().GetComponent<DeathRewards>();
            rewardvoid3.bossPickup = BossItemVoid;
            DeathRewards rewardvoid4 = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/VoidBarnacle/VoidBarnacleBody.prefab").WaitForCompletion().GetComponent<DeathRewards>();
            rewardvoid4.bossPickup = BossItemVoid;
            DeathRewards rewardvoid5 = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/EliteVoid/VoidInfestorBody.prefab").WaitForCompletion().GetComponent<DeathRewards>();
            rewardvoid5.bossPickup = BossItemVoid;





            //Nah the Charged Perforator Lemurian thing is kinda dumb even if funny
            //On.RoR2.BossGroup.OnMemberDefeatedServer += BossDropChanger;

        }

        public static void ModBossDropChanger()
        {
            if (tempClayMan != null)
            {
                DeathRewards rewardclay2 = tempClayMan.bodyPrefab.GetComponent<DeathRewards>();
                rewardclay2.bossPickup = new SerializablePickupIndex() { pickupName = "ItemIndex.SiphonOnLowHealth" };
            }
        }

        //Unused
        public static void BossDropChanger(On.RoR2.BossGroup.orig_OnMemberDefeatedServer orig, global::RoR2.BossGroup self, global::RoR2.CharacterMaster memberMaster, global::RoR2.DamageReport damageReport)
        {
            //Debug.LogWarning(Language.GetString("LEMURIANBRUISER_BODY_NAME"));*/
            if (self.bestObservedName.Contains(Language.GetString("LEMURIAN_BODY_NAME")) && self.bestObservedName.Contains(ol))
            {
                foreach (CharacterMaster charactermaster in self.combatSquad.readOnlyMembersList)
                {
                    //charactermaster.GetBody().GetComponent<DeathRewards>().bossPickup = BossItemElectric;
                }
            }
            orig(self, memberMaster, damageReport);

        }


    }

}

