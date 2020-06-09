using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using System.Linq;

public class GameObjectBase : MonoBehaviour
{
    public Transform cachedTransform { protected set; get; }

    protected void Awake()
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

    //[SerializeField]
    //private Transform blockParent;

    [SerializeField]
    private SpriteAtlas blockAtlas;

    List<Pyramid_Board> boards = new List<Pyramid_Board>();
    List<Pyramid_Player> players = new List<Pyramid_Player>();

    //public SpriteAtlas BlockAtlas { get { return blockAtlas; } }

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

        List<int>[] playerBlockIdxs = new List<int>[playerCount];
        List<Pyramid_BlockType>[] typeList = new List<Pyramid_BlockType>[playerCount];

        int startBlockCountEach = totalBlockCount / playerCount;
        for (int i = 0; i < playerCount; i++)
        {
            playerBlockIdxs[i] = new List<int>();
            typeList[i] = new List<Pyramid_BlockType>();
            for (int j = 0; j < startBlockCountEach; j++)
            {
                int randomIdx = Random.Range(0, startBlockIdxs.Count);
                int blockNum = startBlockIdxs[randomIdx];
                playerBlockIdxs[i].Add(blockNum);
                startBlockIdxs.RemoveAt(randomIdx);

                typeList[i].Add(GetBlockTypeByStartIndex(blockNum));
                //Debug.Log(blockNum + " / type = " + GetBlockTypeByStartIndex(blockNum));
            }
            //Debug.Log("player" + i + ". blocks = " + string.Join(",", playersBlocks[i].ToArray()));

            if (i == 0)
            {
                Pyramid_UIManager.instance.SetPlayerBlocks(typeList[i]);
            }
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

    public Sprite GetSelectedBlockImage(Pyramid_BlockType blockType)
    {
        return blockAtlas.GetSprite(blockType.ToString() + "_Selected");
    }

    public void Restart()
    {
        SuffleAndGiveBlocks();
    }

    public void OnBlockSelected(Pyramid_BlockType blockType)
    {
        ShowPossibleLocation(blockType);
    }

    void ShowPossibleLocation(Pyramid_BlockType blockType)
    {
        //맨위에 보드부터 재귀로 체크하는것 만들자.
        // 바로밑 양옆 보드중 하나라도 블럭이 없으면 밑으로 내려감.
        // 둘다 채워져 있고 색이 같으면 해당 색. 다른색이면 둘 다에 반응..

        List<Pyramid_Board> possibleBoards = boards.Where(board => board.Block == null && board.Index.y == 0).ToList();

        for(int i = 0; i < possibleBoards.Count; i++)
        {
            possibleBoards[i].ShowCanSelect(()=> 
            {
                for (int j = 0; j < possibleBoards.Count; j++)
                {
                    possibleBoards[j].HideCanSelect();
                }

                Pyramid_UIManager.instance.OnBlockSituated();
            });
        }
    }
}
