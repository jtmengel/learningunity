using UnityEngine;

public class LocomotionSMB : StateMachineBehaviour {
  public float m_Damping = 0.15f; // To smooth the transition between input values - to make it less abrupt
  
  // naming convention is from the animator string-to-hash - here we check for the ID int for sake of error output
  private readonly int m_HashHorizontalPara = Animator.StringToHash("Horizontal");
  private readonly int m_HashVerticalPara = Animator.StringToHash("Vertical");
  
  override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    float horizontal = Input.GetAxis("Horizontal");
    float vertical = Input.GetAxis("Vertical");

    Vector2 input = new Vector2(horizontal, vertical).normalized;

    animator.SetFloat(m_HashHorizontalPara, input.x, m_Damping, Time.deltaTime);
    animator.SetFloat(m_HashVerticalPara, input.y, m_Damping, Time.deltaTime);
  }
}