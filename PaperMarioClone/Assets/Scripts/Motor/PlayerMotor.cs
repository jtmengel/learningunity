using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : BaseMotor {

  protected override void UpdateMotor() {
    // Gets Input
    MoveVector = InputDirection();

    // Send the input to a filter (player momentum vs world momentum eg gravity)
    MoveVector = state.ProcessMotion(MoveVector);

    // Move
    //
    Move();

  }

	private Vector3 InputDirection() {
    Vector3 dir = Vector3.zero;

    dir.x = Input.GetAxis("Horizontal");
    dir.z = Input.GetAxis("Vertical");

    if (dir.magnitude > 1) dir.Normalize();

    return dir;
  }
}
