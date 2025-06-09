using System.Collections.Generic;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace VanillaArtifactsPlus
{
    public class Dissonance
    {

        //public static DirectorCard DissoBeetle = null;
        public static DirectorCard DissoGolem = null;
        public static DirectorCard DissoTitan = null;
        public static DirectorCard DissoVermin = null;
        public static DirectorCard DissoVerminFlying = null;
        public static DirectorCard DissoBeetle = null;
        public static DirectorCard DissoBeetleGuard = null;
        public static DirectorCard DissoBeetleQueen = null;

        //public static WeightedSelection<DirectorCard> LunarifiedList = new WeightedSelection<DirectorCard>();
        public static List<SpawnCard> LunarifiedList;

        public static void Start()
        {
            Mixenemymaker();
            On.RoR2.ClassicStageInfo.RebuildCards += DissoanceLunerEliteAll;
            On.RoR2.ClassicStageInfo.HandleMixEnemyArtifact += FindSkinForSkinnedEnemies;
        }

         

        public static void DissoanceLunerEliteAll(On.RoR2.ClassicStageInfo.orig_RebuildCards orig, global::RoR2.ClassicStageInfo self, global::RoR2.DirectorCardCategorySelection forcedMonsterCategory, global::RoR2.DirectorCardCategorySelection forcedInteractableCategory)
        {
            orig(self, forcedMonsterCategory, forcedInteractableCategory);

            if (self != null)
            {
                //Maybe I could've done this like replacing normal EliteTiers with only the Lunar one or whatever but this seems to work even with mods
                if (LunarifiedList != null)
                {
                    Debug.Log("Dissonance : Un-Lunar");
                    for (int k = 0; k < LunarifiedList.Count; k++)
                    {
                        LunarifiedList[k].eliteRules = SpawnCard.EliteRules.Default;
                    }
                    LunarifiedList = null;
                }
                if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.mixEnemyArtifactDef))
                {
                    if (SceneInfo.instance.sceneDef.baseSceneName == "moon2" || SceneInfo.instance.sceneDef.baseSceneName == "itmoon")
                    {
                        Debug.Log("Dissonance : Lunar");
                        WeightedSelection<DirectorCard> CurrentMonsterList = ClassicStageInfo.instance.monsterSelection;
                        LunarifiedList = new List<SpawnCard>();
                        for (int j = 0; j < CurrentMonsterList.Count; j++)
                        {
                            WeightedSelection<DirectorCard>.ChoiceInfo Card2 = CurrentMonsterList.GetChoice(j);
                            if (Card2.value.spawnCard.eliteRules != SpawnCard.EliteRules.Lunar)
                            {
                                Card2.value.spawnCard.eliteRules = SpawnCard.EliteRules.Lunar;
                                LunarifiedList.Add(Card2.value.spawnCard);
                            }    
                        }
                    }
                }

            }
        }



        public static void Mixenemymaker()
        {

            DirectorCardCategorySelection dccsMixEnemy = Addressables.LoadAssetAsync<DirectorCardCategorySelection>(key: "RoR2/Base/MixEnemy/dccsMixEnemy.asset").WaitForCompletion();


            DirectorCard DSScav = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscScav"),
                preventOverhead = false,
                selectionWeight = 1,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard DSHermitCrab = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscHermitCrab"),
                selectionWeight = 1,
                preventOverhead = false,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Far
            };

            dccsMixEnemy.categories[0].selectionWeight = 3;
            dccsMixEnemy.categories[1].selectionWeight = 3;
            dccsMixEnemy.categories[2].selectionWeight = 4;
            //dccsMixEnemy.categories[3].selectionWeight = 0f;
            dccsMixEnemy.categories[3].cards = dccsMixEnemy.categories[3].cards.Remove(dccsMixEnemy.categories[3].cards[0]);
            dccsMixEnemy.AddCard(0, DSScav); //2000


     

            DissoVermin = dccsMixEnemy.categories[2].cards[11];
            DissoVerminFlying = dccsMixEnemy.categories[2].cards[2];

            DissoBeetle = dccsMixEnemy.categories[2].cards[1];
            DissoBeetleGuard = dccsMixEnemy.categories[1].cards[0];
            DissoBeetleQueen = dccsMixEnemy.categories[0].cards[0];

            DissoGolem = dccsMixEnemy.categories[1].cards[5];
            DissoTitan = dccsMixEnemy.categories[0].cards[9];

            DirectorCard SolusProbeTemp = null;
            DirectorCard LunarWispTemp = null;

            SolusProbeTemp = dccsMixEnemy.categories[2].cards[10];
            dccsMixEnemy.categories[2].cards[10] = DSHermitCrab;

            LunarWispTemp = dccsMixEnemy.categories[1].cards[10];
            dccsMixEnemy.categories[1].cards[10] = SolusProbeTemp;

            dccsMixEnemy.AddCard(0, LunarWispTemp); //Lunar Wisp Boss

 

            return;
            /*
            //dccsMixEnemy.AddCard(3, DSVoidInfestor); //60

            for (int ii = 0; dccsMixEnemy.categories[2].cards.Length > ii; ii++)
            {
                if (dccsMixEnemy.categories[2].cards[ii].spawnCard.name.EndsWith("lMini"))
                {
                    SolusProbeTemp = dccsMixEnemy.categories[2].cards[ii];
                    dccsMixEnemy.categories[2].cards[ii] = DSHermitCrab;
                }
                else if (dccsMixEnemy.categories[2].cards[ii].spawnCard.name.Equals("cscVermin"))
                {
                    DissoVermin = dccsMixEnemy.categories[2].cards[ii];
                }
                else if (dccsMixEnemy.categories[2].cards[ii].spawnCard.name.Equals("cscFlyingVermin"))
                {
                    DissoVerminFlying = dccsMixEnemy.categories[2].cards[ii];
                }
                else if (dccsMixEnemy.categories[2].cards[ii].spawnCard.name.Equals("cscBeetle"))
                {
                    DissoBeetle = dccsMixEnemy.categories[2].cards[ii];
                }
            }
            for (int ii = 0; dccsMixEnemy.categories[1].cards.Length > ii; ii++)
            {
                if (dccsMixEnemy.categories[1].cards[ii].spawnCard.name.Equals("cscLunarWisp"))
                {
                    LunarWispTemp = dccsMixEnemy.categories[1].cards[ii];
                    dccsMixEnemy.categories[1].cards[ii] = SolusProbeTemp;
                }
                else if (dccsMixEnemy.categories[1].cards[ii].spawnCard.name.Equals("cscGolem"))
                {
                    DissoGolem = dccsMixEnemy.categories[1].cards[ii];
                }
                else if (dccsMixEnemy.categories[1].cards[ii].spawnCard.name.Equals("cscBeetleGuard"))
                {
                    DissoBeetleGuard = dccsMixEnemy.categories[1].cards[ii];
                }
            }
            for (int ii = 0; dccsMixEnemy.categories[0].cards.Length > ii; ii++)
            {
                if (dccsMixEnemy.categories[0].cards[ii].spawnCard.name.Equals("cscTitanBlackBeach"))
                {
                    DissoTitan = dccsMixEnemy.categories[0].cards[ii];
                }
                else if (dccsMixEnemy.categories[0].cards[ii].spawnCard.name.Equals("cscBeetleQueen"))
                {
                    DissoBeetleQueen = dccsMixEnemy.categories[0].cards[ii];
                }
            }

            dccsMixEnemy.AddCard(0, LunarWispTemp); //550
            */
        }


        public static void FindSkinForSkinnedEnemies(On.RoR2.ClassicStageInfo.orig_HandleMixEnemyArtifact orig, DirectorCardCategorySelection monsterCategories, Xoroshiro128Plus rng)
        {
            orig(monsterCategories, rng);
            if (DissoTitan != null)
            {
                int DecideTitan = rng.RangeInt(1, 5);
                switch (DecideTitan)
                {
                    case 1:
                        DissoTitan.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Titan/cscTitanBlackBeach.asset").WaitForCompletion();
                        break;
                    case 2:
                        DissoTitan.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Titan/cscTitanDampCave.asset").WaitForCompletion();
                        break;
                    case 3:
                        DissoTitan.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Titan/cscTitanGolemPlains.asset").WaitForCompletion();
                        break;
                    case 4:
                        DissoTitan.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Titan/cscTitanGooLake.asset").WaitForCompletion();
                        break;
                }
            }
            if (DissoGolem != null)
            {
                int DecideGolem = rng.RangeInt(1, 5);
                switch (DecideGolem)
                {
                    case 1:
                        DissoGolem.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Golem/cscGolem.asset").WaitForCompletion();
                        break;
                    case 2:
                        DissoGolem.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Golem/cscGolemNature.asset").WaitForCompletion();
                        break;
                    case 3:
                        DissoGolem.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Golem/cscGolemSandy.asset").WaitForCompletion();
                        break;
                    case 4:
                        DissoGolem.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Golem/cscGolemSnowy.asset").WaitForCompletion();
                        break;
                }
            }
            if (DissoVermin != null)
            {
                int DecideVermin = rng.RangeInt(1, 3);
                switch (DecideVermin)
                {
                    case 1:
                        DissoVermin.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/DLC1/Vermin/cscVermin.asset").WaitForCompletion();
                        break;
                    case 2:
                        DissoVermin.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/DLC1/Vermin/cscVerminSnowy.asset").WaitForCompletion();
                        break;
                }
            }
            if (DissoVerminFlying != null)
            {
                int DecideFlyingVermin = rng.RangeInt(1, 3);
                switch (DecideFlyingVermin)
                {
                    case 1:
                        DissoVerminFlying.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/DLC1/FlyingVermin/cscFlyingVermin.asset").WaitForCompletion();
                        break;
                    case 2:
                        DissoVerminFlying.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/DLC1/FlyingVermin/cscFlyingVerminSnowy.asset").WaitForCompletion();
                        break;
                }
            }
            if (DissoBeetle != null)
            {
                int choice = rng.RangeInt(1, 3);
                switch (choice)
                {
                    case 1:
                        DissoBeetle.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Beetle/cscBeetle.asset").WaitForCompletion();
                        break;
                    case 2:
                        DissoBeetle.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Beetle/cscBeetleSulfur.asset").WaitForCompletion();
                        break;
                }
            }
            if (DissoBeetleGuard != null)
            {
                int choice = rng.RangeInt(1, 3);
                switch (choice)
                {
                    case 1:
                        DissoBeetleGuard.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Beetle/cscBeetleGuard.asset").WaitForCompletion();
                        break;
                    case 2:
                        DissoBeetleGuard.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Beetle/cscBeetleGuardSulfur.asset").WaitForCompletion();
                        break;
                }
            }
            if (DissoBeetleQueen != null)
            {
                int choice = rng.RangeInt(1, 3);
                switch (choice)
                {
                    case 1:
                        DissoBeetleQueen.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Beetle/cscBeetleQueen.asset").WaitForCompletion();
                        break;
                    case 2:
                        DissoBeetleQueen.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Beetle/cscBeetleQueenSulfur.asset").WaitForCompletion();
                        break;
                }
            }

        }

    }

    
}