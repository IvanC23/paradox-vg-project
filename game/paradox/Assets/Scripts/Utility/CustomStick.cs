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

        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out m_PointerDownPos);

        if(m_PointerDownPos.x <510){
        Debug.Log((m_PointerDownPos.x) + "Position pressed");

        m_newPos.x = m_PointerDownPos.x + 190;
        m_newPos.y = m_PointerDownPos.y - 70;
        ((RectTransform)transform).anchoredPosition = m_newPos;

        }

    }

    public void OnDrag(PointerEventData eventData)
    {
       
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out m_PointerDownPosDrag);
        
        if(firstDrag){
            startingPointDrag = m_PointerDownPosDrag;
            firstDrag = false;
        }else{
         Debug.Log((startingPointDrag.x) + "First position pressed");
         Debug.Log((m_PointerDownPosDrag.x) + "Second position pressed");
        if (m_PointerDownPosDrag.x + 1000 >= startingPointDrag.x + 1000)
        {
            var startdelta = new Vector2(movementRange, 0.0f);
            ((RectTransform)transform).anchoredPosition = m_newPos + (Vector3)startdelta;
            SendValueToControl(startdelta);
        }
        else if (m_PointerDownPosDrag.x + 1000 < startingPointDrag.x + 1000)
        {
            var startdelta = new Vector2(-movementRange, 0.0f);
            ((RectTransform)transform).anchoredPosition = m_newPos + (Vector3)startdelta;
            SendValueToControl(startdelta);
        }
        
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ((RectTransform)transform).anchoredPosition = m_StartPos;
        SendValueToControl(Vector2.zero);
        firstDrag = true;
    }

    private void Start()
    {
        m_StartPos = ((RectTransform)transform).anchoredPosition;
    }

    private Vector3 m_StartPos;

    private Vector3 m_newPos;

    private Vector2 startingPointDrag;
    private bool firstDrag = true;
    private Vector2 m_PointerDownPos;
    private Vector2 m_PointerDownPosDrag;

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

