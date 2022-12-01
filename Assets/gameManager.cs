using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    [SerializeField]
    private Text timerText;
    [SerializeField]
    private Text lapText;
    [SerializeField]
    private GameObject win;
    private float timer;

    private int lap = 0;
    private void Start()
    {
        lapText.text = "Lap: " + lap.ToString();
    }
    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        timerText.text = "Time: "+((int)timer).ToString();
    }
    public void addLap()
    {
        lap++;
        lapText.text = "Lap: "+ lap.ToString();
        if (lap >= 3)
        {
            win.SetActive(true);
            Controller.instance.canDrive = false;
        }
    }
}
