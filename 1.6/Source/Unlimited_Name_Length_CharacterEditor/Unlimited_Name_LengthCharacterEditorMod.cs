using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
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

        Harmony harmony = new Harmony("Feldoh.rimworld.Unlimited_Name_Length.characteredit");
        harmony.PatchAll();
    }
}

[HarmonyPatch]
public static class CharEditorNamePatch
{
    private static Lazy<FieldInfo> ValidNameRegexFieldInfo = new (() => AccessTools.Field(typeof(CharacterCardUtility), "ValidNameRegex"));

    [HarmonyTargetMethods]
    public static IEnumerable<MethodBase> TargetMethods()
    {
        if (!ModsConfig.IsActive("void.charactereditor")) yield break;

        Type blockBio = AccessTools.FindIncludingInnerTypes(AccessTools.TypeByName("CharacterEditor.CEditor"), t => t.Name == "BlockBio" ? t : null);

        if (blockBio == null)
        {
            Log.Warning("CharacterEditor.CEditor.EditorUI.BlockBio not found, CharacterEditor may have been updated. Please report this to the mod author of Unlimited Name Length.");
            yield break;
        }
        MethodInfo triple = AccessTools.Method(blockBio, "DrawNameTripe");
        MethodInfo single = AccessTools.Method(blockBio, "DrawNameSingle");
        if (triple != null)
        {
            yield return triple;
        }
        else
        {
            Log.Warning("CharacterEditor.CEditor.EditorUI.BlockBio:DrawNameTripe not found, CharacterEditor may have been updated. Please report this to the mod author of Unlimited Name Length.");
        }

        if (single != null)
        {
            yield return single;
        }
        else
        {
            Log.Warning("CharacterEditor.CEditor.EditorUI.BlockBio:DrawNameSingle not found, CharacterEditor may have been updated. Please report this to the mod author of Unlimited Name Length.");
        }
    }

    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> DrawNameTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        bool found = false;
        CodeInstruction lastInstruction = null;
        foreach (CodeInstruction instruction in instructions)
        {
            if (lastInstruction != null)
            {
                if (instruction.opcode == OpCodes.Ldsfld && instruction.LoadsField(ValidNameRegexFieldInfo.Value) && lastInstruction.opcode == OpCodes.Ldc_I4_S)
                {
                    found = true;
                    yield return new CodeInstruction(OpCodes.Ldc_I4, int.MaxValue);
                }
                else
                {
                    yield return lastInstruction;
                }
            }
            lastInstruction = instruction;
        }
        if (!found) Log.Warning("Unlimited Name Length DrawNameTranspiler could not find target and has not applied.");
        yield return lastInstruction;
    }
}
