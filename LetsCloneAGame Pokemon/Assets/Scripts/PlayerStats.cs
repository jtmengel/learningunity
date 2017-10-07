using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {
  // *********************************************************
  // *** Basic Member Variable Declarations
  // *********************************************************
  // The ones exposed in the editor:
  public float mapMovementSpeed;
  public float randomEncounterOdds = 1;

  public GameObject CameraMain;
  public GameObject CameraCombat;

  // Those not exposed in the editor:
  // Meta related
  private bool isMapView;
  private bool isBattleView;
  private bool isEnteringBattle;

  // -- MapView & Movement related
  private Vector3 startMapLocation;
  private Vector3 endMapLocation;
  private bool    isMoving;
  private float   movementIncrement;
  private float   mapTileSize = 1f;
  private float   randFightCounter;
  private float   randFightThreshold;
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
    // Map Init
    startMapLocation   = transform.position;
    endMapLocation     = transform.position;
    isMoving           = false;
    ani                = GetComponent<Animator>();
    ani.enabled        = true;
    randFightCounter   = 0f;
    randFightThreshold = Random.Range(5, 25);
    // Combat Init
    // --
    // Menu Init
    // --
  }

  void Update() { // Update is called once per frame
    if (isMapView) {
      DetermineAvatarMovement();
    } else if (isBattleView) {
      isMoving = false;
      UpdateAvatarAnimation("stationary");
    }
  }

  // ******************************************************************
  // *** Private/Utility functions
  // ******************************************************************
  // The ones exposed in the editor:
  private void UpdateAvatarAnimation(string cueAnimationName, string avatarFacingDir = "") {
    switch (cueAnimationName) {
      case "walking":
        ani.SetBool("isMoving", true);
        break;
      case "stationary":
        ani.SetBool("isMoving", false);
        break;
      default:
        ani.SetBool("isMoving", false);
        break;
    }
    switch (avatarFacingDir) {
      case "": break;
      case "north":
        ani.SetBool("isMovingNorth", true);
        ani.SetBool("isMovingEast",  false);
        ani.SetBool("isMovingSouth", false);
        ani.SetBool("isMovingWest",  false);
        break;
      case "west":
        ani.SetBool("isMovingWest",  true);
        ani.SetBool("isMovingNorth", false);
        ani.SetBool("isMovingEast",  false);
        ani.SetBool("isMovingSouth", false);
        break;
      case "east":
        ani.SetBool("isMovingEast",  true);
        ani.SetBool("isMovingNorth", false);
        ani.SetBool("isMovingSouth", false);
        ani.SetBool("isMovingWest",  false);
        break;
      case "south":
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

  private void UpdateAvatarLocation(float x, float y, float z) {
    Vector3 destination = new Vector3(x, y, z);
    CalculateWalk(destination);
    movementIncrement   = 0;
    isMoving    = true;
    UpdateAvatarAnimation("walking");
    startMapLocation  = transform.position;
    endMapLocation    = destination;
  }

  private void DetermineAvatarMovement() {
    if (movementIncrement <= 1 && isMoving) {
      movementIncrement += mapMovementSpeed / 100;
    } else {
      isMoving = false;
      UpdateAvatarAnimation("stationary");
    }
    
    // If we are moving, let's resolve the movement
    // If we're not moving, we can request a move - check for input
    if (isMoving) {
      transform.position = Vector3.Lerp(startMapLocation, endMapLocation, movementIncrement);
    } else { 
      float verticalMovement = Input.GetAxis("Vertical");
      if (verticalMovement > 0) {
        UpdateAvatarAnimation("walking", "north");
        UpdateAvatarLocation(transform.position.x, transform.position.y, transform.position.z + mapTileSize);
      } else if (verticalMovement < 0) {
        UpdateAvatarAnimation("walking", "south");
        UpdateAvatarLocation(transform.position.x, transform.position.y, transform.position.z - mapTileSize);
      } else {
        float horizontalMovement = Input.GetAxis("Horizontal");
        if (horizontalMovement > 0) {
          UpdateAvatarAnimation("walking", "east");
          UpdateAvatarLocation(transform.position.x + mapTileSize, transform.position.y, transform.position.z);
        } else if (horizontalMovement < 0) {
          UpdateAvatarAnimation("walking", "west");
          UpdateAvatarLocation(transform.position.x - mapTileSize, transform.position.y, transform.position.z);
        }
      }
    }
  }

  private void CalculateWalk(Vector3 destination) {
    CheckForMapEffects(destination);

    if (isEnteringBattle) {
      GoToCombat();
    }
  }
  
  // Mobs who catch the player Line of Sight and trigger combat
  // Random Encounters
  // Doors, teleporters, plot triggers
  private void CheckForMapEffects(Vector3 destination) {
    // yield return new WaitForSeconds(0.3f); // TODO we need to figure out to wait on the "enter combat" logic for a moment until we've traversed onto the new tile
    RaycastHit hitInfo; // What are we standing over
    if (Physics.Raycast(destination, Vector3.down, out hitInfo, 100f)) {
      float distanceToGround = hitInfo.distance;
      
      switch (hitInfo.collider.gameObject.tag) {
        case "GrassEncounter":
          // If we're on a "Wild Tile"
          if ((randFightThreshold*randomEncounterOdds) <= randFightCounter) {
            randFightThreshold = Random.Range(5, 25);
            isEnteringBattle = true;
          } else {
            randFightCounter += 1;
          }
          break;
      }
    }
  }

  private void GoToCombat() {
    UpdateAvatarAnimation("stationary");
    isEnteringBattle = false;
    isBattleView = true;
    CameraCombat.SetActive(true);
    CameraMain.SetActive(false);
    isMapView = false;

    Debug.Log("Something told you to report for CombatView, young whippersnapper...");
  }

  private void GoToMap() {
    isMapView = true;
    CameraMain.SetActive(true);
    isBattleView = false;
    CameraCombat.SetActive(false);

    Debug.Log("Something told you to go back to the MapView, young man...");
  }
}
