#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class BRServerValidationRuleChecks
{
    [MenuItem("Tools/Battle Royale/Run Anti-Cheat Rule Checks")]
    public static void Run()
    {
        RunChecks();
    }

    public static int RunChecks()
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
            BRServerValidationRules.IsShotValid(1.01f, 0.8f, 5f, 10, false), true, ref passed, ref failed);
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


        Check("NaN movement rejected",
            BRServerValidationRules.IsMovementValid(Vector3.zero, new Vector3(float.NaN, 0f, 0f), 1f, 5f, 0.1f, 2f), false, ref passed, ref failed);
        Check("infinite movement speed rejected",
            BRServerValidationRules.IsMovementValid(Vector3.zero, Vector3.one, 1f, float.PositiveInfinity, 0.1f, 2f), false, ref passed, ref failed);
        Check("negative movement tolerance rejected",
            BRServerValidationRules.IsMovementValid(Vector3.zero, Vector3.one, 1f, 5f, -1f, 2f), false, ref passed, ref failed);
        Check("backwards shot timestamp rejected",
            BRServerValidationRules.IsShotValid(0.5f, 1f, 5f, 10, false), false, ref passed, ref failed);
        Check("NaN damage rejected",
            BRServerValidationRules.IsDamageValid(float.NaN, 30f), false, ref passed, ref failed);
        Check("infinite pickup distance rejected",
            BRServerValidationRules.IsPickupValid(Vector3.zero, Vector3.one, float.PositiveInfinity), false, ref passed, ref failed);

        BRAuthoritativeMatchService match = new BRAuthoritativeMatchService(null);
        Check("server player registration accepted",
            match.RegisterPlayer("test-player", Vector3.zero, 0f, 5f, 2, 5, 100, 2), true, ref passed, ref failed);
        Check("duplicate player registration rejected",
            match.RegisterPlayer("test-player", Vector3.zero, 0f), false, ref passed, ref failed);
        Check("authoritative valid movement accepted",
            match.TryMove("test-player", new Vector3(2f, 0f, 0f), 1f), true, ref passed, ref failed);
        Check("authoritative excessive movement rejected",
            match.TryMove("test-player", new Vector3(20f, 0f, 0f), 1.1f), false, ref passed, ref failed);
        Check("authoritative shot accepted",
            match.TryFire("test-player", 1f, 5f, 20f, 30f), true, ref passed, ref failed);
        Check("authoritative rapid repeat shot rejected",
            match.TryFire("test-player", 1.05f, 5f, 20f, 30f), false, ref passed, ref failed);
        Check("authoritative reload started",
            match.TryBeginReload("test-player", 1.1f, 1f), true, ref passed, ref failed);
        Check("authoritative fire blocked during reload",
            match.TryFire("test-player", 1.2f, 5f, 20f, 30f), false, ref passed, ref failed);
        Check("reload cannot finish early",
            match.TryCompleteReload("test-player", 2f), false, ref passed, ref failed);
        Check("reload completes at server deadline",
            match.TryCompleteReload("test-player", 2.1f), true, ref passed, ref failed);
        Vector3 snapshotPosition;
        int snapshotHealth, snapshotAmmo, snapshotReserve;
        bool snapshotEliminated;
        bool gotSnapshot = match.TryGetSnapshot("test-player", out snapshotPosition, out snapshotHealth,
            out snapshotAmmo, out snapshotReserve, out snapshotEliminated);
        Check("snapshot reflects server reload",
            gotSnapshot && snapshotAmmo == 2 && snapshotReserve == 4 && !snapshotEliminated,
            true, ref passed, ref failed);
        Check("authoritative ammo pickup accepted",
            match.TryCollectAmmo("test-player", new Vector3(2.5f, 0f, 0f), 2f, 3, 10), true, ref passed, ref failed);
        Check("authoritative distant ammo pickup rejected",
            match.TryCollectAmmo("test-player", new Vector3(50f, 0f, 0f), 2f, 3, 10), false, ref passed, ref failed);
        Check("authoritative damage applied",
            match.TryApplyServerDamage("test-player", 40), true, ref passed, ref failed);
        Check("authoritative player eliminated at zero health",
            match.TryApplyServerDamage("test-player", 100), true, ref passed, ref failed);
        Check("eliminated player cannot move",
            match.TryMove("test-player", new Vector3(2f, 0f, 0f), 3f), false, ref passed, ref failed);

        string summary = "Anti-cheat rule checks: " + passed + " passed, " + failed + " failed.";
        if (failed == 0) Debug.Log("<color=green>" + summary + "</color>");
        else Debug.LogError(summary);
        return failed;
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
