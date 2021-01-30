using UnityEngine;

public class Movement : MonoBehaviour
{
    public float velMov = 5f;
    public float velRot = 3f;


    void FixedUpdate()
    {
        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f)
            transform.Translate(Input.GetAxis("Vertical") * velMov * Vector3.forward * Time.deltaTime);

        if (Mathf.Abs(Input.GetAxis("Mouse X")) > .01f)
            transform.Rotate(0, Input.GetAxis("Mouse X") * velRot * Time.deltaTime, 0);

        // if (Mathf.Abs(Input.GetAxis("Mouse Y")) > .01f)
        //     Camera.main.transform.Rotate(Input.GetAxis("Mouse Y") * -velRot * Time.deltaTime, 0, 0);
    }
}