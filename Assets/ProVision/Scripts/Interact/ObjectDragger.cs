using UnityEngine;

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
            TrySelectObject();
        }


        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            ReleaseObject();
        }
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