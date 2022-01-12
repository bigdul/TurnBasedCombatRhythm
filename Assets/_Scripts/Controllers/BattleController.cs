using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleController : MonoBehaviour, IGameController
{
    public static BattleController instance;

    public Battle CurrentBattle;
    
    [SerializeField] private List<AgentData> _testAgents;
    private List<BattlePosition> _battlePositions = new List<BattlePosition>();

    // Start is called before the first frame update
    internal void Awake()
    {
        instance = this;
    }

    public void Initialize()
    {
        StartBattle();
        CurrentBattle.SetupNewTurn();
        SubscribeEvents();
    }

    public void SubscribeEvents()
    {

    }

    // Update is called once per frame
    internal void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CurrentBattle.ChangeTurn();
        }
    }

    private void StartBattle()
    {
        //Init test battle
        InitBattle();
        SpawnCharacters();
        RhythmController.EndRhythmEvent += CurrentBattle.ChangeTurn;
    }
    
    ///Private
    //Init a test battle
    private void InitBattle()
    {
        List<Agent> agents = new List<Agent>();
        foreach(var agentData in _testAgents)
        {
            agents.Add(new Agent(agentData));
        }
        CurrentBattle = new Battle(agents);
    }

    //Spawn visuals
    private void SpawnCharacters()
    {
        if(_battlePositions.Count < 1)
            _battlePositions = GameObject.Find("BattlePositionParent").GetComponentsInChildren<BattlePosition>().ToList();

        foreach(var agent in CurrentBattle.BattleAgentsQueue)
        {
            BattlePosition battlePosition;
            GameObject agentObj = Instantiate(PrefabManager.instance.AgentObjectPrefab);
            SpriteRenderer renderer = agentObj.GetComponent<SpriteRenderer>();
            AgentEntity entity = agentObj.GetComponent<AgentEntity>();

            agent.Entity = entity;
            
            if (agent.IsPlayer)
            {
                battlePosition = _battlePositions.First(x => x.IsPlayer && !x.IsTaken);
                renderer.flipX = true;
            }
            else
            {
                battlePosition = _battlePositions.First(x => !x.IsPlayer && !x.IsTaken);
                renderer.flipX = false;
            }

            agentObj.name = agent.Name;
            agentObj.transform.SetParent(battlePosition.transform);
            agentObj.transform.localPosition = Vector3.zero;
            entity.Agent = agent;
            renderer.sprite = agent.Sprite;
            battlePosition.IsTaken = true;
        }
    }
}