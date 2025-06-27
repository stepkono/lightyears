using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollSelector : MonoBehaviour, IEndDragHandler
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private float itemWidth = 10f;
    [SerializeField] private float snapSpeed = 10f;
    [SerializeField] private int selectedIndex;

    private bool isSnapping = false;
    private float targetX;

    void Update()
    {
        if (isSnapping)
        {
            Vector2 pos = content.anchoredPosition;
            pos.x = Mathf.Lerp(pos.x, targetX, Time.deltaTime * snapSpeed);
            content.anchoredPosition = pos;

            if (Mathf.Abs(pos.x - targetX) < 0.1f)
            {
                isSnapping = false;
                content.anchoredPosition = new Vector2(targetX, pos.y);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float x = content.anchoredPosition.x;
        selectedIndex = Mathf.RoundToInt(-x / itemWidth); // negate because content moves left
        selectedIndex = Mathf.Clamp(selectedIndex, 0, content.childCount - 1);

        targetX = -selectedIndex * itemWidth;
        isSnapping = true;
    }

    public int GetSelectedValue()
    {
        var child = content.GetChild(selectedIndex);
        if (child.TryGetComponent(out Text text)) return int.Parse(text.text);
        return selectedIndex; // fallback
    }
}
