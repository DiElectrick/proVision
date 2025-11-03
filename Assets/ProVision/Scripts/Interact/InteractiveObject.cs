using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    [Header("Rygidbody")]
    [SerializeField] float mass = 1f;
    [SerializeField] float drag = 0.5f;
    [SerializeField] float angularDrag = 0.05f;

    [Header("Physics")]
    [SerializeField] float gravityScale = 1f;
    [SerializeField] float maxSpeed = 10f;
    [SerializeField] float maxDistance;

    private Rigidbody2D rb;
    private bool isBeingDragged = false;

    public float rotationSpeed = 2f;        // Скорость поворота
    public float maxAngularVelocity = 90f;  // Максимальная угловая скорость
    public float torqueForce = 100f;        // Сила крутящего момента
    public float targetRotationAngle;       // Целевой угол оворота              

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Настройка физики
        if (rb != null)
        {
            rb.mass = mass;
            rb.linearDamping = drag;
            rb.angularDamping = angularDrag;
        }
    }


    public void StartDragging()
    {
        if (isBeingDragged) return;

        isBeingDragged = true;

    }


    public void StopDragging()
    {
        if (!isBeingDragged) return;

        isBeingDragged = false;


    }

    public void UpdateDragPosition()
    {
        if (!isBeingDragged) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 dir = (mousePos - transform.position);

        float dist = dir.magnitude;

        if (dist < 0.001f) return;

        rb.linearVelocity = dir.normalized * maxSpeed * Mathf.Min(1f, dist / maxDistance);



    }

    void ApplyRotation()
    {
        // Текущий угол объекта
        float currentAngle = rb.rotation;

        // Вычисляем кратчайшую разницу между углами
        float angleDifference = Mathf.DeltaAngle(currentAngle, targetRotationAngle);

        // Вычисляем угловую скорость на основе разницы углов
        float targetAngularVelocity = Mathf.Clamp(angleDifference * rotationSpeed, -maxAngularVelocity, maxAngularVelocity);

        // Плавно интерполируем к целевой угловой скорости
        rb.angularVelocity = Mathf.Lerp(rb.angularVelocity, targetAngularVelocity, torqueForce * Time.fixedDeltaTime);
    }

    void FixedUpdate()
    {
        if (isBeingDragged)
        {
            UpdateDragPosition();
            ApplyRotation();
        }
        else
        {
            rb.linearVelocityY -= gravityScale * Time.deltaTime;
        }

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

    }

    // Геттер для проверки состояния
    public bool IsBeingDragged()
    {
        return isBeingDragged;
    }

}
