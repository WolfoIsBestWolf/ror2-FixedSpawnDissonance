using RoR2;
using UnityEngine;

namespace FixedspawnDissonance
{
    public class Command
    {
        public static void OnArtifactDisable()
        {
        }

        public static void OnArtifactEnable()
        {
        }
       
       
        public static void Start()
        {
            //Fix Void Particles
            On.RoR2.PickupPickerController.SetOptionsInternal += AddVoidParticlesToCommand;

            On.RoR2.PickupTransmutationManager.RebuildPickupGroups += AddEliteEquipmentsToCommand;
            On.RoR2.Run.IsEquipmentAvailable += Make_EliteEquipmentAvailable;
        }

        private static void AddVoidParticlesToCommand(On.RoR2.PickupPickerController.orig_SetOptionsInternal orig, PickupPickerController self, PickupPickerController.Option[] newOptions)
        {
            if(self.name.StartsWith("Command"))
            {
                var pickup = self.GetComponent<PickupIndexNetworker>();
                if (pickup != null)
                {
                    if (pickup.pickupDisplay)
                    {
                        var pickupDef = pickup.pickupIndex.pickupDef;
                        if (pickupDef.itemTier >= ItemTier.VoidTier1 && pickupDef.itemTier <= ItemTier.VoidBoss)
                        {
                            self.gameObject.GetComponent<GenericDisplayNameProvider>().displayToken = "ARTIFACT_COMMAND_CUBE_PINK_NAME";
                            if (!pickup.pickupDisplay.voidParticleEffect)
                            {
                                GameObject newVoidParticle = Object.Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/GenericPickup").GetComponent<GenericPickupController>().pickupDisplay.voidParticleEffect, self.transform.GetChild(0));
                                newVoidParticle.SetActive(true);
                                GameObject newOrb = Object.Instantiate(pickup.pickupDisplay.tier2ParticleEffect.transform.GetChild(2).gameObject, newVoidParticle.transform);
                                newOrb.GetComponent<ParticleSystem>().startColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.VoidItem);
                                pickup.pickupDisplay.voidParticleEffect = newVoidParticle;
                            }
                           
                        }
                    }
                }
                 
            }
            orig(self, newOptions);
        }

     

        private static bool Make_EliteEquipmentAvailable(On.RoR2.Run.orig_IsEquipmentAvailable orig, Run self, EquipmentIndex equipmentIndex)
        {
            EquipmentDef def = EquipmentCatalog.GetEquipmentDef(equipmentIndex);
            if (def && def.passiveBuffDef)
            {               
                return !self.IsEquipmentExpansionLocked(equipmentIndex);
            }
            return orig(self, equipmentIndex); 
        }

        private static void AddEliteEquipmentsToCommand(On.RoR2.PickupTransmutationManager.orig_RebuildPickupGroups orig)
        {
            for (var i = 0; i < EliteCatalog.eliteDefs.Length; i++)
            {
                EliteDef eliteDef = EliteCatalog.eliteDefs[i];
                EquipmentDef tempEliteEquip = eliteDef.eliteEquipmentDef;
                if (tempEliteEquip != null)
                {
                    if (!eliteDef.name.EndsWith("Gold"))
                    {
                        if (tempEliteEquip.dropOnDeathChance > 0)
                        {
                            tempEliteEquip.canDrop = true;
                            tempEliteEquip.isBoss = true;
                            tempEliteEquip.isLunar = false;
                        }
                    }
                }
            }
            orig();
            for (var i = 0; i < EliteCatalog.eliteDefs.Length; i++)
            {
                EliteDef eliteDef = EliteCatalog.eliteDefs[i];
                EquipmentDef tempEliteEquip = eliteDef.eliteEquipmentDef;
 
                if (tempEliteEquip != null)
                {
                    if (tempEliteEquip.dropOnDeathChance > 0)
                    {
                        tempEliteEquip.canDrop = false;
                    }
                }
            }
       
        }
 
       

    }
}