using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] Transform obj;
    void Update()
    {
        if(obj != null) transform.position = obj.position;
    }
}
