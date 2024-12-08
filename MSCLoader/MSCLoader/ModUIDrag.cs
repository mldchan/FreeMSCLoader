using UnityEngine.EventSystems;

namespace MSCLoader;

/// <summary>
///     Make Unity.UI element draggable, attachable to UI gameobject
/// </summary>
public class ModUIDrag : MonoBehaviour, IDragHandler
{
    private RectTransform m_transform;

    private void Start()
    {
        m_transform = GetComponent<RectTransform>();
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        m_transform.position += new Vector3(eventData.delta.x, eventData.delta.y);
    }
}