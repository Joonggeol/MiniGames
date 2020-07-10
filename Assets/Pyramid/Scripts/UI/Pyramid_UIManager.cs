using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UniRx;

public class Pyramid_UIManager : MonoBehaviour
{
    public static Pyramid_UIManager instance;

    [SerializeField]
    Text currentTurnText;
    [SerializeField]
    Text timerText;
    [SerializeField]
    Animator turnAnimator;
    [SerializeField]
    Text turnAnimationText;

    [SerializeField]
    Text gameCountText;

    [SerializeField]
    Text[] scoreTexts;
    [SerializeField]
    Text[] leftBlockCountTexts; // only for computers

    [SerializeField]
    Text rxTimerText;
    

    [SerializeField]
    Pyramid_UIBlock tempUIBlock;
    [SerializeField]
    Transform uiBlockParent;

    [SerializeField]
    Vector2 uiBlockStartPosOffest;

    [SerializeField]
    GameOverPopup gameOverPopup;
    [SerializeField]
    Animator gameEndTextAnim;

    public List<Pyramid_UIBlock> playerBlocks { private set; get; }
    public Pyramid_UIBlock CurrentSelectedBlock { private set; get; }

    public Pyramid_BlockType CurrentSelectedBlockType { private set; get; }
    public bool IsAnimating { private set; get; }

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Pyramid_TurnManager.instance.CurrentTime.SubscribeToText(rxTimerText);
    }

    public void SetPlayerBlocks(List<Pyramid_BlockType> blockTypes)
    {
        playerBlocks = new List<Pyramid_UIBlock>();
        for (int i = 0; i < blockTypes.Count; i++)
        {
            Pyramid_UIBlock block = tempUIBlock.Spawn<Pyramid_UIBlock>(uiBlockParent, new Vector2((i * Pyramid_UIBlock.uiBlockSize.x) + uiBlockStartPosOffest.x , uiBlockStartPosOffest.y));
            block.Init(blockTypes[i], OnBlockSelected);

            playerBlocks.Add(block);
        }
    }

    public void OnRestartClicked()
    {
        if (!IsAnimating)
        {
            RemoveUIBlocks();
            Pyramid_Main.instance.Restart();
            CurrentSelectedBlock = null;
            CurrentSelectedBlockType = Pyramid_BlockType.None;
        }
    }

    public void RemoveUIBlocks()
    {
        for(int i =0; i < playerBlocks.Count; i++)
        {
            playerBlocks[i].Recycle();
        }

        playerBlocks = new List<Pyramid_UIBlock>();
    }
    
    public void NextGame()
    {
        RemoveUIBlocks();
        CurrentSelectedBlock = null;
        CurrentSelectedBlockType = Pyramid_BlockType.None;
    }

    void OnBlockSelected(Pyramid_UIBlock uiBlock)
    {
        CurrentSelectedBlock?.OnDeselected();
        CurrentSelectedBlock = uiBlock;
        CurrentSelectedBlockType = uiBlock.BlockType;

        Pyramid_Main.instance.OnBlockSelected(CurrentSelectedBlockType);
    }

    public void OnBlockSituated(Pyramid_UIBlock selectedBlock)
    {
        playerBlocks.Remove(selectedBlock);
        selectedBlock.Recycle();

        CurrentSelectedBlock?.OnDeselected();
        CurrentSelectedBlock = null;
        CurrentSelectedBlockType = Pyramid_BlockType.None;

        for (int i = 0; i < playerBlocks.Count; i++)
        {
            playerBlocks[i].cachedTransform.localPosition = new Vector2((i * Pyramid_UIBlock.uiBlockSize.x) + uiBlockStartPosOffest.x, uiBlockStartPosOffest.y);
        }
    }

    public void OnStartPlayerTurn(HashSet<Pyramid_BlockType> situatableTypes)
    {
        for(int i = 0; i < playerBlocks.Count; i++)
        {
            if(situatableTypes.Contains(playerBlocks[i].BlockType))
            {
                playerBlocks[i].ShowCanSelect();
            }
        }   
    }

    public void OnEndPlayerTurn()
    {
        for (int i = 0; i < playerBlocks.Count; i++)
        {
            playerBlocks[i].HideCanSelect();
        }
    }

    public void UpdateTimer(float time)
    {
        timerText.text = "Time: " + (Pyramid_TurnManager.limitTime - time).ToString("00");
    }

    public void ShowCurrentTurn(Turn currentTurn, System.Action onAnimEnd)
    {
        StartCoroutine(ShowCurrentTurnRoutine(currentTurn, onAnimEnd));
    }

    IEnumerator ShowCurrentTurnRoutine(Turn currentTurn, System.Action onAnimEnd)
    {
        IsAnimating = true;
        yield return new WaitForSeconds(0.4f);

        turnAnimator.gameObject.SetActive(true);
        turnAnimationText.text = currentTurn.ToString() + "\nTurn"; 
        turnAnimator.Play("ChangeTurn");

        yield return new WaitForSeconds(1f);
        
        turnAnimator.gameObject.SetActive(false);
        currentTurnText.text = currentTurn.ToString();
        onAnimEnd?.Invoke();

        IsAnimating = false;
    }

    public void ShowGameOver()
    {
        StartCoroutine(ShowGameOverRoutine());
    }

    IEnumerator ShowGameOverRoutine()
    {
        gameEndTextAnim.gameObject.SetActive(true);
        gameEndTextAnim.Play("Appear");

        yield return new WaitForSeconds(1f);

        gameEndTextAnim.gameObject.SetActive(false);
        gameOverPopup.Open();
    }

    public void UpdateScore(Turn playerType, int score)
    {
        //Debug.Log(playerType + " UpdateScore");
        scoreTexts[(int)playerType].text = "Score: " + score;
    }

    public void UpdateGameCount()
    {
        gameCountText.text = string.Format("GameCount : {0}/{1}", Pyramid_Main.instance.CurrentGameCount, Pyramid_Main.instance.GameCycleCount);
    }

    public void UpdateLeftBlockCount(Turn playerType, int count)
    {
        switch(playerType)
        {
            case Turn.Computer1:
                leftBlockCountTexts[0].text = "Left Blocks: " + count.ToString();
                break;

            case Turn.Computer2:
                leftBlockCountTexts[1].text = "Left Blocks: " + count.ToString();
                break;
        }
    }
}
