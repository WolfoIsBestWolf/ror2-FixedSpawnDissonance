using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.CharacterAI;
using R2API;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace VanillaArtifactsPlus
{
    public class BlockSoul : MonoBehaviour
    {

    }
    public class Soul
    {
        public static CharacterBody LesserSoulBody;
        public static SkinDefParams paramsLesserSoul = Addressables.LoadAssetAsync<SkinDefParams>(key: "ac9399461a2333e46b76a6d1bc5b1641").WaitForCompletion();
        public static GameObject SoulLesserWispMaster = LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/WispSoulMaster");

        public static CharacterBody GreaterSoulBody;
        public static GameObject SoulGreaterWispBody;
        public static GameObject SoulGreaterWispMaster;

        public static CharacterBody LunarSoulBody;
        public static GameObject SoulLunarWispBody;
        public static GameObject SoulLunarWispMaster;
 
        public static UnlockableDef unlockable = Addressables.LoadAssetAsync<UnlockableDef>(key: "RoR2/Base/WispOnDeath/Artifacts.WispOnDeath.asset").WaitForCompletion();

        public static void Start()
        {
            //Reminder that Void Team does not drop/have souls
            ChangeLesserSoulWisp();

            //Like we can make a blue skinned Greater Wisp but it'd still show up as a normal one for clients.
            IL.RoR2.GlobalEventManager.OnCharacterDeath += SoulsForVoidTeam;
        }

        private static void SoulsForVoidTeam(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld("RoR2.RoR2Content.Artifacts", "get_wispOnDeath"));

            if (c.TryGotoNext(MoveType.After,
             x => x.MatchLdcI4(2)
            ))
            {
                c.Prev.OpCode = OpCodes.Ldc_I4_1;
                c.Next.OpCode = OpCodes.Beq_S;
            }
            else
            {
                Debug.LogWarning("IL Failed : SoulsForVoidTeam");
            }

        }

        public static void CallLate()
        {
            Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/EliteEarth/AffixEarthHealerBody.prefab").WaitForCompletion().AddComponent<BlockSoul>();
            Addressables.LoadAssetAsync<GameObject>(key: "1553966a1aafdb743bf87ff4d6602c22").WaitForCompletion().AddComponent<BlockSoul>();
       
            SoulLesserWispMaster.AddComponent<GivePickupsOnStart>().itemDefInfos = new GivePickupsOnStart.ItemDefInfo[]
            {
                new GivePickupsOnStart.ItemDefInfo
                {
                    count = 1,
                    itemDef = RoR2Content.Items.LunarBadLuck,
                },
                 new GivePickupsOnStart.ItemDefInfo
                {
                    count = 1,
                    itemDef = RoR2Content.Items.AutoCastEquipment,
                }
            };


            if (!SoulGreaterWispBody)
            {
                return;
            }
  
            SoulGreaterWispMaster.AddComponent<GivePickupsOnStart>().itemDefInfos = new GivePickupsOnStart.ItemDefInfo[]
            {
                new GivePickupsOnStart.ItemDefInfo
                {
                    count = 1,
                    itemDef = RoR2Content.Items.LunarBadLuck,
                },
                 new GivePickupsOnStart.ItemDefInfo
                {
                    count = 1,
                    itemDef = RoR2Content.Items.AutoCastEquipment,
                }
            };

            SoulLunarWispMaster.AddComponent<GivePickupsOnStart>().itemDefInfos = new GivePickupsOnStart.ItemDefInfo[]
            {
                new GivePickupsOnStart.ItemDefInfo
                {
                    count = 5,
                    itemDef = RoR2Content.Items.LunarBadLuck,
                },
                 new GivePickupsOnStart.ItemDefInfo
                {
                    count = 1,
                    itemDef = RoR2Content.Items.AutoCastEquipment,
                }
            };
        }

        public static void ChangeLesserSoulWisp()
        {
            GameObject SoulLesserWispBody = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/WispSoulBody");
            LesserSoulBody = SoulLesserWispBody.GetComponent<CharacterBody>();
            SoulLesserWispBody.AddComponent<BlockSoul>();

            float mostCommonHealth = 75; //35 + 40
            LesserSoulBody.baseMaxHealth = mostCommonHealth;
            LesserSoulBody.levelMaxHealth = mostCommonHealth * 0.3f;
            LesserSoulBody.baseRegen = mostCommonHealth * 0.02f;
            LesserSoulBody.levelRegen = mostCommonHealth * 0.02f * 0.2f;
            LesserSoulBody.baseDamage *= 0.75f;
            LesserSoulBody.levelDamage *= 0.75f;

            LesserSoulBody.portraitIcon = Assets.Bundle.LoadAsset<Texture2D>("Assets/ArtifactsVanilla/soulLesser.png");
            LesserSoulBody.baseNameToken = "SOULWISP_BODY_NAME";
 
            SoulLesserWispMaster.AddComponent<MasterSuicideOnTimer>().lifeTimer = 16f;
            SoulLesserWispMaster.GetComponent<BaseAI>().fullVision = true;



        }


        public static void MakeNewSoulWisp()
        {
            /*unlockable = ScriptableObject.CreateInstance<UnlockableDef>();
            ContentAddition.AddUnlockableDef(unlockable);
            unlockable.cachedName = "Logs.SoulWisps";
            unlockable.nameToken = "LOG_SOULS";*/
            //unlockable = Addressables.LoadAssetAsync<UnlockableDef>(key: "212fdcb501ac2784abde6de18bed51bd").WaitForCompletion();

            if (!WConfig.DisplaySoulInLog.Value)
            {
                unlockable = null;
            }
            LesserSoulBody.GetComponent<DeathRewards>().logUnlockableDef = unlockable;
   
            MakeGreaterSoulWisp();
            MakeLunarSoulWisp();
        }

        

        public static void MakeLunarSoulWisp()
        {
            SoulLunarWispBody = R2API.PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/LunarWispBody"), "SoulLunarWispBody", true);
            SoulLunarWispMaster = R2API.PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterMasters/LunarWispMaster"), "SoulLunarWispMaster", true);
            SoulLunarWispMaster.GetComponent<CharacterMaster>().bodyPrefab = SoulLunarWispBody;
            ContentAddition.AddBody(SoulLunarWispBody);
            ContentAddition.AddMaster(SoulLunarWispMaster);
            LunarSoulBody = SoulLunarWispBody.GetComponent<CharacterBody>();

 
            SkinDef skinLunarWisp = Addressables.LoadAssetAsync<SkinDef>(key: "b897f511a7c359e49aaaeca7c3903488").WaitForCompletion();
            SkinDefParams paramsLunarWisp = Addressables.LoadAssetAsync<SkinDefParams>(key: "5959ddaa1bbb1034c9b537428423ad21").WaitForCompletion();

            SkinDef skinLunarSoul = GameObject.Instantiate(skinLunarWisp);
            SkinDefParams paramsLunarSoul = GameObject.Instantiate(paramsLunarWisp);
            skinLunarSoul.name = "skinLunarSoul";
            skinLunarSoul.skinDefParams = paramsLunarSoul;
            skinLunarSoul.skinDefParamsAddress = new AssetReferenceT<SkinDefParams>("");

            int length = paramsLunarWisp.lightReplacements.Length;
            paramsLunarSoul.lightReplacements = new CharacterModel.LightInfo[length];
            Array.Copy(paramsLunarWisp.lightReplacements, paramsLunarSoul.lightReplacements, length);

            length = paramsLunarWisp.rendererInfos.Length;
            paramsLunarSoul.rendererInfos = new CharacterModel.RendererInfo[length];
            Array.Copy(paramsLunarWisp.rendererInfos, paramsLunarSoul.rendererInfos, length);

            Material matWispSoul = GameObject.Instantiate(Addressables.LoadAssetAsync<Material>(key: "b14722fc35f087d4e88c9c661575ed4c").WaitForCompletion());
            Material matWispSoulFire = GameObject.Instantiate(Addressables.LoadAssetAsync<Material>(key: "5fdd259286e9b124fb4b640add0b7b3e").WaitForCompletion());

            paramsLunarSoul.rendererInfos[0].defaultMaterial = matWispSoul; //matLunarWisp
            paramsLunarSoul.rendererInfos[1].defaultMaterial = matWispSoulFire; //matLunarWispStones
            paramsLunarSoul.rendererInfos[2].defaultMaterial = matWispSoulFire; //matLunarWispFlames
            paramsLunarSoul.rendererInfos[3].defaultMaterial = matWispSoulFire; //matLunarWispFlames
            paramsLunarSoul.rendererInfos[4].defaultMaterial = matWispSoulFire; //matLunarWispFlames
            paramsLunarSoul.rendererInfos[0].defaultMaterialAddress = null;
            paramsLunarSoul.rendererInfos[1].defaultMaterialAddress = null;
            paramsLunarSoul.rendererInfos[2].defaultMaterialAddress = null;
            paramsLunarSoul.rendererInfos[3].defaultMaterialAddress = null;
            paramsLunarSoul.rendererInfos[4].defaultMaterialAddress = null;


            //matWispSoulFire.SetColor("_TintColor", new Color(0f, 1f, 1f, 0.8f));

            //Lowest Health = 600f
            float mostCommonHealth = 1210f;
            LunarSoulBody.baseMaxHealth = mostCommonHealth;
            LunarSoulBody.levelMaxHealth = mostCommonHealth * 0.3f;
            LunarSoulBody.baseRegen = 0f;
            LunarSoulBody.levelRegen = 0f;
            LunarSoulBody.baseAcceleration *= 3f;
            LunarSoulBody.baseMoveSpeed = 12;
            LunarSoulBody.baseAttackSpeed = 1.2f;
            LunarSoulBody.baseDamage *= 0.6f;
            LunarSoulBody.levelDamage *= 0.6f;

            LunarSoulBody.portraitIcon = Assets.Bundle.LoadAsset<Texture2D>("Assets/ArtifactsVanilla/soulLunar.png");
            LunarSoulBody.baseNameToken = "SOULBOSSWISP_BODY_NAME";

            SoulLunarWispMaster.AddComponent<MasterSuicideOnTimer>().lifeTimer = 32f;
            SoulLunarWispMaster.GetComponent<BaseAI>().fullVision = true;
            SoulLunarWispBody.transform.GetChild(0).GetChild(0).GetComponent<ModelSkinController>().skins[0] = skinLunarSoul;
            SoulLunarWispBody.GetComponent<DeathRewards>().logUnlockableDef = unlockable;
            SoulLunarWispBody.AddComponent<BlockSoul>();

            AISkillDriver[] ai = SoulLunarWispMaster.GetComponents<AISkillDriver>();
            //ai[0].skillSlot = SkillSlot.Secondary; //Backup
            //ai[1].skillSlot = SkillSlot.Secondary; //Fire Bomb
            ai[2].skillSlot = SkillSlot.Secondary; //Minigun
            ai[3].skillSlot = SkillSlot.Secondary; //Chase?
            ai[4].skillSlot = SkillSlot.Secondary; //Strafe
 

        }
 
        public static void MakeGreaterSoulWisp()
        {
            SoulGreaterWispBody = R2API.PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/GreaterWispBody"), "GreaterWispSoulBody", true);
            SoulGreaterWispMaster = R2API.PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/GreaterWispMaster"), "GreaterWispSoulMaster", true);
            SoulGreaterWispMaster.GetComponent<CharacterMaster>().bodyPrefab = SoulGreaterWispBody;
            R2API.ContentAddition.AddBody(SoulGreaterWispBody);
            R2API.ContentAddition.AddMaster(SoulGreaterWispMaster);
            GreaterSoulBody = SoulGreaterWispBody.GetComponent<CharacterBody>();

            SkinDef skinGreaterWisp = Addressables.LoadAssetAsync<SkinDef>(key: "9a6ffa135f04dc249ae85dbec44c7f21").WaitForCompletion();
            SkinDefParams paramsGreaterWisp = Addressables.LoadAssetAsync<SkinDefParams>(key: "661f410354db1d245b119347d0a0c8a5").WaitForCompletion();

            SkinDef skinGreaterSoul = GameObject.Instantiate(skinGreaterWisp);
            SkinDefParams paramsGreaterSoul = GameObject.Instantiate(paramsGreaterWisp);
            skinGreaterSoul.name = "skinGreaterSoul";
            skinGreaterSoul.skinDefParams = paramsGreaterSoul;
            skinGreaterSoul.skinDefParamsAddress = new AssetReferenceT<SkinDefParams>("");

            int length = paramsGreaterWisp.lightReplacements.Length;
            paramsGreaterSoul.lightReplacements = new CharacterModel.LightInfo[length];
            Array.Copy(paramsGreaterWisp.lightReplacements, paramsGreaterSoul.lightReplacements, length);

            length = paramsGreaterWisp.rendererInfos.Length;
            paramsGreaterSoul.rendererInfos = new CharacterModel.RendererInfo[length];
            Array.Copy(paramsGreaterWisp.rendererInfos, paramsGreaterSoul.rendererInfos, length);

            Material matWispSoul = GameObject.Instantiate(Addressables.LoadAssetAsync<Material>(key: "b14722fc35f087d4e88c9c661575ed4c").WaitForCompletion());
            Material matWispSoulFire = GameObject.Instantiate(Addressables.LoadAssetAsync<Material>(key: "5fdd259286e9b124fb4b640add0b7b3e").WaitForCompletion());

            paramsGreaterSoul.lightReplacements[0].defaultColor = paramsLesserSoul.lightReplacements[0].defaultColor;
            paramsGreaterSoul.rendererInfos[0].defaultMaterial = matWispSoul;
            paramsGreaterSoul.rendererInfos[1].defaultMaterial = matWispSoulFire;
            paramsGreaterSoul.rendererInfos[0].defaultMaterialAddress = null;
            paramsGreaterSoul.rendererInfos[1].defaultMaterialAddress = null;

            matWispSoulFire.SetTexture("_RemapTex", Assets.Bundle.LoadAsset<Texture2D>("Assets/ArtifactsVanilla/texRampWispSoulAlt.png"));
            matWispSoulFire.SetColor("_CutoffScroll", new Color(15, 13, 15, 15));
            matWispSoulFire.SetColor("_TintColor", new Color(1, 1, 1.8f, 0.6f));

            float lowestHealth = 320f;
            GreaterSoulBody.baseMaxHealth = lowestHealth;
            GreaterSoulBody.levelMaxHealth = lowestHealth*0.3f; 
            GreaterSoulBody.baseRegen = lowestHealth * 0.01f;
            GreaterSoulBody.levelRegen = lowestHealth * 0.01f*0.2f;
            GreaterSoulBody.baseAcceleration *= 3f;
            GreaterSoulBody.baseAttackSpeed = 1.2f;
            GreaterSoulBody.baseDamage *= 0.5f;
            GreaterSoulBody.levelDamage *= 0.5f;
            GreaterSoulBody.portraitIcon = Assets.Bundle.LoadAsset<Texture2D>("Assets/ArtifactsVanilla/soulGreater.png");
            GreaterSoulBody.baseNameToken = "SOULGREATERWISP_BODY_NAME";

            SoulGreaterWispMaster.AddComponent<MasterSuicideOnTimer>().lifeTimer = 24f;
            SoulGreaterWispMaster.GetComponent<BaseAI>().fullVision = true;
            SoulGreaterWispBody.transform.GetChild(0).GetChild(0).GetComponent<ModelSkinController>().skins[0] = skinGreaterSoul;
            SoulGreaterWispBody.GetComponent<DeathRewards>().logUnlockableDef = unlockable;
            SoulGreaterWispBody.AddComponent<BlockSoul>();
        }

        public static CharacterMaster SoulSpawnGreaterUniversal(On.RoR2.MasterSummon.orig_Perform orig, global::RoR2.MasterSummon self)
        {
            if (self.masterPrefab == SoulLesserWispMaster && self.summonerBodyObject)
            {
                if (self.summonerBodyObject.GetComponent<BlockSoul>())
                {
                    return null;
                }
                CharacterBody victimBody = self.summonerBodyObject.GetComponent<CharacterBody>();
                uint SoulMoney;
                if (victimBody.baseMaxHealth > 849 || victimBody.inventory.GetItemCount(RoR2Content.Items.InvadingDoppelganger) > 0)
                {
                    self.masterPrefab = SoulLunarWispMaster;
                    SoulMoney = victimBody.master.money;
                    float health = 160 + victimBody.baseMaxHealth * 0.5f;
                    LunarSoulBody.baseMaxHealth = health;
                    LunarSoulBody.levelMaxHealth = health * 0.3f;
                }
                else if (victimBody.baseMaxHealth > 399) //480 is most minibosses, 425 scorchling
                {
                    self.masterPrefab = SoulGreaterWispMaster;
                    SoulMoney = victimBody.master.money / 2;
                    float health = 80f + victimBody.baseMaxHealth * 0.5f;
                    GreaterSoulBody.baseMaxHealth = health;
                    GreaterSoulBody.levelMaxHealth = health * 0.3f;
                    GreaterSoulBody.baseRegen = health * 0.01f;
                    GreaterSoulBody.levelRegen = GreaterSoulBody.baseRegen * 0.2f;
                }
                else
                {
                    SoulMoney = victimBody.master.money / 4;
                    float health = 35f + victimBody.baseMaxHealth * 0.5f;
                    LesserSoulBody.baseMaxHealth = health;
                    LesserSoulBody.levelMaxHealth = health * 0.3f;
                    LesserSoulBody.baseRegen = health * 0.02f;
                    LesserSoulBody.levelRegen = GreaterSoulBody.baseRegen * 0.2f;
                }
                self.inventoryToCopy = victimBody.inventory;
                CharacterMaster tempmaster = orig(self);
                tempmaster.money = SoulMoney;
                return tempmaster;
            }
            return orig(self);
        }


    }
 

}