using R2API;
using RoR2;
using RoR2.Artifacts;
using RoR2.UI.LogBook;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace VanillaArtifactsPlus
{
    public class Enigma
    {
        public static ItemTierDef artifactItemTier = null;
        public static ItemDef enigmaFragmentDef = null;
        public static ItemDef eliteEnigmaFragmentDef = null;
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


            Addressables.LoadAssetAsync<BuffDef>(key: "0cabe49fb119dcf4188d2a8c2e65edd2").WaitForCompletion().eliteDef = null;
        }

        public static void MakeEnigmaFragment()
        {
           
            #region Artifact Tier
            artifactItemTier = ScriptableObject.CreateInstance<ItemTierDef>();
            artifactItemTier.name = "ArtifactItemsItemTierDef";
            artifactItemTier.isDroppable = false;
            if (WConfig.DisplayEnigmaInLog.Value)
            {
                artifactItemTier.isDroppable = true;
            }
            artifactItemTier.canScrap = false;
            artifactItemTier.canRestack = true;
            artifactItemTier.colorIndex = ColorCatalog.ColorIndex.Artifact;
            artifactItemTier.darkColorIndex = ColorCatalog.ColorIndex.Artifact;
            artifactItemTier.tier = ItemTier.AssignedAtRuntime;
            artifactItemTier.bgIconTexture = Assets.Bundle.LoadAsset<Texture2D>("Assets/ArtifactsVanilla/artifactBg.png");
            R2API.ContentAddition.AddItemTierDef(artifactItemTier);

            #endregion
            GameObject pickupEnigma = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/PickupModels/Artifacts/PickupEnigma"), "PickupEnigmaFragment", false);
            GameObject cameraPos = new GameObject("cameraPos");
            GameObject focusPoint = new GameObject("focusPoint");
            cameraPos.transform.SetParent(pickupEnigma.transform, false);
            focusPoint.transform.SetParent(pickupEnigma.transform, false);
            cameraPos.transform.localPosition = new Vector3(-1.5f, 1f, 3f);

            ModelPanelParameters panel = pickupEnigma.AddComponent<ModelPanelParameters>();
            panel.cameraPositionTransform = cameraPos.transform;
            panel.focusPointTransform = focusPoint.transform;

            GameObject pickupEliteEnigma = PrefabAPI.InstantiateClone(pickupEnigma, "PickupEnigmaFragmentElite", false);


            enigmaFragmentDef = ScriptableObject.CreateInstance<ItemDef>();
            enigmaFragmentDef.name = "EnigmaFragment";
            enigmaFragmentDef.nameToken = "ITEM_ENIGMAFRAGMENT_NAME";
            enigmaFragmentDef.pickupToken = "ITEM_ENIGMAFRAGMENT_PICKUP";
            enigmaFragmentDef.descriptionToken = "ITEM_ENIGMAFRAGMENT_DESC";
            enigmaFragmentDef.pickupIconSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/ArtifactsVanilla/enigmaFragment.png");
            enigmaFragmentDef.pickupModelPrefab = pickupEnigma;
            enigmaFragmentDef.loreToken = "";
            enigmaFragmentDef.hidden = false;
            enigmaFragmentDef.canRemove = false;
            
            enigmaFragmentDef.deprecatedTier = ItemTier.NoTier;
            ItemAPI.Add(new CustomItem(enigmaFragmentDef, Array.Empty<ItemDisplayRule>()));

            On.RoR2.Inventory.CalculateEquipmentCooldownScale += EnigmaCooldownReduction;

            #region Elite
            Material matElite = GameObject.Instantiate(Addressables.LoadAssetAsync<Material>(key: "926e293c755c1964bb9a6612e94e76df").WaitForCompletion());
            MeshRenderer mesh = pickupEliteEnigma.transform.GetChild(0).GetComponent<MeshRenderer>();
            matElite.SetTexture("_NormalTex", mesh.material.GetTexture("_NormalTex"));
            matElite.SetFloat("_NormalStrength", 2);
            mesh.material = matElite;


            eliteEnigmaFragmentDef = ScriptableObject.CreateInstance<ItemDef>();
            eliteEnigmaFragmentDef.name = "EnigmaFragmentElite";
            eliteEnigmaFragmentDef.nameToken = "ITEM_ENIGMAFRAGMENTELITE_NAME";
            eliteEnigmaFragmentDef.pickupToken = "ITEM_ENIGMAFRAGMENTELITE_PICKUP";
            eliteEnigmaFragmentDef.descriptionToken = "ITEM_ENIGMAFRAGMENTELITE_DESC";
            eliteEnigmaFragmentDef.pickupIconSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/ArtifactsVanilla/enigmaFragmentElite.png");
            eliteEnigmaFragmentDef.pickupModelPrefab = pickupEliteEnigma;
            eliteEnigmaFragmentDef.loreToken = "";
            eliteEnigmaFragmentDef.hidden = false;
            eliteEnigmaFragmentDef.canRemove = false;
     
            eliteEnigmaFragmentDef.deprecatedTier = ItemTier.NoTier;
            ItemAPI.Add(new CustomItem(eliteEnigmaFragmentDef, Array.Empty<ItemDisplayRule>()));
 
            EquipmentSlot.onServerEquipmentActivated += EquipmentSlot_onServerEquipmentActivated;
            #endregion

           
            ItemTierCatalog.availability.CallWhenAvailable(SetTier);
            ArtifactCatalog.availability.CallWhenAvailable(SetLang);
           

        }

        public static void SetLang()
        {
            RoR2Content.Artifacts.Enigma.descriptionToken = "ARTIFACT_ENIGMA_NEW_DESCRIPTION";
        }

        public static void SetTier()
        {
            enigmaFragmentDef._itemTierDef = artifactItemTier;
            eliteEnigmaFragmentDef._itemTierDef = artifactItemTier;
            artifactItemTier.dropletDisplayPrefab = LegacyResourcesAPI.LoadAsync<GameObject>("Prefabs/ItemPickups/EquipmentOrb").WaitForCompletion();
        }

        private static void EquipmentSlot_onServerEquipmentActivated(EquipmentSlot self, EquipmentIndex eq)
        {
            if (self.characterBody && self.inventory)
            {
                int itemCount = self.inventory.GetItemCount(eliteEnigmaFragmentDef);
                if (itemCount > 0)
                {
                    EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(eq);
                    
                    BuffIndex def = BuffCatalog.eliteBuffIndices[self.rng.RangeInt(0, BuffCatalog.eliteBuffIndices.Length)];
                    if (def != BuffIndex.None)
                    {
                        self.characterBody.AddTimedBuff(def, equipmentDef.cooldown * 0.75f * itemCount);
                    }
                }
            }
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
            int reduc = (int)((1 - EnigmaFragmentCooldownReduction) * 100);
            LanguageAPI.Add("ITEM_ENIGMAFRAGMENT_DESC", string.Format(Language.GetString("ITEM_ENIGMAFRAGMENT_DESC"), reduc));



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

            if (enigmaFragmentDef != null)
            {
                On.RoR2.UI.LogBook.LogBookController.BuildPickupEntries += LogBookController_BuildPickupEntries;
                On.RoR2.UI.LogBook.LogBookController.GetPickupStatus += LogBookController_GetPickupStatus;
            }
           
            
        }

        private static RoR2.UI.LogBook.EntryStatus LogBookController_GetPickupStatus(On.RoR2.UI.LogBook.LogBookController.orig_GetPickupStatus orig, ref RoR2.UI.LogBook.Entry entry, UserProfile viewerProfile)
        {
            PickupIndex pickupIndex = (PickupIndex)entry.extraData;
            PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
            ItemIndex itemIndex = (pickupDef != null) ? pickupDef.itemIndex : ItemIndex.None;
            if (itemIndex != ItemIndex.None)
            {
                ItemDef def = ItemCatalog.GetItemDef(itemIndex);
                if (def._itemTierDef == artifactItemTier)
                {
                    if (viewerProfile.HasUnlockable(def.unlockableDef))
                    {
                        return EntryStatus.Available;
                    }
                }
            }
            return orig(ref entry, viewerProfile);
        }

        private static RoR2.UI.LogBook.Entry[] LogBookController_BuildPickupEntries(On.RoR2.UI.LogBook.LogBookController.orig_BuildPickupEntries orig, System.Collections.Generic.Dictionary<RoR2.ExpansionManagement.ExpansionDef, bool> expansionAvailability)
        {
            if (enigmaFragmentDef)
            {
                //set late as to not make the achievement an item unlock
                UnlockableDef enigmaUnlock = Addressables.LoadAssetAsync<UnlockableDef>(key: "5f4704377f63edd48930460f5aa7a964").WaitForCompletion();
                enigmaFragmentDef.unlockableDef = enigmaUnlock;
                eliteEnigmaFragmentDef.unlockableDef = enigmaUnlock;
                PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex(enigmaFragmentDef.itemIndex)).unlockableDef = enigmaUnlock;
                PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex(eliteEnigmaFragmentDef.itemIndex)).unlockableDef = enigmaUnlock;

            }
            var temp = orig(expansionAvailability);
            artifactItemTier.isDroppable = false;
            return temp;
        }

        public static GenericPickupController EnigmaFragmentMaker(On.RoR2.GenericPickupController.orig_CreatePickup orig, ref GenericPickupController.CreatePickupInfo createPickupInfo)
        {
            //So Enigma in RoRR just replaces the Equipments when dropping
            PickupDef tempPickup = PickupCatalog.GetPickupDef(createPickupInfo.pickupIndex);
            if (tempPickup != null && tempPickup.equipmentIndex != EquipmentIndex.None)
            {
                EquipmentDef tempEquip = EquipmentCatalog.GetEquipmentDef(tempPickup.equipmentIndex);
                bool elite = tempEquip.passiveBuffDef && tempEquip.passiveBuffDef.isElite;
                bool change = false;
                if (elite)
                {
                    change = true;
                    createPickupInfo.pickupIndex = PickupCatalog.FindPickupIndex(eliteEnigmaFragmentDef.itemIndex);
                }
                else if (tempEquip.enigmaCompatible)
                {
                    change = true;
                    createPickupInfo.pickupIndex = PickupCatalog.FindPickupIndex(enigmaFragmentDef.itemIndex);
                }
                GenericPickupController tempGPC = orig(ref createPickupInfo);
                if (change && tempGPC.gameObject && tempGPC.pickupDisplay)
                {
                    RoR2.Util.PlaySound("Play_item_proc_crit_cooldown", tempGPC.gameObject);
                    if (elite)
                    {
                        //RoR2.Util.PlaySound("Play_UI_item_spawn_tier3", tempGPC.gameObject);
                        tempGPC.pickupDisplay.tier3ParticleEffect.SetActive(true);
                        tempGPC.pickupDisplay.bossParticleEffect.SetActive(true);
                    }
                    else
                    {
                       // RoR2.Util.PlaySound("Play_UI_item_spawn_tier1", tempGPC.gameObject);
                        tempGPC.pickupDisplay.tier1ParticleEffect.SetActive(true);
                    }                    
                    tempGPC.pickupDisplay.equipmentParticleEffect.SetActive(true);
                }
                return tempGPC;
            }
            return orig(ref createPickupInfo);
        }


        public static void EnigmaEquipmentGranter(On.RoR2.GenericPickupController.orig_AttemptGrant orig, global::RoR2.GenericPickupController self, global::RoR2.CharacterBody body)
        {
            EquipmentIndex eq = self.pickupIndex.equipmentIndex;
            if (eq == EquipmentIndex.None)
            {
                orig(self, body);
                return;
            }
            bool hasAnEquip = body.inventory.currentEquipmentIndex != EquipmentIndex.None;

            if (!hasAnEquip && PickupCatalog.GetPickupDef(self.pickupIndex).itemIndex == enigmaFragmentDef.itemIndex)
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

           
            orig(self, body);
            if (hasAnEquip)
            {
                EquipmentDef tempA = EquipmentCatalog.GetEquipmentDef(eq);
                EquipmentIndex index = RoR2Content.Equipment.Fruit.equipmentIndex;
                if (tempA.passiveBuffDef && tempA.passiveBuffDef.isElite)
                {
                    index = RoR2Content.Equipment.AffixRed.equipmentIndex;
                }
                GenericPickupController.CreatePickup(new GenericPickupController.CreatePickupInfo
                {
                    position = self.transform.position,
                    rotation = self.transform.rotation,
                    pickupIndex = PickupCatalog.FindPickupIndex(index)
                });
                GameObject.Destroy(self.gameObject);
            }
        }

        
    }
}