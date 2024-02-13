using Verse;
using UnityEngine;
using HarmonyLib;

namespace Unlimited_Name_Length;

public class Unlimited_Name_LengthMod : Verse.Mod
{

    public Unlimited_Name_LengthMod(ModContentPack content) : base(content)
    {
#if DEBUG
        Harmony.DEBUG = true;
#endif

        Harmony harmony = new Harmony("Feldoh.rimworld.Unlimited_Name_Length.main");	
        harmony.PatchAll();
    }
}
