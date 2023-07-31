using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    // Start is called before the first frame update
    void Update()
    {
        transform.Rotate(Vector3.right * 30 * Time.deltaTime);
    }
}
