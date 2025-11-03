using UnityEngine;

public class CursorFolover : MonoBehaviour
{
    [Header("Настройки следования")]
    [Tooltip("Скорость следования (0-1). Чем больше, тем быстрее")]
    public float followSpeed = 0.1f;

    [Tooltip("Ограничивать движение в пределах экрана")]
    public bool clampToScreen = true;

    [Tooltip("Отступ от краев экрана (в единицах мира)")]
    public float screenMargin = 0.5f;

    private Camera mainCamera;
    float zPosition;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (mainCamera == null) return;

        Vector3 targetPosition = GetCursorWorldPosition();

        // Плавное перемещение с использованием Lerp
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime * 10f);
    }

    Vector3 GetCursorWorldPosition()
    {
        Vector3 cursorScreenPosition = Input.mousePosition;


        cursorScreenPosition.z = -10;



        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(cursorScreenPosition);

        // Сохраняем оригинальную Z-координату
        worldPosition.z = -10;

        // Применяем ограничения экрана если нужно
        if (clampToScreen)
        {
            worldPosition = ClampPosition2D(worldPosition);
        }

        return worldPosition;
    }



    Vector3 ClampPosition2D(Vector3 position)
    {
        // Для 2D используем границы видимой области камеры
        float cameraHeight = mainCamera.orthographicSize * 2;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        float minX = mainCamera.transform.position.x - cameraWidth / 2 + screenMargin;
        float maxX = mainCamera.transform.position.x + cameraWidth / 2 - screenMargin;
        float minY = mainCamera.transform.position.y - cameraHeight / 2 + screenMargin;
        float maxY = mainCamera.transform.position.y + cameraHeight / 2 - screenMargin;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        return position;
    }

}
