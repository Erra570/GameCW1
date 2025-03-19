using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger2 : MonoBehaviour
{
    [SerializeField] public InteractionPromptUI _interactionPromptUI;

    void OnTriggerEnter(){
        if (!_interactionPromptUI.IsDisplayed){
            _interactionPromptUI.SetUp("An other message !");
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
