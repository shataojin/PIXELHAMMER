using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseLessCode : MonoBehaviour
{
    [Header("Vertical Movement Settings")]
    [SerializeField] private float verticalSpeed = 2.0f;
    [SerializeField] private float verticalAmplitude = 1.0f;

    [Header("Circular XZ Movement Settings")]
    [SerializeField] private float circleRadius = 3.0f;
    [SerializeField] private float circleSpeed = 1.0f;

    private Vector3 initialPosition;
    private float verticalTimer;
    private float circleTimer;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        MoveVertically();
        MoveInCircleXZ();
    }

    private void MoveVertically()
    {
        verticalTimer += Time.deltaTime * verticalSpeed;
        float newY = initialPosition.y + Mathf.Sin(verticalTimer) * verticalAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void MoveInCircleXZ()
    {
        circleTimer += Time.deltaTime * circleSpeed;
        float x = initialPosition.x + Mathf.Cos(circleTimer) * circleRadius;
        float z = initialPosition.z + Mathf.Sin(circleTimer) * circleRadius;
        transform.position = new Vector3(x, transform.position.y, z);
    }
}
