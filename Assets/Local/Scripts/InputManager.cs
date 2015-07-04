using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

	[HideInInspector]
	public enum ClickType{
		None,
		Drag,
		Select,
		Move,
		MoveQuickly,
		Attack,
		Special
	};

	private ClickType curClick;
	private RaycastHit clickInfo;

	// Use this for initialization
	void Start () {
		curClick = ClickType.None;
	}
	
	// Update is called once per frame
	void Update () {
		curClick = ClickType.None;
		if(Input.GetMouseButtonDown(0)){
			curClick = ClickType.Select;
			Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			Physics.Raycast(camRay, out clickInfo);
		}
		if(Input.GetMouseButtonDown(1)){
			if(Input.GetKey(KeyCode.LeftControl)){
				curClick = ClickType.Attack;
				Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

				Physics.Raycast(camRay, out clickInfo);
			}else if(Input.GetKey(KeyCode.LeftShift)){
				curClick = ClickType.MoveQuickly;
				Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

				Physics.Raycast(camRay, out clickInfo);
			}else{
				curClick = ClickType.Move;
				Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

				Physics.Raycast(camRay, out clickInfo);
			}
		}
		if(Input.GetKeyDown(KeyCode.Space)){
			// center on selected character
		}
		if(Input.GetKeyDown(KeyCode.S)){
			curClick = ClickType.Special;
		}
	}

	public ClickType GetCurClick{
		get {
			return curClick;
		}
	}

	public Vector3 GetClickPosn{
		get {
			return clickInfo.point;
		}
	}

	public Transform GetClickTarget{
		get {
			if(clickInfo.transform.tag == "Character"){
				return clickInfo.transform;
			}else{
				return null;
			}
		}
	}

	public float GetTargetDistance{
		get {
			return clickInfo.distance;
		}
	}
}
