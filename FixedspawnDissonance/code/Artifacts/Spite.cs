using BepInEx;
using RoR2;
using UnityEngine;
using RoR2.Artifacts;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace FixedspawnDissonance
{
    public class Spite
    {
        public static void SpiteChangesCalledLate()
        {
            if (BombArtifactManager.maxBombCount == 30) //Max Bombs spwaned per enemy
            {
                BombArtifactManager.maxBombCount = 12; //Golem spawns 15 with default setting, rarely would more ever be needed
            }
            if (BombArtifactManager.extraBombPerRadius == 4)
            {
                BombArtifactManager.extraBombPerRadius = 7; //Most small enemies have 1.82 radius
            }
            //
            if (BombArtifactManager.bombSpawnBaseRadius == 3)
            {
                BombArtifactManager.bombSpawnBaseRadius = 36;
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
                BombArtifactManager.maxBombFallDistance = 240;//but like, it just deletes them if they're too far
            }
            if (BombArtifactManager.maxBombStepUpDistance == 8)
            {
                BombArtifactManager.maxBombStepUpDistance = 16;
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
            x => x.MatchLdfld("RoR2.DamageReport", "victimTeamIndex"));


            if (c.TryGotoNext(MoveType.After,
                x => x.MatchLdfld("RoR2.DamageReport", "victimTeamIndex")))
            {
                c.EmitDelegate<Func<TeamIndex, TeamIndex>>((teamIndex) =>
                {
                    return TeamIndex.Void;
                });
                //Debug.Log("IL Found: IL.RoR2.BombArtifactManager_OnServerCharacterDeath");
            }
            else
            {
                Debug.LogWarning("IL Failed: IL.BombArtifactManager_OnServerCharacterDeath");
            }
        }
    }
}