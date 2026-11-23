using System;
namespace UnityEngine
{
    public class MonoBehaviour { }
    public struct Vector3
    {
        public float x, y, z;
        public Vector3(float x, float y, float z) { this.x=x; this.y=y; this.z=z; }
        public static Vector3 zero => new Vector3(0,0,0);
        public static Vector3 one => new Vector3(1,1,1);
        public float sqrMagnitude => x*x + y*y + z*z;
        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.x-b.x,a.y-b.y,a.z-b.z);
    }
    public static class Mathf
    {
        public static float Abs(float x) => Math.Abs(x);
        public static float Max(float a, float b) => Math.Max(a,b);
        public static int Max(int a, int b) => Math.Max(a,b);
        public static int Min(int a, int b) => Math.Min(a,b);
        public static int RoundToInt(float value) => (int)Math.Round(value, MidpointRounding.AwayFromZero);
    }
    public static class Debug
    {
        public static void Log(object message) => Console.WriteLine(message);
        public static void LogWarning(object message) => Console.WriteLine("WARN: " + message);
        public static void LogError(object message) => Console.Error.WriteLine(message);
    }
}
namespace UnityEditor
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MenuItem : Attribute
    {
        public MenuItem(string path) { }
    }
}
public class BRAuditReporter
{
    public void Report(string playerId, string rule, string detail) { }
}
