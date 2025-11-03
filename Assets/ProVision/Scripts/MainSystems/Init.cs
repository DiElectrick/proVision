using System.Collections.Generic;
using UnityEngine;

public class Init : MonoBehaviour
{
    [SerializeField] List<GameObject> activate = new List<GameObject>();
    [SerializeField] List<GameObject> deactivate = new List<GameObject>();
    private void Awake()
    {
        foreach (GameObject go in activate)
        {
            go.SetActive(true);
        }
        foreach (GameObject go in deactivate)
        {
            go.SetActive(false);
        }
    }

}
