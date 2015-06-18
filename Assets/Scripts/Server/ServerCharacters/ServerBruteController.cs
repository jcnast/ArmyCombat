using UnityEngine;
using System.Collections;

public class ServerBruteController : BaseCharacter {

	public bool jumpAttack = false;
	public float jumpAttackRange = 0;

	// Use this for initialization
	protected override void Start () {
		base.Start();
		_Animator.SetBool("Waiting", true);
	}
	
	protected override void SelectClick(){
		base.SelectClick();
	}

	protected override void MoveClick(){
		base.MoveClick();

		_Animator.ResetTrigger("Attacking");
		_Animator.ResetTrigger("JumpAttack");
		_Animator.SetBool("Waiting", false);
		_Animator.SetBool("Running", false);
	}

	protected override void MoveQuicklyClick(){
		base.MoveQuicklyClick();

		_Animator.ResetTrigger("Attacking");
		_Animator.ResetTrigger("JumpAttack");
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
		jumpAttack = specialActive;
	}

	protected override void IdleUpdate(){
		base.IdleUpdate();
		if(!selected && waitStart + LongWaitTime <= Time.time){
			// shield interaction?
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

	protected override void AttackUpdate(){
		base.AttackUpdate();
		if(!canAttack){
			if(jumpAttack && Vector3.Distance(transform.position, attackTarget.position) <= jumpAttackRange){
				_Animator.SetBool("Running", false);
				_Animator.SetTrigger("JumpAttack");
				specialActive = false;
				jumpAttack = false;
			}else{
				_Animator.SetBool("Running", true);
			}
		}else{
			_Animator.SetBool("Running", false);
			if(stopAttacking){
				_Animator.ResetTrigger("Attacking");
				_Animator.ResetTrigger("JumpAttack");

				stopAttacking = false;
			}else{
				_Animator.SetTrigger("Attacking");
			}
		}
	}
}
