using UnityEngine;
using UnityEditor;

public class GizmosOnSelected : MonoBehaviour
{

    private void OnDrawGizmosSelected()
    {
        Handles.Label(transform.position, gameObject.name);
    }

}
