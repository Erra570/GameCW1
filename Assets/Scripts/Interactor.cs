using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{

    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionPointRadius = 0.5f;
    [SerializeField] public LayerMask _interactableMask;
    [SerializeField] private InteractionPromptUI _interactionPromptUI;

    public GameObject canvas;
    private readonly Collider[] _colliders = new Collider[3];

    [SerializeField] private int _numFound;

    private IInteractable _interactable;

    private void Update()
    {
        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders, _interactableMask);

        // If some interactable object is found
        if(_numFound > 0){
            _interactable = _colliders[0].GetComponent<IInteractable>();

            // Display popup message
            if(_interactable != null){
                if (!_interactionPromptUI.IsDisplayed){
                    _interactionPromptUI.SetUp(_interactable.InteractionPrompt);
                }

                //if e is pressed, then execute desired action
                if (Keyboard.current.eKey.wasPressedThisFrame)
                    _interactable.Interact(this);
            }
        }
        
        // Close popup when there are no interaction available
        else
        {
            if (_interactable != null)
                _interactable = null;
            if(_interactionPromptUI.IsDisplayed)
                _interactionPromptUI.Close();
        }
    }

    private void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
    }
}
