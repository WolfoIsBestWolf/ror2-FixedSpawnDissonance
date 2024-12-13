using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace FixedspawnDissonance
{
    public class Kin
    {
        public static DirectorCardCategorySelection KinBackup = ScriptableObject.CreateInstance<DirectorCardCategorySelection>();
        public static SpawnCard KinNoRepeat;

        public static void Start()
        {
            KinBackup.name = "dcccBackupKinHelper";
            IL.RoR2.ClassicStageInfo.HandleSingleMonsterTypeArtifact += IL_ClassicStageInfo_HandleSingleMonsterTypeArtifact1;

            On.RoR2.ClassicStageInfo.HandleSingleMonsterTypeArtifact += ClassicStageInfo_HandleSingleMonsterTypeArtifact;
        }

        private static void IL_ClassicStageInfo_HandleSingleMonsterTypeArtifact1(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(40f)))
            {
                c.Next.Operand = 50f;
                c.Index++;
            
                c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(50f));
                c.Next.Operand = 60f;
            }
            else
            {
                Debug.LogWarning("IL Failed: IL.IL_ClassicStageInfo_HandleMixEnemyArtifact");
            }
        }

        private static void IL_ClassicStageInfo_HandleMixEnemyArtifact(MonoMod.Cil.ILContext il)
        {

        }

        private static void ClassicStageInfo_HandleSingleMonsterTypeArtifact(On.RoR2.ClassicStageInfo.orig_HandleSingleMonsterTypeArtifact orig, DirectorCardCategorySelection monsterCategories, Xoroshiro128Plus rng)
        {
            KinBackup.CopyFrom(monsterCategories);
            orig(monsterCategories, rng);

            int r = 0;
            SpawnCard spwncard = monsterCategories.categories[0].cards[0].spawnCard;
            if (spwncard == KinNoRepeat)
            {
                //Debug.Log("Repeat Spawn");
                do
                {
                    r++;
                    monsterCategories.CopyFrom(KinBackup);
                    orig(monsterCategories, rng);
                    spwncard = monsterCategories.categories[0].cards[0].spawnCard;
                    //Debug.LogWarning(monsterCategories.categories[0].cards[0].spawnCard);
                } while (r < 15 && spwncard == KinNoRepeat);
            }
            if (spwncard != KinNoRepeat)
            {
                Debug.Log(r + " Cycles until non repeat");
                KinNoRepeat = spwncard;
            }
            else if (r == 10)
            {
                Debug.Log(r + " Cycles, stop looking for non repeat");
            }
            KinBackup.Clear();
        }
    }
}