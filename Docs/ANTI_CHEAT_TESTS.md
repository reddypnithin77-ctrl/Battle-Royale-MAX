# Anti-cheat rule check runner

A Unity Editor validation harness checks the deterministic validation rules and the in-memory authoritative match service.

## Run it

1. Open the Unity project in Unity Editor and wait for scripts to compile.
2. Choose **Tools → Battle Royale → Run Anti-Cheat Rule Checks**.
3. Open **Window → General → Console** and review each PASS/FAIL line.
4. The final summary reports the total passed and failed checks. Any failure is logged as an error.

## Coverage

The runner includes 44 checks covering:
- Normal, excessive, vertical, non-finite, and invalid-tolerance movement.
- Fire cadence, backwards timestamps, empty magazines, and reload blocking.
- Damage bounds, pickup distance, and inventory limits.
- Player registration and duplicate registration.
- Authoritative movement and firing state.
- Reload start, early completion rejection, and successful completion.
- Server snapshots, ammo pickups, damage, elimination, and post-elimination movement rejection.
- Server line-of-sight rejection for wall-obstructed hits.
- Normal headshots, suspicious headshot ratios/streaks, and backwards hit timestamps.
- Authoritative hit application and blocked-hit rejection.

## Important limitation

This is a local Unity Editor logic test, not a live multiplayer security test. It does not prove a deployed server is secure, validate networking/authentication, or test server-side hit detection and persistence. The Unity Editor runner has been added and reviewed, but has **not been executed in Unity in this environment**. Run it locally to obtain an actual PASS/FAIL result.
