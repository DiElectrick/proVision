using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.U2D.IK;

public class EyeGenerator : MonoBehaviour
{
    public static EyeGenerator Instance { get; private set; }

    [SerializeField] EyeElementsLib lib;
    [SerializeField] public GameObject curentEye;
    [SerializeField] EyeController controller;

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
        if(controller = null) controller = curentEye.GetComponent<EyeController>();
    }

    public void GenerateEye(Diagnosis diagnosis)
    {
        controller = curentEye.GetComponent<EyeController>();

       // Debug.Log("gen");

        foreach (Transform child in curentEye.transform)
        {
            if (child.gameObject.tag != "notDelete") Destroy(child.gameObject);
        }

      //  Debug.Log("gen1");

        if (lib.applePrefabs.Count > 0)
        {


            Instantiate(lib.applePrefabs[UnityEngine.Random.Range(0, lib.applePrefabs.Count)],
            curentEye.transform);
          //  Debug.Log("gen2");
        }


        if (lib.rainbowPrefabs.Count > 0)
        {
            GameObject rainbow = Instantiate(lib.rainbowPrefabs[UnityEngine.Random.Range(0, lib.rainbowPrefabs.Count)],
            curentEye.transform);

            controller.rainbow = rainbow;
          //  Debug.Log("gen3");
        }

        if (lib.pupilPrefabs.Count > 0)
        {
            GameObject pupil = Instantiate(lib.pupilPrefabs[UnityEngine.Random.Range(0, lib.pupilPrefabs.Count)],
            curentEye.transform);
           
            controller.pupil = pupil;
        }

        if (lib.headsPrefabs.Count > 0)
        {
            Instantiate(lib.headsPrefabs[UnityEngine.Random.Range(0, lib.headsPrefabs.Count)],
                curentEye.transform);
            //Debug.Log("gen5");
        }
        if (lib.footsPrefabs.Count > 0)
        {
            Instantiate(lib.footsPrefabs[UnityEngine.Random.Range(0, lib.footsPrefabs.Count)],
            curentEye.transform);
           // Debug.Log("gen6");
        }

        GameObject vens = Instantiate(diagnosis.diseases[(int)Diseases.Capillaries] ? lib.vensPrefabsDisease : lib.vensPrefabs,
        curentEye.transform);
        controller.capilares = vens;
       // Debug.Log("gen7");
        controller.diagnosis = diagnosis;
    }
}
