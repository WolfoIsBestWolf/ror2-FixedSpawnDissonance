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
            On.RoR2.ClassicStageInfo.HandleSingleMonsterTypeArtifact += (orig2, monsterCategories, rng) =>
            {
                KinBackup.CopyFrom(monsterCategories);
                orig2(monsterCategories, rng);

                int r = 0;
                SpawnCard spwncard = monsterCategories.categories[0].cards[0].spawnCard;
                if (spwncard == KinNoRepeat)
                {
                        //Debug.Log("Repeat Spawn");
                        do
                    {
                        r++;
                        monsterCategories.CopyFrom(KinBackup);
                        orig2(monsterCategories, rng);
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
            };

        }
    }
}