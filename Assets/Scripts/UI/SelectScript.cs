using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectScript : MonoBehaviour {

	[HideInInspector]
	public enum SelectClickType{
		None,
		Select
	};

	public LayerMask layerToScan;

	private SelectClickType curClick;
	private RaycastHit clickInfo;
	private Vector3 initPosn;
	private Vector3 dragPosn;

	private List<BaseCharacter> selectedCharacters = new List<BaseCharacter>();

	// Use this for initialization
	void Start () {
		curClick = SelectClickType.None;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)){
			curClick = SelectClickType.Select;
			Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			Physics.Raycast(camRay, out clickInfo, layerToScan);

			initPosn = clickInfo.point + Vector3.up;
			transform.position = clickInfo.point + Vector3.up;
		}else if(Input.GetMouseButtonUp(0)){
			curClick = SelectClickType.None;

			for(int i = 0; i < selectedCharacters.Count; i++){
				selectedCharacters[i].SetSelected();
			}
		}

		switch (curClick){
			case SelectClickType.None:
				break;
			case SelectClickType.Select:
				SelectUpdate();
				break;
		}
	}

	void SelectUpdate(){
		if(Input.GetMouseButton(0)){
			Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			Physics.Raycast(camRay, out clickInfo);

			dragPosn = clickInfo.point;
		}

		transform.localScale = new Vector3(Mathf.Abs(initPosn.x - dragPosn.x)/10, 1, Mathf.Abs(initPosn.z - dragPosn.z)/10);
		transform.position = new Vector3((initPosn.x + dragPosn.x)/2, 0, (initPosn.z + dragPosn.z)/2); 
	}

	void OnTriggerEnter(Collider col){
		BaseCharacter newChar = col.transform.GetComponent<BaseCharacter>();
		if(newChar != null && !selectedCharacters.Contains(newChar)){
			selectedCharacters.Add(newChar);
		}
	}

	void OnTriggerExit(Collider col){
		BaseCharacter newChar = col.transform.GetComponent<BaseCharacter>();
		if(newChar != null && selectedCharacters.Contains(newChar)){
			selectedCharacters.Remove(newChar);
		}
	}
}
