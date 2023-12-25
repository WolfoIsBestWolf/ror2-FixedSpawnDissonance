/*

    //LanguageAPI.Add("ARTIFACT_ENIGMA_DESCRIPTION", "Spawn with a Enigma Box which activates random equipment. Equipment can be turned into equipment cooldown reduction", "en");
    //LanguageAPI.Add("ARTIFACT_ENIGMA_DESCRIPTION", "Spawn with a Enigma Box which activates random equipment. Duplicate Enigma Boxes will reduce cooldown.", "en");
                


                
                RoR2Content.Equipment.Enigma.pickupModelPrefab = EnigmaArtifactDisplay;
                RoR2Content.Equipment.Enigma.pickupIconSprite = texItemEnigmaOrangeS;
                RoR2Content.Equipment.Enigma.canDrop = false;
                RoR2Content.Equipment.Enigma.appearsInSinglePlayer = false;
                RoR2Content.Equipment.Enigma.appearsInMultiPlayer = false;
                RoR2Content.Equipment.Enigma.cooldown = EnigmaCooldown.Value;
                RoR2Content.Equipment.Enigma.nameToken = "EQUIPMENT_ENIGMAEQUIPMENT_NAME";
                RoR2Content.Equipment.Enigma.pickupToken = "EQUIPMENT_ENIGMAEQUIPMENT_PICKUP";
                RoR2Content.Equipment.Enigma.descriptionToken = "EQUIPMENT_ENIGMAEQUIPMENT_DESC";
                RoR2Content.Equipment.Lightning.enigmaCompatible = false;
                RoR2Content.Equipment.PassiveHealing.enigmaCompatible = false;


            if (EnigmaUpgrade.Value == true)
            {
                LanguageAPI.Add("ARTIFACT_ENIGMA_DESCRIPTION", "Spawn with a random equipment that changes every time it's activated. Additional equipment turned into cooldown reduction", "en");

                On.RoR2.Artifacts.EnigmaArtifactManager.OnServerEquipmentActivated += (orig, equipmentSlot, equipmentIndex) =>
                {
                    equipmentSlot.characterBody.inventory.SetEquipmentIndex(RoR2Content.Equipment.Enigma.equipmentIndex);
                };
            }



            //PickupDef enigmaequip = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex("EquipmentIndex.Enigma"));
            enigmaequip.displayPrefab = EnigmaArtifactDisplay;
            enigmaequip.iconSprite = texItemEnigmaOrangeS;
            EnigmaDisplayPurple = R2API.PrefabAPI.InstantiateClone(enigmaequip.displayPrefab, "PickupEnigmaPurple", false);
                EnigmaDisplayPurple.transform.GetChild(0).GetChild(0).GetComponent<Light>().color = new Color32(140, 114, 219, 255);
                MeshRenderer displaypurplerender = EnigmaDisplayPurple.transform.GetChild(1).GetChild(0).GetComponent<MeshRenderer>();
                displaypurplerender.material = Instantiate(displaypurplerender.material);
                displaypurplerender.material.SetColor("_TintColor", new Color(2, 1, 3, 1));

                boostequipdef.displayPrefab = EnigmaDisplayPurple;


On.RoR2.Artifacts.EnigmaArtifactManager.OnPlayerCharacterBodyStartServer += (orig, characterBody) =>
                {
                    if (characterBody.isPlayerControlled == true)
                    {
                        Inventory inventory = characterBody.inventory;
                        //int val = (characterBody.bodyIndex == BodyCatalog.FindBodyIndex("ToolbotBody")) ? 2 : 1;
                        //int num = inventory.GetEquipmentSlotCount();
                        int num = 1;
                        for (int i = 0; i < num; i++)
                        {
                            //if (inventory.GetEquipment((uint)i).equipmentIndex == EquipmentIndex.None)

                            EquipmentState equipmentState = new EquipmentState(RoR2Content.Equipment.Enigma.equipmentIndex, Run.FixedTimeStamp.negativeInfinity, 1);
                            inventory.SetEquipment(equipmentState, (uint)i);

                        }
                    }
                };




            On.RoR2.EquipmentSlot.PerformEquipmentAction += EnigmaEquipmentChanger;
        public static bool EnigmaEquipmentChanger(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, global::RoR2.EquipmentSlot self, global::RoR2.EquipmentDef equipmentDef)
        {
            if (equipmentDef == RoR2Content.Equipment.Enigma)
            {
                EquipmentDef tempdef;
                do
                {
                    int i = random.Next(EquipmentCatalog.equipmentCount);
                    tempdef = EquipmentCatalog.GetEquipmentDef((EquipmentIndex)i);


                    if ((EquipmentIndex)i == RoR2Content.Equipment.BurnNearby.equipmentIndex && self.characterBody.teamComponent.teamIndex == TeamIndex.Monster)
                    {
                        tempdef = RoR2Content.Equipment.Recycle;
                    }

                }
                while (tempdef.enigmaCompatible == false);

                equipmentDef = tempdef;
                string token = "<style=cEvent>Your Enigma invokes " + Language.GetString(equipmentDef.nameToken) + "</style>";
                NetworkConnection clientAuthorityOwner = self.characterBody.gameObject.GetComponent<NetworkIdentity>().clientAuthorityOwner;
                Chat.SimpleChatMessage simpleChatMessage = new Chat.SimpleChatMessage
                {
                    baseToken = token
                };
                NetworkWriter networkWriter = new NetworkWriter();
                networkWriter.StartMessage(59);
                networkWriter.Write(simpleChatMessage.GetTypeIndex());
                networkWriter.Write(simpleChatMessage);
                networkWriter.FinishMessage();
                if (clientAuthorityOwner != null)
                {
                    clientAuthorityOwner.SendWriter(networkWriter, RoR2.Networking.QosChannelIndex.chat.intVal);
                }
            }

            return orig(self, equipmentDef);
        }








        public static GenericPickupController EnigmaPickupChanger(On.RoR2.GenericPickupController.orig_CreatePickup orig, ref global::RoR2.GenericPickupController.CreatePickupInfo createPickupInfo)
        {
            if (PickupCatalog.GetPickupDef(createPickupInfo.pickupIndex).equipmentIndex != EquipmentIndex.None)
            {
                createPickupInfo.pickupIndex = PickupCatalog.FindPickupIndex(RoR2Content.Equipment.Enigma.equipmentIndex);
            }
            return orig(ref createPickupInfo);
        }






                //Debug.LogWarning(EnigmaArtifactDisplay);

                /*
                Texture2D texItemEnigmaOrange = new Texture2D(128, 128, TextureFormat.DXT5, false);
                texItemEnigmaOrange.LoadImage(Properties.Resources.texItemEnigmaOrange, false);
                texItemEnigmaOrange.filterMode = FilterMode.Bilinear;
                texItemEnigmaOrange.wrapMode = TextureWrapMode.Clamp;
                Sprite texItemEnigmaOrangeS = Sprite.Create(texItemEnigmaOrange, rec128, half);

                Texture2D texItemEnigma = new Texture2D(128, 128, TextureFormat.DXT5, false);
                texItemEnigma.LoadImage(Properties.Resources.texItemEnigma, false);
                texItemEnigma.filterMode = FilterMode.Bilinear;
                texItemEnigma.wrapMode = TextureWrapMode.Clamp;
                Sprite texItemEnigmaS = Sprite.Create(texItemEnigma, rec128, half);
                
*/












/*
            /*
            RoR2.CharacterAI.AISkillDriver[] skilllist;
            RoR2.CharacterAI.AISkillDriver tempskillai = null;
            RoR2.CharacterAI.AISkillDriver tempskillai2 = null;
            RoR2.CharacterAI.AISkillDriver tempskillai3 = null;
            RoR2.CharacterAI.AISkillDriver tempskillai4 = null;
            skilllist = Resources.Load<GameObject>("prefabs/charactermasters/CaptainMonsterMaster").GetComponents<RoR2.CharacterAI.AISkillDriver>();
            for (var i = 0; i < skilllist.Length; i++)
            {
                skilllist[i].shouldSprint = true;



                if (skilllist[i].customName.Contains("FireShotgun"))
                {
                    tempskillai = skilllist[i];
                    //Debug.LogWarning(skilllist[i].customName);
                    skilllist[i].shouldFireEquipment = true;
                    skilllist[i].maxDistance = float.PositiveInfinity;
                    skilllist[i].requiredSkill = null;
                    tempskillai3.activationRequiresTargetLoS = false;
                    tempskillai3.activationRequiresAimConfirmation = false;
                    //tempskillai3.selectionRequiresTargetLoS = false;
                }

                if (skilllist[i].skillSlot == SkillSlot.Utility)
                {
                    RoR2.Skills.SkillDef tempdef = RoR2.Skills.SkillCatalog.GetSkillDef(RoR2.Skills.SkillCatalog.FindSkillIndexByName("CaptainPrepSupplyDrop"));
                    Debug.LogWarning(tempdef);

                    tempskillai3 = skilllist[i].gameObject.AddComponent<RoR2.CharacterAI.AISkillDriver>();
                    tempskillai3.shouldFireEquipment = true;
                    tempskillai3.aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy;
                    //tempskillai3.maxDistance = float.PositiveInfinity;
                    tempskillai3.skillSlot = SkillSlot.Special;
                    tempskillai3.requiredSkill = null;
                    tempskillai3.shouldSprint = false;
                    tempskillai3.activationRequiresAimConfirmation = false;
                    tempskillai3.activationRequiresTargetLoS = false;
                    tempskillai3.selectionRequiresTargetLoS = false;
                    tempskillai3.minDistance = 0;
                    tempskillai3.movementType = RoR2.CharacterAI.AISkillDriver.MovementType.Stop;
                    tempskillai3.requiredSkill = tempdef;
                    tempskillai3.customName = "ActivateSupplyDrop2";

                    skilllist[i].shouldFireEquipment = true;
                    skilllist[i].aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy;
                    skilllist[i].maxDistance = float.PositiveInfinity;
                    skilllist[i].minDistance = 20;
                    skilllist[i].skillSlot = SkillSlot.Special;
                    skilllist[i].requiredSkill = null;
                    skilllist[i].shouldSprint = false;
                    skilllist[i].activationRequiresAimConfirmation = false;
                    skilllist[i].activationRequiresTargetLoS = false;
                    skilllist[i].selectionRequiresTargetLoS = false;
                    skilllist[i].requiredSkill = tempdef;
                    tempskillai2 = skilllist[i];
                    //skilllist[i].buttonPressType = RoR2.CharacterAI.AISkillDriver.ButtonPressType.TapContinuous;
                }
                else if (skilllist[i].skillSlot == SkillSlot.Secondary)
                {
                    skilllist[i].shouldFireEquipment = true;
                    //skilllist[i].maxDistance = float.PositiveInfinity;
                    skilllist[i].requiredSkill = null;
                    skilllist[i].maxDistance = 20;
                    tempskillai4 = skilllist[i];
                }
                /*
                if (skilllist[i].customName.Equals("MarkOrbitalStrike"))
                {
                    skilllist[i].shouldFireEquipment = true;
                    skilllist[i].aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy;
                    skilllist[i].requiredSkill = null;
                    skilllist[i].shouldSprint = true;
                    skilllist[i].maxDistance = float.PositiveInfinity;
                }
                */

/*
    if (skilllist[i].skillSlot != SkillSlot.Primary && skilllist[i].skillSlot != SkillSlot.None)
    {
        //Destroy(skilllist[i]);
        //skilllist[i].enabled = false;
    }

    if (skilllist[i].customName.Equals("FireLongRange") || skilllist[i].customName.Equals("ShortStrafe") || skilllist[i].customName.Equals("MarkOrbitalStrike") || skilllist[i].customName.Equals("BackUpIfClose"))
    {
        Destroy(skilllist[i]);
        //skilllist[i].enabled = false;
    }
    else if (skilllist[i].customName.Equals("ChaseEnemy"))
    {
        skilllist[i].driverUpdateTimerOverride = 1;
        skilllist[i].minDistance = 20;
    }

    */
/*
if (skilllist[i].customName.Equals("MarkOrbitalStrike"))
{
    skilllist[i].shouldFireEquipment = true;
    skilllist[i].aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy;
    //skilllist[i].buttonPressType = RoR2.CharacterAI.AISkillDriver.ButtonPressType.TapContinuous;
}
else if (skilllist[i].customName.Equals("FireLongRange"))
{
    skilllist[i].shouldFireEquipment = true;
    skilllist[i].maxDistance = 120;
    skilllist[i].minDistance = 60;
    skilllist[i].movementType = RoR2.CharacterAI.AISkillDriver.MovementType.FleeMoveTarget;
}
else if (skilllist[i].customName.Contains("FireTazer") || skilllist[i].customName.Contains("FireShotgun") || skilllist[i].customName.Contains("BackUpIfClose"))
{
    //Debug.LogWarning(skilllist[i].customName);
    skilllist[i].shouldFireEquipment = true;
}
*//*
}
tempskillai2.nextHighPriorityOverride = tempskillai;
tempskillai3.nextHighPriorityOverride = tempskillai4;
tempskillai3.maxDistance = tempskillai4.maxDistance;
*/

/*
On.RoR2.Artifacts.DoppelgangerInvasionManager.PickItemDrop += (orig, inventory, rng) =>
{
    if (inventory.GetTotalItemCountOfTier(ItemTier.Tier2) > 0 || inventory.GetTotalItemCountOfTier(ItemTier.Tier3) > 0 || inventory.GetTotalItemCountOfTier(ItemTier.Boss) > 0)
    {
        ItemIndex tempIndex = ItemIndex.None;
        ItemDef tempdef = null;
        do
        {
            int randomindex = random.Next(inventory.itemAcquisitionOrder.Count);
            tempIndex = inventory.itemAcquisitionOrder[randomindex];
            tempdef = ItemCatalog.GetItemDef(tempIndex);
            if (tempdef == RoR2Content.Items.ExtraLifeConsumed)
            {
                tempdef = RoR2Content.Items.ExtraLife;
                tempIndex = RoR2Content.Items.ExtraLife.itemIndex;
            }
        }
        while (tempdef.tier == ItemTier.Tier1 | tempdef.tier == ItemTier.NoTier | tempdef.tier == ItemTier.Lunar);
        return (tempIndex);

    }
    return orig(inventory, rng);
};
*/