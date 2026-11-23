# Anti-cheat rule check runner

A small Unity Editor validation harness is included for the deterministic rule helpers.

1. Open the Unity project and wait for compilation.
2. Choose **Tools → Battle Royale → Run Anti-Cheat Rule Checks**.
3. Open the Console window and review the PASS/FAIL entries and summary.

The checks cover accepted/rejected movement, vertical movement bounds, fire cadence, empty ammo, reload state, damage bounds, pickup distance, and inventory/resource limits.

These checks exercise rule logic only. They do not simulate a network attacker, prove a server deployment is secure, or replace integration tests against a trusted authoritative server. No Unity execution has been performed by this repository update.
