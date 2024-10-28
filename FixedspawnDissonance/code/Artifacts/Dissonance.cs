using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FixedspawnDissonance
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

        public static WeightedSelection<DirectorCard> LunarifiedList = new WeightedSelection<DirectorCard>();
        public static int LunarDone = 0;

        public static void Start()
        {
            Mixenemymaker();
            On.RoR2.ClassicStageInfo.RebuildCards += DissoanceLunerEliteAll;
            On.RoR2.ClassicStageInfo.HandleMixEnemyArtifact += ClassicStageInfo_HandleMixEnemyArtifact;

            On.RoR2.FamilyDirectorCardCategorySelection.OnSelected += FamilyDirectorCardCategorySelection_OnSelected;
        }

        private static void FamilyDirectorCardCategorySelection_OnSelected(On.RoR2.FamilyDirectorCardCategorySelection.orig_OnSelected orig, FamilyDirectorCardCategorySelection self, ClassicStageInfo stageInfo)
        {
            if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.MixEnemy))
            {

            }
            else
            {
                orig(self, stageInfo);
            }

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
                        dccsMixEnemy.AddCard(1, DC_ArchWisp); */
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
                        dccsMixEnemy.AddCard(2, DC_ClayMan);  //30
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
                        dccsMixEnemy.AddCard(0, DC_AncientWisp);  //30*/
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
                        dccsMixEnemy.AddCard(1, DC_cscSigmaConstruct);  //30*/
                        //If only Sigma Construct was a good enemy
                        break;
                }
            }
        }


        public static void DissoanceLunerEliteAll(On.RoR2.ClassicStageInfo.orig_RebuildCards orig, global::RoR2.ClassicStageInfo self, global::RoR2.DirectorCardCategorySelection forcedMonsterCategory, global::RoR2.DirectorCardCategorySelection forcedInteractableCategory)
        {
            orig(self, forcedMonsterCategory, forcedInteractableCategory);

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

            DirectorCardCategorySelection dccsMixEnemy = Addressables.LoadAssetAsync<DirectorCardCategorySelection>(key: "RoR2/Base/MixEnemy/dccsMixEnemy.asset").WaitForCompletion();


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



            dccsMixEnemy.categories[0].selectionWeight = 3;
            dccsMixEnemy.categories[1].selectionWeight = 3;
            dccsMixEnemy.categories[2].selectionWeight = 4;
            //dccsMixEnemy.categories[3].selectionWeight = 0f;
            dccsMixEnemy.categories[3].cards = dccsMixEnemy.categories[3].cards.Remove(dccsMixEnemy.categories[3].cards[0]);
            dccsMixEnemy.AddCard(0, DSScav); //2000


            bool Child = false;
            DirectorCard SolusProbeTemp = null;
            DirectorCard LunarWispTemp = null;
            //dccsMixEnemy.AddCard(3, DSVoidInfestor); //60

            for (int ii = 0; dccsMixEnemy.categories[2].cards.Length > ii; ii++)
            {
                if (dccsMixEnemy.categories[2].cards[ii].spawnCard.name.EndsWith("Child"))
                {
                    Child = true;
                }
                else if (dccsMixEnemy.categories[2].cards[ii].spawnCard.name.EndsWith("lMini"))
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
                    DissoTitan = dccsMixEnemy.categories[1].cards[ii];
                }
                else if (dccsMixEnemy.categories[0].cards[ii].spawnCard.name.Equals("cscBeetleQueen"))
                {
                    DissoBeetleQueen = dccsMixEnemy.categories[0].cards[ii];
                }
            }

            dccsMixEnemy.AddCard(0, LunarWispTemp); //550
            if (Child == false)
            {
                DirectorCard DLC2_Child = new DirectorCard
                {
                    spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/DLC2/Child/cscChild.asset").WaitForCompletion(),
                    selectionWeight = 1,
                    preventOverhead = false,
                    minimumStageCompletions = 0,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
                };
                DirectorCard DLC2_Scorch = new DirectorCard
                {
                    spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/DLC2/Scorchling/cscScorchling.asset").WaitForCompletion(),
                    selectionWeight = 1,
                    preventOverhead = false,
                    minimumStageCompletions = 0,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
                };
                DirectorCard DLC2_Halc = new DirectorCard
                {
                    spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/DLC2/Halcyonite/cscHalcyonite.asset").WaitForCompletion(),
                    selectionWeight = 1,
                    preventOverhead = false,
                    minimumStageCompletions = 0,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
                };
                dccsMixEnemy.AddCard(2, DLC2_Child);
                dccsMixEnemy.AddCard(1, DLC2_Scorch);
                dccsMixEnemy.AddCard(0, DLC2_Halc);
            }
        }


        public static void ClassicStageInfo_HandleMixEnemyArtifact(On.RoR2.ClassicStageInfo.orig_HandleMixEnemyArtifact orig, DirectorCardCategorySelection monsterCategories, Xoroshiro128Plus rng)
        {
            orig(monsterCategories, rng);
            if (DissoTitan != null)
            {
                int DecideTitan = Main.Random.Next(1, 5);
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
                int DecideGolem = Main.Random.Next(1, 5);
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
                int DecideVermin = Main.Random.Next(1, 3);
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
                int DecideFlyingVermin = Main.Random.Next(1, 3);
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
        }

    }
}