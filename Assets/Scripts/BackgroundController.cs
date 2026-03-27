using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private float startPosX, startPosY;
    private float lengthX, lengthY;

    public GameObject cam;
    public float parallaxEffect;
    void Start()
    {
        //startPosX = transform.position.x;
        //startPosY = transform.position.y;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        lengthX = sr.bounds.size.x;
        lengthY = sr.bounds.size.y;

        startPosX = cam.transform.position.x;
        startPosY = cam.transform.position.y;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        float distanceX = cam.transform.position.x * parallaxEffect;
        float distanceY = cam.transform.position.y * parallaxEffect;

        float movementX = cam.transform.position.x * (1 - parallaxEffect);
        float movementY = cam.transform.position.y * (1 - parallaxEffect);

        transform.position = new Vector3(
            startPosX + distanceX,
            startPosY + distanceY,
            transform.position.z
        );

        if (movementX > startPosX + lengthX * 0.5f)
            startPosX += lengthX;
        else if (movementX < startPosX - lengthX * 0.5f)
            startPosX -= lengthX;

        if (movementY > startPosY + lengthY)
            startPosY += lengthY;
        else if (movementY < startPosY - lengthY)
            startPosY -= lengthY;

    }
}
