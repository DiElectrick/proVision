using UnityEngine;

public class BlikController : MonoBehaviour
{
    [Header("Light Follow Settings")]
    [SerializeField] float moveKoef = 0.1f;
    [SerializeField] float maxRange = 1.0f;

    private void Update()
    {
        MoveTowardsLight();
    }

    void MoveTowardsLight()
    {
        Vector3 lightDirection = GetNearestLightDirection();
        transform.localPosition = CalculateBlikPosition(lightDirection);
    }

    Vector3 GetNearestLightDirection()
    {
        GameObject[] lightObjects = GameObject.FindGameObjectsWithTag("Light");

        if (lightObjects.Length == 0)
        {
            return Vector3.zero;
        }

        GameObject nearestLight = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject lightObj in lightObjects)
        {
            float distance = Vector3.Distance(transform.position, lightObj.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestLight = lightObj;
            }
        }

        if (nearestLight != null)
        {
            return nearestLight.transform.position - transform.parent.position;
        }

        return Vector3.zero;
    }

    Vector3 CalculateBlikPosition(Vector3 dir)
    {
        float distance = dir.magnitude * moveKoef;
        distance = Mathf.Min(distance, maxRange);

        return dir.normalized * distance;
    }
}