#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class BRServerValidationRuleChecks
{
    [MenuItem("Tools/Battle Royale/Run Anti-Cheat Rule Checks")]
    public static void Run()
    {
        int passed = 0;
        int failed = 0;

        Check("normal movement accepted",
            BRServerValidationRules.IsMovementValid(Vector3.zero, new Vector3(2f, 0f, 0f), 1f, 5f, 0.1f, 2f), true, ref passed, ref failed);
        Check("excessive movement rejected",
            BRServerValidationRules.IsMovementValid(Vector3.zero, new Vector3(20f, 0f, 0f), 1f, 5f, 0.1f, 2f), false, ref passed, ref failed);
        Check("excessive vertical movement rejected",
            BRServerValidationRules.IsMovementValid(Vector3.zero, new Vector3(0f, 5f, 0f), 1f, 5f, 0.1f, 2f), false, ref passed, ref failed);
        Check("valid shot accepted",
            BRServerValidationRules.IsShotValid(1f, 0.8f, 5f, 10, false), true, ref passed, ref failed);
        Check("too-fast shot rejected",
            BRServerValidationRules.IsShotValid(1f, 0.95f, 5f, 10, false), false, ref passed, ref failed);
        Check("empty-magazine shot rejected",
            BRServerValidationRules.IsShotValid(1f, 0f, 5f, 0, false), false, ref passed, ref failed);
        Check("reloading shot rejected",
            BRServerValidationRules.IsShotValid(1f, 0f, 5f, 10, true), false, ref passed, ref failed);
        Check("damage within limit accepted",
            BRServerValidationRules.IsDamageValid(20f, 30f), true, ref passed, ref failed);
        Check("damage above limit rejected",
            BRServerValidationRules.IsDamageValid(40f, 30f), false, ref passed, ref failed);
        Check("nearby pickup accepted",
            BRServerValidationRules.IsPickupValid(Vector3.zero, new Vector3(1f, 0f, 0f), 2f), true, ref passed, ref failed);
        Check("distant pickup rejected",
            BRServerValidationRules.IsPickupValid(Vector3.zero, new Vector3(5f, 0f, 0f), 2f), false, ref passed, ref failed);
        Check("resource increase within cap accepted",
            BRServerValidationRules.IsResourceChangeValid(5, 3, 10), true, ref passed, ref failed);
        Check("resource overflow rejected",
            BRServerValidationRules.IsResourceChangeValid(9, 3, 10), false, ref passed, ref failed);
        Check("negative resource total rejected",
            BRServerValidationRules.IsResourceChangeValid(2, -3, 10), false, ref passed, ref failed);

        string summary = "Anti-cheat rule checks: " + passed + " passed, " + failed + " failed.";
        if (failed == 0) Debug.Log("<color=green>" + summary + "</color>");
        else Debug.LogError(summary);
    }

    private static void Check(string label, bool actual, bool expected, ref int passed, ref int failed)
    {
        if (actual == expected)
        {
            passed++;
            Debug.Log("[PASS] " + label);
        }
        else
        {
            failed++;
            Debug.LogError("[FAIL] " + label + " (expected " + expected + ", got " + actual + ")");
        }
    }
}
#endif
