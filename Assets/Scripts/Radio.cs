using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Code related to the radio here.
public class Radio : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    public string InteractionPrompt => _prompt; //useless for the moment
    private bool isInteracting = false;
    public RectTransform radioCursor;  // Radio cursor
    public Button leftButton;    // Left movement button
    public Button rightButton;   // Right movement button
    public float moveRange = 200f;  // Distance to move left or right
    private Vector2 initialPosition; // Start position of the cursor
    private Vector2 currentPosition; // Current position of the cursor

    void Start(){

        initialPosition = radioCursor.anchoredPosition; // Store the initial position of the cursor
        currentPosition = initialPosition;

        leftButton.onClick.AddListener(MoveLeft); // Set up button listener
        rightButton.onClick.AddListener(MoveRight);
    }

    // Move the cursor to the left
    void MoveLeft()
    {
        if(currentPosition.x > -400){
            // Move the cursor to the left by reducing it's anchoredPosition value (X position)
            radioCursor.anchoredPosition = new Vector2(currentPosition.x - moveRange, radioCursor.anchoredPosition.y);
            currentPosition = radioCursor.anchoredPosition; //update currentPosition
        }
    }

    // Move the cursor to the right
    void MoveRight()
    {
        if(currentPosition.x < 400){
            // Move the cursor to the right by incrementing it's anchoredPosition value (X position)
            radioCursor.anchoredPosition = new Vector2(currentPosition.x + moveRange, radioCursor.anchoredPosition.y);
            currentPosition = radioCursor.anchoredPosition; //update currentPosition
        }
    }

    public bool Interact(Interactor interactor)
    {
        isInteracting = !isInteracting;
        if(isInteracting){
            interactor.canvas.SetActive(true);
            Time.timeScale = 0f; //stop time when interacting

            // Show system mouse cursor and unlock it
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        else{
            interactor.canvas.SetActive(false);
            Time.timeScale = 1f; //resume when stopping interaction

            // Hide system mouse cursor and lock it to the center of the screen
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        Debug.Log("Interacting with radio!");
        return true;
    }
}
