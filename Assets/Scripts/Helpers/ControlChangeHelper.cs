using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// 
///     Attached to each menu panel
///     Selects a button on the current menu screen if the player switches from Mouse/Keyboard to Controller
/// 
/// </summary>

public class ControlChangeHelper : MonoBehaviour
{
    [SerializeField] Button button; // the button that should be selected on the current menu when the player presses a button on the controller
    [SerializeField] PlayerInput input;// Input System reference

    public void Update()
    {
        // when the player uses a controller while there is no button selected (i.e. the player clicks the background with the mouse)
        if (input.currentControlScheme.Equals("Gamepad") && EventSystem.current.currentSelectedGameObject == null)
        {
            // select the assigned button
            button.Select();
        }
    }
}
