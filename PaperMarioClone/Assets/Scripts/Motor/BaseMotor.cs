using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMotor : MonoBehaviour {

  protected CharacterController controller;
  protected BaseState state;
  protected Transform thisTransform;

  private float movementSpeed = 5f;
  private float gravitySpeed = 25f;

  public float Speed { get{ return movementSpeed; } }
  public float Gravity { get{ return gravitySpeed; } }
  public Vector3 MoveVector { set; get; }

  protected abstract void UpdateMotor();

  protected virtual void Start() {
    controller = gameObject.AddComponent<CharacterController>();
    thisTransform = transform;

    state = gameObject.AddComponent<WalkingState>();
    state.Contstruct();
  }
	// Update is called once per frame
	void Update () {
    UpdateMotor();
	}

  protected virtual void Move() {
    controller.Move(MoveVector * Time.deltaTime);
  }
}
