using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;
    public bool isRock;
    public GameManager manager;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor" && !isRock)
        {
            Destroy(gameObject, 3);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isMelee && other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
