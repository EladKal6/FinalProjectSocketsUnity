using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanShootTimer : MonoBehaviour
{
    public static bool canShoot = false;

    public static int countdown = 10;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Shooting city started!");
        ServerSend.StartShootTimer(countdown);
        StartCoroutine(ShootingCountDown());
    }

    private IEnumerator ShootingCountDown()
    {
        yield return new WaitForSeconds(countdown);

        canShoot = true;
    }
}
