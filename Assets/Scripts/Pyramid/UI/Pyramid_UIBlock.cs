using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Pyramid_UIBlock : GameObjectBase
{
    public readonly static Vector2 uiBlockSize = new Vector2(40f, 40f);

    [SerializeField]
    Image image;

    Action<Pyramid_UIBlock> onClick;
    bool isSelected;

    public Pyramid_BlockType BlockType { private set; get; }

    public void Init(Pyramid_BlockType blockType, Action<Pyramid_UIBlock> onClick)
    {
        this.BlockType = blockType;
        image.sprite = Pyramid_Main.instance.GetBlockImage(blockType);
        this.onClick = onClick;
        isSelected = false;
    }

    public void OnDeselected()
    {
        isSelected = false;
        image.sprite = Pyramid_Main.instance.GetBlockImage(BlockType);
    }

    public void OnClick()
    {
        if (!isSelected)
        {
            isSelected = true;
            image.sprite = Pyramid_Main.instance.GetSelectedBlockImage(BlockType);
            onClick?.Invoke(this);
        }
    }
}
