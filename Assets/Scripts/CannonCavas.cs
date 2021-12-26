using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonCavas : MonoBehaviour
{
    public Transform cam;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// LateUpdate is called every frame, if the Behaviour is enabled.
    /// It is called after all Update functions have been called.
    /// </summary>
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
