using UnityEngine;
using System.Collections;

public class ServerArcherController : BaseCharacter {

	public float attackMod;

	public Transform Bow;
	public GameObject Arrow;
	public Transform ReleasePoint;
	private SkinnedMeshRenderer bowMesh;

	// Use this for initialization
	protected override void Start () {
		bowMesh = Bow.GetComponent<SkinnedMeshRenderer>();
		base.Start();
		_Animator.SetBool("Waiting", true);
		bowMesh.enabled = false;
	}
	
	protected override void SelectClick(){
		base.SelectClick();
		if(selected){
			_Animator.SetBool("Selected", true);
			_Animator.ResetTrigger("LongWait");
			if(!bowMesh.enabled){
				bowMesh.enabled = true;
			}
		}else{
			_Animator.SetBool("Selected", false);
		}
	}

	protected override void MoveClick(){
		base.MoveClick();

		_Animator.ResetTrigger("Drawing");
		//_Animator.ResetTrigger("Aiming");
		//_Animator.ResetTrigger("Recovering");
		_Animator.SetBool("Waiting", false);
		_Animator.SetBool("Running", false);
	}

	protected override void MoveQuicklyClick(){
		base.MoveQuicklyClick();

		_Animator.ResetTrigger("Drawing");
		//_Animator.ResetTrigger("Aiming");
		//_Animator.ResetTrigger("Recovering");
		_Animator.SetBool("Waiting", false);
		_Animator.SetBool("Walking", false);
	}

	protected override void AttackClick(){
		base.AttackClick();

		if(attackTarget != null){
			_Animator.SetBool("Walking", false);
			_Animator.SetBool("Running", false);
			_Animator.SetBool("Waiting", false);
		}
	}

	protected override void SpecialClick(){
		base.SpecialClick();
		_Animator.SetBool("Special", specialActive);
	}

	protected override void IdleUpdate(){
		base.IdleUpdate();
		if(!selected && waitStart + LongWaitTime <= Time.time){
			_Animator.SetTrigger("LongWait");
			bowMesh.enabled = false;
		}
	}

	protected override void WalkUpdate(){
		base.WalkUpdate();
		if(Vector3.Distance(transform.position, moveTarget) <= 0.5){
			_Animator.SetBool("Waiting", true);

			_Animator.SetBool("Walking", false);
		}else{
			_Animator.SetBool("Walking", true);
		}
	}

	protected override void RunUpdate(){
		base.RunUpdate();
		if(Vector3.Distance(transform.position, moveTarget) <= 0.5){
			_Animator.SetBool("Waiting", true);
			
			_Animator.SetBool("Running", false);
		}else{
			_Animator.SetBool("Running", true);
		}
	}

	public bool fire = false;
	public bool firing = false;
	private int fireCount = 1;
	protected override void AttackUpdate(){
		base.AttackUpdate();
		if(stopAttacking){
			_Animator.ResetTrigger("Drawing");
			//_Animator.ResetTrigger("Aiming");
			//_Animator.ResetTrigger("Recovering");

			stopAttacking = false;
		}else if(!canAttack){
			_Animator.SetBool("Running", true);
		}else{
			_Animator.SetBool("Running", false);
			if(!firing){
				fireCount = 1;
			}
			if(fire && fireCount != 0){
				GameObject arrow = (GameObject) Instantiate(Arrow, ReleasePoint.position, transform.rotation);
				ArrowBehaviour arrowInfo = arrow.GetComponent<ArrowBehaviour>();
				arrowInfo.target = attackTarget;
				if(specialActive){
					arrowInfo.damage = damage * attackMod;
				}else{
					arrowInfo.damage = damage;
				}
				fireCount--;
			}
			if(specialActive){
				_Animator.SetTrigger("Drawing");
				//_Animator.SetTrigger("Aiming");
				//_Animator.SetTrigger("Recovering");
			}else{
				_Animator.SetTrigger("Drawing");
				//_Animator.SetTrigger("Recovering");
			}
		}
	}
}