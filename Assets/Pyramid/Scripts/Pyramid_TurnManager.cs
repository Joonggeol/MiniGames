using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

public enum Turn
{
    Player,
    Computer1,
    Computer2,
    Max,
}

public class Pyramid_TurnManager : MonoBehaviour
{
    public static Pyramid_TurnManager instance;
    public static readonly float limitTime = 10f;

    bool timerStart = false;
    float currentTime = 0;

    List<Pyramid_Player> players = new List<Pyramid_Player>();

    ReactiveProperty<int> timerReactiveProperty = new ReactiveProperty<int>();

    public ReadOnlyReactiveProperty<int> CurrentTime { get { return timerReactiveProperty.ToReadOnlyReactiveProperty(); } }

    public Turn CurrentTurn { private set; get; }

    private void Awake()
    {
        instance = this;
    }

    public void Init(List<Pyramid_Player> players)
    {
        CurrentTurn = Turn.Player;
        this.players = players;
        timerStart = false;

        //CheckGameEndOrNextTurn();
        GetSituatableTypes();
        StartCoroutine(ShowTurnAnimAndChangeNextTurnRoutine());
        Pyramid_UIManager.instance.UpdateGameCount();
    }

    void SetTimer()
    {
        Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1))
         .Select(x => (int)(limitTime - x))
         .Subscribe(x => timerReactiveProperty.Value = x--); //.AddTo(gameObject);

        //Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1))
        //    .Select(x => (int)(limitTime - x))
        //    .Subscribe( x => timerReactiveProperty.Value--); //.AddTo(gameObject);
    }

    public void Restart()
    {
        Debug.Log("TurnManager Restart");
        players[(int)CurrentTurn].EndTurn();
        CurrentTurn = Turn.Player;
        timerStart = false;

        //CheckGameEndOrNextTurn();
        GetSituatableTypes();
        StartCoroutine(ShowTurnAnimAndChangeNextTurnRoutine());
        Pyramid_UIManager.instance.UpdateGameCount();
    }
    
    private void Update()
    {
        //타이머 & Unirx 여기 적용해보기.
        if (timerStart)
        {
            Pyramid_UIManager.instance.UpdateTimer(currentTime);

            currentTime += Time.deltaTime * Time.timeScale;
            if(currentTime >= limitTime)
            {
                timerStart = false;
                players[(int)CurrentTurn].SituateRandomBlockByTimeOver();
            }
        }
    }
    
    public void NextTurn()
    {
        timerStart = false;
        ResetTimer();

        players[(int)CurrentTurn].EndTurn();

        CurrentTurn++;

        if (CurrentTurn >= Turn.Max)
            CurrentTurn = Turn.Player;

        CheckGameEndOrNextTurn();
    }

    void CheckGameEndOrNextTurn()
    {
        GetSituatableTypes();

        if (CheckGameEnd())
        {
            //Debug.Log("CheckGameEndOrNextTurn  " + Pyramid_Main.instance.CurrentGameCount + " / " + Pyramid_Main.instance.GameCycleCount);
            if(Pyramid_Main.instance.CurrentGameCount < Pyramid_Main.instance.GameCycleCount)
            {
                Pyramid_Main.instance.NextGame();
            }
            else
            {
                Pyramid_UIManager.instance.ShowGameOver();
            }
        }
        else
        {
            //Debug.Log("Next Turn");
            StartCoroutine(ShowTurnAnimAndChangeNextTurnRoutine());
        }
    }

    HashSet<Pyramid_BlockType> situatableTypes = new HashSet<Pyramid_BlockType>();
    void GetSituatableTypes()
    {
        situatableTypes = Pyramid_Main.instance.GetSituatableTypes();
    }

    bool CheckGameEnd()
    {
        //Debug.Log("CheckGameEnd");
        if (situatableTypes.Count > 0)
        {
            for(int i = 0; i < players.Count; i++)
            {
                if (players[i].HasSituatableBlock(situatableTypes))
                    return false;
            }
            
            return true;
        }

        return false;
    }

    IEnumerator ShowTurnAnimAndChangeNextTurnRoutine()
    {
        bool isAnimating = true;
        Pyramid_UIManager.instance.ShowCurrentTurn(CurrentTurn, () => { isAnimating = false; });

        yield return new WaitUntil(() => isAnimating == false);

        ResetTimer();
        timerStart = true;
        //SetTimer();

        if (situatableTypes.Count > 0)
        {
            if (players[(int)CurrentTurn].HasSituatableBlock(situatableTypes))
            {
                players[(int)CurrentTurn].SetRandomTypes(situatableTypes);

                if (CurrentTurn != Turn.Player)
                    yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 2f));

                //Debug.Log("TurnManager ShowTurnAnimAndChangeNextTurnRoutine.CurrentTurn : " + CurrentTurn);
                players[(int)CurrentTurn].StartTurn();
            }
            else
            {
                NextTurn();
                Debug.Log("Skip Turn");
            }
        }
    }

    void ResetTimer()
    {
        currentTime = 0f;
    }

}
