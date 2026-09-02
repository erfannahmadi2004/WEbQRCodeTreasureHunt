using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler
{
    private RectTransform rectTransform;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        GameEvents.InProgress += ResetPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    private void ResetPosition(int num)
    {
        transform.position = initialPosition;
    }

}