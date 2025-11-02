using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    [Header("Interaction Effects")]
    public ParticleSystem collisionParticles;
    public AudioClip collisionSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        InteractiveObject otherObject = collision.gameObject.GetComponent<InteractiveObject>();

        if (otherObject != null)
        {
            HandleObjectCollision(collision, otherObject);
        }
    }

    void HandleObjectCollision(Collision2D collision, InteractiveObject otherObject)
    {
        // Воспроизводим эффекты
        if (collisionParticles != null)
        {
            ContactPoint2D contact = collision.contacts[0];
            Instantiate(collisionParticles, contact.point, Quaternion.identity);
        }

        if (collisionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(collisionSound, Mathf.Clamp01(collision.relativeVelocity.magnitude * 0.1f));
        }

        // Специфические взаимодействия в зависимости от типов объектов
        CheckSpecialInteractions(otherObject);
    }

    void CheckSpecialInteractions(InteractiveObject otherObject)
    {
        // Пример: если это ингредиент и столкновение с котлом
        if (gameObject.CompareTag("Ingredient") && otherObject.CompareTag("Cauldron"))
        {
            HandleIngredientInCauldron();
        }
        
    }

    void HandleIngredientInCauldron()
    {
        // Логика добавления ингредиента в зелье
        Debug.Log("Ingredient added to cauldron!");

        // Уничтожаем или скрываем объект
        StartCoroutine(DissolveObject());
    }

    System.Collections.IEnumerator DissolveObject()
    {
        // Анимация растворения
        float dissolveTime = 1f;
        float elapsed = 0f;
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        Color originalColor = renderer.color;

        while (elapsed < dissolveTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / dissolveTime);
            renderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }
}