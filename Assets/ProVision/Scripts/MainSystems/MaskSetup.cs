using UnityEngine;

public class MaskSetup : MonoBehaviour
{

    [Header("Mask References")]
    public SpriteMask mask1;
    public SpriteMask mask2;

    [Header("Target Objects")]
    public SpriteRenderer targetSprite;

    [Header("Debug")]
    public bool showInIntersectionOnly = true;

    private void Update()
    {
        if (targetSprite == null) {
            targetSprite = GameObject.FindGameObjectWithTag("loopAndApple").GetComponent<SpriteRenderer>();
        }
        
        bool inIntersection = CheckMasksIntersection();

      
            if (showInIntersectionOnly)
            {
                // Показываем спрайт только если он внутри обеих масок
                targetSprite.enabled = inIntersection;
            }
        
    }



    private bool CheckMasksIntersection()
    {
        if (mask1 == null || mask2 == null) return false;

        // Получаем bounds масок
        Bounds bounds1 = GetMaskBounds(mask1);
        Bounds bounds2 = GetMaskBounds(mask2);

        // Проверяем пересечение bounds
        bool boundsIntersect = bounds1.Intersects(bounds2);

        if (!boundsIntersect) return false;

        // Дополнительная проверка позиций спрайтов относительно масок
     
            if (targetSprite != null)
            {
                Vector3 spritePos = targetSprite.transform.position;
                bool inMask1 = IsInMask(spritePos, mask1);
                bool inMask2 = IsInMask(spritePos, mask2);

                if (inMask1 && inMask2)
                {
                    return true;
                }
            }
        

        return false;
    }

    private Bounds GetMaskBounds(SpriteMask mask)
    {
        SpriteRenderer spriteRenderer = mask.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            return spriteRenderer.bounds;
        }

        // Альтернативный способ получения bounds
        return new Bounds(mask.transform.position, Vector3.one);
    }

    private bool IsInMask(Vector3 position, SpriteMask mask)
    {
        // Преобразуем мировые координаты в локальные координаты маски
        Vector3 localPos = mask.transform.InverseTransformPoint(position);

        // Получаем спрайт маски
        Sprite maskSprite = mask.sprite;
        if (maskSprite == null) return false;

        // Проверяем, находится ли точка внутри bounds спрайта
        Rect spriteRect = maskSprite.rect;
        float pixelsPerUnit = maskSprite.pixelsPerUnit;

        Vector2 spriteLocalPos = new Vector2(localPos.x * pixelsPerUnit, localPos.y * pixelsPerUnit);

        return spriteRect.Contains(spriteLocalPos);
    }
}