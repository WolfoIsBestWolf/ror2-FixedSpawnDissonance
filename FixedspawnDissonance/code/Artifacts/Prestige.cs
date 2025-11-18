using MonoMod.Cil;
using Mono.Cecil.Cil;
using RoR2;
using UnityEngine;
using System;
 
namespace VanillaArtifactsPlus
{
    public class Prestige
    {
        
        public static void Start()
        {
            On.RoR2.ShrineBossBehavior.Start += ShrineBossBehavior_Start;
            On.RoR2.TeleporterInteraction.AddShrineStack += TeleporterInteraction_AddShrineStack;
        }

        private static void TeleporterInteraction_AddShrineStack(On.RoR2.TeleporterInteraction.orig_AddShrineStack orig, TeleporterInteraction self)
        {
            orig(self);
            if (!(self.activationState <= TeleporterInteraction.ActivationState.IdleToCharging))
            {
                self.Network_shrineBonusStacks = self._shrineBonusStacks + 1;
            }
        }

        private static void ShrineBossBehavior_Start(On.RoR2.ShrineBossBehavior.orig_Start orig, ShrineBossBehavior self)
        {
            orig(self);
            if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(DLC3Content.Artifacts.Prestige))
            {
                self.purchaseInteraction.setUnavailableOnTeleporterActivated = false;
            }
        }
    }
}