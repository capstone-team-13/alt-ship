using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraInput : MonoBehaviour, AxisState.IInputAxisProvider
{
    [HideInInspector] public InputAction horizontal;
    [HideInInspector] public InputAction vertical;

    private static CameraInput _instance;
    public static CameraInput Instance => _instance;

    private Vector2 inputValues;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        horizontal.Enable();
        vertical.Enable();
    }

    private void OnDisable()
    {
        horizontal.Disable();
        vertical.Disable();
    }

    private void FixedUpdate()
    {
        // Read input values in FixedUpdate for consistency
        inputValues = horizontal.ReadValue<Vector2>();
    }

    public float GetAxisValue(int axis)
    {
        return axis switch
        {
            0 => inputValues.x, // Horizontal Input
            1 => inputValues.y, // Vertical Input
            2 => vertical.ReadValue<float>(), // Scroll / Other
            _ => 0
        };
    }
}