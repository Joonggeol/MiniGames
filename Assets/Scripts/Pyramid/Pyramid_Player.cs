using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pyramid_Player : MonoBehaviour
{
    List<Pyramid_Block> blocks = new List<Pyramid_Block>();

    public void Init(List<Pyramid_Block> blocks)
    {
        this.blocks = blocks;
    }
}
