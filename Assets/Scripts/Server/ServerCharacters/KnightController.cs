using UnityEngine;
using System.Collections;

public class KnightController : BaseCharacter {

	public GameObject shieldEffect;
	public MeshRenderer shieldEffectRenderer;
	public float blockMod = 0;

	// Use this for initialization
	protected override void Start () {
		base.Start();
		_Animator.SetBool("Waiting", true);

		shieldEffectRenderer = shieldEffect.GetComponent<MeshRenderer>();
		shieldEffectRenderer.enabled = false;
	}
	
	protected override void SelectClick(){
		base.SelectClick();
	}

	protected override void MoveClick(){
		base.MoveClick();

		_Animator.ResetTrigger("Attacking");
		_Animator.SetBool("Waiting", false);
		_Animator.SetBool("Blocking", false);
		_Animator.SetBool("Running", false);
	}

	protected override void MoveQuicklyClick(){
		base.MoveQuicklyClick();

		_Animator.ResetTrigger("Attacking");
		_Animator.SetBool("Waiting", false);
		_Animator.SetBool("Blocking", false);
		_Animator.SetBool("Walking", false);
	}

	protected override void AttackClick(){
		base.AttackClick();

		if(attackTarget != null){
			_Animator.SetBool("Walking", false);
			_Animator.SetBool("Running", false);
			_Animator.SetBool("Waiting", false);
			_Animator.SetBool("Blocking", false);
		}
	}

	protected override void SpecialClick(){
		base.SpecialClick();
		shieldEffectRenderer.enabled = specialActive;
	}

	protected override void IdleUpdate(){
		base.IdleUpdate();
		_Animator.SetBool("Blocking", specialActive);
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
		if(specialActive){
			curState = CharacterState.Walking;

			_Animator.SetBool("Running", false);
			WalkUpdate();
		}else{
			base.RunUpdate();
			if(Vector3.Distance(transform.position, moveTarget) <= 0.5){
				_Animator.SetBool("Waiting", true);
				
				_Animator.SetBool("Running", false);
			}else{
				_Animator.SetBool("Running", true);
			}
		}
	}

	public bool jumping = false;
	protected override void AttackUpdate(){
		base.AttackUpdate();
		if(!canAttack){
			if(specialActive){
				_Rigidbody.velocity = (attackTarget.position - transform.position).normalized * WalkSpeed;
				_Animator.SetBool("Walking", true);
			}else{
				_Animator.SetBool("Running", true);
			}
		}else{
			_Animator.SetBool("Walking", false);
			_Animator.SetBool("Running", false);
			if(stopAttacking){
				_Animator.ResetTrigger("Attacking");

				stopAttacking = false;
			}else{
				_Animator.SetTrigger("Attacking");
			}
		}
	}

	public override void ApplyDamage(float damage){
		if(specialActive){
			health -= damage * dmgReduction * blockMod;
		}else{
			health -= damage * dmgReduction;
		}
		if(health <= 0){
			curState = CharacterState.Dead;
		}
	}
}
