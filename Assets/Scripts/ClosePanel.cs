using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosePanel : MonoBehaviour
{
    public GameObject panel;
    public GameObject player;

    public void Close()    {
        Debug.Log("Closing. . .");
        player.GetComponent<ThirdViewMvt>().enabled = true; // as it capture the mouse
        panel.SetActive(false); 
        Time.timeScale = 1;
    }
}
