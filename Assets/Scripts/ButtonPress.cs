using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Vector2 _shiftOffset = new Vector2(0f, -6f);
    private RectTransform _targetToShift;

    private void Awake()
    {
        _targetToShift = transform.GetChild(0).GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_targetToShift != null)
            _targetToShift.anchoredPosition += _shiftOffset;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_targetToShift != null)
            _targetToShift.anchoredPosition -= _shiftOffset;
    }
}