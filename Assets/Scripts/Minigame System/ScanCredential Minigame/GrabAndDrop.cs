using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Allows a RectTransform UI element to be dragged across the screen,
/// but only if it is the top-most element under the pointer.
/// Works with mouse (PC) and touch (mobile) using the new Input System.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class GrabAndDrop : MonoBehaviour
{
    [Header("Input Action Asset")]
    [Tooltip("Input Action Asset that contains the pointer position and press actions.")]
    [SerializeField] private InputActionAsset inputActions;

    [Header("Action Map and Actions")]
    [Tooltip("The name of the Action Map to use (e.g. 'UI').")]
    [SerializeField] private string actionMapName = "UI";

    [Tooltip("The name of the action used for pointer position (Vector2).")]
    [SerializeField] private string pointerPositionAction = "Point";

    [Tooltip("The name of the action used for pointer press (Button).")]
    [SerializeField] private string pointerPressAction = "Click";

    private RectTransform rectTransform;
    private Canvas canvas;
    private InputAction positionAction;
    private InputAction pressAction;

    private bool isDragging;
    private Vector2 offset;

    private GraphicRaycaster raycaster;
    private EventSystem eventSystem;

    /// <summary>
    /// Setup references and bind input actions.
    /// </summary>
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        raycaster = canvas.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;

        var map = inputActions.FindActionMap(actionMapName, true);
        positionAction = map.FindAction(pointerPositionAction, true);
        pressAction = map.FindAction(pointerPressAction, true);

        pressAction.performed += OnPress;
        pressAction.canceled += OnRelease;
    }

    private void OnEnable()
    {
        positionAction.Enable();
        pressAction.Enable();
    }

    private void OnDisable()
    {
        positionAction.Disable();
        pressAction.Disable();
    }

    /// <summary>
    /// Handles the press (start of drag).
    /// Checks if this object is the top-most under the pointer.
    /// </summary>
    private void OnPress(InputAction.CallbackContext context)
    {
        Vector2 screenPos = positionAction.ReadValue<Vector2>();

        if (!IsTopMostAtPosition(screenPos))
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            canvas.worldCamera,
            out Vector2 localPoint);

        offset = (Vector2)rectTransform.localPosition - localPoint;
        isDragging = true;
    }

    /// <summary>
    /// Handles the release (end of drag).
    /// </summary>
    private void OnRelease(InputAction.CallbackContext context)
    {
        isDragging = false;
    }

    /// <summary>
    /// Update is called once per frame. Moves the RectTransform while dragging.
    /// </summary>
    private void Update()
    {
        if (!isDragging) return;

        Vector2 screenPos = positionAction.ReadValue<Vector2>();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            canvas.worldCamera,
            out Vector2 localPoint))
        {
            rectTransform.localPosition = localPoint + offset;
        }
    }

    /// <summary>
    /// Checks if this object is the top-most UI element under the pointer.
    /// </summary>
    private bool IsTopMostAtPosition(Vector2 screenPos)
    {
        if (raycaster == null || eventSystem == null) return false;

        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = screenPos
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        if (results.Count == 0) return false;

        // Top-most object is the first in results
        return results[0].gameObject == gameObject;
    }
}
