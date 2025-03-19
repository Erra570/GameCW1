using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger1 : MonoBehaviour
{

    private GameObject door;
    [SerializeField] public InteractionPromptUI _interactionPromptUI;

    void Start(){
        door = GameObject.Find("TestDoor");
    }

    void OnTriggerEnter(){
        Destroy(door);
    }

    void OnTriggerExit(){

        if (!_interactionPromptUI.IsDisplayed){
            _interactionPromptUI.SetUp("Door opened !");
        }

        StartCoroutine(ShowMessage(3f));
    }

    IEnumerator ShowMessage(float delay)
    {
        yield return new WaitForSeconds(delay);

        if(_interactionPromptUI.IsDisplayed)
            _interactionPromptUI.Close();
        
        Destroy(gameObject); //avoid loading unecessary things
    }
    
}
