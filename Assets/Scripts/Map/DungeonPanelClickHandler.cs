using UnityEngine;
using UnityEngine.EventSystems;

public class DungeonPanelClickHandler : MonoBehaviour, IPointerClickHandler
{
    public Camera mainCamera;

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out localPos
        );

        // RawImage上のクリック座標 → ワールド座標
        Vector2 norm = Rect.PointToNormalized(GetComponent<RectTransform>().rect, localPos);
        Ray ray = mainCamera.ViewportPointToRay(norm);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("MiniMap clicked: " + hit.collider.name);
        }
    }
}
