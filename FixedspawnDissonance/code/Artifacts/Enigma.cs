using R2API;
using RoR2;
using RoR2.Artifacts;
using UnityEngine;

namespace FixedspawnDissonance
{
    public class Enigma
    {
        public static ItemDef enigmaFragmentDef = null;
        public static float EnigmaFragmentCooldownReduction = 0.88f;
        public static Color CustomColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Artifact);

        public static void Start()
        {
            //Lightning and Volcanic Egg both are interuptive but are okay
            if (WConfig.EnigmaInterrupt.Value == false)
            {
                LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/BossHunter").enigmaCompatible = false;
                LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/BossHunterConsumed").enigmaCompatible = false;
                LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/Recycle").enigmaCompatible = false;
            }

            if (WConfig.EnigmaMovement.Value == true)
            {
                LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/Jetpack").enigmaCompatible = true;
                LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/FireBallDash").enigmaCompatible = true;
            }
            else
            {
                LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/Jetpack").enigmaCompatible = false;
                LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/FireBallDash").enigmaCompatible = false;
            }

            LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/Tonic").enigmaCompatible = false;
            LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/Meteor").enigmaCompatible = false;
            LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/BurnNearby").enigmaCompatible = false;
            LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/CrippleWard").enigmaCompatible = true;

          
        }

        public static void MakeEnigmaFragment()
        {
            //var EnigmaArtifactDisplay = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex("ArtifactIndex.Enigma")).displayPrefab;

            ArtifactDef EnigmaArtifactDefTemp = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/Enigma");
            ItemDef EnigmaFragment = ScriptableObject.CreateInstance<ItemDef>();

            EnigmaFragment.name = "EnigmaFragment_ArtifactHelper";
            EnigmaFragment.deprecatedTier = ItemTier.NoTier;
            EnigmaFragment.pickupModelPrefab = EnigmaArtifactDefTemp.pickupModelPrefab;
            //EnigmaFragment.pickupIconSprite = ;
            EnigmaFragment.nameToken = "ITEM_ENIGMAEQUIPMENTBOOST_NAME";
            EnigmaFragment.pickupToken = "ITEM_ENIGMAEQUIPMENTBOOST_PICKUP";
            EnigmaFragment.descriptionToken = "ITEM_ENIGMAEQUIPMENTBOOST_DESC";
            EnigmaFragment.loreToken = "";
            EnigmaFragment.hidden = false;
            EnigmaFragment.canRemove = false;
            EnigmaFragment.tags = new ItemTag[]
            {
                ItemTag.WorldUnique
            };
            ItemDisplayRule[] def = new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                }
            };
            CustomItem customItemEnigmaFragment = new CustomItem(EnigmaFragment, new ItemDisplayRule[0]);
            ItemAPI.Add(customItemEnigmaFragment);
            enigmaFragmentDef = EnigmaFragment;

            EnigmaFragmentCooldownReduction = 1 - WConfig.EnigmaCooldownReduction.Value / 100;

            On.RoR2.Inventory.CalculateEquipmentCooldownScale += EnigmaCooldownReduction;
           
        }

        private static float EnigmaCooldownReduction(On.RoR2.Inventory.orig_CalculateEquipmentCooldownScale orig, Inventory self)
        {
            int itemCount = self.GetItemCount(enigmaFragmentDef);
            if (itemCount > 0)
            {
                float tempfloat = orig(self);

                tempfloat *= Mathf.Pow(EnigmaFragmentCooldownReduction, (float)itemCount);

                return tempfloat;
            }
            return orig(self);
        }

        public static void CallLate()
        {
            EquipmentDef tempDef = EquipmentCatalog.GetEquipmentDef(EquipmentCatalog.FindEquipmentIndex("EQUIPMENT_IMPULSEFROSTSHIELD"));
            if (tempDef != null)
            {
                tempDef.enigmaCompatible = false;
                EquipmentCatalog.enigmaEquipmentList.Remove(tempDef.equipmentIndex);
            }
            tempDef = EquipmentCatalog.GetEquipmentDef(EquipmentCatalog.FindEquipmentIndex("EQUIPMENT_REUSER"));
            if (tempDef != null)
            {
                tempDef.enigmaCompatible = false;
                EquipmentCatalog.enigmaEquipmentList.Remove(tempDef.equipmentIndex);
            }
            tempDef = EquipmentCatalog.GetEquipmentDef(EquipmentCatalog.FindEquipmentIndex("EQUIPMENT_Reducer"));
            if (tempDef != null)
            {
                tempDef.enigmaCompatible = false;
                EquipmentCatalog.enigmaEquipmentList.Remove(tempDef.equipmentIndex);
            }


            if (enigmaFragmentDef)
            {
                LanguageAPI.Add("ITEM_ENIGMAEQUIPMENTBOOST_DESC", string.Format(Language.GetString("ITEM_ENIGMAEQUIPMENTBOOST_DESC"), WConfig.EnigmaCooldownReduction.Value, WConfig.EnigmaCooldownReduction.Value));


                //Idk why I call this late but I dont wanna bother finding out
                Texture2D texItemEnigmaP = Assets.Bundle.LoadAsset<Texture2D>("Assets/ArtifactsVanilla/texItemEnigmaP.png");
                texItemEnigmaP.wrapMode = TextureWrapMode.Clamp;
                Sprite texItemEnigmaPS = Sprite.Create(texItemEnigmaP, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));

                enigmaFragmentDef.pickupIconSprite = texItemEnigmaPS;


                PickupDef boostequipdef = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex("ItemIndex.EnigmaFragment_ArtifactHelper"));
                ColorUtility.TryParseHtmlString("#AE6BCB", out CustomColor);
                boostequipdef.baseColor = CustomColor;
                boostequipdef.darkColor = CustomColor;

                boostequipdef.displayPrefab = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex("ArtifactIndex.Enigma")).displayPrefab;
                boostequipdef.dropletDisplayPrefab = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex("EquipmentIndex.Fruit")).dropletDisplayPrefab;
                boostequipdef.iconSprite = texItemEnigmaPS;
            }
        }

        public static GenericPickupController EnigmaFragmentMaker(On.RoR2.GenericPickupController.orig_CreatePickup orig, ref GenericPickupController.CreatePickupInfo createPickupInfo)
        {
            //So Enigma in RoRR just replaces the Equipments when dropping
            PickupDef tempPickup = PickupCatalog.GetPickupDef(createPickupInfo.pickupIndex);
            if (tempPickup != null && tempPickup.equipmentIndex != EquipmentIndex.None)
            {
                EquipmentDef tempEquip = EquipmentCatalog.GetEquipmentDef(tempPickup.equipmentIndex);
                if (tempEquip.enigmaCompatible)
                {
                    createPickupInfo.pickupIndex = PickupCatalog.FindPickupIndex(enigmaFragmentDef.itemIndex);
                    GenericPickupController tempGPC = orig(ref createPickupInfo);
                    if (tempGPC.gameObject && tempGPC.pickupDisplay)
                    {
                        RoR2.Util.PlaySound("Play_item_proc_crit_cooldown", tempGPC.gameObject);
                        RoR2.Util.PlaySound("Play_item_proc_crit_cooldown", tempGPC.gameObject);

                        tempGPC.pickupDisplay.tier1ParticleEffect.SetActive(true);
                        tempGPC.pickupDisplay.equipmentParticleEffect.SetActive(true);
                    }
                    return tempGPC;
                }
            }

            return orig(ref createPickupInfo);
        }


        public static void EnigmaEquipmentGranter(On.RoR2.GenericPickupController.orig_AttemptGrant orig, global::RoR2.GenericPickupController self, global::RoR2.CharacterBody body)
        {
            if (body.inventory.currentEquipmentIndex == EquipmentIndex.None && PickupCatalog.GetPickupDef(self.pickupIndex).itemIndex == enigmaFragmentDef.itemIndex)
            {
                self.pickupIndex = PickupCatalog.FindPickupIndex(EnigmaArtifactManager.GetRandomEquipment(EnigmaArtifactManager.serverInitialEquipmentRng, 0));
                RoR2.Util.PlaySound("Play_UI_insufficient_funds", self.gameObject);
                RoR2.Util.PlaySound("Play_UI_insufficient_funds", self.gameObject);
                if (body.master.hasAuthority)
                {
                    CharacterMasterNotificationQueue notificationQueueForMaster = CharacterMasterNotificationQueue.GetNotificationQueueForMaster(body.master);
                    CharacterMasterNotificationQueue.TransformationInfo transformation = new CharacterMasterNotificationQueue.TransformationInfo(CharacterMasterNotificationQueue.TransformationType.Default, enigmaFragmentDef);
                    CharacterMasterNotificationQueue.NotificationInfo info = new CharacterMasterNotificationQueue.NotificationInfo(EquipmentCatalog.GetEquipmentDef(self.pickupIndex.pickupDef.equipmentIndex), transformation);
                    notificationQueueForMaster.PushNotification(info, 6f);
                }
            }
            var Pickup = PickupCatalog.GetPickupDef(self.pickupIndex).equipmentIndex;
            bool hasEquipisEquip = body.inventory.currentEquipmentIndex != EquipmentIndex.None && Pickup != EquipmentIndex.None;
            orig(self, body);
            if (hasEquipisEquip)
            {
                EquipmentDef tempA = EquipmentCatalog.GetEquipmentDef(Pickup);
                if (tempA.passiveBuffDef && tempA.passiveBuffDef.isElite)
                {
                    GenericPickupController.CreatePickup(new GenericPickupController.CreatePickupInfo
                    {
                        position = self.transform.position,
                        rotation = self.transform.rotation,
                        pickupIndex = PickupCatalog.FindPickupIndex(RoR2Content.Items.ShinyPearl.itemIndex)
                    });
                    RoR2.Util.PlaySound("Play_item_proc_crit_cooldown", self.gameObject);
                    RoR2.Util.PlaySound("Play_item_proc_crit_cooldown", self.gameObject);
                }
                else
                {
                    GenericPickupController.CreatePickup(new GenericPickupController.CreatePickupInfo
                    {
                        position = self.transform.position,
                        rotation = self.transform.rotation,
                        pickupIndex = PickupCatalog.FindPickupIndex(RoR2Content.Equipment.Fruit.equipmentIndex)
                    });
                }
                Object.Destroy(self.gameObject);
            }
        }

        
    }
}