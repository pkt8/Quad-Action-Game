using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMissile : Bullet
{
    public Transform target;
    NavMeshAgent nav;

    // Start is called before the first frame update
    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(MissileFw());
    }

    IEnumerator MissileFw()
    {
        nav.SetDestination(target.position);
        yield return new WaitForSeconds(10f);
    }
}
