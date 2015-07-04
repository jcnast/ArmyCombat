using UnityEngine;
using System.Collections;

public class ArrowBehaviour : MonoBehaviour {

	public float damage = 5;

	public Transform target;

	private Vector3 halfWay;
	private bool atHalf = false;
	public float speed = 0;
	public float maxHeight = 0;

	private Rigidbody _Rigidbody;

	// Use this for initialization
	void Start () {
		_Rigidbody = transform.GetComponent<Rigidbody>();
		halfWay = Vector3.Lerp(transform.position, target.position, 0.5f);
		halfWay = new Vector3(halfWay.x, halfWay.y + maxHeight, halfWay.z);
		transform.LookAt(halfWay);
	}
	
	// Update is called once per frame
	void Update () {
		if(!atHalf){
			if(Vector3.Distance(halfWay, transform.position) >= 0.5){
				transform.LookAt(halfWay);
				_Rigidbody.velocity = (halfWay - transform.position).normalized * speed;
			}else{
				atHalf = true;
			}
		}else{
			transform.LookAt(target.position);
			_Rigidbody.velocity = (target.position - transform.position).normalized * speed;
		}
	}

	void OnTriggerEnter(Collider coll){
		if(coll.transform == target){
			target.GetComponent<BaseCharacter>().ApplyDamage(damage);

			Destroy(gameObject);
		}
	}
}
