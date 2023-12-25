using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FixedspawnDissonance
{
    public class Dissonance
    {

        public static DirectorCard DissoBeetle = null;
        public static DirectorCard DissoGolem = null;
        public static DirectorCard DissoTitan = null;
        public static DirectorCard DissoVermin = null;
        public static DirectorCard DissoVerminFlying = null;

        public static WeightedSelection<DirectorCard> LunarifiedList = new WeightedSelection<DirectorCard>();
        public static int LunarDone = 0;

        public static void Start()
        {
            Mixenemymaker();
            On.RoR2.ClassicStageInfo.RebuildCards += DissoanceLunerEliteAll;
            On.RoR2.ClassicStageInfo.HandleMixEnemyArtifact += ClassicStageInfo_HandleMixEnemyArtifact;
        }

        public static void ModdedEnemiesSupport()
        {
            //Most Modded Enemies get added via R2API which adds to MixEnemy

            CharacterSpawnCard[] CSCList = Object.FindObjectsOfType(typeof(CharacterSpawnCard)) as CharacterSpawnCard[];
            for (var i = 0; i < CSCList.Length; i++)
            {
                //Debug.LogWarning(CSCList[i]);
                switch (CSCList[i].name)
                {
                    case "cscArchWisp":
                        /*DirectorCard DC_ArchWisp = new DirectorCard
                        {
                            spawnCard = CSCList[i],
                            selectionWeight = 1,
                            preventOverhead = true,
                            minimumStageCompletions = 0,
                            spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
                        };
                        RoR2Content.mixEnemyMonsterCards.AddCard(1, DC_ArchWisp); */
                        break;
                    case "cscClayMan":
                        /*DirectorCard DC_ClayMan = new DirectorCard
                        {
                            spawnCard = CSCList[i],
                            selectionWeight = 1,
                            preventOverhead = false,
                            minimumStageCompletions = 0,
                            spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
                        };
                        RoR2Content.mixEnemyMonsterCards.AddCard(2, DC_ClayMan);  //30
                        BossPickupEdit.tempClayMan = CSCList[i].prefab.GetComponent<CharacterMaster>();*/
                        break;
                    case "cscAncientWisp":
                        /*DirectorCard DC_AncientWisp = new DirectorCard
                        {
                            spawnCard = CSCList[i],
                            selectionWeight = 1,
                            preventOverhead = true,
                            minimumStageCompletions = 0,
                            spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
                        };
                        RoR2Content.mixEnemyMonsterCards.AddCard(0, DC_AncientWisp);  //30*/
                        break;
                    case "cscSigmaConstruct":
                        /*CSCList[i].directorCreditCost = 125;
                        DirectorCard DC_cscSigmaConstruct = new DirectorCard
                        {
                            spawnCard = CSCList[i],
                            selectionWeight = 1,
                            preventOverhead = true,
                            minimumStageCompletions = 0,
                            spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
                        };
                        RoR2Content.mixEnemyMonsterCards.AddCard(1, DC_cscSigmaConstruct);  //30*/
                        //If only Sigma Construct was a good enemy
                        break;
                }
            }
        }


        public static void DissoanceLunerEliteAll(On.RoR2.ClassicStageInfo.orig_RebuildCards orig, ClassicStageInfo self)
        {
            orig(self);

            if (self != null)
            {
                //Maybe I could've done this like replacing normal EliteTiers with only the Lunar one or whatever but this seems to work even with mods
                if (LunarDone == 1)
                {
                    Debug.Log("UnLunarify");
                    for (int k = 0; k < LunarifiedList.Count; k++)
                    {
                        WeightedSelection<DirectorCard>.ChoiceInfo Card3 = LunarifiedList.GetChoice(k);
                        if (!(Card3.value.spawnCard.name == "cscLunarWisp" || Card3.value.spawnCard.name == "cscLunarGolem" || Card3.value.spawnCard.name == "cscLunarExploder"))
                        {
                            //Debug.Log(Card3.value.spawnCard);
                            Card3.value.spawnCard.eliteRules = SpawnCard.EliteRules.Default;
                        }
                    }
                    LunarDone = 0;
                }
                if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.mixEnemyArtifactDef))
                {
                    if (SceneInfo.instance.sceneDef.baseSceneName == "moon2" || SceneInfo.instance.sceneDef.baseSceneName == "itmoon")
                    {
                        WeightedSelection<DirectorCard> CurrentMonsterList = new WeightedSelection<DirectorCard>();
                        CurrentMonsterList = ClassicStageInfo.instance.monsterSelection;

                        LunarDone = 1;
                        LunarifiedList = CurrentMonsterList;

                        Debug.Log("Lunarified");
                        for (int j = 0; j < CurrentMonsterList.Count; j++)
                        {
                            WeightedSelection<DirectorCard>.ChoiceInfo Card2 = CurrentMonsterList.GetChoice(j);
                            Card2.value.spawnCard.eliteRules = SpawnCard.EliteRules.Lunar;
                        }
                    }
                }

            }
        }



        public static void Mixenemymaker()
        {
            DirectorCard DSScav = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscScav"),
                preventOverhead = false,
                selectionWeight = 1,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };


            DirectorCard DSHermitCrab = new DirectorCard
            {
                spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscHermitCrab"),
                selectionWeight = 1,
                preventOverhead = false,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Far
            };


            RoR2Content.mixEnemyMonsterCards.categories[0].selectionWeight = 2;
            RoR2Content.mixEnemyMonsterCards.categories[1].selectionWeight = 2;
            RoR2Content.mixEnemyMonsterCards.categories[2].selectionWeight = 3;
            //RoR2Content.mixEnemyMonsterCards.categories[3].selectionWeight = 0f;
            RoR2Content.mixEnemyMonsterCards.categories[3].cards = RoR2Content.mixEnemyMonsterCards.categories[3].cards.Remove(RoR2Content.mixEnemyMonsterCards.categories[3].cards[0]);
            RoR2Content.mixEnemyMonsterCards.AddCard(0, DSScav); //2000



            DirectorCard SolusProbeTemp = null;
            DirectorCard LunarWispTemp = null;
            //RoR2Content.mixEnemyMonsterCards.AddCard(3, DSVoidInfestor); //60

            for (int i = RoR2Content.mixEnemyMonsterCards.categories.Length - 1; 0 <= i; i--)
            {
                //Debug.LogWarning(i);
                for (int ii = 0; RoR2Content.mixEnemyMonsterCards.categories[i].cards.Length > ii; ii++)
                {
                    //Debug.LogWarning(RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii].spawnCard);
                    //Debug.LogWarning(ii);

                    if (RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii].spawnCard.name.Equals("cscRoboBallMini"))
                    {
                        SolusProbeTemp = RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii];
                        RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii] = DSHermitCrab;
                    }
                    else if (RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii].spawnCard.name.Equals("cscLunarWisp"))
                    {
                        LunarWispTemp = RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii];
                        RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii] = SolusProbeTemp;
                    }//
                    else if (RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii].spawnCard.name.Equals("cscTitanBlackBeach"))
                    {
                        DissoTitan = RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii];
                    }
                    else if (RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii].spawnCard.name.Equals("cscGolem"))
                    {
                        DissoGolem = RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii];
                    }
                    else if (RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii].spawnCard.name.Equals("cscVermin"))
                    {
                        DissoVermin = RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii];
                    }
                    else if (RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii].spawnCard.name.Equals("cscFlyingVermin"))
                    {
                        DissoVerminFlying = RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii];
                    }
                    else if (RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii].spawnCard.name.Equals("cscBeetle"))
                    {
                        DissoBeetle = RoR2Content.mixEnemyMonsterCards.categories[i].cards[ii];
                    }
                }
            }
            RoR2Content.mixEnemyMonsterCards.AddCard(0, LunarWispTemp); //550


            //Logger.LogMessage($"Added Cards.");
            //cardsadded = true;
        }


        public static void ClassicStageInfo_HandleMixEnemyArtifact(On.RoR2.ClassicStageInfo.orig_HandleMixEnemyArtifact orig, DirectorCardCategorySelection monsterCategories, Xoroshiro128Plus rng)
        {
            int DecideTitan = Main.Random.Next(1, 5);
            int DecideGolem = Main.Random.Next(1, 5);
            int DecideVermin = Main.Random.Next(1, 3);
            int DecideFlyingVermin = Main.Random.Next(1, 3);
            int DecideBeetle = Main.Random.Next(1, 51);

            switch (DecideTitan)
            {
                case 1:
                    DissoTitan.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Titan/cscTitanBlackBeach.asset").WaitForCompletion(); ;
                    break;
                case 2:
                    DissoTitan.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Titan/cscTitanDampCave.asset").WaitForCompletion(); ;
                    break;
                case 3:
                    DissoTitan.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Titan/cscTitanGolemPlains.asset").WaitForCompletion(); ;
                    break;
                case 4:
                    DissoTitan.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Titan/cscTitanGooLake.asset").WaitForCompletion(); ;
                    break;
            }
            switch (DecideGolem)
            {
                case 1:
                    DissoGolem.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Golem/cscGolem.asset").WaitForCompletion(); ;
                    break;
                case 2:
                    DissoGolem.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Golem/cscGolemNature.asset").WaitForCompletion(); ;
                    break;
                case 3:
                    DissoGolem.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Golem/cscGolemSandy.asset").WaitForCompletion(); ;
                    break;
                case 4:
                    DissoGolem.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Golem/cscGolemSnowy.asset").WaitForCompletion(); ;
                    break;
            }
            switch (DecideVermin)
            {
                case 1:
                    DissoVermin.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/DLC1/Vermin/cscVermin.asset").WaitForCompletion(); ;
                    break;
                case 2:
                    DissoVermin.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/DLC1/Vermin/cscVerminSnowy.asset").WaitForCompletion(); ;
                    break;
            }
            switch (DecideFlyingVermin)
            {
                case 1:
                    DissoVerminFlying.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/DLC1/FlyingVermin/cscFlyingVermin.asset").WaitForCompletion(); ;
                    break;
                case 2:
                    DissoVerminFlying.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/DLC1/FlyingVermin/cscFlyingVerminSnowy.asset").WaitForCompletion(); ;
                    break;
            }
            if (DecideBeetle == 50)
            {
                Debug.LogWarning("Shiny Beetle");
                DissoBeetle.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Beetle/cscBeetleSulfur.asset").WaitForCompletion(); ;
            }
            else
            {
                DissoBeetle.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Beetle/cscBeetle.asset").WaitForCompletion(); ;
            }
            orig(monsterCategories, rng);
        }

    }
}