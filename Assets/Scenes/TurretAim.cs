using UnityEngine;
using UnityEngine.InputSystem;

public class TurretAim : MonoBehaviour
{
    [Header("References")]
    public Transform turretPivot;
    public Camera mainCamera;

    [Header("Rotation")]
    public float rotationSpeed = 720f;
    public float angleOffset = 0f;

    [Header("Deadzone / Angle Limit")]
    public bool limitRotation = true;
    public float minAngle = 0f;
    public float maxAngle = 180f;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (turretPivot == null)
            turretPivot = transform;
    }

    private void Update()
    {
        AimAtMouse();
    }

    private void AimAtMouse()
    {
        if (Mouse.current == null || mainCamera == null || turretPivot == null)
            return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        Vector2 direction = mouseWorldPos - turretPivot.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        targetAngle += angleOffset;

        if (limitRotation)
        {
            targetAngle = ClampAngle(targetAngle, minAngle, maxAngle);
        }

        float currentAngle = turretPivot.eulerAngles.z;

        float newAngle = Mathf.MoveTowardsAngle(
            currentAngle,
            targetAngle,
            rotationSpeed * Time.deltaTime
        );

        turretPivot.rotation = Quaternion.Euler(0f, 0f, newAngle);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        angle = NormalizeAngle360(angle);
        min = NormalizeAngle360(min);
        max = NormalizeAngle360(max);

        bool isInsideRange;

        if (min <= max)
        {
            isInsideRange = angle >= min && angle <= max;
        }
        else
        {
            isInsideRange = angle >= min || angle <= max;
        }

        if (isInsideRange)
            return angle;

        float distanceToMin = Mathf.Abs(Mathf.DeltaAngle(angle, min));
        float distanceToMax = Mathf.Abs(Mathf.DeltaAngle(angle, max));

        return distanceToMin < distanceToMax ? min : max;
    }

    private float NormalizeAngle360(float angle)
    {
        angle %= 360f;

        if (angle < 0f)
            angle += 360f;

        return angle;
    }
}