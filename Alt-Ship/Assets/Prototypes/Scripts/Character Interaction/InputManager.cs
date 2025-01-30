using UnityEngine;

namespace Boopoo.Utilities
{
    public static class InputManager
    {
        private const string HORIZONTAL_AXIS = "Horizontal";
        private const string VERTICAL_AXIS = "Vertical";

        public static float GetHorizontalInput()
        {
            return Input.GetAxis(HORIZONTAL_AXIS);
        }

        public static float GetVerticalInput()
        {
            return Input.GetAxis(VERTICAL_AXIS);
        }

        public static float GetHorizontalInputRaw()
        {
            return Input.GetAxisRaw(HORIZONTAL_AXIS);
        }

        public static float GetVerticalInputRaw()
        {
            return Input.GetAxisRaw(VERTICAL_AXIS);
        }

        public static Vector3 GetDirectionalInput()
        {
            float horizontal = Input.GetAxis(HORIZONTAL_AXIS);
            float vertical = Input.GetAxis(VERTICAL_AXIS);
            return new Vector3(horizontal, 0, vertical);
        }

        public static Vector3 CalculateMoveDirection(Transform cameraTransform)
        {
            float horizontalInput = GetHorizontalInputRaw();
            float verticalInput = GetVerticalInputRaw();

            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0;
            right.y = 0;

            forward = forward.normalized;
            right = right.normalized;

            Vector3 relativeRightInput = horizontalInput * right;
            Vector3 relativeForwardInput = verticalInput * forward;

            return relativeForwardInput + relativeRightInput;
        }
    }
}