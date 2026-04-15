using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class UIInputSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject firstSelected;
    [SerializeField] private InputActionReference navigateAction;

    private Vector2 lastMousePosition;

    void Start()
    {
        if (Mouse.current != null)
            lastMousePosition = Mouse.current.position.ReadValue();
        navigateAction.action.Enable();
    }

    void Update()
    {
        HandleMouse();
        HandleNavigation();
    }

    void HandleMouse()
    {
        if (Mouse.current == null) return;

        Vector2 currentMousePosition = Mouse.current.position.ReadValue();

        if (currentMousePosition != lastMousePosition)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        lastMousePosition = currentMousePosition;
    }

    void HandleNavigation()
    {
        if (navigateAction == null) return;

        Vector2 nav = navigateAction.action.ReadValue<Vector2>();
        
        if (nav.magnitude > 0.2f)
        {
            ActivateUISelection();
        }
    }

    void ActivateUISelection()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }
}