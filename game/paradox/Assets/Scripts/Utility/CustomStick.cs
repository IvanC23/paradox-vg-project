using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.Serialization;

[AddComponentMenu("Input/On-Screen Button")]
public class CustomStick : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out m_PointerDownPos);
  
        Debug.Log((m_PointerDownPos.x) + "Position pressed");
        
        
        if (m_PointerDownPos.x > -938)
        {
            var startdelta = new Vector2(movementRange, 0.0f);
            ((RectTransform)transform).anchoredPosition = m_StartPos + (Vector3)startdelta;
            SendValueToControl(startdelta);
        }
        else if (m_PointerDownPos.x < -938)
        {
            var startdelta = new Vector2(-movementRange, 0.0f);
            ((RectTransform)transform).anchoredPosition = m_StartPos + (Vector3)startdelta;
            SendValueToControl(startdelta);
        }



    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out var position);
        var delta = position - m_PointerDownPos;
        delta.y = 0;

        delta = Vector2.ClampMagnitude(delta, movementRange);
        ((RectTransform)transform).anchoredPosition = m_StartPos + (Vector3)delta;

        var newPos = new Vector2(delta.x / movementRange, delta.y / movementRange);
        SendValueToControl(newPos);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ((RectTransform)transform).anchoredPosition = m_StartPos;
        SendValueToControl(Vector2.zero);
    }

    private void Start()
    {
        m_StartPos = ((RectTransform)transform).anchoredPosition;
    }

    private Vector3 m_StartPos;
    private Vector2 m_PointerDownPos;

    [FormerlySerializedAs("movementRange")]
    [SerializeField]
    private float movementRange = 50;

    [InputControl(layout = "Vector2")]
    [SerializeField]
    private string m_ControlPath;

    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }
}

