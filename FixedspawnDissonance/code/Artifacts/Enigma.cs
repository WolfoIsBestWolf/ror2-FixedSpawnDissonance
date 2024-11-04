using R2API;
using RoR2;
using RoR2.Artifacts;
using UnityEngine;

namespace FixedspawnDissonance
{
    public class Enigma
    {
        public static ItemDef EnigmaFragmentPurple = null;
        public static float EnigmaFragmentCooldownReduction = 0.88f;
        public static Color CustomColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Artifact);

        public static void Start()
        {
            //On.RoR2.UI.GenericNotification.SetItem += PickupItemNotification;
            ColorUtility.TryParseHtmlString("#AE6BCB", out CustomColor);



            if (WConfig.EnigmaInterrupt.Value == false)
            {
                RoR2.LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/BossHunter").enigmaCompatible = false;
                RoR2.LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/BossHunterConsumed").enigmaCompatible = false;
                RoR2.LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/Recycle").enigmaCompatible = false;
            }

            if (WConfig.EnigmaMovement.Value == true)
            {
                RoR2.LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/Jetpack").enigmaCompatible = true;
                RoR2.LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/FireBallDash").enigmaCompatible = true;
            }
            else
            {
                RoR2.LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/Jetpack").enigmaCompatible = false;
                RoR2.LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/FireBallDash").enigmaCompatible = false;
            }

            RoR2.LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/Tonic").enigmaCompatible = false;
            RoR2.LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/Meteor").enigmaCompatible = false;
            RoR2.LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/BurnNearby").enigmaCompatible = false;
            RoR2.LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/CrippleWard").enigmaCompatible = true;

            if (!WConfig.DisableNewContent.Value)
            {
                MakeEnigmaFragment();
            }
        }

        public static void MakeEnigmaFragment()
        {
            //var EnigmaArtifactDisplay = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex("ArtifactIndex.Enigma")).displayPrefab;
  
            ArtifactDef EnigmaArtifactDefTemp = RoR2.LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/Enigma");
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
            EnigmaFragmentPurple = EnigmaFragment;

            EnigmaFragmentCooldownReduction = 1 - WConfig.EnigmaCooldownReduction.Value / 100;

            On.RoR2.Inventory.CalculateEquipmentCooldownScale += (orig, self) =>
            {
                int itemCount = self.GetItemCount(EnigmaFragmentPurple);
                if (itemCount > 0)
                {
                    float tempfloat = orig(self);

                    tempfloat *= Mathf.Pow(EnigmaFragmentCooldownReduction, (float)itemCount);

                    return tempfloat;
                }
                return orig(self);
            };
        }

        public static void EnigmaCallLate()
        {
            if (!WConfig.DisableNewContent.Value)
            {
                //Idk why I call this late but I dont wanna bother finding out
                Texture2D texItemEnigmaP = Assets.Bundle.LoadAsset<Texture2D>("Assets/ArtifactsVanilla/texItemEnigmaP.png");
                texItemEnigmaP.wrapMode = TextureWrapMode.Clamp;
                Sprite texItemEnigmaPS = Sprite.Create(texItemEnigmaP, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));

                EnigmaFragmentPurple.pickupIconSprite = texItemEnigmaPS;


                PickupDef boostequipdef = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex("ItemIndex.EnigmaFragment_ArtifactHelper"));

                boostequipdef.baseColor = CustomColor;
                boostequipdef.darkColor = CustomColor;

                boostequipdef.displayPrefab = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex("ArtifactIndex.Enigma")).displayPrefab;
                boostequipdef.dropletDisplayPrefab = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex("EquipmentIndex.Fruit")).dropletDisplayPrefab;
                boostequipdef.iconSprite = texItemEnigmaPS;
            }


            //Mod Support
            EquipmentDef tempDef = EquipmentCatalog.GetEquipmentDef(EquipmentCatalog.FindEquipmentIndex("EQUIPMENT_IMPULSEFROSTSHIELD"));
            if (tempDef != null)
            {
                tempDef.enigmaCompatible = false;
            }
        }

        public static GenericPickupController EnigmaFragmentMaker(On.RoR2.GenericPickupController.orig_CreatePickup orig, ref GenericPickupController.CreatePickupInfo createPickupInfo)
        {
            //So Enigma in RoRR just replaces the Equipments when dropping
            PickupDef tempPickup = PickupCatalog.GetPickupDef(createPickupInfo.pickupIndex);
            if (tempPickup != null && tempPickup.equipmentIndex != EquipmentIndex.None)
            {
                EquipmentDef tempEquip = EquipmentCatalog.GetEquipmentDef(tempPickup.equipmentIndex);
                //Only transform Equipment that you'd get anyways but not Recyclers/BossHunter
                //If Elite Equipment drop 15 Wake of Vultures real?
                if (tempEquip.enigmaCompatible)
                {
                    createPickupInfo.pickupIndex = PickupCatalog.FindPickupIndex(EnigmaFragmentPurple.itemIndex);
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
            if (body.inventory.currentEquipmentIndex == EquipmentIndex.None && PickupCatalog.GetPickupDef(self.pickupIndex).itemIndex == EnigmaFragmentPurple.itemIndex)
            {
                self.pickupIndex = PickupCatalog.FindPickupIndex(EnigmaArtifactManager.GetRandomEquipment(EnigmaArtifactManager.serverInitialEquipmentRng, 0));
                RoR2.Util.PlaySound("Play_UI_insufficient_funds", self.gameObject);
                RoR2.Util.PlaySound("Play_UI_insufficient_funds", self.gameObject);
                RoR2.Util.PlaySound("Play_UI_insufficient_funds", self.gameObject);
                /*if (body.master.hasAuthority)
                {
                    CharacterMasterNotificationQueue notificationQueueForMaster = CharacterMasterNotificationQueue.GetNotificationQueueForMaster(body.master);
                    CharacterMasterNotificationQueue.TransformationInfo transformation = new CharacterMasterNotificationQueue.TransformationInfo(CharacterMasterNotificationQueue.TransformationType.Default, EnigmaFragmentPurple);
                    CharacterMasterNotificationQueue.NotificationInfo info = new CharacterMasterNotificationQueue.NotificationInfo(EquipmentCatalog.GetEquipmentDef(self.pickupIndex.pickupDef.equipmentIndex), transformation);
                    notificationQueueForMaster.PushNotification(info, 6f);
                    return;
                }*/
            }
            bool hasEquip = body.inventory.currentEquipmentIndex != EquipmentIndex.None && PickupCatalog.GetPickupDef(self.pickupIndex).equipmentIndex != EquipmentIndex.None;
            orig(self, body);
            //Debug.LogWarning(replace);
            if (hasEquip)
            {
                if (PickupCatalog.GetPickupDef(self.pickupIndex).equipmentIndex != EquipmentIndex.None)
                {
                    EquipmentDef tempA = EquipmentCatalog.GetEquipmentDef(PickupCatalog.GetPickupDef(self.pickupIndex).equipmentIndex);
                    if (tempA.passiveBuffDef && tempA.passiveBuffDef.isElite)
                    {
                        RoR2.GenericPickupController.CreatePickup(new GenericPickupController.CreatePickupInfo
                        {
                            position = self.transform.position,
                            rotation = self.transform.rotation,
                            pickupIndex = PickupCatalog.FindPickupIndex(RoR2Content.Items.ShinyPearl.itemIndex)
                        });
                        RoR2.Util.PlaySound("Play_item_proc_crit_cooldown", self.gameObject);
                        RoR2.Util.PlaySound("Play_item_proc_crit_cooldown", self.gameObject);
                        RoR2.Util.PlaySound("Play_item_proc_crit_cooldown", self.gameObject);
                    }
                    else
                    {
                        RoR2.GenericPickupController.CreatePickup(new GenericPickupController.CreatePickupInfo
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

        public static void PickupItemNotification(On.RoR2.UI.GenericNotification.orig_SetItem orig, RoR2.UI.GenericNotification self, global::RoR2.ItemDef itemDef)
        {
            orig(self, itemDef);

            if (itemDef == EnigmaFragmentPurple)
            {
                self.titleTMP.color = CustomColor;
            }
        }
    }
}