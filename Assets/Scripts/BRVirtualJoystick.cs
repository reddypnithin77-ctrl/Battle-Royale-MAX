using UnityEngine;
using UnityEngine.EventSystems;

// Attach to the joystick's draggable background. Assign the movable handle RectTransform.
public class BRVirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public RectTransform handle;
    public float handleRange = 65f;
    private RectTransform rectTransform;
    private Vector2 input;

    private void Awake() { rectTransform = transform as RectTransform; }

    public void OnPointerDown(PointerEventData eventData) { OnDrag(eventData); }

    public void OnDrag(PointerEventData eventData)
    {
        if (rectTransform == null) return;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 local)) return;
        Vector2 half = rectTransform.rect.size * 0.5f;
        input = new Vector2(local.x / Mathf.Max(1f, half.x), local.y / Mathf.Max(1f, half.y));
        input = Vector2.ClampMagnitude(input, 1f);
        if (handle != null) handle.anchoredPosition = input * handleRange;
        if (MobileInputBridge.Instance != null) MobileInputBridge.Instance.SetMove(input);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;
        if (handle != null) handle.anchoredPosition = Vector2.zero;
        if (MobileInputBridge.Instance != null) MobileInputBridge.Instance.SetMove(Vector2.zero);
    }
}
