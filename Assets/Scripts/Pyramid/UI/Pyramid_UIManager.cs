using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pyramid_UIManager : MonoBehaviour
{
    public static Pyramid_UIManager instance;

    [SerializeField]
    Pyramid_UIBlock tempUIBlock;
    [SerializeField]
    Transform uiBlockParent;

    [SerializeField]
    Vector2 uiBlockStartPosOffest;

    List<Pyramid_UIBlock> blocks = new List<Pyramid_UIBlock>();

    Pyramid_UIBlock CurrentSelectedBlock;

    public Pyramid_BlockType CurrentSelectedBlockType;

    void Awake()
    {
        instance = this;
    }

    public void SetPlayerBlocks(List<Pyramid_BlockType> blockTypes)
    {
        for(int i = 0; i < blockTypes.Count; i++)
        {
            Pyramid_UIBlock block = tempUIBlock.Spawn<Pyramid_UIBlock>(uiBlockParent, new Vector2((i * Pyramid_UIBlock.uiBlockSize.x) + uiBlockStartPosOffest.x , uiBlockStartPosOffest.y));
            block.Init(blockTypes[i], OnBlockSelected);

            blocks.Add(block);
        }
    }

    public void OnRestartClicked()
    {
        RemoveUIBlocks();
        Pyramid_Main.instance.Restart();
    }

    private void RemoveUIBlocks()
    {
        for(int i =0; i < blocks.Count; i++)
        {
            blocks[i].Recycle();
        }

        blocks = new List<Pyramid_UIBlock>();
    }

    void OnBlockSelected(Pyramid_UIBlock uiBlock)
    {
        CurrentSelectedBlock?.OnDeselected();
        CurrentSelectedBlock = uiBlock;
        CurrentSelectedBlockType = uiBlock.BlockType;

        Pyramid_Main.instance.OnBlockSelected(CurrentSelectedBlockType);
    }

    public void OnBlockSituated()
    {
        blocks.Remove(CurrentSelectedBlock);
        CurrentSelectedBlock.Recycle();

        CurrentSelectedBlockType = Pyramid_BlockType.None;

        for (int i = 0; i < blocks.Count; i++)
        {
            blocks[i].cachedTransform.localPosition = new Vector2((i * Pyramid_UIBlock.uiBlockSize.x) + uiBlockStartPosOffest.x, uiBlockStartPosOffest.y);
        }
    }
}
