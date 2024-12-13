using RoR2;
using UnityEngine;

namespace FixedspawnDissonance
{
    public class Soul
    {
        public static GameObject SoulLesserWispBody = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/WispSoulBody");
        public static GameObject SoulLesserWispMaster = LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/WispSoulMaster");

        public static GameObject SoulGreaterWispBody;
        public static GameObject SoulGreaterWispMaster;

        public static GameObject SoulArchWispBody;
        public static GameObject SoulArchWispMaster;
        //Moffeine has a mod that fixes NullRefs for Arch Wisp but there just don't seem to be any nullrefs with it so idk dude

        public static BodyIndex SoulGreaterWispIndex = BodyIndex.None;
        public static BodyIndex SoulArchWispIndex = BodyIndex.None;
        public static BodyIndex IndexAffixHealingCore = BodyIndex.None;

        //public static Inventory SoulInventoryToCopy = null;
        //public static uint SoulMoney = 0;
        //public static int SoulGreaterDecider = 0;

        public static void Start()
        {
            //Reminder that Void Team does not drop/have souls
            SoulWispCreator();
        }

        public static void SoulWispCreator()
        {

            CharacterBody SoulWispBody = SoulLesserWispBody.GetComponent<CharacterBody>();

            Texture2D TexSoulWisp = Assets.Bundle.LoadAsset<Texture2D>("Assets/ArtifactsVanilla/texBodyWispSoul.png");
            TexSoulWisp.wrapMode = TextureWrapMode.Clamp;

            SoulWispBody.baseMaxHealth = 105;  //35 base
            SoulWispBody.levelMaxHealth = 31.5f; //10 base
            SoulWispBody.baseMoveSpeed *= 1.1f;
            SoulWispBody.baseAcceleration *= 1.1f;
            SoulWispBody.baseAttackSpeed = 1.1f;
            SoulWispBody.baseRegen = 0f;
            SoulWispBody.levelRegen = 0f;
            SoulWispBody.baseDamage *= 0.65f;
            SoulWispBody.levelDamage *= 0.65f;      

 
            SoulLesserWispMaster.AddComponent<MasterSuicideOnTimer>().lifeTimer = 15f;

            if (!WConfig.DisableNewContent.Value)
            {

                SoulGreaterWispBody = R2API.PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/GreaterWispBody"), "GreaterWispSoulBody", true);
                SoulGreaterWispMaster = R2API.PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/GreaterWispMaster"), "GreaterWispSoulMaster", true);

                SoulArchWispBody = R2API.PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/ArchWispBody"), "SoulArchWispBody", true);
                SoulArchWispMaster = R2API.PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterMasters/ArchWispMaster"), "SoulArchWispMaster", true);



                Texture2D texRampWispSoulAlt = Assets.Bundle.LoadAsset<Texture2D>("Assets/ArtifactsVanilla/texRampWispSoulAlt.png");
                texRampWispSoulAlt.filterMode = FilterMode.Point;
                texRampWispSoulAlt.wrapMode = TextureWrapMode.Clamp;

                Texture2D texRampWispSoulAlt2 = Assets.Bundle.LoadAsset<Texture2D>("Assets/ArtifactsVanilla/texRampWispSoulAlt2.png");
                texRampWispSoulAlt2.filterMode = FilterMode.Point;
                texRampWispSoulAlt2.wrapMode = TextureWrapMode.Clamp;

                //
                CharacterModel GreaterSoulCharacterModel = SoulGreaterWispBody.transform.GetChild(0).GetChild(0).GetComponent<CharacterModel>();

                GreaterSoulCharacterModel.baseLightInfos[0].defaultColor = SoulLesserWispBody.transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<Light>().color;
                GreaterSoulCharacterModel.baseRendererInfos[0].defaultMaterial = SoulLesserWispBody.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().material; //Main Mesh
                GreaterSoulCharacterModel.baseRendererInfos[1].defaultMaterial = SoulLesserWispBody.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(3).GetComponent<ParticleSystemRenderer>().material; //Fire
                GreaterSoulCharacterModel.baseRendererInfos[1].defaultMaterial.SetTexture("_RemapTex", texRampWispSoulAlt);
                GreaterSoulCharacterModel.baseRendererInfos[1].defaultMaterial.SetColor("_CutoffScroll", new Color(15, 13, 15, 15));
                GreaterSoulCharacterModel.baseRendererInfos[1].defaultMaterial.SetColor("_TintColor", new Color(1, 1, 1.8f, 0.6f));
                //
                CharacterModel SoulArchWispModel = SoulArchWispBody.transform.GetChild(0).GetChild(0).GetComponent<CharacterModel>();

                SoulArchWispModel.baseLightInfos[0].defaultColor = new Color(0f, 0.3476f, 1f, 1f);//1 0 0.3476 1
                SoulArchWispModel.baseRendererInfos[0].defaultMaterial = SoulLesserWispBody.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().material; //Main Mesh
                SoulArchWispModel.baseRendererInfos[1].defaultMaterial = SoulLesserWispBody.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(3).GetComponent<ParticleSystemRenderer>().material; //Fire
                SoulArchWispModel.baseRendererInfos[1].defaultMaterial.SetTexture("_RemapTex", texRampWispSoulAlt2);
                SoulArchWispModel.baseRendererInfos[1].defaultMaterial.SetColor("_CutoffScroll", new Color(15, 13, 15, 15));
                SoulArchWispModel.baseRendererInfos[1].defaultMaterial.SetColor("_TintColor", new Color(0.9f, 0.9f, 1.8f, 0.6f));
                //
                SoulGreaterWispMaster.GetComponent<CharacterMaster>().bodyPrefab = SoulGreaterWispBody;
                SoulArchWispMaster.GetComponent<CharacterMaster>().bodyPrefab = SoulArchWispBody;


                CharacterBody GreaterSoulWispBody = SoulGreaterWispBody.GetComponent<CharacterBody>();
                CharacterBody ArchSoulWispBody = SoulArchWispBody.GetComponent<CharacterBody>();

                SoulGreaterWispBody.GetComponent<DeathRewards>().logUnlockableDef = null;
                //SoulArchWispBody.GetComponent<DeathRewards>().logUnlockableDef = null;

                Texture2D TexGreaterSoulWisp = Assets.Bundle.LoadAsset<Texture2D>("Assets/ArtifactsVanilla/texBodyGreaterWispSoul.png");
                TexGreaterSoulWisp.wrapMode = TextureWrapMode.Clamp;

                Texture2D texBodyArchSoul = Assets.Bundle.LoadAsset<Texture2D>("Assets/ArtifactsVanilla/texBodyArchSoul.png");
                texBodyArchSoul.wrapMode = TextureWrapMode.Clamp;
                //
                SoulWispBody.portraitIcon = TexSoulWisp;
                SoulWispBody.baseNameToken = "SOULWISP_BODY_NAME";

                GreaterSoulWispBody.portraitIcon = TexGreaterSoulWisp;
                GreaterSoulWispBody.baseNameToken = "SOULGREATERWISP_BODY_NAME";

                ArchSoulWispBody.portraitIcon = texBodyArchSoul;
                ArchSoulWispBody.baseNameToken = "SOULARCHWISP_BODY_NAME";
                //


                //
                GreaterSoulWispBody.baseMaxHealth = 480; //750
                GreaterSoulWispBody.levelMaxHealth = 144;   //225
                GreaterSoulWispBody.baseMoveSpeed *= 1.2f;
                GreaterSoulWispBody.baseAcceleration *= 1.2f;
                GreaterSoulWispBody.baseAttackSpeed = 1.2f;
                GreaterSoulWispBody.baseRegen = 0f;
                GreaterSoulWispBody.levelRegen = 0f;
                GreaterSoulWispBody.baseDamage *= 0.65f;
                GreaterSoulWispBody.levelDamage *= 0.65f;         
                //
                ArchSoulWispBody.baseMaxHealth = 1000;  //1000
                ArchSoulWispBody.levelMaxHealth = 300;  //300
                ArchSoulWispBody.baseMoveSpeed *= 1.3f;
                ArchSoulWispBody.baseAcceleration *= 1.3f;
                ArchSoulWispBody.baseAttackSpeed = 1.3f;
                ArchSoulWispBody.baseRegen = 0f;
                ArchSoulWispBody.levelRegen = 0f;
                ArchSoulWispBody.baseDamage *= 0.65f;
                ArchSoulWispBody.levelDamage *= 0.65f;  
                //

 

                SoulGreaterWispMaster.AddComponent<MasterSuicideOnTimer>().lifeTimer = 22.5f;
                SoulArchWispMaster.AddComponent<MasterSuicideOnTimer>().lifeTimer = 30f;

                //
                R2API.ContentAddition.AddBody(SoulGreaterWispBody);
                R2API.ContentAddition.AddMaster(SoulGreaterWispMaster);
                R2API.ContentAddition.AddBody(SoulArchWispBody);
                R2API.ContentAddition.AddMaster(SoulArchWispMaster);
            }
        }

        public static CharacterMaster SoulSpawnGreaterUniversal(On.RoR2.MasterSummon.orig_Perform orig, global::RoR2.MasterSummon self)
        {
            //if (self.masterPrefab == GlobalEventManager.CommonAssets.wispSoulMasterPrefabMasterComponent.gameObject)
            //Idk how this comparison works so I have no idea how optimal this is
            if (self.masterPrefab == SoulLesserWispMaster && self.summonerBodyObject)
            {
                CharacterBody victimBody = self.summonerBodyObject.GetComponent<CharacterBody>();
                uint SoulMoney;         
                if (victimBody.bodyIndex == IndexAffixHealingCore || victimBody.bodyIndex == SoulGreaterWispIndex || victimBody.bodyIndex == SoulArchWispIndex)
                {
                    return null;
                }
                else if (victimBody.isChampion || victimBody.inventory.GetItemCount(RoR2Content.Items.InvadingDoppelganger) > 0)
                {
                    self.masterPrefab = SoulArchWispMaster;
                    SoulMoney = victimBody.master.money;
                }
                else if (victimBody.baseMaxHealth > 475) //520 cuts off after Geep, 500 is Geep, 480 is most minibosses
                {
                    self.masterPrefab = SoulGreaterWispMaster;
                    SoulMoney = victimBody.master.money / 2;
                }
                else
                {
                    SoulMoney = victimBody.master.money / 3;
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