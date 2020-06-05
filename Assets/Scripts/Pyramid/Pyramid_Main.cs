using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class GameObjectBase : MonoBehaviour
{
    public Transform cachedTransform { protected set; get; }

    private void Awake()
    {
        cachedTransform = transform;
    }
}

public class Pyramid_Main : GameObjectBase
{
    public static Pyramid_Main instance { private set; get; }

    readonly int maxHeight = 8;
    readonly int totalBlockCount = 36;
    readonly int playerCount = 3;

    [SerializeField]
    private Pyramid_Board tempBoard;

    [SerializeField]
    private Transform boardParent;

    [SerializeField]
    private Pyramid_Block tempBlock;

    [SerializeField]
    private Transform blockParent;

    [SerializeField]
    private SpriteAtlas blockAtlas;

    List<Pyramid_Board> boards = new List<Pyramid_Board>();
    List<Pyramid_Player> players = new List<Pyramid_Player>();

    private void Awake()
    {
        instance = this;
        cachedTransform = transform;
    }

    private void Start()
    {
        SetBoards();
        SetPlayers();
        SuffleAndGiveBlocks();
    }

    void SetBoards()
    {
        int currentHeight = maxHeight;

        for (int y = 0; y < maxHeight; y++)
        {
            for (int x = 0; x < currentHeight; x++)
            {
                Pyramid_Board board = Instantiate<Pyramid_Board>(tempBoard, boardParent);
                board.Init(new Vector2(x, y));
                boards.Add(board);
            }

            currentHeight--;
        }
    }

    void SetPlayers()
    {

    }

    void SuffleAndGiveBlocks()
    {
        List<int> startBlockIdxs = new List<int>();
        for(int i = 0; i < totalBlockCount; i++)
        {
            startBlockIdxs.Add(i);
        }

        //조커1
        //5종류 종류별로 7개. 3명한테 분배.

        List<int>[] playersBlocks = new List<int>[playerCount];
        
        int startBlockCountEach = totalBlockCount / playerCount;
        for (int i = 0; i < playerCount; i++)
        {
            playersBlocks[i] = new List<int>();
            for(int j = 0; j < startBlockCountEach; j++)
            {
                int randomIdx = Random.Range(0, startBlockIdxs.Count);
                int blockNum = startBlockIdxs[randomIdx];
                playersBlocks[i].Add(blockNum);
                startBlockIdxs.RemoveAt(randomIdx);

                if (i == 0)
                {
                    Pyramid_BlockType blockType = GetBlockTypeByStartIndex(blockNum);
                    Pyramid_Block block = tempBlock.Spawn(blockParent, new Vector2(0, 0));
                    block.cachedTransform.localScale = Vector3.one;
                    block.cachedTransform.localPosition = new Vector3(0, 0, 0);
                    block.Init(blockType, GetBlockImage(blockType));
                }

                //Debug.Log(blockNum + " / type = " + GetBlockTypeByStartIndex(blockNum));
            }
            //Debug.Log("player" + i + ". blocks = " + string.Join(",", playersBlocks[i].ToArray()));
        }


    }

    Pyramid_BlockType GetBlockTypeByStartIndex(int startIdx)
    {
        return (Pyramid_BlockType)((startIdx / 7) + 1);
    }


    public Sprite GetBlockImage(Pyramid_BlockType blockType)
    {
        return blockAtlas.GetSprite(blockType.ToString());
    }
}
