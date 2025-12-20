using Verse;
using HarmonyLib;

namespace VFESecurity
{
    public class VFESecurityMod : Mod
    {
        public VFESecurityMod(ModContentPack pack) : base(pack)
        {
            new Harmony("VFESecurityMod").PatchAll();
        }
    }
}
