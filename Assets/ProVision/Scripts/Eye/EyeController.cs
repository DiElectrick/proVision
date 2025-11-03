using UnityEngine;

public class EyeController : MonoBehaviour
{
    [SerializeField] public GameObject rainbow;
    [SerializeField] public GameObject pupil;
    [SerializeField] public GameObject capilares;
    [SerializeField] Transform cursorFolower;

    [Header("Rotate")]
    [SerializeField] float rainbowCircleKoef = 0.1f;
    [SerializeField] float rainbowMaxRange = 1.0f;
    [SerializeField] float pupilCircleKoef = 0.1f;
    [SerializeField] float pupilMaxRange = 1.0f;

    [Header("Scale")]
    [SerializeField] float maxScaleDistance;
    [SerializeField] float maxScale;
    [SerializeField] float minScale;

    [Header("Disease scale")]
    [SerializeField] float maxScaleDistanceRandomRange = 0.1f;
    [SerializeField] float maxScaleRandomRange = 0.1f;
    [SerializeField] float minScaleRandomRange = 0.1f;

    [SerializeField] public Diagnosis diagnosis = new Diagnosis();

    [SerializeField] Vector3 mousePos;

    private void Update()
    {
        RotateEye();
        ScaleEye();

    }

    void ScaleEye() {
        float dist = NearestLightDistance();
        float scale = minScale + ((maxScale - minScale) * Mathf.Min(1f, dist / maxScaleDistance));

        pupil.transform.localScale = new Vector2(scale,scale);
    }

  

    float NearestLightDistance()
    {
        GameObject[] lightObjects = GameObject.FindGameObjectsWithTag("Light");

        if (lightObjects.Length == 0)
        {
            return 0f;
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
        
        return closestDistance;
    }

    void RotateEye() {

        if (!diagnosis.diseases[(int)Diseases.Rotation]) mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        else mousePos = cursorFolower.position;

        rainbow.transform.localPosition = CalculateElementPosition(rainbowCircleKoef, rainbowMaxRange, mousePos - transform.position);
        capilares.transform.localPosition = rainbow.transform.localPosition;
        pupil.transform.localPosition = CalculateElementPosition(pupilCircleKoef, pupilMaxRange, mousePos - transform.position);
    }

    Vector3 CalculateElementPosition(float circleKoef, float maxRange, Vector3 dir)
    {
        float distance = dir.magnitude * circleKoef;
        distance = Mathf.Min(distance, maxRange);

        return dir.normalized * distance;
    }
}
