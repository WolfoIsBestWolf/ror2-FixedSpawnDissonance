using RoR2;

namespace VanillaArtifactsPlus
{
    public class Swarms
    {

        public static int SwarmsDeployableLimitChanger(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
        {
            int count = orig(self, slot);
            switch (slot)
            {
                case DeployableSlot.RoboBallMini:
                case DeployableSlot.VoidMegaCrabItem:
                case DeployableSlot.DroneWeaponsDrone:
                    count *= 2;
                    break;
            }
            return count;
        }
    }
}