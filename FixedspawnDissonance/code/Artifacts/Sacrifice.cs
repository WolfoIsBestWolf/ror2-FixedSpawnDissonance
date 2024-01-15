using RoR2;
using UnityEngine;


namespace FixedspawnDissonance
{
    public class Sacrifice
    {

        public static BasicPickupDropTable dtSacrificeArtifact = LegacyResourcesAPI.Load<BasicPickupDropTable>("DropTables/dtSacrificeArtifact");
        public static BasicPickupDropTable dtSacrificeArtifactBoss = ScriptableObject.CreateInstance<BasicPickupDropTable>();
        public static BasicPickupDropTable dtSacrificeArtifactVoid = ScriptableObject.CreateInstance<BasicPickupDropTable>();


        public static void Start()
        {
            dtSacrificeArtifact.tier1Weight = 0.8f; //0.7 default
            dtSacrificeArtifact.tier2Weight = 0.25f; //0.3 default
            dtSacrificeArtifact.tier3Weight = 0.015f; //0.01 default

            dtSacrificeArtifactBoss.tier1Weight = 0;
            dtSacrificeArtifactBoss.tier2Weight = 0.75f;
            dtSacrificeArtifactBoss.tier3Weight = 0.2f;
            dtSacrificeArtifactBoss.bossWeight = 0.2f;
            dtSacrificeArtifactBoss.equipmentWeight = 0f;
            dtSacrificeArtifactBoss.name = "dtSacrificeArtifactBoss";


            dtSacrificeArtifactVoid.tier1Weight = 0;
            dtSacrificeArtifactVoid.tier2Weight = 0;
            dtSacrificeArtifactVoid.tier3Weight = 0;
            dtSacrificeArtifactVoid.equipmentWeight = 0;
            dtSacrificeArtifactVoid.voidTier1Weight = 6; //6
            dtSacrificeArtifactVoid.voidTier2Weight = 3; //3
            dtSacrificeArtifactVoid.voidTier3Weight = 1; //1
            dtSacrificeArtifactVoid.name = "dtSacrificeArtifactVoid";

            if (WConfig.SacrificeMoreEnemySpawns.Value) 
            {
                On.RoR2.Artifacts.SacrificeArtifactManager.OnPrePopulateSceneServer += (orig, sceneDirector) =>
                {
                    sceneDirector.monsterCredit = (int)(sceneDirector.monsterCredit * 1.6f);
                    orig(sceneDirector);
                };
            }  
        }

        public static void SacrificeArtifactManager_OnServerCharacterDeath(On.RoR2.Artifacts.SacrificeArtifactManager.orig_OnServerCharacterDeath orig, DamageReport damageReport)
        {
            if (damageReport.victimTeamIndex == TeamIndex.Void || damageReport.victimBody && damageReport.victimBody.bodyFlags.HasFlag(CharacterBody.BodyFlags.Void))
            {
                RoR2.Artifacts.SacrificeArtifactManager.dropTable = dtSacrificeArtifactVoid;
            }
            else if (damageReport.victimBody.isChampion)
            {
                RoR2.Artifacts.SacrificeArtifactManager.dropTable = dtSacrificeArtifactBoss;
            }
            else
            {
                RoR2.Artifacts.SacrificeArtifactManager.dropTable = dtSacrificeArtifact;
            }
            orig(damageReport);
        }
    }
}