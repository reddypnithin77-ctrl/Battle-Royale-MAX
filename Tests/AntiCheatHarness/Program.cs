using System;
public static class Program
{
    public static int Main()
    {
        int failures = BRServerValidationRuleChecks.RunChecks();
        Console.WriteLine(failures == 0 ? "AUTOMATED ANTI-CHEAT TESTS PASSED" : "AUTOMATED ANTI-CHEAT TESTS FAILED");
        return failures == 0 ? 0 : 1;
    }
}
