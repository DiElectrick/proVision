using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.U2D.IK;

public class EyeGenerator : MonoBehaviour
{
    public static EyeGenerator Instance { get; private set; }

    [SerializeField] EyeElementsLib lib;
    [SerializeField] public GameObject curentEye;
    EyeController controller;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        controller = curentEye.GetComponent<EyeController>();
    }

    public void GenerateEye(Diagnosis diagnosis)
    {

        foreach (Transform child in curentEye.transform)
        {
            if (child.gameObject.tag != "notDelete") Destroy(child.gameObject);
        }


        if (lib.applePrefabs.Count > 0)
            Instantiate(lib.applePrefabs[UnityEngine.Random.Range(0, lib.applePrefabs.Count)],
            curentEye.transform);

        if (lib.rainbowPrefabs.Count > 0)
        {
            GameObject rainbow = Instantiate(lib.rainbowPrefabs[UnityEngine.Random.Range(0, lib.rainbowPrefabs.Count)],
            curentEye.transform);

            controller.rainbow = rainbow;
        }

        if (lib.pupilPrefabs.Count > 0)
        {
            GameObject pupil = Instantiate(lib.pupilPrefabs[UnityEngine.Random.Range(0, lib.pupilPrefabs.Count)],
            curentEye.transform);

            controller.pupil = pupil;
        }

        if (lib.headsPrefabs.Count > 0)
            Instantiate(lib.headsPrefabs[UnityEngine.Random.Range(0, lib.headsPrefabs.Count)],
                curentEye.transform);

        if (lib.footsPrefabs.Count > 0)
            Instantiate(lib.footsPrefabs[UnityEngine.Random.Range(0, lib.footsPrefabs.Count)],
            curentEye.transform);


        GameObject vens = Instantiate(diagnosis.diseases[(int)Diseases.Capillaries] ? lib.vensPrefabsDisease : lib.vensPrefabs,
        curentEye.transform);
        controller.capilares = vens;

        controller.diagnosis = diagnosis;
    }
}
