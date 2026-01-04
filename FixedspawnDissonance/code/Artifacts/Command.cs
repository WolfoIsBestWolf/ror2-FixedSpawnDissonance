using RoR2;
using System.Collections.Generic;

namespace VanillaArtifactsPlus
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

            On.RoR2.PickupTransmutationManager.RebuildPickupGroups += AddEliteEquipmentsToCommand;
            On.RoR2.Run.IsEquipmentAvailable += Make_EliteEquipmentAvailable;
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
            List<EquipmentDef> list = new List<EquipmentDef>();
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
                            list.Add(tempEliteEquip);
                            tempEliteEquip.canDrop = true;
                            tempEliteEquip.isBoss = true;
                            tempEliteEquip.isLunar = false;
                        }
                    }
                }
            }
            orig();
            for (var i = 0; i < list.Count; i++)
            {
                list[i].canDrop = false;
            }

        }



    }
}