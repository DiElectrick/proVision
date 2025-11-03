using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectDragger : MonoBehaviour
{
    [Header("Dragging Settings")]
    public float heightWhenDragged = 0.5f;
    public LayerMask interactableLayer;

    private InteractiveObject selectedObject;
    private Camera mainCamera;
    private bool isDragging = false;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Проверяем, не кликнули ли мы по UI элементу
            if (!IsPointerOverUI())
            {
                TrySelectObject();
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            ReleaseObject();
        }
    }

    // Проверка, находится ли курсор над UI элементом
    private bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    void TrySelectObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, interactableLayer);

        if (hit.collider != null)
        {
            InteractiveObject obj = hit.collider.GetComponent<InteractiveObject>();
            if (obj != null)
            {
                obj.StartDragging();
                selectedObject = obj;
                isDragging = true;
            }
        }
    }

    void ReleaseObject()
    {
        if (selectedObject != null)
        {
            selectedObject.StopDragging();
        }

        selectedObject = null;
        isDragging = false;
    }
}