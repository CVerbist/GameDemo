using UnityEngine;
using System.Collections;

public class DestroyOutsideCamera : MonoBehaviour 
{
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

}
