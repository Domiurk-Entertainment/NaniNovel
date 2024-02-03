using Naninovel;
using UnityEngine;

[CommandAlias("changeGM")]
public class ChangeGameMode : Command
{
    [ParameterAlias(NamelessParameterAlias)]
    public BooleanParameter OnNovel;
    [ParameterAlias("script")] public StringParameter ScriptName;
    [ParameterAlias("cameraName")] public StringParameter CameraName = "MainCamera";

    public override async UniTask ExecuteAsync(AsyncToken asyncToken = default)
    {
        // 2. Switch cameras.
        var advCamera = GameObject.Find(CameraName).GetComponent<Camera>();
        var naniCamera = Engine.GetService<ICameraManager>().Camera;
        var inputManager = Engine.GetService<IInputManager>();
        var scriptPlayer = Engine.GetService<IScriptPlayer>();
        var stateManager = Engine.GetService<IStateManager>();

        inputManager.ProcessInput = OnNovel;
        advCamera.enabled = !OnNovel;
        naniCamera.enabled = OnNovel;

        if(OnNovel){ // On Novel (Novel = true)
            if(Assigned(ScriptName)){
                await scriptPlayer.PreloadAndPlayAsync(ScriptName);
            }
        }
        else{ // Off Novel  (Novel = false)
            scriptPlayer.Stop();

            await stateManager.ResetStateAsync();
        }
    }
}