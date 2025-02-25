using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WinAreaTrigger : MonoBehaviour
{
    public string tagPlayer = "Player";
    public UnityEvent onTriggerEnter;
    
    void OnTriggerEnter(Collider collider)
    {
        if (!collider.gameObject.CompareTag(tagPlayer))
            return;
        onTriggerEnter.Invoke();
        Destroy(gameObject); //to not trigger again
    }
}
