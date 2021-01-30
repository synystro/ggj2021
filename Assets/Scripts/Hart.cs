using System.Collections;
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
    private bool isChasingPlayer;
    private bool caughtPlayer;
    private Door door;
    private NavMeshAgent agent;
    private Transform target;
    private Rigidbody rigidBody;

    float seenSpokenCD = 15f;
    float seenSpokenTimer;
    bool seenSpoken = false; // falou ao ver o player

    AudioSource audioSource;
    AudioClip[] playerFoundLines; // vozes pra quando o player eh avistado

    void Awake() {
        agent = this.GetComponent<NavMeshAgent>();
        rigidBody = this.GetComponent<Rigidbody>();
        
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
    }

    void SearchForPlayer() {
        // look for player in chasingRange.
        if(target == null) {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, chasingRange / 2, Vector3.down);
            foreach(RaycastHit hit in hits)
                if(hit.transform.CompareTag("Player")) {
                    target = hit.transform;                    
                    CheckIfPlayerVisible();
                }
        }
        // check if player isn't behind any obstacle.
        if(target != null) {
            CheckIfPlayerVisible();
        }
    }

    void CheckIfPlayerVisible() {
        // hart eye-sight position.
        Vector3 sightPosition = new Vector3(
            transform.position.x,
            transform.position.y + eyesHeightOffset,
            transform.position.z
            );

        // debug eye-sight position to the player.
        Debug.DrawRay(sightPosition, (target.position - transform.position), Color.green);

        // check if is still seeing the player and chasing it.
        RaycastHit hit;
        if(Physics.Raycast(sightPosition, (target.position - transform.position), out hit)) {
            if(hit.transform.gameObject == target.gameObject) {
                isPlayerVisible = true;
                isChasingPlayer = true;

                if(!seenSpoken && seenSpokenTimer == 0f)
                    Talk(playerFoundLines);

                target = hit.transform;
                playerLastKnownPosition = target.position;

                StartCoroutine(ChasePlayer());
            }
            else {
                isPlayerVisible = false;
                seenSpokenTimer -= Time.deltaTime;
                if(seenSpokenTimer <= 0f)
                    seenSpokenTimer = 0f;
                seenSpoken = false;
                StopCoroutine(ToPlayerLastKnownPosition());
                if(playerLastKnownPosition != new Vector3(0, 0, 0) && !caughtPlayer) {
                    StartCoroutine(ToPlayerLastKnownPosition());
                }
            }
        }
    }

    IEnumerator ChasePlayer() {
        //chase player
        print("chasing player");        
        agent.SetDestination(playerLastKnownPosition);
        yield return null;
    }

    IEnumerator ToPlayerLastKnownPosition() {
        //gotoplayerslastknowlocation
        agent.SetDestination(playerLastKnownPosition);
        yield return null;
    }

    void OpenDoor(Collider col) {
        Door door = col.transform.GetComponent<Door>();
        if(door.IsLocked) {
            print("hart tried to open a locked door");
            // try to find a way around to the player.
        }
        else {
            door.Interact();
            agent.enabled = true;
            agent.stoppingDistance = 0;
        }
    }

    void Talk(AudioClip[] lines) {
        int randomLineIndex = Random.Range(0, lines.Length);
        audioSource.clip = lines[randomLineIndex];        
        audioSource.Play();

        seenSpoken = true;
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