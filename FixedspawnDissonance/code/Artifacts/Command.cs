using BepInEx;
using RoR2;
using UnityEngine;

namespace FixedspawnDissonance
{
    public class Command
    {

        public static PickupPickerController.Option[] EliteEquipmentChoicesForCommand = new PickupPickerController.Option[0];

        public static void Start()
        {
            On.RoR2.Artifacts.CommandArtifactManager.OnDropletHitGroundServer += CommandArtifactManager_OnDropletHitGroundServer;
            On.RoR2.PickupPickerController.GetOptionsFromPickupIndex += CommandGiveAffixChoices;

           //Fix Void Particles
            On.RoR2.PickupPickerController.SetOptionsFromPickupForCommandArtifact += (orig, self, pickupIndex) =>
            {
                orig(self, pickupIndex);

                if (pickupIndex.pickupDef.itemIndex != ItemIndex.None)
                {
                    if (pickupIndex.pickupDef.itemTier == ItemTier.VoidTier1 || pickupIndex.pickupDef.itemTier == ItemTier.VoidTier2 || pickupIndex.pickupDef.itemTier == ItemTier.VoidTier3 || pickupIndex.pickupDef.itemTier == ItemTier.VoidBoss)
                    {
                        self.gameObject.GetComponent<GenericDisplayNameProvider>().displayToken = "Pink Command Essence";
                        GameObject tempobj = Object.Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/GenericPickup").GetComponent<GenericPickupController>().pickupDisplay.voidParticleEffect, self.transform.GetChild(0));
                        tempobj.SetActive(true);
                    }
                }
            };
        }

        public static void LateRunningMethod()
        {
            for (var i = 0; i < EliteCatalog.eliteDefs.Length; i++)
            {
                //EquipmentDef tempEliteEquip = EquipmentCatalog.equipmentDefs[i];
                EquipmentDef tempEliteEquip = EliteCatalog.eliteDefs[i].eliteEquipmentDef;
                //Debug.LogWarning(EliteCatalog.eliteDefs[i]);

                if (tempEliteEquip != null && !EliteCatalog.eliteDefs[i].name.Equals("edLunarEulogy"))
                {
                    if (tempEliteEquip.dropOnDeathChance == 0 || tempEliteEquip.name.Contains("Gold") || tempEliteEquip.name.Contains("Echo") || tempEliteEquip.name.Contains("Yellow"))
                    {
                    }
                    else
                    {
                        //Debug.LogWarning(tempEliteEquip);
                        //EliteEquipmentEquipmentIndexCommand.Add(tempEliteEquip.equipmentIndex);
                        PickupIndex equippickupIndex = PickupCatalog.FindPickupIndex(tempEliteEquip.equipmentIndex);
                        PickupPickerController.Option newoption = new PickupPickerController.Option { pickupIndex = equippickupIndex, available = true };

                        bool dontAdd = false;

                        for (int e = 0; e < Command.EliteEquipmentChoicesForCommand.Length; e++)
                        {
                            if (Command.EliteEquipmentChoicesForCommand[e].pickupIndex == equippickupIndex)
                            {
                                //Debug.Log("Not Adding " + tempEliteEquip);
                                dontAdd = true;
                            }
                        }
                        if (dontAdd == false)
                        {
                            //Debug.Log("Adding " + tempEliteEquip);
                            Command.EliteEquipmentChoicesForCommand = Command.EliteEquipmentChoicesForCommand.Add(newoption);
                        }


                        if (EliteCatalog.eliteDefs[i].name.EndsWith("Honor"))
                        {

                            Honor.EliteEquipmentDefs.Add(tempEliteEquip);
                        }
                    }
                }
            }

        }


        public static void CommandArtifactManager_OnDropletHitGroundServer(On.RoR2.Artifacts.CommandArtifactManager.orig_OnDropletHitGroundServer orig, ref GenericPickupController.CreatePickupInfo createPickupInfo, ref bool shouldSpawn)
        {
            PickupIndex pickupIndex = createPickupInfo.pickupIndex;
            PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
            if (pickupDef != null)
            {
                PickupPickerController.Option[] options = PickupPickerController.GetOptionsFromPickupIndex(pickupIndex);
                //Debug.LogWarning(options.Length);
                if (options.Length <= 1)
                {
                    return;
                }
            }
            orig(ref createPickupInfo, ref shouldSpawn);
        }


        public static PickupPickerController.Option[] CommandGiveAffixChoices(On.RoR2.PickupPickerController.orig_GetOptionsFromPickupIndex orig, PickupIndex pickupIndex)
        {
            PickupPickerController.Option[] temp = orig(pickupIndex);
            if (pickupIndex.equipmentIndex != EquipmentIndex.None)
            {
                EquipmentDef tempequipdef = EquipmentCatalog.GetEquipmentDef(pickupIndex.equipmentIndex);
                if (tempequipdef.passiveBuffDef && tempequipdef.passiveBuffDef.isElite && tempequipdef.dropOnDeathChance > 0)
                {
                    return EliteEquipmentChoicesForCommand;
                }
            }
            return temp;
        }


    }
}