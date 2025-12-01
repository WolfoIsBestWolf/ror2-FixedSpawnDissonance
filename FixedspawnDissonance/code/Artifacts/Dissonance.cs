using System;
using System.Collections.Generic;
using System.ComponentModel;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.Networking;
 
namespace VanillaArtifactsPlus
{
    public class Dissonance
    {

        public static DirectorCard DissoGolem = null;
        public static DirectorCard DissoTitan = null;
        public static DirectorCard DissoVermin = null;
        public static DirectorCard DissoVerminFlying = null;
        public static DirectorCard DissoBeetle = null;
        public static DirectorCard DissoBeetleGuard = null;
        public static DirectorCard DissoBeetleQueen = null;

        public static DirectorCardCategorySelection dccsShrineHalcyoniteDissonance = ScriptableObject.CreateInstance<DirectorCardCategorySelection>();
        //public static DirectorCard DissoVoidInfestor = null;
        //public static DirectorCardCategorySelection dccsDissonanceVoidInfestor = ScriptableObject.CreateInstance<DirectorCardCategorySelection>();

        public static void Start()
        {
            Mixenemymaker();

            On.RoR2.ClassicStageInfo.HandleMixEnemyArtifact += FindSkinForSkinnedEnemies;

            On.RoR2.ClassicStageInfo.RebuildCards += DissoanceLunerEliteAll;
            //On.RoR2.DirectorCore.OnEnable += PerfectedVoidForRelevantStage;
            On.RoR2.HalcyoniteShrineInteractable.Start += HalcyoniteShrineInteractable_Start;
            dccsShrineHalcyoniteDissonance.AddCategory("Golem", 10);
            dccsShrineHalcyoniteDissonance.AddCard(0, new DirectorCard()
            {
                preventOverhead = false,
                selectionWeight = 10,
            }); 
            dccsShrineHalcyoniteDissonance.name = "dccsShrineHalcyoniteDissonance";

         
        }

        private static void PerfectedVoidForRelevantStage(On.RoR2.DirectorCore.orig_OnEnable orig, DirectorCore self)
        {
            orig(self);
            if (NetworkServer.active)
            {
                if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.MixEnemy))
                {
                    string stage = SceneInfo.instance.sceneDef.baseSceneName;
                    if (stage == "moon2")
                    {
                        CombatDirector[] combatDirectors = self.GetComponents<CombatDirector>();
                        foreach (CombatDirector comb in combatDirectors)
                        {
                            comb.onSpawnedServer.AddListener(new UnityAction<GameObject>(PerfectedDissonanceOnMoon3));
                        }

                        CombatDirector phase2 = GameObject.Find("SceneInfo/BrotherMissionController/BrotherEncounter, Phase 2/PhaseObjects/CombatDirector").GetComponent<CombatDirector>();
                        CombatDirector phase3 = GameObject.Find("SceneInfo/BrotherMissionController/BrotherEncounter, Phase 3/PhaseObjects/CombatDirector").GetComponent<CombatDirector>();
                        phase2.onSpawnedServer.AddListener(new UnityAction<GameObject>(PerfectedDissonanceOnMoon));
                        phase3.onSpawnedServer.AddListener(new UnityAction<GameObject>(PerfectedDissonanceOnMoon));
                    }
                    /*else if (stage == "voidstage")
                    {
                        CombatDirector[] combatDirectors = self.GetComponents<CombatDirector>();
                        foreach (CombatDirector comb in combatDirectors)
                        {
                            comb.onSpawnedServer.AddListener(new UnityAction<GameObject>(VoidElitesVoidStage));
                        }
                    }*/
                }
               
            }
        }

        private static void HalcyoniteShrineInteractable_Start(On.RoR2.HalcyoniteShrineInteractable.orig_Start orig, HalcyoniteShrineInteractable self)
        {
            orig(self);
            if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.MixEnemy))
            {
                //3400 on stage 5
                int limit = (int)(self.monsterCredit * 0.6f);
                var deck = RoR2Content.mixEnemyMonsterCards.GenerateDirectorCardWeightedSelection();
                int count = deck.Count;
                while (count > 0)
                {
                    count--;
                    WeightedSelection<DirectorCard>.ChoiceInfo choice = deck.GetChoice(count);
                    if (choice.value.cost > limit)
                    {
                        //Debug.Log(choice.value.GetSpawnCard());
                        deck.RemoveChoice(count);
                    }

                }
                var card = deck.Evaluate(ClassicStageInfo.instance.rng.nextNormalizedFloat);
                Debug.Log("Halcyonite Card: " + card.GetSpawnCard());
                dccsShrineHalcyoniteDissonance.categories[0].cards[0].spawnCard = card.GetSpawnCard();
                dccsShrineHalcyoniteDissonance.categories[0].cards[0].spawnDistance = card.spawnDistance;
                self.activationDirector.eliteBias = 1.5f;
                self.activationDirector.monsterCards = dccsShrineHalcyoniteDissonance;
 
            }

        }

        public static void VoidElitesVoidStage(GameObject masterObject)
        {
            if (Run.instance.spawnRng.RangeInt(0, 3) == 0)
            {
                CharacterMaster characterMaster = masterObject.GetComponent<CharacterMaster>();
                if (characterMaster && characterMaster.hasBody)
                {
                    if (characterMaster.inventory.currentEquipmentIndex == EquipmentIndex.None)
                    {
                        if (!characterMaster.GetBody().bodyFlags.HasFlag(CharacterBody.BodyFlags.Void))
                        {
                            characterMaster.inventory.SetEquipmentIndex(DLC1Content.Equipment.EliteVoidEquipment.equipmentIndex);
                        }
                    }
                }

            }
        }
       
        public static void PerfectedDissonanceOnMoon3(GameObject masterObject)
        {
            Inventory inventory = masterObject.GetComponent<Inventory>();
            //EquipmentDef def = inventory.currentEquipmentState.equipmentDef;
            if (inventory.currentEquipmentIndex == EquipmentIndex.None && Run.instance.spawnRng.nextBool)
            {
                inventory.SetEquipmentIndex(RoR2Content.Equipment.AffixLunar.equipmentIndex);
            }
            /*else if (def && def.passiveBuffDef && def.passiveBuffDef.eliteDef && def.passiveBuffDef.eliteDef.devotionLevel == EliteDef.DevotionEvolutionLevel.Low)
            {
                inventory.SetEquipmentIndex(RoR2Content.Equipment.AffixLunar.equipmentIndex);
            }*/
 
        }
        public static void PerfectedDissonanceOnMoon(GameObject masterObject)
        {
            if (!masterObject.name.StartsWith("Lun"))
            {
                Inventory inventory = masterObject.GetComponent<Inventory>();
                inventory.GiveItem(RoR2Content.Items.BoostHp, 10);
                inventory.SetEquipmentIndex(RoR2Content.Equipment.AffixLunar.equipmentIndex);
            }
        }

        //public static CharacterSpawnCard cscVoidInfestorDisso;
        public static void Mixenemymaker()
        {
            /*CharacterSpawnCard cscVoidInfestor = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "dd372e81d0aa2b941923f6308898faaf").WaitForCompletion();
            ItemDef healthdecay = Addressables.LoadAssetAsync<ItemDef>(key: "0dd4987a058b5ba4ba0c41a12d18fd6f").WaitForCompletion();

            cscVoidInfestorDisso = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            //cscVoidInfestorDisso.teamIndexOverride = TeamIndex.Lunar;
            cscVoidInfestorDisso.name = "cscVoidInfestorDisso";
            cscVoidInfestorDisso.forbiddenAsBoss = true;
            cscVoidInfestorDisso.noElites = true;
            cscVoidInfestorDisso.forbiddenFlags = cscVoidInfestor.forbiddenFlags;
            cscVoidInfestorDisso.sendOverNetwork = true;
            cscVoidInfestorDisso.prefab = cscVoidInfestor.prefab;
            cscVoidInfestorDisso.directorCreditCost = 1;
            cscVoidInfestorDisso.itemsToGrant = new ItemCountPair[]
            {
                new ItemCountPair
                {
                    count = 300,
                    itemDef = healthdecay
                }
            };

            dccsDissonanceVoidInfestor.AddCategory("Infestor", 10);
            dccsDissonanceVoidInfestor.AddCard(0, new DirectorCard()
            {
                spawnCard = cscVoidInfestorDisso,
                selectionWeight = 1,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Far,
            });
            dccsDissonanceVoidInfestor.name = "dccsDissonanceVoidInfestor";*/


            DirectorCardCategorySelection dccsMixEnemy = Addressables.LoadAssetAsync<DirectorCardCategorySelection>(key: "RoR2/Base/MixEnemy/dccsMixEnemy.asset").WaitForCompletion();

            DirectorCard DSScav = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscScav"),
                preventOverhead = false,
                selectionWeight = 1,
                minimumStageCompletions = 0,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
 

            dccsMixEnemy.categories[0].selectionWeight = 3;
            dccsMixEnemy.categories[1].selectionWeight = 3;
            dccsMixEnemy.categories[2].selectionWeight = 4;
            //dccsMixEnemy.categories[3].selectionWeight = 0f;
            dccsMixEnemy.categories[3].cards = dccsMixEnemy.categories[3].cards.Remove(dccsMixEnemy.categories[3].cards[0]);
            dccsMixEnemy.AddCard(0, DSScav); //2000


            //Alpha and Barnacle spawn close?
            //dccsMixEnemy.categories[0].cards[9].spawnDistance = DirectorCore.MonsterSpawnDistance.Close;
            //dccsMixEnemy.categories[0].cards[9].spawnDistance = DirectorCore.MonsterSpawnDistance.Close;

            DissoVermin = dccsMixEnemy.categories[2].cards[11];
            DissoVerminFlying = dccsMixEnemy.categories[2].cards[2];

            DissoBeetle = dccsMixEnemy.categories[2].cards[1];
            DissoBeetleGuard = dccsMixEnemy.categories[1].cards[0];
            DissoBeetleQueen = dccsMixEnemy.categories[0].cards[0];

            DissoGolem = dccsMixEnemy.categories[1].cards[5];
            DissoTitan = dccsMixEnemy.categories[0].cards[9];
 
            dccsMixEnemy.AddCard(0, dccsMixEnemy.categories[1].cards[10]); //Move Lunar Wisp to Boss
            dccsMixEnemy.AddCard(0, dccsMixEnemy.categories[1].cards[14]); //Move Halcyonite to Boss
         
            HG.ArrayUtils.ArrayRemoveAtAndResize(ref dccsMixEnemy.categories[1].cards, 10);
            HG.ArrayUtils.ArrayRemoveAtAndResize(ref dccsMixEnemy.categories[1].cards, 14);


            dccsMixEnemy.categories[2].cards[9].spawnDistance = DirectorCore.MonsterSpawnDistance.Close;
            dccsMixEnemy.categories[2].cards[12].spawnDistance = DirectorCore.MonsterSpawnDistance.Close;

            dccsMixEnemy.AddCard(2, new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscHermitCrab"),
                selectionWeight = 1,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Far
            });
         
        }

        public static List<SpawnCard> LunarifiedList;

        public static void DissoanceLunerEliteAll(On.RoR2.ClassicStageInfo.orig_RebuildCards orig, global::RoR2.ClassicStageInfo self, global::RoR2.DirectorCardCategorySelection forcedMonsterCategory, global::RoR2.DirectorCardCategorySelection forcedInteractableCategory)
        {
            orig(self, forcedMonsterCategory, forcedInteractableCategory);

            if (self != null)
            {
                if (LunarifiedList != null)
                {
                    for (int k = 0; k < LunarifiedList.Count; k++)
                    {
                        //LunarifiedList[k].equipmentToGrant = Array.Empty<EquipmentDef>();
                        LunarifiedList[k].eliteRules = SpawnCard.EliteRules.Default;
                    }
                    LunarifiedList = null;
                }
                if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.mixEnemyArtifactDef))
                {
                    if (LunarifiedList == null)
                    {
                        LunarifiedList = new List<SpawnCard>();
                    }
                    string baseS = SceneInfo.instance.sceneDef.baseSceneName;
                    if (baseS == "moon2" || baseS == "itmoon")
                    {
                        Debug.Log("Dissonance : Lunar Stage");
                        WeightedSelection<DirectorCard> CurrentMonsterList = ClassicStageInfo.instance.monsterSelection;
                        for (int j = 0; j < CurrentMonsterList.Count; j++)
                        {
                            SpawnCard Card2 = CurrentMonsterList.GetChoice(j).value.GetSpawnCard();
                            if (Card2.eliteRules == SpawnCard.EliteRules.Default)
                            {
                                LunarifiedList.Add(Card2);
                                Card2.eliteRules = SpawnCard.EliteRules.Lunar;
                            }
                        }
                    }
                }

            }
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
                        DissoBeetleGuard.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "03978ad4a8751804a80940d5cfd4038b").WaitForCompletion();
                        break;
                    case 2:
                        DissoBeetleGuard.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "00c29c914e813454384bddaf6271eff1").WaitForCompletion();
                        break;
                }
            }
            if (DissoBeetleQueen != null)
            {
                int choice = rng.RangeInt(1, 3);
                switch (choice)
                {
                    case 1:
                        DissoBeetleQueen.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "56a5ee1553328b34eb5996e7f77710c9").WaitForCompletion();
                        break;
                    case 2:
                        DissoBeetleQueen.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "0414c8a55b86c1d4e81037e3a7e45788").WaitForCompletion();
                        break;
                }
            }


        }

    }


}