using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{

    [SerializeField] private bool loopX;
    [SerializeField] private bool loopY;
    [SerializeField] private bool centralize;

    private float startPosX, startPosY;
    private float lengthX, lengthY;
    private float camStartPosX, camStartPosY;

    public GameObject cam;
    public float parallaxEffect;
    void Start()
    {
        camStartPosX = cam.transform.position.x;
        camStartPosY = cam.transform.position.y;

        if (centralize)
        {
            startPosX = camStartPosX;
            startPosY = camStartPosY;
        }
        else
        {
            startPosX = transform.position.x;
            startPosY = transform.position.y;
        }

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        lengthX = sr.bounds.size.x;
        lengthY = sr.bounds.size.y;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        float distanceX, distanceY;
        float movementX, movementY;

        if (loopX || loopY)
        {
            distanceX = cam.transform.position.x * parallaxEffect;
            distanceY = cam.transform.position.y * parallaxEffect;

            movementX = cam.transform.position.x * (1 - parallaxEffect);
            movementY = cam.transform.position.y * (1 - parallaxEffect);

        }
        else
        {
            float relativeCamDistX = cam.transform.position.x - camStartPosX;
            float relativeCamDistY = cam.transform.position.y - camStartPosY;

            distanceX = relativeCamDistX * parallaxEffect;
            distanceY = relativeCamDistY * parallaxEffect;

            movementX = relativeCamDistX * (1 - parallaxEffect);
            movementY = relativeCamDistY * (1 - parallaxEffect);

        }

        transform.position = new Vector3(
            startPosX + distanceX,
            loopY ? startPosY + distanceY : transform.position.y,
            transform.position.z
        );

        if (loopX)
        {
            if (movementX > startPosX + lengthX * 0.5f)
                startPosX += lengthX;
            else if (movementX < startPosX - lengthX * 0.5f)
                startPosX -= lengthX;
        }

        if (loopY)
        {
            if (movementY > startPosY + lengthY)
                startPosY += lengthY;
            else if (movementY < startPosY - lengthY)
                startPosY -= lengthY;
        }

    }
}
