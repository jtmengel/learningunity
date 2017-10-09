using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingState : BaseState {

  public override Vector3 ProcessMotion(Vector3 input) {
    return input * motor.Speed;
  }
}
