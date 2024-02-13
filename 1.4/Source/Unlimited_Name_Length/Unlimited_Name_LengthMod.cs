using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Unlimited_Name_Length;

public class Unlimited_Name_LengthMod : Mod
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

[HarmonyPatch(typeof(CharacterCardUtility), "DoNameInputRect")]
public static class CharacterCardUtilityLengthPatch
{
    [HarmonyPrefix]
    public static bool DoNameInputRect(ref int maxLength)
    {
        maxLength = int.MaxValue;
        return true;
    }
}

[HarmonyPatch]
public static class GiveNameLengthPatch
{
    [HarmonyTargetMethods]
    public static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.PropertyGetter(typeof(Dialog_NamePlayerFaction), "FirstCharLimit");
        yield return AccessTools.PropertyGetter(typeof(Dialog_NamePlayerSettlement), "FirstCharLimit");
        yield return AccessTools.PropertyGetter(typeof(Dialog_NamePlayerFactionAndSettlement), "FirstCharLimit");
        yield return AccessTools.PropertyGetter(typeof(Dialog_NamePlayerFactionAndSettlement), "SecondCharLimit");
        yield return AccessTools.PropertyGetter(typeof(Dialog_GiveName), "FirstCharLimit");
        yield return AccessTools.PropertyGetter(typeof(Dialog_GiveName), "SecondCharLimit");
        yield return AccessTools.PropertyGetter(typeof(Dialog_Rename), "MaxNameLength");
    }

    [HarmonyPostfix]
    public static void MaxNameLength(ref int __result)
    {
        __result = int.MaxValue;
    }
}

[HarmonyPatch]
public static class ValidityPatch
{
    [HarmonyTargetMethods]
    public static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(NamePlayerFactionDialogUtility), nameof(NamePlayerFactionDialogUtility.IsValidName));
        yield return AccessTools.Method(typeof(NamePlayerSettlementDialogUtility), nameof(NamePlayerSettlementDialogUtility.IsValidName));
    }

    [HarmonyPostfix]
    public static void IsValidName(ref bool __result, string s)
    {
        __result = s.Length > 0;
    }
}
