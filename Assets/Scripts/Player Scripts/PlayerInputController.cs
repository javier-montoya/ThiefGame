﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof (PlatformerCharacter2D))]
public class PlayerInputController : MonoBehaviour
{
	private PlatformerCharacter2D m_Character;
	private bool m_Jump;
	private bool afterJumpPress;
	private bool m_Dash;

	private bool dashEnabled = true;
	private bool pushDashFlag = true;
	float dashTimer = 0;

	Animator armAnimator;
	float inputAxis;

	private void Awake()
	{
		m_Character = GetComponent<PlatformerCharacter2D>();
	}


	private void LateUpdate()
	{
		armAnimator = transform.Find ("Arm").GetComponent<Animator>();

		InterpreteKeys ();
		SetDashingValue ();
		SetAttackAnimation ();

		inputAxis = Input.GetAxisRaw ("Horizontal");

		m_Character.Move(inputAxis, m_Dash);
		m_Character.JumpBehavior(m_Jump, afterJumpPress);
		m_Jump = false;
	}

	private void InterpreteKeys () {

		if (!m_Jump)
			m_Jump = Input.GetKeyDown (KeyCode.Z) ? true : false;

		afterJumpPress = Input.GetKey (KeyCode.Z) ? true : false;

		if(Input.GetKeyDown(KeyCode.R))
			SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex ) ;

		if (m_Character.animator.GetBool ("InGround") && Input.GetAxisRaw ("Horizontal") != 0)
			m_Character.animator.SetBool ("Run", true);
		else
			m_Character.animator.SetBool ("Run", false);
	}

	float dashCooldown = 0.05f;
	private void SetDashingValue(){

		if (!pushDashFlag && dashEnabled && Input.GetKeyDown (KeyCode.C)) {
			pushDashFlag = true;		
		}

		if (pushDashFlag && Input.GetKey (KeyCode.C)) {

			if(	!m_Character.m_Grounded ||
				// (m_Character.m_FacingRight && Input.GetAxisRaw ("Horizontal") == -1) ||
				// (!m_Character.m_FacingRight && Input.GetAxisRaw ("Horizontal") == 1) ||
				(dashTimer >= 0.3 )	){

				StopDashing (dashCooldown);
				return;
			}

			m_Character.animator.SetBool ("Dash", true);
			m_Dash = true;
			dashTimer += Time.deltaTime;

		} else if(pushDashFlag && Input.GetKeyUp (KeyCode.C)){

			StopDashing (dashCooldown);
			
		} else {
			
			m_Character.animator.SetBool ("Dash", false);
			m_Dash = false;
			dashTimer = 0;
		}
	}

	void SetAttackAnimation(){

		if (Input.GetKeyDown (KeyCode.X) && 
			!m_Character.animator.GetCurrentAnimatorStateInfo (0).IsName ("Damage") &&
			!m_Character.animator.GetCurrentAnimatorStateInfo (0).IsName ("Death")) {

			armAnimator.SetTrigger ("Swing");
		}

		if (m_Character.animator.GetCurrentAnimatorStateInfo (0).IsName ("Damage") &&
			m_Character.animator.GetCurrentAnimatorStateInfo (0).IsName ("Death")) {

			armAnimator.CrossFade ("Invisible", 0);
			armAnimator.ResetTrigger ("Swing");
		}

		if (armAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Swing")) {
			m_Character.animator.SetLayerWeight (1, 1);
		} else {
			m_Character.animator.SetLayerWeight (1, 0);
		}
	}

	void StopDashing(float cooldown){
		dashTimer = 0;
		m_Character.animator.SetBool ("Dash", false);
		m_Dash = false;
		dashEnabled = false;
		pushDashFlag = false;
		Invoke ("ToggleDashEnabled", cooldown);
	}

	void ToggleDashEnabled(){
		dashEnabled = !dashEnabled;
	}
		
}