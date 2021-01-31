using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Hart : MonoBehaviour {

    float chasingRange = 50;

    [SerializeField] private float eyesHeightOffset;
    [SerializeField] private float interactionDistance;

    private Vector3 playerLastKnownPosition;
    private int movementIndex;
    private bool isPlayerVisible;
    private bool isWalking;
    private bool isChasingPlayer;
    private bool caughtPlayer;
    private Door door;
    private NavMeshAgent agent;

    [SerializeField] Transform waypointsParent;
    [SerializeField] Transform player;
    [SerializeField] Transform nextWaypoint;
    //[SerializeField] private Transform target;
    private Rigidbody rigidBody;

    float seenSpokenCD = 15f;
    float seenSpokenTimer;
    //bool seenSpoken = false; // falou ao ver o player

    BoxCollider ownCollider;
    UnityEngine.Animator animator;
    AudioSource audioSource;
    AudioClip[] playerFoundLines; // vozes pra quando o player eh avistado

    void Awake() {
        agent = this.GetComponent<NavMeshAgent>();
        rigidBody = this.GetComponent<Rigidbody>();
        ownCollider = this.GetComponent<BoxCollider>();
        
        animator = this.GetComponentInChildren<UnityEngine.Animator>();     
        audioSource = this.GetComponent<AudioSource>();
        playerFoundLines = Resources.LoadAll<AudioClip>("Audio/Hart/playerFoundLines");
    }

    void Update() {
        if(!caughtPlayer) {
            SearchForPlayer();
        }
        else {
            agent.enabled = false;
        }

        isWalking = agent.velocity != Vector3.zero ? true : false;

        if(!isWalking)
            audioSource.Pause();
        else
            audioSource.UnPause();

        animator.SetBool("isWalking", isWalking);
    }

    void SearchForPlayer() {
        // check if player isn't behind any obstacle.
        if(player != null)
            CheckIfPlayerVisible();
        else
            Debug.LogError("Please set Hart's target (player)");
    }

    void CheckIfPlayerVisible() {
        // hart eye-sight position.
        Vector3 sightPosition = new Vector3(
            transform.position.x,
            transform.position.y + eyesHeightOffset,
            transform.position.z
            );

        // debug eye-sight position to the player.
        Debug.DrawRay(sightPosition, (player.position - transform.position), Color.green);

        // check if is still seeing the player and chasing it.
        RaycastHit hit;
        if(Physics.Raycast(sightPosition, (player.position - transform.position), out hit)) {
            if(hit.transform.gameObject == player.gameObject && hit.collider != ownCollider) {
                isPlayerVisible = true;
                isChasingPlayer = true;

                //if(!seenSpoken && seenSpokenTimer == 0f)
                //    Talk(playerFoundLines);

                playerLastKnownPosition = player.position;

                StartCoroutine(ChasePlayer());
            }
            else {
                isPlayerVisible = false;
                isChasingPlayer = false;
                seenSpokenTimer -= Time.deltaTime;
                if(seenSpokenTimer <= 0f)
                    seenSpokenTimer = 0f;
                //seenSpoken = false;
                StopCoroutine(ToPlayerLastKnownPosition());
                if(playerLastKnownPosition != Vector3.zero && !caughtPlayer)
                    StartCoroutine(ToPlayerLastKnownPosition());
                else if(playerLastKnownPosition == this.transform.position)                  
                    playerLastKnownPosition = Vector3.zero;                    
                else if( playerLastKnownPosition == Vector3.zero && nextWaypoint != null)
                    agent.SetDestination(nextWaypoint.position);
                else if(nextWaypoint == null)
                    nextWaypoint = GetNextWaypoint();
            }
        }
    }

    IEnumerator ChasePlayer() {
        //chase player
        //print("chasing player");        
        agent.SetDestination(playerLastKnownPosition);
        yield return null;
    }

    IEnumerator ToPlayerLastKnownPosition() {
        //gotoplayerslastknowlocation
        agent.SetDestination(playerLastKnownPosition);
        yield return null;
    }

    Transform GetNextWaypoint() {
        List<Waypoint> avaiableWaypoints = new List<Waypoint>();
        foreach(Transform waypointT in waypointsParent) {
            Waypoint waypoint = waypointT.GetComponent<Waypoint>();
            if(waypoint.IsAccesible)
                avaiableWaypoints.Add(waypoint);
        }
        int randomInt = Random.Range(0, avaiableWaypoints.Count); 
        Transform randomWaypoint = avaiableWaypoints[randomInt].transform;

        return randomWaypoint;
    }

    void OpenDoor(Collider col) {
        Door door = col.transform.GetComponent<Door>();
        if(door.IsLocked) {
            print("hart tried to open a locked door");
            // try to find a way around to the player.
        }
        else if(door.IsClosed){
            door.Interact();
            agent.enabled = true;
            agent.stoppingDistance = 0;
        }
    }

    void Talk(AudioClip[] lines) {
        int randomLineIndex = Random.Range(0, lines.Length);
        audioSource.clip = lines[randomLineIndex];        
        audioSource.Play();

        //seenSpoken = true;
        seenSpokenTimer = seenSpokenCD;
    }

    void OnTriggerEnter(Collider col) {
        // player collision
        if(col.gameObject.tag == "Player") {
            if(!caughtPlayer && isPlayerVisible) {
                caughtPlayer = true;
                print("caught player");
                // freeze hart's rotation.
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            }
        }

        // door collision
        if(isChasingPlayer) {
            if(col.transform.GetComponent<Door>()) {
                door = col.transform.GetComponent<Door>();
                if(door && !isPlayerVisible) {
                    agent.enabled = false;
                    OpenDoor(col);

                }
            }
        }
    }
}