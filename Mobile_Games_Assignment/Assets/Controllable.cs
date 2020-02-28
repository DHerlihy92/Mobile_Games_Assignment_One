using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controllable : MonoBehaviour
{
    Renderer myRenderer;
    private int direction;
    private Vector3 dragDestination;

    // Start is called before the first frame update
    void Start()
    {
        dragDestination = transform.position;
        myRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, dragDestination) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, dragDestination, 0.1f);
        }
    }

    internal void deselect()
    {
        goDefault();
    }

    internal void goDefault()
    {
        myRenderer.material.color = Color.white;
    }

    internal void changeColor(Color newColor)
    {
        myRenderer.material.color = newColor;
    }

    internal void latestDragPosition(Vector3 desiredDestination) => dragDestination = desiredDestination;

}
