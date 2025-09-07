
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class InputManager : MonoBehaviour
{
    [Header("Cache")] 
    [SerializeField]private Vector3 mousePositionWS;
    private bool mousePositionWS_Dirty = true;
    
    [SerializeField]private Ray mouseRay;
    private bool mouseRay_Dirty = true;
    
    [SerializeField]private HexCell mouseRayCell;
    private bool mouseRayCell_Dirty = true;
    
    private Vector3 mousePos=new Vector3(-1,-1,-1);
    
    [Header("Gizmos")] 
    [SerializeField] private bool drawGizmos = false;
    [SerializeField] private float len = 3f;

    
    
    private InputStateMachine inputStateMachine;
    
    private InputPlayState inputPlayState;
    private InputEditorState inputEditorState;
    
    
    public static InputManager instance;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        inputStateMachine = new InputStateMachine();

        inputPlayState = GetComponent<InputPlayState>();
        inputEditorState = GetComponent<InputEditorState>();
        
        
        inputStateMachine.Initialize(inputPlayState);
    }

    // Update is called once per frame
    public void OnUpdate()
    {
        if (mousePos != Input.mousePosition)
        {
            mousePos = Input.mousePosition;
            mouseRay_Dirty = true;
            mousePositionWS_Dirty = true;
            mouseRayCell_Dirty = true;
        }
    

        if (Input.GetKeyDown(KeyCode.C))
        {
            inputStateMachine.ChangeInputState(inputEditorState);
        }
        
        inputStateMachine.Update();
    }

    #region Get Date

    public Ray GetMouseRay()
    {
        if (mouseRay_Dirty)
        {
            mouseRay = CameraManager.instance.GetMainCamera().ScreenPointToRay(Input.mousePosition);
            mouseRay_Dirty = false;
        }
        return mouseRay;
    }

    public Vector3 GetMousePositionWS()
    {
        if (mousePositionWS_Dirty)
        {
            var camera = CameraManager.instance.GetMainCamera();
            mousePos.z = camera.nearClipPlane;
            mousePositionWS = camera.ScreenToWorldPoint(mousePos);
            
            mousePositionWS_Dirty = false;
        }
        return mousePositionWS;
    }

    public Vector3 GetMousePosition()
    {
        return Input.mousePosition;
    }

    public HexCell GetMouseRayCell()
    {
        if (mouseRayCell_Dirty)
        {
            mouseRayCell = HexCellMapManager.instance.root.GetMinDistanceHexCellByRay(GetMouseRay());
            mouseRayCell_Dirty = false;
        }
        return mouseRayCell;
    }

    #endregion

   
    
   


    #region Gizmos

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(GetMouseRay().origin, GetMouseRay().direction * len);
        
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(mousePositionWS, 0.2f);
    }

    #endregion

    
}
