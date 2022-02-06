using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortLayer : MonoBehaviour
{
    Renderer rend;
    public int layer=0;
    // Start is called before the first frame update
    void Start()
    {
        rend = gameObject.GetComponent<Renderer>();
        rend.sortingOrder = layer;
       // rend.sortingLayerName = "front";
    }

    // Update is called once per frame
    void Update()
    {
        rend.sortingOrder = layer;
    }
}
