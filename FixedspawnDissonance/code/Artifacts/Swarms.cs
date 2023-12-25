using RoR2;


namespace FixedspawnDissonance
{
    public class Swarms
    {
        public static void BaseSplitDeath_OnEnter(On.EntityStates.Gup.BaseSplitDeath.orig_OnEnter orig, EntityStates.Gup.BaseSplitDeath self)
        {
            //Gup splits into twice the Geep
            self.spawnCount *= 2;
            orig(self);
        }

        public static int SwarmsDeployableLimitChanger(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
        {
            int count = orig(self, slot);
            switch (slot)
            {
                case DeployableSlot.RoboBallMini:
                case DeployableSlot.GummyClone:
                case DeployableSlot.VendingMachine:
                case DeployableSlot.VoidMegaCrabItem:
                case DeployableSlot.DroneWeaponsDrone:
                case DeployableSlot.MinorConstructOnKill:
                    count *= 2;
                    break;
            }
            return count;
        }
    }
}