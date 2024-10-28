using R2API;
using RoR2;
using UnityEngine;

namespace FixedspawnDissonance
{
    public class Soul
    {
        public static GameObject SoulLesserWispBody = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/WispSoulBody");
        public static GameObject SoulLesserWispMaster = RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/WispSoulMaster");

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

            Texture2D TexSoulWisp = new Texture2D(128, 128, TextureFormat.DXT5, false);
            TexSoulWisp.LoadImage(Properties.Resources.texBodyWispSoul, true);
            TexSoulWisp.wrapMode = TextureWrapMode.Clamp;

            SoulWispBody.baseMaxHealth = 50;  //35 base
            SoulWispBody.levelMaxHealth = 50; //10 base
            SoulWispBody.baseMoveSpeed *= 1.25f;
            SoulWispBody.baseAcceleration *= 1.25f;
            SoulWispBody.baseRegen = 0f;
            SoulWispBody.levelRegen = 0f;
            SoulWispBody.baseDamage *= 0.65f;
            SoulWispBody.levelDamage *= 0.65f;
            SoulWispBody.baseAttackSpeed *= 1.1f;
            //SoulWispBody.bodyFlags = CharacterBody.BodyFlags.OverheatImmune | CharacterBody.BodyFlags.ResistantToAOE | CharacterBody.BodyFlags.ImmuneToGoo | CharacterBody.BodyFlags.ImmuneToVoidDeath;
            SoulWispBody.autoCalculateLevelStats = false;

            GivePickupsOnStart.ItemInfo Cooldown = new GivePickupsOnStart.ItemInfo { itemString = ("AlienHead"), count = 1, };
            GivePickupsOnStart.ItemInfo DeathMark = new GivePickupsOnStart.ItemInfo { itemString = ("DeathMark"), count = 2, };
            //GivePickupsOnStart.ItemInfo StunGrenade = new GivePickupsOnStart.ItemInfo { itemString = ("StunChanceOnHit"), count = 20, };
            GivePickupsOnStart.ItemInfo SlowOnHit1 = new GivePickupsOnStart.ItemInfo { itemString = ("SlowOnHit"), count = 1, };
            //GivePickupsOnStart.ItemInfo SlowOnHit2 = new GivePickupsOnStart.ItemInfo { itemString = ("SlowOnHit"), count = 2, };
            GivePickupsOnStart.ItemInfo[] SoulStartingItemInfo = new GivePickupsOnStart.ItemInfo[0];
            //GivePickupsOnStart.ItemInfo[] Soul2StartingItemInfo = new GivePickupsOnStart.ItemInfo[0];

            SoulStartingItemInfo = SoulStartingItemInfo.Add(Cooldown, SlowOnHit1, DeathMark);
            //Soul2StartingItemInfo = Soul2StartingItemInfo.Add(Cooldown, SlowOnHit2, DeathMark);

            SoulLesserWispMaster.AddComponent<GivePickupsOnStart>().itemInfos = SoulStartingItemInfo;
            SoulLesserWispMaster.AddComponent<MasterSuicideOnTimer>().lifeTimer = 15f;

            if (!WConfig.DisableNewContent.Value)
            {

                SoulGreaterWispBody = R2API.PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/GreaterWispBody"), "GreaterWispSoulBody", true);
                SoulGreaterWispMaster = R2API.PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/GreaterWispMaster"), "GreaterWispSoulMaster", true);

                SoulArchWispBody = R2API.PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/ArchWispBody"), "SoulArchWispBody", true);
                SoulArchWispMaster = R2API.PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterMasters/ArchWispMaster"), "SoulArchWispMaster", true);



                Texture2D texRampWispSoulAlt = new Texture2D(256, 16, TextureFormat.DXT5, false);
                texRampWispSoulAlt.LoadImage(Properties.Resources.texRampWispSoulAlt, true);
                texRampWispSoulAlt.filterMode = FilterMode.Point;
                texRampWispSoulAlt.wrapMode = TextureWrapMode.Clamp;

                Texture2D texRampWispSoulAlt2 = new Texture2D(256, 16, TextureFormat.DXT5, false);
                texRampWispSoulAlt2.LoadImage(Properties.Resources.texRampWispSoulAlt2, true);
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

                Texture2D TexGreaterSoulWisp = new Texture2D(128, 128, TextureFormat.DXT5, false);
                TexGreaterSoulWisp.LoadImage(Properties.Resources.texBodyGreaterWispSoul, true);
                TexGreaterSoulWisp.wrapMode = TextureWrapMode.Clamp;

                Texture2D texBodyArchSoul = new Texture2D(128, 128, TextureFormat.DXT5, false);
                texBodyArchSoul.LoadImage(Properties.Resources.texBodyArchSoul, true);
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
                GreaterSoulWispBody.baseMaxHealth = 500; //750
                GreaterSoulWispBody.levelMaxHealth = 500;   //225
                GreaterSoulWispBody.baseMoveSpeed *= 1.5f;
                GreaterSoulWispBody.baseAcceleration *= 1.5f;
                GreaterSoulWispBody.baseRegen = 0f;
                GreaterSoulWispBody.levelRegen = 0f;
                GreaterSoulWispBody.baseDamage *= 0.65f;
                GreaterSoulWispBody.levelDamage *= 0.65f;
                GreaterSoulWispBody.baseAttackSpeed *= 1.2f;
                //GreaterSoulWispBody.bodyFlags = CharacterBody.BodyFlags.OverheatImmune | CharacterBody.BodyFlags.ResistantToAOE | CharacterBody.BodyFlags.ImmuneToGoo | CharacterBody.BodyFlags.ImmuneToVoidDeath;
                GreaterSoulWispBody.autoCalculateLevelStats = false;
                //
                ArchSoulWispBody.baseMaxHealth = 1500;       //1500
                ArchSoulWispBody.levelMaxHealth = 1500;   //300
                ArchSoulWispBody.baseMoveSpeed *= 1.75f;
                ArchSoulWispBody.baseAcceleration *= 1.75f;
                ArchSoulWispBody.baseRegen = 0f;
                ArchSoulWispBody.levelRegen = 0f;
                ArchSoulWispBody.baseDamage *= 0.65f;
                ArchSoulWispBody.levelDamage *= 0.65f;
                ArchSoulWispBody.baseAttackSpeed *= 1.35f;
                //ArchSoulWispBody.bodyFlags = CharacterBody.BodyFlags.OverheatImmune | CharacterBody.BodyFlags.ResistantToAOE | CharacterBody.BodyFlags.ImmuneToGoo | CharacterBody.BodyFlags.ImmuneToVoidDeath;
                ArchSoulWispBody.autoCalculateLevelStats = false;
                //


                SoulGreaterWispMaster.AddComponent<GivePickupsOnStart>().itemInfos = SoulStartingItemInfo;
                SoulArchWispMaster.AddComponent<GivePickupsOnStart>().itemInfos = SoulStartingItemInfo;


                SoulGreaterWispMaster.AddComponent<MasterSuicideOnTimer>().lifeTimer = 25f;
                SoulArchWispMaster.AddComponent<MasterSuicideOnTimer>().lifeTimer = 40f;

                //
                R2API.ContentAddition.AddBody(SoulGreaterWispBody);
                R2API.ContentAddition.AddMaster(SoulGreaterWispMaster);
                R2API.ContentAddition.AddBody(SoulArchWispBody);
                R2API.ContentAddition.AddMaster(SoulArchWispMaster);
            }
        }

        public static RoR2.CharacterMaster SoulSpawnGreaterUniversal(On.RoR2.MasterSummon.orig_Perform orig, global::RoR2.MasterSummon self)
        {
            //if (self.masterPrefab == GlobalEventManager.CommonAssets.wispSoulMasterPrefabMasterComponent.gameObject)
            //Idk how this comparison works so I have no idea how optimal this is
            if (self.masterPrefab == SoulLesserWispMaster && self.summonerBodyObject)
            {
                CharacterBody victimBody = self.summonerBodyObject.GetComponent<CharacterBody>();

                uint SoulMoney;
                self.inventoryToCopy = victimBody.inventory;
                if (victimBody.bodyIndex == SoulGreaterWispIndex || victimBody.bodyIndex == SoulArchWispIndex || victimBody.bodyIndex == IndexAffixHealingCore)
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

                CharacterMaster tempmaster = orig(self);
                tempmaster.money = SoulMoney;
                return tempmaster;
            }
            return orig(self);
        }


    }
}