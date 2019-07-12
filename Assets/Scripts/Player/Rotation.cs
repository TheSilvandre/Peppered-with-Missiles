using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {
	
    [SerializeField]
    int rotationSpeed = 200;
    [SerializeField]
    Vector3 axis;

    void Update () {
        transform.Rotate(axis, rotationSpeed * Time.deltaTime);
	}
}
