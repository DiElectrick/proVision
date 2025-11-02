using UnityEngine;

public class LookAnimation : MonoBehaviour
{
    [SerializeField] GameObject rainbow;
    [SerializeField] GameObject pupil;

    [SerializeField] float rainbowCircleKoef = 0.1f;
    [SerializeField] float rainbowMaxRange = 1.0f;

    [SerializeField] float pupilCircleKoef = 0.1f;
    [SerializeField] float pupilMaxRange = 1.0f;

    private void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        rainbow.transform.position = CalculateElementPosition(rainbowCircleKoef, rainbowMaxRange, mousePos - transform.position);
        pupil.transform.position = CalculateElementPosition(pupilCircleKoef, pupilMaxRange, mousePos - transform.position);

    }

    Vector3 CalculateElementPosition(float circleKoef, float maxRange, Vector3 dir) { 
        float distance = dir.magnitude * circleKoef;
        distance = Mathf.Min(distance, maxRange);

        return dir.normalized * distance;
    }
}
