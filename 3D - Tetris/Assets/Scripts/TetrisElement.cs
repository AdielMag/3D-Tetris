using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisElement : MonoBehaviour
{
    // Start is called before the first frame update
    public TetrisApplication app
    {
        get { return TetrisApplication.instance; }
    }

}
