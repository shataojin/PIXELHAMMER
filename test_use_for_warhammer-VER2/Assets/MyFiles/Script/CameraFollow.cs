using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // 需要跟随的目标（玩家）

    [Header("Follow Settings")]
    public bool followOn = true;
    public float smoothSpeed = 0.125f; // 跟随的平滑速度
    private Vector3 offset; // 相机相对于玩家的偏移
    private float currentRange = 0;
    public float maxRange = 50;
    private float lastSavePoint = 0;
    public bool CamLocalCheck = false;

    [Header("Background Shift Settings")]
    public Transform sky;
    public Transform far;
    public Transform near;
    public float SkyAdajust;
    public float FarAdajust;
    public float NearAdajust;


    private Vector3 previousTargetPosition;

    void Start()
    {
        previousTargetPosition = target.position;
    }

    private void FixedUpdate()
    {
        if (followOn)
        {
            UpdatePlayerLocation();
            FollowPlayer();
            CamLocalCheck = false;
        }
        else
        {
            BackgroundShift();
            if (!CamLocalCheck)
            {
                CamLocalCheck = true;
            }
        }
        previousTargetPosition = target.position; // Update previous position every frame
    }

    private void FollowPlayer()
    {
        // 目标位置 + 偏移
        Vector3 desiredPosition = new Vector3(target.position.x, transform.position.y, transform.position.z) + offset;
        // 平滑插值从当前位置到目标位置
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        // 设置相机位置
        transform.position = smoothedPosition;
    }

    private void UpdatePlayerLocation()
    {
        currentRange = Mathf.Abs(target.position.x - lastSavePoint);
        if (currentRange > maxRange)
        {
            lastSavePoint = target.position.x;
            followOn = false; // 停止跟随
        }
    }

    // 重新启用跟随功能
    public void EnableFollow()
    {
        followOn = true;
    }

    private void BackgroundShift()
    {
        float deltaX = previousTargetPosition.x- target.position.x ;

        sky.Translate(deltaX * SkyAdajust, 0, 0);
        far.Translate(deltaX * FarAdajust, 0, 0);
        near.Translate(deltaX * NearAdajust, 0, 0);
    }
}
