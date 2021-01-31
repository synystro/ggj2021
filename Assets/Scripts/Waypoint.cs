using UnityEngine;

public class Waypoint : MonoBehaviour {

    [SerializeField] bool isAccessible;
    public bool IsAccesible { get { return isAccessible; } }

    public void SetAccessible() {
        isAccessible = true;
    }
}
