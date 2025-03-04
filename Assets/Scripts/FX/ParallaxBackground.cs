using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private GameObject cam;

    [SerializeField] private float parallaxEffect;

    private float xPosition;
    private float length;

    private void Start()
    {
        cam = GameObject.Find("Main Camera");

        xPosition = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        float BGPositionOffset = cam.transform.position.x * (1 - parallaxEffect);
        float distanceToMove = cam.transform.position.x * parallaxEffect;

        transform.position = new Vector3(xPosition + distanceToMove, transform.position.y);

        if (BGPositionOffset > xPosition + length)
        {
            
            xPosition += length;
        }
        else if (BGPositionOffset < xPosition - length)
        {
            xPosition -= length;
        }
    }
}
