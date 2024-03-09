using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectLevelHelper : MonoBehaviour
{
    [SerializeField] Scrollbar scrollbar;
    [SerializeField] float scrollbarValue;

    private void Update()
    {
        if(EventSystem.current.currentSelectedGameObject == this.gameObject)
        {
            scrollbar.value = scrollbarValue;
        }
    }
}
