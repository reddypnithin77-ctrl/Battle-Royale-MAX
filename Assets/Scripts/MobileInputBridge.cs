using UnityEngine;

public class MobileInputBridge : MonoBehaviour
{
    public static MobileInputBridge Instance { get; private set; }
    public Vector2 Move { get; private set; }
    public Vector2 LookDelta { get; private set; }
    public bool FireHeld { get; private set; }
    public bool JumpPressed { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void SetMove(Vector2 value) => Move = Vector2.ClampMagnitude(value, 1f);
    public void SetFire(bool held) => FireHeld = held;
    public void PressJump() => JumpPressed = true;

    private void Update()
    {
        LookDelta = Vector2.zero;
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.position.x < Screen.width * 0.48f) continue;
            if (touch.phase == TouchPhase.Moved) LookDelta += touch.deltaPosition;
        }
    }

    public bool ConsumeJump()
    {
        bool result = JumpPressed;
        JumpPressed = false;
        return result;
    }
}
