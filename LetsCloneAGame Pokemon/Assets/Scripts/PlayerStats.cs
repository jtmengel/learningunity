using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {
  // Are exposed in the editor:
  public Vector3  startPoint;
  public Vector3  endPoint;
  public float    speed;
  public bool     isMoving;
  public Animator ani;

  // Not exposed in the editor:
  private float increment;
  private float tileSize;
  

  // Use this for initialization
  void Start () {
    startPoint = transform.position;
    endPoint   = transform.position;
    isMoving   = false;
    tileSize   = 1f;
    ani = GetComponent<Animator>();
    ani.enabled = true;
  }

  void SetDirection(string facing) {
    switch (facing)
    {
      case "up":
        ani.SetBool("isMovingNorth", true);
        ani.SetBool("isMovingEast", false);
        ani.SetBool("isMovingSouth", false);
        ani.SetBool("isMovingWest", false);
        break;
      case "left":
        ani.SetBool("isMovingNorth", false);
        ani.SetBool("isMovingEast", false);
        ani.SetBool("isMovingSouth", false);
        ani.SetBool("isMovingWest", true);
        break;
      case "right":
        ani.SetBool("isMovingNorth", false);
        ani.SetBool("isMovingEast", true);
        ani.SetBool("isMovingSouth", false);
        ani.SetBool("isMovingWest", false);
        break;
      default:
        ani.SetBool("isMovingNorth", false);
        ani.SetBool("isMovingEast", false);
        ani.SetBool("isMovingSouth", true);
        ani.SetBool("isMovingWest", false);
        break;
    }
  }

  void ApplyMovement(float x, float y, float z) {
    increment   = 0;
    isMoving    = true;
    ani.SetBool("isMoving", true);
    startPoint  = transform.position;
    endPoint    = new Vector3(x, y, z);
  }

  // Update is called once per frame
  void Update () {

    if (increment <= 1 && isMoving) {
      Debug.Log("Is Moving");
      increment += speed / 100;
    } else {
      isMoving = false;
      ani.SetBool("isMoving", false);
      Debug.Log("Stationary");
    }

    if (isMoving) {
      transform.position = Vector3.Lerp(startPoint, endPoint, increment);
    }

    if( isMoving==false ) {
      float verticalMovement = Input.GetAxis("Vertical");
      if (verticalMovement > 0) { // Press: w
        SetDirection("up");
        ApplyMovement(transform.position.x, transform.position.y, transform.position.z + tileSize);
      } else if (verticalMovement < 0) { // Press: s
        SetDirection("down");
        ApplyMovement(transform.position.x, transform.position.y, transform.position.z - tileSize);
      } else {
        float horizontalMovement = Input.GetAxis("Horizontal");
        if (horizontalMovement > 0) { // Press: d
          SetDirection("right");
          ApplyMovement(transform.position.x + tileSize, transform.position.y, transform.position.z);
        } else if (horizontalMovement < 0) { //Press: a
          SetDirection("left");
          ApplyMovement(transform.position.x - tileSize, transform.position.y, transform.position.z);
        }
      }
    }
  }
}
