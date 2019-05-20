using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class UIChessToggle : MonoBehaviour {

    public string ChessName;
    private void Start()
    {
        ChessName = GetComponent<RawImage>().texture.name;
    }
    public void Enable()
    {
        GetComponent<RawImage>().enabled = true;
    }
    public void Disable()
    {
        GetComponent<RawImage>().enabled = false;
    }
}
