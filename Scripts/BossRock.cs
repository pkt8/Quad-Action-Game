using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    // Start is called before the first frame update
    Rigidbody rigid;
    float angularPower = 2;
    float scaleValue = 0.1f;
    bool isShoot;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());
    }

    IEnumerator GainPowerTimer()
    {
        yield return new WaitForSeconds(2.2f);
        isShoot = true;
    }

    IEnumerator GainPower()
    {
        while (!isShoot)
        {
            angularPower += 4f;
            scaleValue += 0.002f;
            transform.localScale = Vector3.one * scaleValue;
            if(scaleValue > 1.2f)
                yield break;
            rigid.AddTorque(transform.right * angularPower * Time.deltaTime, ForceMode.Acceleration );
            yield return null;
        }

    }
}
