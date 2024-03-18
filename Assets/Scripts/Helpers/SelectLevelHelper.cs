using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// 
///     This script is attached to each of the Level Select Menu Buttons.
///     It is a helper for the controller input.
///     It moves the scrollbar in the Level Select screen to align the viewport with the currently selected level button.
/// 
/// </summary>

public class SelectLevelHelper : MonoBehaviour
{
    [SerializeField] private Scrollbar scrollbar; // the draggable scrollbar for the horizontal viewport in the Level Select screen 
    [SerializeField] private float scrollbarValue; // the position that the scrollbar value is set to when a level button is selected
    [SerializeField] private PlayerInput input; // Input System reference

    // the update loop runs every frame in Unity
    private void Update()
    {
        // if gamepad is the current control method and this is the currently selected button
        if (input.currentControlScheme.Equals("Gamepad") && EventSystem.current.currentSelectedGameObject == this.gameObject)
        {
            // move the scrollbar to the set value
            scrollbar.value = scrollbarValue;
        }
    }
}
