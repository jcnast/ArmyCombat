using UnityEngine;
using System.Collections;

public class ServerBaseCharacter : MonoBehaviour {

	[HideInInspector]
	public enum CharacterState{
		Idle,
		Walking,
		Running,
		Attacking,
		Dead,
	};

	public InputManager input;

	public float health = 100;
	public float dmgReduction = 0;

	public float damage = 5;
	public float range = 3;
	protected Transform attackTarget;

	public float WalkSpeed = 0;
	public float RunSpeed = 0;
	public float LongWaitTime = 0;

	protected bool selected = false;
	protected bool specialActive = false;
	public CharacterState curState;

	protected Vector3 moveTarget;

	protected Collider _Collider;
	protected Rigidbody _Rigidbody;
	protected Animator _Animator;

	// Use this for initialization
	protected virtual void Start () {
		_Collider = transform.GetComponent<Collider>();
		_Rigidbody = transform.GetComponent<Rigidbody>();
		_Animator = transform.GetComponent<Animator>();

		curState = CharacterState.Idle;
	}
	
	// Update is called once per frame
	void Update () {
		if(input.GetCurClick != InputManager.ClickType.None){
			if(input.GetCurClick == InputManager.ClickType.Select){
				SelectClick();
			}else if(selected && input.GetCurClick == InputManager.ClickType.Move){
				MoveClick();
			}else if(selected && input.GetCurClick == InputManager.ClickType.MoveQuickly){
				MoveQuicklyClick();
			}else if(selected && input.GetCurClick == InputManager.ClickType.Attack){
				AttackClick();
			}else if(selected && input.GetCurClick == InputManager.ClickType.Special){
				SpecialClick();
			}
		}

		switch (curState){
			case CharacterState.Idle:
				IdleUpdate();
				break;
			case CharacterState.Walking:
				WalkUpdate();
				break;
			case CharacterState.Running:
				RunUpdate();
				break;
			case CharacterState.Attacking:
				AttackUpdate();
				break;
			case CharacterState.Dead:
				Destroy(gameObject);
				break;
		}
	}

	protected virtual void SelectClick(){
		if(_Collider.bounds.Contains(input.GetClickPosn)){
			selected = true;
		}else{
			selected = false;
		}
	}

	protected virtual void MoveClick(){
		curState = CharacterState.Walking;
		moveTarget = input.GetClickPosn;
		transform.LookAt(moveTarget);
	}

	protected virtual void MoveQuicklyClick(){
		curState = CharacterState.Running;
		moveTarget = input.GetClickPosn;
		transform.LookAt(moveTarget);
	}

	protected virtual void AttackClick(){
		attackTarget = input.GetClickTarget;
		if(attackTarget != null){
			curState = CharacterState.Attacking;
			transform.LookAt(attackTarget);
		}
	}

	protected virtual void SpecialClick(){
		specialActive = !(specialActive);
	}

	protected float waitStart = 0;
	protected virtual void IdleUpdate(){
	
	}

	protected virtual void WalkUpdate(){
		if(Vector3.Distance(transform.position, moveTarget) <= 0.5){
			moveTarget = transform.position;
			curState = CharacterState.Idle;
			waitStart = Time.time;

			_Rigidbody.velocity = Vector3.zero;
		}else{
			_Rigidbody.velocity = (moveTarget - transform.position).normalized * WalkSpeed;
		}
	}

	protected virtual void RunUpdate(){
		if(Vector3.Distance(transform.position, moveTarget) <= 0.5){
			moveTarget = transform.position;
			curState = CharacterState.Idle;
			waitStart = Time.time;

			_Rigidbody.velocity = Vector3.zero;
		}else{
			_Rigidbody.velocity = (moveTarget - transform.position).normalized * RunSpeed;
		}
	}

	protected bool stopAttacking = false;
	protected bool canAttack = false;
	protected virtual void AttackUpdate(){
		if(attackTarget == null){
			curState = CharacterState.Idle;
			stopAttacking = true;
		}
		if(Vector3.Distance(transform.position, attackTarget.position) >= range){
			canAttack = false;
		}else{
			canAttack = true;
		}
		if(!canAttack){
			_Rigidbody.velocity = (attackTarget.position - transform.position).normalized * RunSpeed;
		}else{
			_Rigidbody.velocity = Vector3.zero;
		}
	}

	public virtual void ApplyDamage(float damage){
		health -= damage * dmgReduction;
		if(health <= 0){
			curState = CharacterState.Dead;
		}
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info){
		Vector3 syncPosition = Vector3.zero;
		int syncState = 0;
		bool syncSpecial = false;
		if (stream.isWriting)
		{
			syncPosition = transform.position;
			stream.Serialize(ref syncPosition);

			syncState = StateToInt(curState);
			stream.Serialize(ref syncState);

			syncSpecial = specialActive;
			stream.Serialize(ref syncSpecial);
		}
		else
		{
			stream.Serialize(ref syncPosition);
			transform.position = syncPosition;

			stream.Serialize(ref syncState);
			curState = IntToState(syncState);

			stream.Serialize(ref syncSpecial);
			specialActive = syncSpecial;
		}
	}

	int StateToInt(CharacterState state){
		if(state == CharacterState.Idle){
			return 0;
		}else if(state == CharacterState.Walking){
			return 1;
		}else if(state == CharacterState.Running){
			return 2;
		}else if(state == CharacterState.Attacking){
			return 3;
		}else if(state == CharacterState.Dead){
			return 4;
		}else{
			return -1;
		}
	}

	CharacterState IntToState(int sint){
		if(sint == 0){
			return CharacterState.Idle;
		}else if(sint == 1){
			return CharacterState.Walking;
		}else if(sint == 2){
			return CharacterState.Running;
		}else if(sint == 3){
			return CharacterState.Attacking;
		}else if(sint == 4){
			return CharacterState.Dead;
		}else{
			return CharacterState.Idle;
		}
	}
}
