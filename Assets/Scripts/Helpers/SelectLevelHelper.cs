using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Windows;

public class SelectLevelHelper : MonoBehaviour
{
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private float scrollbarValue;

    [SerializeField] private PlayerInput input;

    private void Update()
    {
        if (input.currentControlScheme.Equals("Gamepad") && EventSystem.current.currentSelectedGameObject == this.gameObject)
        {
            scrollbar.value = scrollbarValue;
        }
    }
}
