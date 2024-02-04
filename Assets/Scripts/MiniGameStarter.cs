using DTT.MinigameMemory;
using Naninovel;
using Naninovel.Commands;
using Naninovel.UI;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class MiniGameStarter : MonoBehaviour
{
    private const string QuestVariableName = "QuestDescription";
    [SerializeField] private Camera cameraForMiniGame;
    [SerializeField] private MemoryGameManager memoryGameManager;
    [SerializeField] private MemoryGameSettings memoryGameSetting;

    private ICustomVariableManager variableManager;
    private string lastLog;

    private void Start()
    {
        variableManager ??= Engine.GetService<ICustomVariableManager>();
        variableManager.OnVariableUpdated += HandlerCustomVariable;
    }

    private void HandlerCustomVariable(CustomVariableUpdatedArgs variable)
    {
        if(string.Equals(variable.Name, QuestVariableName))
            SaveLog(variable.Value);
    }

    private void OnEnable()
    {
        memoryGameManager.Finish += EndGame;
    }

    private void OnDisable()
    {
        memoryGameManager.Finish -= EndGame;
    }

    public void StartGame()
    {
        cameraForMiniGame.enabled = true;
        memoryGameManager.StartGame(memoryGameSetting);
    }

    public void SaveLog(string value)
    {
        if(!string.IsNullOrEmpty(lastLog)){
            var manageUI = Engine.GetService<UIManager>();
            manageUI.GetUI<BacklogPanel>().AddMessage($"Quest Completed: {lastLog}");
        }

        lastLog = value;
    }

    private void EndGame(MemoryGameResults memoryGameResults)
    {
        if(memoryGameManager == null)
            throw new NullReferenceException("Memory Game Manager is not found");
        cameraForMiniGame.enabled = false;
        new ChangeGameMode{ ScriptName = "Location2", Label = "AfterMiniGame" }.ExecuteAsync().Forget();
    }
}