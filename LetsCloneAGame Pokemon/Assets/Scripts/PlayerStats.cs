using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {
  // ***
  // *** Basic Member Variable Declarations
  // ***
  // The ones exposed in the editor:
  public float mapMovementSpeed;
  public float randomEncounterOdds = 1;

  public GameObject CameraMain;
  public GameObject CameraCombat;

  // Those not exposed in the editor:
  // Meta related
  private bool isMapView;
  private bool isBattleView;

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

  // ***
  // *** Unity/Public functions
  // ***
  void Start() { // Use this for initialization
    // Meta Init
    isMapView = true;
    isBattleView = false;
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
      UpdateAvatarAnimation("stop");
    }
  }

  // ***
  // *** Private/Utility functions
  // ***
  // The ones exposed in the editor:
  private void UpdateAvatarAnimation(string facing) {
    switch (facing)
    {
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
      case "start":
        ani.SetBool("isMoving", true);
        break;
      case "stop":
        ani.SetBool("isMoving", false);
        break;
      default: // Default is to stop and face the camera "say whaaaaat?!"
        ani.SetBool("isMoving",      true);
        ani.SetBool("isMovingSouth", true);
        ani.SetBool("isMovingNorth", false);
        ani.SetBool("isMovingEast",  false);
        ani.SetBool("isMovingWest",  false);
        break;
    }
  }

  private void UpdateAvatarLocation(float x, float y, float z) {
    movementIncrement   = 0;
    isMoving    = true;
    UpdateAvatarAnimation("start");
    startMapLocation  = transform.position;
    endMapLocation    = new Vector3(x, y, z);
  }

  private void DetermineAvatarMovement() {
    if (movementIncrement <= 1 && isMoving) {
      movementIncrement += mapMovementSpeed / 100;
    } else {
      isMoving = false;
      UpdateAvatarAnimation("stop");
    }

    if (isMoving) {
      transform.position = Vector3.Lerp(startMapLocation, endMapLocation, movementIncrement);
    }

    if ( isMoving == false ) {
      float verticalMovement = Input.GetAxis("Vertical");
      if (verticalMovement > 0) { // eg Press: w or uparrow key
        CalculateWalk();
        UpdateAvatarAnimation("north");
        UpdateAvatarLocation(transform.position.x, transform.position.y, transform.position.z + mapTileSize);
      } else if (verticalMovement < 0) { // eg, Press: s or down arrow key
        CalculateWalk();
        UpdateAvatarAnimation("south");
        UpdateAvatarLocation(transform.position.x, transform.position.y, transform.position.z - mapTileSize);
      } else {
        float horizontalMovement = Input.GetAxis("Horizontal");
        if (horizontalMovement > 0) { // eg, Press: d or -> arrow key
          CalculateWalk();
          UpdateAvatarAnimation("east");
          UpdateAvatarLocation(transform.position.x + mapTileSize, transform.position.y, transform.position.z);
        } else if (horizontalMovement < 0) { //eg, Press: a or <- arrow key
          CalculateWalk();
          UpdateAvatarAnimation("west");
          UpdateAvatarLocation(transform.position.x - mapTileSize, transform.position.y, transform.position.z);
        }
      }
    }
  }

  private void CalculateWalk() {
    // if CheckForChallengers(); // Mobs who catch the player Line of Sight and trigger combat
    // else if 
    if ( CheckForRandomCombat() ) {
      GoToCombat();
    }
    // else if CheckForTilesEffects(); // Doors, teleporters, plot triggers
  }

  private bool CheckForRandomCombat() {
    // Determine if we need to proc some event like battle, challenge, story, etc
    if ((randFightThreshold * randomEncounterOdds) <= randFightCounter) {
      randFightThreshold = Random.Range(5, 25);
      return true;
    }
    else {
      randFightCounter += 1;
      return false;
    }
  }

  private void GoToCombat() {
    UpdateAvatarAnimation("stop");
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
