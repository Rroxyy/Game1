using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class InputEditorState : InputState
{
    [FormerlySerializedAs("changeRadius")]
    [Header("Base")] 
    [SerializeField, Range(0, 10)] private int radius = 1;
    
    [Header("Color Mode")]
    [SerializeField]private HexCellColorEnum hexCellColorEnum = HexCellColorEnum.White;
    
    [Header("Height Mode")]
    [SerializeField]private uint height = 2;
    
    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
        if (Input.GetKey(KeyCode.Mouse0))
        {
            var cell = InputManager.instance.GetMouseRayCell();
            if (cell == null) return;
            List<HexCell> neighbors = new List<HexCell>();
            // List<HexCellDirection> directions = new List<HexCellDirection>();
            
            neighbors.Add(cell);
            HexCellMapManager.instance.GetCellNeighbors(cell, ref neighbors,(uint)radius);

            foreach (var it in neighbors)
            {
                ChangeHexCellColor(it);
                ChangeHexCellHeight(it);
            }
        }

        float dt = Time.deltaTime;

        // 前后
        if (Input.GetKey(KeyCode.W))
            CameraManager.instance.cameraController.MoveForward(dt);
        if (Input.GetKey(KeyCode.S))
            CameraManager.instance.cameraController.MoveBackward(dt);

        // 左右
        if (Input.GetKey(KeyCode.A))
            CameraManager.instance.cameraController.MoveLeft(dt);
        if (Input.GetKey(KeyCode.D))
            CameraManager.instance.cameraController.MoveRight(dt);

        // 上下
        if (Input.GetKey(KeyCode.E))
            CameraManager.instance.cameraController.MoveUp(dt);
        if (Input.GetKey(KeyCode.Q))
            CameraManager.instance.cameraController.MoveDown(dt);
        
        if (Input.GetMouseButtonDown(1)) // 刚按下右键
        {
            Cursor.lockState = CursorLockMode.Locked; // 锁定到屏幕中心
            Cursor.visible = false;                   // 隐藏鼠标
        }

        if (Input.GetMouseButtonUp(1)) // 松开右键
        {
            Cursor.lockState = CursorLockMode.None;   // 解锁
            Cursor.visible = true;                    // 显示鼠标
        }
        
        // 旋转（右键按住时，根据鼠标移动量调整相机角度）
        if (Input.GetMouseButton(1)) // 右键
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            CameraManager.instance.cameraController.RotateCamera(mouseX, mouseY);
        }
        
    }

    private void ChangeHexCellHeight(HexCell cell)
    {
        if (cell == null) return;
        cell.SetHeight(height);
    }

    private void ChangeHexCellColor(HexCell cell)
    {
        if (cell == null) return;
        cell.SetColor(HexCellColorManager.instance.GetColor(hexCellColorEnum));
    }
}