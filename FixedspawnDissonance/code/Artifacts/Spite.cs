using MonoMod.Cil;
using RoR2;
using RoR2.Artifacts;
using System;
using UnityEngine;
using Mono.Cecil.Cil;

namespace VanillaArtifactsPlus
{
    public class Spite
    {
        public static void SpiteChangesCalledLate()
        {
            if (BombArtifactManager.maxBombCount == 30) //Max Bombs spwaned per enemy
            {
                BombArtifactManager.maxBombCount = 16; //Golem spawns 15 with default setting, rarely would more ever be needed
            }
            if (BombArtifactManager.extraBombPerRadius == 4)
            {
                BombArtifactManager.extraBombPerRadius = 7; //Most small enemies have 1.82 radius
            }
            //
            if (BombArtifactManager.bombSpawnBaseRadius == 3)
            {
                BombArtifactManager.bombSpawnBaseRadius = 30;
            }
            if (BombArtifactManager.bombSpawnRadiusCoefficient == 4)
            {
                BombArtifactManager.bombSpawnRadiusCoefficient = 1;
            }
            //
            if (BombArtifactManager.bombBlastRadius == 7)
            {
                BombArtifactManager.bombBlastRadius = 8.5f;
            }
            //
            if (BombArtifactManager.maxBombFallDistance == 60)//Max Distance away from the killed
            {
                BombArtifactManager.maxBombFallDistance = 180;//but like, it just deletes them if they're too far
            }
            if (BombArtifactManager.maxBombStepUpDistance == 8)
            {
                BombArtifactManager.maxBombStepUpDistance = 12;
            }
            //
            if (BombArtifactManager.bombFuseTimeout == 8)
            {
                BombArtifactManager.bombFuseTimeout = 7f;
            }
            //BombArtifactManager.cvSpiteBombCoefficient.SetString("0.5");


            IL.RoR2.Artifacts.BombArtifactManager.OnServerCharacterDeath += BombArtifactManager_OnServerCharacterDeath;
         
        }

        private static void BombArtifactManager_OnServerCharacterDeath(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.TryGotoNext(MoveType.After,
            x => x.MatchLdfld("RoR2.DamageReport", "victimTeamIndex"),
            x => x.MatchLdcI4(2));

            c.Prev.OpCode = OpCodes.Ldc_I4_1;
            c.Next.OpCode = OpCodes.Bne_Un_S;

            if (c.TryGotoNext(MoveType.After,
                x => x.MatchLdfld("RoR2.DamageReport", "victimTeamIndex")))
            {
                c.EmitDelegate<Func<TeamIndex, TeamIndex>>((teamIndex) =>
                {
                    return TeamIndex.Lunar;
                });
            }
            else
            {
                Debug.LogWarning("IL Failed: IL.BombArtifactManager_OnServerCharacterDeath");
            }
        }
    }
}