using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Pathea.AdventureMachineNs;
using System;
using System.Collections.Generic;
using System.Reflection;


[BepInPlugin(NAMESPACE, NAME, VERSION)]
public class AdventureMachineAlwaysWinMoney : BaseUnityPlugin
{
    public const string NAMESPACE = "elfinhuangcat.sandrockmods.advmachinecheat";
    public const string NAME = "AdventureMachineAlwaysWinMoney";
    public const string VERSION = "1.0.0";

    public static ManualLogSource Log;
    /// <summary>
    /// Load the mod and patch the necessary methods using Harmony.
    /// </summary>
    private void Awake()
    {
        Log = Logger;

        var harmony = new Harmony(NAMESPACE);
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    /// <summary>
    /// Patcthe method that checks if a bet was successful in the Adventure Machine game,
    /// and reward the first bet placed by the player.
    /// </summary>
    [HarmonyPatch(typeof(BattleInfo), "IsBetSuccess")]
    static class IsBetSuccess_Patch
    {
        // 1) __instance: the BattleInfo object
        // 2) out int rewardGold: the original out parameter
        // 3) ref bool __result: intercepts the return value
        static bool Prefix(BattleInfo __instance,
                           out int rewardGold,
                           ref bool __result)
        {
            // Get the field "betInfos", which should contain the bets that the player put down. 
            FieldInfo fiBetInfos = AccessTools.Field(typeof(BattleInfo), "betInfos");
            List<BetInfo> betInfos = (List<BetInfo>)fiBetInfos.GetValue(__instance);

            // Check if the player has placed any bets
            if (betInfos != null || betInfos.Count > 0)
            {
                // Reward based on the first bet
                for (int i = 0; i < betInfos.Count; i++)
                {
                    if (betInfos[i] != null && betInfos[i].betCount > 0)
                    {
                        rewardGold = betInfos[i].rewardGold * betInfos[i].betCount;
                        __result = true;  // Indicate that the bet was successful
                        return false;  // skip the original method entirely
                    }
                }
            }
            rewardGold = 0;  // No bets, no reward
            return true;  // allow the original method to run
        }
    }
}

