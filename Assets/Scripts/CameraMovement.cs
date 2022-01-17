using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform Target;

    Vector3 startDistance, moveVector;

    // Start is called before the first frame update
    void Start()
    {
        startDistance = transform.position - Target.position;
    }

    // Update is called once per frame
    void Update()
    {
        moveVector = Target.position + startDistance;

        moveVector.z = 0;
        moveVector.y = startDistance.y;

        transform.position = moveVector;
    }
}