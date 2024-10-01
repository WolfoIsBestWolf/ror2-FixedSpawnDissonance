using BepInEx;
using RoR2;
using UnityEngine;

namespace FixedspawnDissonance
{
	public class Devotion
	{

		public static void Start()
		{
			On.RoR2.DevotionInventoryController.OnDevotionArtifactEnabled += DevotionInventoryController_OnDevotionArtifactEnabled;
		}

		private static void DevotionInventoryController_OnDevotionArtifactEnabled(On.RoR2.DevotionInventoryController.orig_OnDevotionArtifactEnabled orig, RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
		{
			orig(runArtifactManager, artifactDef);
			if (artifactDef != CU8Content.Artifacts.Devotion)
			{
				return;
			}
			if (!DevotionInventoryController.lowLevelEliteBuffs.Contains(DLC2Content.Elites.Aurelionite.eliteEquipmentDef.equipmentIndex))
			{
				if (DLC2Content.Elites.Aurelionite.IsAvailable())
				{
					DevotionInventoryController.lowLevelEliteBuffs.Add(DLC2Content.Elites.Aurelionite.eliteEquipmentDef.equipmentIndex);
					DevotionInventoryController.highLevelEliteBuffs.Add(DLC2Content.Elites.Aurelionite.eliteEquipmentDef.equipmentIndex);
					DevotionInventoryController.highLevelEliteBuffs.Add(DLC2Content.Elites.Bead.eliteEquipmentDef.equipmentIndex);
				}
			}
		}
	}
}