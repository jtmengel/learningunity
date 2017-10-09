using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour {
  // Create the Animator instance we'll use to update the run
  public Animator anim;
  public enum NavsetInput { MoveJump,  MoveLeft,  MoveRight,  MoveToCam,  MoveAway,
                            ResetJump, ResetLeft, ResetRight, ResetToCam, ResetAway};
  
	// Use this for initialization
	void Start () {
    anim = GetComponent<Animator>();
	}
	
  // Just supporting 4 directions of movement as of now
  public void SetAnimationEvent(NavsetInput cmd) {
    switch (cmd) {
      case NavsetInput.MoveJump:
        break;
      case NavsetInput.MoveLeft:
        anim.SetTrigger("MoveLeft");
        anim.ResetTrigger("MoveRight");
        break;
      case NavsetInput.MoveRight:
        anim.SetTrigger("MoveRight");
        anim.ResetTrigger("MoveLeft");
        break;
      case NavsetInput.MoveToCam:
        anim.SetTrigger("MoveToCam");
        anim.ResetTrigger("MoveAway");
        break;
      case NavsetInput.MoveAway:
        anim.SetTrigger("MoveAway");
        anim.ResetTrigger("MoveToCam");
        break;

      case NavsetInput.ResetJump:
        anim.ResetTrigger("MoveJump");
        break;
      case NavsetInput.ResetLeft:
        anim.ResetTrigger("MoveLeft");
        break;
      case NavsetInput.ResetRight:
        anim.ResetTrigger("MoveRight");
        break;
      case NavsetInput.ResetToCam:
        anim.ResetTrigger("MoveToCam");
        break;
      case NavsetInput.ResetAway:
        anim.ResetTrigger("MoveAway");
        break;
    }
  }

	void Update () {
  }
}
