using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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

    [SerializeField] GameObject waypointPrefab;
    [SerializeField] LayerMask waypointLayerMask;

    [SerializeField] Transform waypointsParent;
    [SerializeField] Transform player;
    [SerializeField] Transform playerLastPosWaypoint;
    [SerializeField] Transform nextWaypoint;
    [SerializeField] Transform previousWaypoint;
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

        if (nextWaypoint != null)
            agent.SetDestination(nextWaypoint.position);
    }
    void Update() {
        if (!caughtPlayer) {
            SearchForPlayer();
        } else {
            agent.enabled = false;
        }

        isWalking = agent.velocity != Vector3.zero ? true : false;

        if (!isWalking)
            audioSource.Pause();
        else
            audioSource.UnPause();

        animator.SetBool("isWalking", isWalking);
    }
    void FixedUpdate() {
        CheckForWaypointCollision();
    }
    void OnDrawGizmos() {
        // draw box for waypoint collision check        
        Vector3 boxPos = new Vector3(
            this.transform.position.x,
            this.transform.position.y - 1.27f,
            this.transform.position.z
        );
        Gizmos.matrix = Matrix4x4.TRS(boxPos, Quaternion.identity, transform.localScale / 2);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
    }    
    void CheckForWaypointCollision() {
        Vector3 boxPos = new Vector3(
            this.transform.position.x,
            this.transform.position.y - 1.27f,
            this.transform.position.z
        );
        Collider[] hitColliders = Physics.OverlapBox(boxPos, transform.localScale / 2, Quaternion.identity, waypointLayerMask);
        if (hitColliders.Length > 0 && nextWaypoint != null) {
            foreach (Collider hit in hitColliders) {
                if (hit.transform == nextWaypoint) {
                    if(nextWaypoint == playerLastPosWaypoint)
                        RemovePlayerLastPosWaypoint();
                    else 
                        previousWaypoint = nextWaypoint;
                    nextWaypoint = null;
                    StartCoroutine("WaitThenNextWaypoint");
                } else if (hit.transform.GetComponent<Door>()) {
                    door = hit.transform.GetComponent<Door>();     
                    if(door.IsClosed)         
                        OpenDoor(door);                    
                }
            }
        }
    }
    void SearchForPlayer() {
        // check if player isn't behind any obstacle.
        if (player != null)
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
        Debug.DrawRay(sightPosition, (player.position - transform.position), Color.blue);
        // check if is still seeing the player and chasing it.
        RaycastHit hit;
        if (Physics.Raycast(sightPosition, (player.position - transform.position), out hit)) {
            if (hit.transform.gameObject == player.gameObject && hit.collider != ownCollider) {
                isPlayerVisible = true;
                isChasingPlayer = true;

                //if(!seenSpoken && seenSpokenTimer == 0f)
                //    Talk(playerFoundLines);

                RemovePlayerLastPosWaypoint();

                playerLastKnownPosition = player.position;                

                //StartCoroutine(ChasePlayer());
                CheckPath(playerLastKnownPosition);
            } else {                
                isPlayerVisible = false;
                isChasingPlayer = false;
                seenSpokenTimer -= Time.deltaTime;
                if (seenSpokenTimer <= 0f)
                    seenSpokenTimer = 0f;
                //seenSpoken = false;

                if(playerLastPosWaypoint == null && playerLastKnownPosition != Vector3.zero)
                    SetPlayerLastPosWaypoint();
                else if(nextWaypoint == null && playerLastPosWaypoint == null)
                    StartCoroutine("WaitThenNextWaypoint");         
            }
        }
    }
    IEnumerator WaitThenNextWaypoint() {
        //Random.InitState((int)System.DateTime.Now.Ticks);
        //int seconds = UnityEngine.Random.Range(0, 8);
        int seconds = 2;
        yield return new WaitForSeconds(seconds);
        print("waited " + seconds + " seconds");
        nextWaypoint = GetNextWaypoint();
        //agent.SetDestination(nextWaypoint.position);
        CheckPath(nextWaypoint.position);
        StopCoroutine("WaitThenNextWaypoint");
    }
    IEnumerator WaitAttackAnimation() {
        float seconds = 1f;
        yield return new WaitForSeconds(seconds);
        GotKilled();
        StopCoroutine("WaitThenNextWaypoint");
    }
    // IEnumerator ChasePlayer() {
    //     //print("chasing player");        
    //     //agent.SetDestination(playerLastKnownPosition);
    //     CheckPath(playerLastKnownPosition);
    //     yield return null;
    // }
    Transform GetNextWaypoint() {
        print("getting next wp");
        List<Waypoint> avaiableWaypoints = new List<Waypoint>();
        foreach (Transform waypointT in waypointsParent) {
            Waypoint waypoint = waypointT.GetComponent<Waypoint>();
            if (waypoint.IsAccesible)
                avaiableWaypoints.Add(waypoint);
        }
        int randomInt = Random.Range(0, avaiableWaypoints.Count);
        Transform randomWaypoint = avaiableWaypoints[randomInt].transform;

        return randomWaypoint;
    }
    void SetPlayerLastPosWaypoint() {
        print("setting last pos waypoint");
        Vector3 waypointPos = new Vector3(
                        playerLastKnownPosition.x,
                        playerLastKnownPosition.y - 1f, //player height offset
                        playerLastKnownPosition.z
                    );
        playerLastPosWaypoint = Instantiate(waypointPrefab, waypointPos, Quaternion.identity).transform;
        nextWaypoint = playerLastPosWaypoint;
    }

    void RemovePlayerLastPosWaypoint() {
        if(playerLastPosWaypoint == null)
            return;
        Destroy(playerLastPosWaypoint.gameObject);
        playerLastPosWaypoint = null;
        playerLastKnownPosition = Vector3.zero;            
    }

    void CheckPath(Vector3 targetPosition) {
        NavMeshPath navMeshPath = new NavMeshPath();
        if (agent.CalculatePath(targetPosition, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete) {
            //move to target
            agent.SetPath(navMeshPath);
        } else {
            Debug.Log("Hart can't get to " + nextWaypoint + " because it can't be reached.");
        }
    }

    void OpenDoor(Door door) {
        if (door.IsLocked) {
            Debug.LogError("hart tried to open a locked door");
            // try to find a way around to the player.
        } else {
            door.Interact();
            agent.enabled = true;
        }
    }

    void Talk(AudioClip[] lines) {
        int randomLineIndex = Random.Range(0, lines.Length);
        audioSource.clip = lines[randomLineIndex];
        audioSource.Play();

        //seenSpoken = true;
        seenSpokenTimer = seenSpokenCD;
    }
    void GotKilled() {
        GameObject.Find("Canvas/BlackScreen").GetComponent<Image>().color = new Color(0, 0, 0, 255);
        AudioSource source = GameObject.Find("Killed").GetComponent<AudioSource>();

        AudioClip axeHit = Resources.Load<AudioClip>("Audio/axe_hit");
        source.clip = axeHit;
        source.Play();
        GameObject.Find("Canvas/RawImage").SetActive(false);
        Application.Quit();
    }
    void OnTriggerEnter(Collider col) {
        // player collision
        if (col.gameObject.tag == "Player") {
            if (!caughtPlayer && isPlayerVisible) {
                caughtPlayer = true;
                print("caught player");
                player.GetComponent<Movement>().enabled = false;
                player.GetComponent<InputManager>().enabled = false;
                player.GetComponentInChildren<MouseLook>().enabled = false;
                player.LookAt(this.transform.position);
                this.transform.LookAt(player);
                animator.SetTrigger("caughtPlayer");
                StartCoroutine("WaitAttackAnimation");
                // freeze hart's rotation.
                //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            }
        }
    }
}