using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInputActions inputActions;
    public CharacterController characterController;
    public Animator animator;

    private bool inputBlocker = false;
    private float lastInputStrike;
    private Coroutine attackManage;

    private Vector2 stanceVec;
    private Vector2 animVec;
    private enum Stance
    {
        ForwardGuard,
        HighGuardR,
        HighGuardL,
        RearGuardR,
        RearGuardL,
        StandingGuardR,
        StandingGuardL,
        LowGuard
    }

    private Stance currentStance; 

    private void Awake()
    {
        
        inputActions = new PlayerInputActions();
        inputActions.Player._1Hit.performed += context => inputToManager(1);
        inputActions.Player._2Hit.performed += context => inputToManager(2);
        inputActions.Player._3Hit.performed += context => inputToManager(3);
        inputActions.Player._4Hit.performed += context => inputToManager(4);
        inputActions.Player._5Hit.performed += context => inputToManager(5);
        inputActions.Player._6Hit.performed += context => inputToManager(6);
        inputActions.Player._7Hit.performed += context => inputToManager(7);
        inputActions.Player._8Hit.performed += context => inputToManager(8);

        inputActions.Player._1Parry.performed += context => ParryManager(Stance.HighGuardR);
        inputActions.Player._2Parry.performed += context => ParryManager(Stance.StandingGuardR);
        inputActions.Player._3Parry.performed += context => ParryManager(Stance.LowGuard);
        inputActions.Player._4Parry.performed += context => ParryManager(Stance.LowGuard);
        inputActions.Player._5Parry.performed += context => ParryManager(Stance.LowGuard);
        inputActions.Player._6Parry.performed += context => ParryManager(Stance.StandingGuardL);
        inputActions.Player._7Parry.performed += context => ParryManager(Stance.HighGuardL);
        inputActions.Player._8Parry.performed += context => ParryManager(Stance.HighGuardR);
        inputActions.Player._8Parry.canceled += context => exitStance();

        currentStance = Stance.ForwardGuard;
    }
    private void OnEnable() { inputActions.Enable(); }
    private void OnDisable() { inputActions.Disable(); }

    private void FixedUpdate()
    {
        stanceVec = StanceCoordLookup(currentStance);
        if(stanceVec != animVec)
        {
            animator.SetFloat("BlendX", stanceVec.x);
            animator.SetFloat("BlendY", stanceVec.y);
        }
        Vector2 vector = inputActions.Player.Movement.ReadValue<Vector2>();
        characterController.Move(8 * new Vector3(vector.x * Time.deltaTime,0,vector.y * Time.deltaTime));
    }


    void inputToManager(float ctx)
    {
        lastInputStrike = ctx; 
        if(!inputBlocker )
        {
            inputBlocker = !inputBlocker;
            attackManage = StartCoroutine(InputManager(ctx));
        }
    }
    private void ParryManager(Stance stance)
    {   
        StopCoroutine(attackManage);
        currentStance = stance;
        inputBlocker = !inputBlocker;
        print(currentStance);
    }

    private void exitStance()
    {
        currentStance = Stance.ForwardGuard;
        print(currentStance);
    }

    private IEnumerator InputManager(float ctx)
    {
        yield return new WaitForSeconds(0.05f); //adds small buffer for simultaneous inputs
        float strikeN = lastInputStrike; //lock in strike
        currentStance = changeMyAStance(strikeN);
        yield return new WaitForSeconds(0.2f);
        print(currentStance);
        yield return new WaitForSeconds(0.25f);
        //  print("Attack Complete...   " + strikeN);
        print(stanceVec);
        inputBlocker = false;
    }

    private Stance changeMyAStance(float strikeNum)
    {
        float n = 0;
        if(strikeNum % 2 == 0) { n = Random.value; }
        switch (strikeNum)
        {
            case 1:
                return Stance.RearGuardR;         
            case 3:
                return Stance.StandingGuardR;         
            case 5:
                return Stance.StandingGuardL;
            case 7:
                return Stance.RearGuardL;

            case 2:
                if (n < 0.5f)
                {
                    return Stance.StandingGuardR;

                }
                else
                {
                    return Stance.RearGuardR;
                }
            case 6:
                if (n < 0.5f)
                {
                    return Stance.StandingGuardL;

                }
                else
                {
                    return Stance.RearGuardL;
                }

            case 8:
            case 4:
                if (n < 0.5f) {
                    return Stance.RearGuardR; 
                }
                else {
                    return Stance.RearGuardL; }
            default:
                return Stance.ForwardGuard;
        }
    }

    private Vector2 StanceCoordLookup(Stance stance)
    {
        Vector2 vec;
        switch (stance)
        {
            case Stance.RearGuardR:
                vec.x = 1;
                vec.y= -1;
                break;
            case Stance.RearGuardL:
                vec.x = -1;
                vec.y = -1;
                break;
            case Stance.StandingGuardR:
                vec.x = 1;
                vec.y = 0;
                break;
            case Stance.StandingGuardL:
                vec.x = -1;
                vec.y = 0;
                break;
            case Stance.HighGuardR:
                vec.x = 1;
                vec.y = 1;
                break;
            case Stance.HighGuardL:
                vec.x = -1;
                vec.y = 1;
                break;
            default:
                vec.x = 0;
                vec.y = 0;
                break;
        }
        return vec;
    }

}
