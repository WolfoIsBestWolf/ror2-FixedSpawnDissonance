using MonoMod.Cil;
using Mono.Cecil.Cil;
using RoR2;
using UnityEngine;
using System;
 
namespace FixedspawnDissonance
{
    public class Swarms
    {
        public static void Start()
        {
            IL.RoR2.Artifacts.SwarmsArtifactManager.OnSpawnCardOnSpawnedServerGlobal += VengenceAndEnemyGooboFix;
        }

        private static void VengenceAndEnemyGooboFix(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld("RoR2.SpawnCard/SpawnResult", "spawnRequest"),
                x => x.MatchCallvirt("RoR2.DirectorCore","TrySpawnObject")))
            {
                c.EmitDelegate<Func<SpawnCard.SpawnResult, SpawnCard.SpawnResult>>((result) =>
                {
                    if (result.spawnedInstance)
                    {
                         if (result.spawnRequest.spawnCard is MasterCopySpawnCard)
                         {
                            result.spawnRequest.spawnCard = MasterCopySpawnCard.FromMaster(result.spawnedInstance.GetComponent<CharacterMaster>(), true, true, null);
                         }
                    }
                    return result;
                });
            }
            else
            {
                Debug.LogWarning("IL Failed: SwarmsArtifactManager_OnSpawnCardOnSpawnedServerGlobal");
            }
        }

        

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