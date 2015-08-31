using UnityEngine;
using System.Collections;

//Version 2
public class FreezeAxis : MonoBehaviour {

    public bool freezeXRot;
    public bool freezeYRot;
    public bool freezeZRot;
    public bool freezeXTrans;
    public bool freezeYTrans;
    public bool freezeZTrans;

    private Quaternion initialRotation;
    private Vector3 initialPosition;

	void Start () {
        initialRotation = gameObject.transform.rotation;
        initialPosition = gameObject.transform.position;
    }
	
	void Update () {
        updateRotation();
        updatePosition();
    }

    private void updateRotation() {
        Vector3 currRotation = transform.rotation.eulerAngles;
        if (freezeXRot)
            currRotation.x = initialRotation.x;
        if (freezeYRot)
            currRotation.y = initialRotation.y;
        if (freezeZRot)
            currRotation.z = initialRotation.z;
        gameObject.transform.rotation = Quaternion.Euler(currRotation);
    }

    private void updatePosition() {
        Vector3 currTranslation = transform.position;
        if (freezeXTrans)
            currTranslation.x = initialPosition.x;
        if (freezeYTrans)
            currTranslation.y = initialPosition.y;
        if (freezeZTrans)
            currTranslation.z = initialPosition.z;
        gameObject.transform.position = currTranslation;
    }
}
