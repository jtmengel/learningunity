using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementDefinitions {
  North, East, South, West,
  Walking, Stationary,
  NoChange
}

public class PlayerStats : MonoBehaviour {
  // *********************************************************
  // *** Basic Member Variable Declarations
  // *********************************************************
  // The ones exposed in the editor:
  public float mapMovementSpeed;
  public float randomBaddleOdds = 1;

  public GameObject CameraMain;
  public GameObject CameraCombat;

  // Those not exposed in the editor:
  // Meta related
  private bool isMapView;
  private bool isBattleView;
  private bool isEnteringBattle;

  // -- MapView & Movement related
  private Vector3 confirmedOrigin;
  private Vector3 confirmedDestination;
  private bool isMoving;
  private bool freezePlayerInput;
  private float movementLerpPercent;
  private float mapTileSize = 1f;
  private float randomBattleFuse;
  private float randomBattleThreshold;

  // "momentum" and "blocked" are tie-breaker tools for simultaneous inputs
  private MovementDefinitions moveMomentum;
  private MovementDefinitions moveBlocked;
  // -- Rendering related
  private Animator ani;

  // *********************************************************************
  // *** Unity/Public functions
  // *********************************************************************
  void Start() { // Use this for initialization
    // Meta Init
    isMapView = true;
    isBattleView = false;
    isEnteringBattle = false;
    freezePlayerInput = false;
    // Map Init
    confirmedOrigin = transform.position;
    confirmedDestination = transform.position;
    isMoving = false;
    randomBattleFuse = 0f;
    randomBattleThreshold = Random.Range(5, 25);
    // Avatar Animation Init
    ani = GetComponent<Animator>();
    ani.enabled = true;
    moveMomentum = MovementDefinitions.NoChange;
    moveBlocked  = MovementDefinitions.NoChange;
    /*int setMoveSpeed;
    int.TryParse(mapMovementSpeed, out setMoveSpeed);
    ani.SetInteger("MovementSpeed", setMoveSpeed);*/
  }

  void Update() { // Update is called once per frame
    if (isMapView) {
      DetermineAvatarMovement();
    } else if (isBattleView && isMoving) {
      isMoving = false;
      UpdateAvatarAnimation(MovementDefinitions.Stationary);
    }
  }

  // ******************************************************************
  // *** Private/Utility functions
  // ******************************************************************
  // The ones exposed in the editor:
  private void UpdateAvatarAnimation( MovementDefinitions cueAnimationName, 
                                      MovementDefinitions avatarFacingDir = MovementDefinitions.NoChange ) {
    switch (cueAnimationName) {
      case MovementDefinitions.Walking:
        ani.SetBool("isWalking", true);
        break;
      case MovementDefinitions.Stationary:
        ani.SetBool("isWalking", false);
        break;
      default:
        ani.SetBool("isWalking", false);
        break;
    }
    switch (avatarFacingDir) {
      case MovementDefinitions.NoChange: break;
      case MovementDefinitions.North:
        ani.SetBool("isMovingNorth", true);
        ani.SetBool("isMovingEast",  false);
        ani.SetBool("isMovingSouth", false);
        ani.SetBool("isMovingWest",  false);
        break;
      case MovementDefinitions.West:
        ani.SetBool("isMovingWest",  true);
        ani.SetBool("isMovingNorth", false);
        ani.SetBool("isMovingEast",  false);
        ani.SetBool("isMovingSouth", false);
        break;
      case MovementDefinitions.East:
        ani.SetBool("isMovingEast",  true);
        ani.SetBool("isMovingNorth", false);
        ani.SetBool("isMovingSouth", false);
        ani.SetBool("isMovingWest",  false);
        break;
      case MovementDefinitions.South:
        ani.SetBool("isMovingSouth", true);
        ani.SetBool("isMovingNorth", false);
        ani.SetBool("isMovingEast",  false);
        ani.SetBool("isMovingWest",  false);
        break;
      default:
        ani.SetBool("isMovingSouth", true);
        ani.SetBool("isMovingNorth", false);
        ani.SetBool("isMovingEast", false);
        ani.SetBool("isMovingWest", false);
        break;
    }
  }

  // TODO this section is hacky AF to make movement input a little more graceful
  // this code could be 30% lighter if we just constructed some kind of domain lang/objects
  public bool GetPlayerMoveInput(out Vector3 requestedChange, out MovementDefinitions dir) {
    requestedChange = new Vector3();
    dir             = MovementDefinitions.NoChange;

    float verticalMovement   = Input.GetAxis("Vertical");
    float horizontalMovement = Input.GetAxis("Horizontal");

    if ((moveBlocked != MovementDefinitions.North) && verticalMovement > 0) {
      requestedChange = new Vector3(transform.position.x, transform.position.y, transform.position.z + mapTileSize);
      dir = MovementDefinitions.North;
      return true;
    } else if ((moveBlocked != MovementDefinitions.South) && verticalMovement < 0) {
      requestedChange = new Vector3(transform.position.x, transform.position.y, transform.position.z - mapTileSize);
      dir = MovementDefinitions.South;
      return true;
    } else if ((moveBlocked != MovementDefinitions.East) && horizontalMovement > 0) {
      requestedChange = new Vector3(transform.position.x + mapTileSize, transform.position.y, transform.position.z);
      dir = MovementDefinitions.East;
      return true;
    } else if ((moveBlocked != MovementDefinitions.West) && horizontalMovement < 0) {
      requestedChange = new Vector3(transform.position.x - mapTileSize, transform.position.y, transform.position.z);
      dir = MovementDefinitions.West;
      return true;
    }
    return false;
  }

  private void DetermineAvatarMovement() {
    // We want to complete any movement we've started, and if we're fully 
    //   on a tile we want to either wait for or process the next move
    if (isMoving) {
      if (movementLerpPercent <= 1) {
        movementLerpPercent += mapMovementSpeed / 100;
      } else {
        isMoving = false;
      }
      // Continue resolving an outstanding tile move
      transform.position = Vector3.Lerp(confirmedOrigin, confirmedDestination, movementLerpPercent);
    } else if (isMapView) {
      Vector3 requestedDest; // Decide if the player wants to move (to requestedDest)
      MovementDefinitions moveDir;

      if ( GetPlayerMoveInput(out requestedDest, out moveDir) ) {
        Vector3 origin = transform.position;
        ResolveWalkRequest(origin, requestedDest, moveDir);
      } else {
        UpdateAvatarAnimation(MovementDefinitions.Stationary);
      }
    }
  }

  // This function is only called when there's a change in the avatar's Tile
  private void ConfirmWalkTo(Vector3 start, Vector3 end, MovementDefinitions moveDir) {
    isMoving             = true;
    moveMomentum         = moveDir;
    moveBlocked          = MovementDefinitions.NoChange;
    confirmedOrigin      = start;
    confirmedDestination = end;
    movementLerpPercent  = 0;
    UpdateAvatarAnimation(MovementDefinitions.Walking, moveDir);
  }

  private void ResolveWalkRequest(Vector3 candidateOrigin, Vector3 candidateDest, MovementDefinitions moveDir) {
    RaycastHit requestedTileHit; // What we have been directed to face
    //RaycastHit currentTileHit; // What are we standing over
    Vector3 inputDir = (candidateDest -  candidateOrigin).normalized;

    if ((Physics.Raycast(candidateOrigin, inputDir,     out requestedTileHit, 1f))/* && 
        (Physics.Raycast(candidateDest,   Vector3.down, out currentTileHit,   2f))*/ ){
      switch (requestedTileHit.collider.gameObject.tag) { // Checking ONLY where we're going ATM
        case "Untraversable": // Is this a wall/tree/water?
          UpdateAvatarAnimation(MovementDefinitions.Stationary, moveDir);
          moveMomentum = MovementDefinitions.NoChange;
          moveBlocked  = moveDir;
          break;
        case "Traversable": // Animate the Avatar facing and walking that direction, move to the destination
          ConfirmWalkTo(candidateOrigin, candidateDest, moveDir);
          break;
        case "TraversablePortal": // Animate the Avatar facing and walking that direction, move to the destination, and then teleport
          ConfirmWalkTo(candidateOrigin, candidateDest, moveDir);

          break;
        case "TraversableRandomEncounter": // Am I moving into a "GrassEncounter"?
          ConfirmWalkTo(candidateOrigin, candidateDest, moveDir);
          if ((randomBattleThreshold / randomBaddleOdds) > randomBattleFuse) randomBattleFuse += 1;
          else {
            randomBattleThreshold = Random.Range(5, 25);
            isEnteringBattle = true;
          }
          break;
      }
    } else { // DEFAULT - face the "mysterious" tile
      UpdateAvatarAnimation(MovementDefinitions.Stationary, moveDir);
      moveMomentum = MovementDefinitions.NoChange;
      moveBlocked = moveDir;
    }

    if (isEnteringBattle) {
      GoToCombat();
    }
  }

  private void GoToCombat() {
    UpdateAvatarAnimation(MovementDefinitions.Stationary);
    isEnteringBattle = false;
    isBattleView = true;
    CameraCombat.SetActive(true);
    CameraMain.SetActive(false);
    isMapView = false;
  }

  private void GoToMap() {
    isMapView = true;
    CameraMain.SetActive(true);
    isBattleView = false;
    CameraCombat.SetActive(false);
  }
}
