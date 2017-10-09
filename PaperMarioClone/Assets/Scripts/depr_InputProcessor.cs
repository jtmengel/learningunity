using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputProcessor : MonoBehaviour {
  public float moveSpeed;

  // Create the AnimController instance we'll push animation commands to
  private AnimController animC;
  private Rigidbody rigidB;

  // Use this for initialization
  void Start () {
    animC = GetComponent<AnimController>();
    rigidB = GetComponent<Rigidbody>();
  }

  private void SetMovement(float x, float y, float z) {

  }
	
	// Update is called once per frame
	void Update () {
    if (Input.GetKeyDown("w")) {
      animC.SetAnimationEvent(AnimController.NavsetInput.MoveAway);
      rigidB.velocity = new Vector3(0, 0, moveSpeed);
    } else if (Input.GetKeyDown("s")) {
      animC.SetAnimationEvent(AnimController.NavsetInput.MoveToCam);
      rigidB.velocity = new Vector3(0, 0, -moveSpeed);
    } else {
      animC.SetAnimationEvent(AnimController.NavsetInput.ResetToCam);
      animC.SetAnimationEvent(AnimController.NavsetInput.ResetAway);
      rigidB.velocity = new Vector3(0, 0, 0);
    }
    
    if (Input.GetKeyDown("a")) {
      animC.SetAnimationEvent(AnimController.NavsetInput.MoveLeft);
      rigidB.velocity = new Vector3(moveSpeed, 0, rigidB.velocity.z);
    } else if (Input.GetKeyDown("d")) {
      animC.SetAnimationEvent(AnimController.NavsetInput.MoveRight);
      rigidB.velocity = new Vector3(-moveSpeed, 0, rigidB.velocity.z);
    } else {
      animC.SetAnimationEvent(AnimController.NavsetInput.ResetRight);
      animC.SetAnimationEvent(AnimController.NavsetInput.ResetLeft);
      rigidB.velocity = new Vector3(0, 0, 0);
    }
    //done
  }
}
