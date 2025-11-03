using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EyeElementsLib", menuName = "Scriptable Objects/EyeElementsLib")]
public class EyeElementsLib : ScriptableObject
{
    public GameObject eyePrefab;
    public List<GameObject> applePrefabs = new List<GameObject>();
    public List<GameObject> rainbowPrefabs = new List<GameObject>();
    public List<GameObject> pupilPrefabs = new List<GameObject>();
    public List<GameObject> headsPrefabs = new List<GameObject>();
    public List<GameObject> footsPrefabs = new List<GameObject>();
    public GameObject vensPrefabs;
    public GameObject vensPrefabsDisease;

}
