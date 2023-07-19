using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loadWorkaround : MonoBehaviour
{
    // Start is called before the first frame update
    gameState state;
    void Start()
    {
        state = GameObject.Find("gameState").GetComponent<gameState>();
        GetComponent<SpriteRenderer>().color = state.loadColors[state.phase];
    }

}
