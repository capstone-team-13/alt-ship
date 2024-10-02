using System;
using UnityEngine;

namespace EE.Prototype.CC
{
    public class TwoJointsIKSolver : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Transform m_joint1 = null; // Joint 1 transform

        [SerializeField] private Transform m_joint2 = null; // Joint 2 transform
        [SerializeField] private double m_l1 = 1.0; // Length of the first segment
        [SerializeField] private double m_l2 = 1.0; // Length of the second segment
        [SerializeField] private Transform m_followingPoint = null; // Target point for IK to reach

        // Output angles for each joint
        private double m_theta1;
        private double m_theta2;

        private void Start()
        {
            double[] initialGuess = { 0.0, 0.0 };
            var result = InverseKinematics(initialGuess);
            Debug.Log($"IK Result: dx = {result[0]}, dy = {result[1]}");
        }

        // This method calculates the end-effector position based on the given joint angles (theta1, theta2)
        private double[] CalculateEndEffectorPosition(double[] theta)
        {
            var theta1 = theta[0];
            var theta2 = theta[1];

            // Calculate the end-effector's x and y position based on forward kinematics
            var x = m_l1 * Math.Cos(theta1) + m_l2 * Math.Cos(theta1 + theta2);
            var y = m_l1 * Math.Sin(theta1) + m_l2 * Math.Sin(theta1 + theta2);

            return new[] { x, y };
        }

        // This method defines the inverse kinematics function
        // It returns the difference between the current end-effector position and the target position (m_followingPoint)
        private double[] InverseKinematics(double[] theta)
        {
            // Calculate the current end-effector position based on the joint angles
            var currentPosition = CalculateEndEffectorPosition(theta);

            // Get the target position directly from m_followingPoint
            double targetX = m_followingPoint.position.x;
            double targetY = m_followingPoint.position.y;

            // Return the differences between the current position and the target
            return new[]
            {
                currentPosition[0] - targetX, // Difference in X
                currentPosition[1] - targetY // Difference in Y
            };
        }
    }
}