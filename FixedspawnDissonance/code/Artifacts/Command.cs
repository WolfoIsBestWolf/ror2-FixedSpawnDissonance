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
            //On.RoR2.Artifacts.CommandArtifactManager.OnDropletHitGroundServer += CommandArtifactManager_OnDropletHitGroundServer;
            //On.RoR2.PickupDropletController.CreateCommandCube += PickupDropletController_CreateCommandCube;
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

        private static void PickupDropletController_CreateCommandCube(On.RoR2.PickupDropletController.orig_CreateCommandCube orig, PickupDropletController self)
        {
            throw new System.NotImplementedException();
        }

        public static void MakeEliteLists()
        {
            for (var i = 0; i < EliteCatalog.eliteDefs.Length; i++)
            {
                EliteDef eliteDef = EliteCatalog.eliteDefs[i];
                EquipmentDef tempEliteEquip = eliteDef.eliteEquipmentDef;
                //Debug.LogWarning(eliteDef);

                if (tempEliteEquip != null)
                {
                    if (eliteDef.name.EndsWith("Eulogy") || tempEliteEquip.name.Contains("Gold") || tempEliteEquip.name.Contains("Echo") || tempEliteEquip.name.Contains("Yellow"))
                    {
                    }
                    else
                    {
                        if (tempEliteEquip.dropOnDeathChance > 0)
                        {
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
                        }

                        if (EliteCatalog.eliteDefs[i].name.EndsWith("Honor"))
                        {
                            Honor.EliteEquipmentDefs.Add(tempEliteEquip);
                        }
                    }
                }
            }

            if (Honor.EliteEquipmentDefs.Count == 0)
            {
                Debug.LogWarning("No Honor Elites found : Who messed up the EliteCatalog");
                Honor.EliteEquipmentDefs.Add(RoR2Content.Elites.FireHonor.eliteEquipmentDef);
                Honor.EliteEquipmentDefs.Add(RoR2Content.Elites.LightningHonor.eliteEquipmentDef);
                Honor.EliteEquipmentDefs.Add(RoR2Content.Elites.IceHonor.eliteEquipmentDef);
            }
            if (EliteEquipmentChoicesForCommand.Length == 0)
            {
                Debug.LogWarning("No Elites Found: Who messed up the EliteCatalog");
            }

        }


        /*public static void CommandArtifactManager_OnDropletHitGroundServer(On.RoR2.Artifacts.CommandArtifactManager.orig_OnDropletHitGroundServer orig, ref GenericPickupController.CreatePickupInfo createPickupInfo, ref bool shouldSpawn)
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
        }*/


        public static PickupPickerController.Option[] CommandGiveAffixChoices(On.RoR2.PickupPickerController.orig_GetOptionsFromPickupIndex orig, PickupIndex pickupIndex)
        {
            PickupPickerController.Option[] temp = orig(pickupIndex);
            if (pickupIndex.equipmentIndex != EquipmentIndex.None)
            {
                EquipmentDef tempequipdef = EquipmentCatalog.GetEquipmentDef(pickupIndex.equipmentIndex);
                if (tempequipdef.passiveBuffDef && tempequipdef.passiveBuffDef.isElite && tempequipdef.dropOnDeathChance > 0)
                {
                    if (EliteEquipmentChoicesForCommand.Length > 0)
                    {
                        return EliteEquipmentChoicesForCommand;
                    }     
                }
            }
            return temp;
        }


    }
}