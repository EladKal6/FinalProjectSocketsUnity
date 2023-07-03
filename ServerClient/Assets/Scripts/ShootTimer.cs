using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShootTimer : MonoBehaviour
{
    public static ShootTimer instance;


    public GameObject ShootingCityCanvas;
    public TextMeshProUGUI shootTimerText;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void StartShootTimer(int countdown)
    {
        StartCoroutine(instance.IStartShootTimer(countdown));
    }

    public IEnumerator IStartShootTimer(int countdown)
    {
        for (int i = countdown; i > 0; i--)
        {
            shootTimerText.text = "Time left until shooting is enabled: " + i;
            yield return new WaitForSeconds(1);
        }

        ShootingCityCanvas.gameObject.SetActive(false);
        foreach (PlayerManager player in GameManager.players.Values)
        {
            player.ShowGun();
        }
    }
}
