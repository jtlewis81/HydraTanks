using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControlChangeHelper : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] PlayerInput input;

    public void Update()
    {
        if (input.currentControlScheme.Equals("Gamepad") && EventSystem.current.currentSelectedGameObject == null)
        {
            button.Select();
        }
    }
}
