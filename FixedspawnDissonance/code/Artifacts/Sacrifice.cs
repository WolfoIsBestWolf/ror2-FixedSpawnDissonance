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
            dtSacrificeArtifact.tier2Weight = 0.24f; //0.3 default
            dtSacrificeArtifact.tier3Weight = 0.01f; //0.01 default

            dtSacrificeArtifactBoss.tier1Weight = 0;
            dtSacrificeArtifactBoss.tier2Weight = 0.8f;
            dtSacrificeArtifactBoss.tier3Weight = 0.2f;
            dtSacrificeArtifactBoss.bossWeight = 0.05f;
            dtSacrificeArtifactBoss.equipmentWeight = 0f;
            dtSacrificeArtifactBoss.name = "dtSacrificeArtifactBoss";


            dtSacrificeArtifactVoid.tier1Weight = 0;
            dtSacrificeArtifactVoid.tier2Weight = 0;
            dtSacrificeArtifactVoid.tier3Weight = 0;
            dtSacrificeArtifactVoid.equipmentWeight = 0;
            dtSacrificeArtifactVoid.voidTier1Weight = 7f; //6
            dtSacrificeArtifactVoid.voidTier2Weight = 3; //3
            dtSacrificeArtifactVoid.voidTier3Weight = 0.5f; //1
            dtSacrificeArtifactVoid.name = "dtSacrificeArtifactVoid";

            On.RoR2.Artifacts.SacrificeArtifactManager.OnPrePopulateSceneServer += (orig, sceneDirector) =>
            {
                if (WConfig.SacrificeMoreEnemySpawns.Value)
                {
                    sceneDirector.monsterCredit = (int)(sceneDirector.monsterCredit * 1.5f);
                }
                orig(sceneDirector);
            };
        }

        public static void SacrificeArtifactManager_OnServerCharacterDeath(On.RoR2.Artifacts.SacrificeArtifactManager.orig_OnServerCharacterDeath orig, DamageReport damageReport)
        {
            if (!damageReport.victimMaster)
            {
                return;
            }
            if (damageReport.victimBody)
            {
                if (damageReport.victimTeamIndex == TeamIndex.Void || damageReport.victimBody.bodyFlags.HasFlag(CharacterBody.BodyFlags.Void))
                {
                    RoR2.Artifacts.SacrificeArtifactManager.dropTable = dtSacrificeArtifactVoid;
                }
                else if (damageReport.victimIsElite && damageReport.victimBody.baseMaxHealth > 690 || damageReport.victimIsChampion || damageReport.victimIsBoss)
                {
                    //Elite Elders and Champions
                    RoR2.Artifacts.SacrificeArtifactManager.dropTable = dtSacrificeArtifactBoss;
                }
                else
                {
                    RoR2.Artifacts.SacrificeArtifactManager.dropTable = dtSacrificeArtifact;
                }
            }
            else
            {
                RoR2.Artifacts.SacrificeArtifactManager.dropTable = dtSacrificeArtifact;
            }
            orig(damageReport);
        }
    }
}