using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private Camera camera;

    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Rotate Settings")]
    [SerializeField] private float rotateSpeed = 2f;

    private float yaw;   // 绕Y轴（水平旋转）
    private float pitch; // 绕X轴（俯仰）

    // --------- 移动方法 ---------
    public void MoveForward(float deltaTime)  => transform.position += GetForward() * moveSpeed * deltaTime;
    public void MoveBackward(float deltaTime) => transform.position -= GetForward() * moveSpeed * deltaTime;
    public void MoveRight(float deltaTime)    => transform.position += GetRight() * moveSpeed * deltaTime;
    public void MoveLeft(float deltaTime)     => transform.position -= GetRight() * moveSpeed * deltaTime;
    public void MoveUp(float deltaTime)       => transform.position += Vector3.up * moveSpeed * deltaTime;
    public void MoveDown(float deltaTime)     => transform.position -= Vector3.up * moveSpeed * deltaTime;

    // --------- 镜头朝向方法 ---------
    /// <summary>
    /// 根据鼠标移动旋转相机
    /// mouseDeltaX: 鼠标水平移动
    /// mouseDeltaY: 鼠标垂直移动
    /// </summary>
    public void RotateCamera(float mouseDeltaX, float mouseDeltaY)
    {
        yaw   += mouseDeltaX * rotateSpeed;
        pitch -= mouseDeltaY * rotateSpeed; // 鼠标上移 -> 视角下移

        // 限制俯仰角，避免翻转
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        // 应用旋转（始终保持 up = Vector3.up）
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    // --------- 辅助方法 ---------
    private Vector3 GetForward()
    {
        Vector3 forward = transform.forward;
        forward.y = 0; // 保持水平前进
        return forward.normalized;
    }

    private Vector3 GetRight()
    {
        Vector3 right = transform.right;
        right.y = 0;
        return right.normalized;
    }
}