using Naninovel;
using Naninovel.UI;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Audio;
using Object = UnityEngine.Object;

[CommandAlias("changeGM")]
public class ChangeGameMode : Command
{
    private static SavingVariable saveVariable;

    [ParameterAlias("script")] public StringParameter ScriptName;
    [ParameterAlias("label")] public StringParameter Label;
    [ParameterAlias("save")] public StringParameter Save;

    public override async UniTask ExecuteAsync(AsyncToken asyncToken = default)
    {
        bool novelEnable = Assigned(ScriptName);

        Camera naniCamera = Engine.GetService<ICameraManager>().Camera;
        IInputManager inputManager = Engine.GetService<IInputManager>();
        IScriptPlayer scriptPlayer = Engine.GetService<IScriptPlayer>();
        ICustomVariableManager customVariableManager = Engine.GetService<ICustomVariableManager>();

        inputManager.ProcessInput = novelEnable;
        naniCamera.enabled = novelEnable;

        if(novelEnable){
            if(!SavingVariable.IsNull(saveVariable))
                customVariableManager.SetVariableValue(saveVariable.Name,saveVariable.Value);

            if(ScriptName.HasValue)
                await scriptPlayer.PreloadAndPlayAsync(ScriptName, label: Label);
        }
        else{
            IUIManager uiManager = Engine.GetService<IUIManager>();
            IActorManager actorManager = Engine.GetService<IActorManager>();
            ICharacterManager characterManager = Engine.GetService<ICharacterManager>();

            foreach(ICharacterActor character in characterManager.GetAllActors())
                character.Visible = false;

            foreach(IManagedUI manage in uiManager.GetManagedUIs())
                manage.Visible = false;

            // foreach(IActor actor in actorManager.GetAllActors())
                // actor.Visible = false;

            if(Save.HasValue && customVariableManager.VariableExists(Save.Value)){
                saveVariable = new SavingVariable(Save.Value, customVariableManager.GetVariableValue(Save.Value));
            }

            MiniGameStarter starter = Object.FindObjectOfType<MiniGameStarter>();
            scriptPlayer.Stop();
            starter.StartGame();
        }
    }

    private readonly struct SavingVariable
    {
        public readonly string Name;
        public readonly string Value;

        public SavingVariable(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public static bool IsNull(SavingVariable variable)
            => string.IsNullOrEmpty(variable.Name) && string.IsNullOrEmpty(variable.Value);
    }
}