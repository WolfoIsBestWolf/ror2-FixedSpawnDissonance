using MonoMod.Cil;
using RoR2;
using RoR2.Artifacts;
using RoR2.Items;
using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FixedspawnDissonance
{
    public class Glass
    {
        public static void Start()
        {
            IL.RoR2.CharacterModel.UpdateOverlays += CharacterModel_UpdateOverlayStates;
            IL.RoR2.CharacterModel.UpdateOverlayStates += CharacterModel_UpdateOverlayStates;

            
        }

        private static void CharacterModel_UpdateOverlayStates(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.Before,
            x => x.MatchLdsfld("RoR2.RoR2Content/Items", "LunarDagger")))
            {
                c.Index += 4;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<System.Func<bool, CharacterModel, bool>>((yes, model) =>
                {
                    if (model.body.isGlass)
                    {
                        return true;
                    }
                    return yes;
                });
            }
            else
            {
                Debug.LogWarning("IL Failed: IL.CharacterModel_UpdateOverlays");
            }
        }

 
    }
}