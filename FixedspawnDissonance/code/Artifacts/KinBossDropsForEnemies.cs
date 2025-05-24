using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FixedspawnDissonance
{
    public class KinBossDropsForEnemies
    {
        public static CharacterMaster tempClayMan;
 
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



            DeathRewards deathRewards = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/GolemBody").GetComponent<DeathRewards>();
            deathRewards.bossPickup = new SerializablePickupIndex() { pickupName = "ItemIndex.Knurl" };
            deathRewards = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC2/Halcyonite/HalcyoniteBody.prefab").WaitForCompletion().GetComponent<DeathRewards>();
            deathRewards.bossPickup = new SerializablePickupIndex() { pickupName = "ItemIndex.Knurl" };
            #region Beetle
            deathRewards = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/BeetleBody").GetComponent<DeathRewards>();
            deathRewards.bossPickup = BossItemBeetle;
            deathRewards = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/BeetleGuardBody").GetComponent<DeathRewards>();
            deathRewards.bossPickup = BossItemBeetle;
            #endregion
            #region Jellyfish
            deathRewards = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/JellyfishBody").GetComponent<DeathRewards>();
            deathRewards.bossPickup = new SerializablePickupIndex() { pickupName = "ItemIndex.NovaOnLowHealth" };
            #endregion
            deathRewards = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/WispBody").GetComponent<DeathRewards>();
            deathRewards.bossPickup = BossItemWisp;
            deathRewards = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/GreaterWispBody").GetComponent<DeathRewards>();
            deathRewards.bossPickup = BossItemWisp;
            deathRewards = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/ArchWispBody").GetComponent<DeathRewards>();
            if (deathRewards)
            {
                deathRewards.bossPickup = BossItemWisp;
            }       

            deathRewards = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/LemurianBody").GetComponent<DeathRewards>();
            deathRewards.bossPickup = BossItemMagma;
            deathRewards = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/LemurianBruiserBody").GetComponent<DeathRewards>();
            deathRewards.bossPickup = BossItemMagma;
            deathRewards = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC2/Scorchling/ScorchlingBody.prefab").WaitForCompletion().GetComponent<DeathRewards>();
            deathRewards.bossPickup = BossItemMagma;


            deathRewards = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/ClayBruiserBody").GetComponent<DeathRewards>();
            deathRewards.bossPickup = new SerializablePickupIndex() { pickupName = "ItemIndex.SiphonOnLowHealth" };
            deathRewards = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/ClayGrenadierBody").GetComponent<DeathRewards>();
            deathRewards.bossPickup = new SerializablePickupIndex() { pickupName = "ItemIndex.SiphonOnLowHealth" };


            deathRewards = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/ImpBody").GetComponent<DeathRewards>();
            deathRewards.bossPickup = new SerializablePickupIndex() { pickupName = "ItemIndex.BleedOnHitAndExplode" };

            deathRewards = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/ParentBody").GetComponent<DeathRewards>();
            deathRewards.bossPickup = new SerializablePickupIndex() { pickupName = "ItemIndex.ParentEgg" };
            deathRewards = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC2/Child/ChildBody.prefab").WaitForCompletion().GetComponent<DeathRewards>();
            deathRewards.bossPickup = new SerializablePickupIndex() { pickupName = "ItemIndex.ParentEgg" };

            deathRewards = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/RoboBallMiniBody").GetComponent<DeathRewards>();
            deathRewards.bossPickup = BossItemRoboBall;
            deathRewards = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/VultureBody").GetComponent<DeathRewards>();
            deathRewards.bossPickup = BossItemRoboBall;

            deathRewards = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/LunarExploderBody").GetComponent<DeathRewards>();
            deathRewards.bossPickup = BossItemShinyPearl;
            deathRewards = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/LunarGolemBody").GetComponent<DeathRewards>();
            deathRewards.bossPickup = BossItemShinyPearl;
            deathRewards = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/LunarWispBody").GetComponent<DeathRewards>();
            deathRewards.bossPickup = BossItemShinyPearl;
            deathRewards = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/BrotherBody").GetComponent<DeathRewards>();
            deathRewards.bossPickup = BossItemShinyPearl;
            deathRewards = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/BrotherHurtBody").GetComponent<DeathRewards>();
            deathRewards.bossPickup = BossItemShinyPearl;


            deathRewards = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/MinorConstructBody").GetComponent<DeathRewards>();
            deathRewards.bossPickup = new SerializablePickupIndex() { pickupName = "ItemIndex.MinorConstructOnKill" };



            deathRewards = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/VoidMegaCrabBody").GetComponent<DeathRewards>();
            deathRewards.bossPickup = BossItemVoid;
            deathRewards = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/NullifierBody").GetComponent<DeathRewards>();
            deathRewards.bossPickup = BossItemVoid;
            deathRewards = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/VoidJailer/VoidJailerBody.prefab").WaitForCompletion().GetComponent<DeathRewards>();
            deathRewards.bossPickup = BossItemVoid;
            deathRewards = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/VoidBarnacle/VoidBarnacleBody.prefab").WaitForCompletion().GetComponent<DeathRewards>();
            deathRewards.bossPickup = BossItemVoid;
            deathRewards = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/EliteVoid/VoidInfestorBody.prefab").WaitForCompletion().GetComponent<DeathRewards>();
            deathRewards.bossPickup = BossItemVoid;
            deathRewards = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/VoidRaidCrab/MiniVoidRaidCrabBodyPhase1.prefab").WaitForCompletion().AddComponent<DeathRewards>();
            deathRewards.bossPickup = BossItemVoid;
            deathRewards = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/VoidRaidCrab/MiniVoidRaidCrabBodyPhase2.prefab").WaitForCompletion().AddComponent<DeathRewards>();
            deathRewards.bossPickup = BossItemVoid;
            deathRewards = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/VoidRaidCrab/MiniVoidRaidCrabBodyPhase3.prefab").WaitForCompletion().GetComponent<DeathRewards>();
            deathRewards.bossPickup = BossItemVoid;


            deathRewards = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC2/FalseSonBoss/FalseSonBossBody.prefab").WaitForCompletion().AddComponent<DeathRewards>();
            deathRewards.bossPickup = new SerializablePickupIndex() { pickupName = "ItemIndex.TitanGoldDuringTP" };
            deathRewards = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC2/FalseSonBoss/FalseSonBossBodyLunarShard.prefab").WaitForCompletion().AddComponent<DeathRewards>();
            deathRewards.bossPickup = new SerializablePickupIndex() { pickupName = "ItemIndex.TitanGoldDuringTP" };
            deathRewards = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC2/FalseSonBoss/FalseSonBossBodyBrokenLunarShard.prefab").WaitForCompletion().GetComponent<DeathRewards>();
            deathRewards.bossPickup = new SerializablePickupIndex() { pickupName = "ItemIndex.TitanGoldDuringTP" };


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

        


    }

}

