using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static Action onConnectToNetwork;
    public static Action onCreateRoom;
    public static Action<string> onJoinRoom;
    public static Action onLeaveRoom;
    public static Func<bool> onGetIsConnected;
    
    public static Action onClearLine;
    public static Action onCalibrate;

    public static Action onNetworkConnected;

    #region NETWORKING
    public static Action<ulong, int, List<Vector3>> onDrawingLine;
    public static Action<ulong> onPhoneSideShaked;
    public static Action<ulong, float> onPhoneTilted;
    public static Action<ulong, byte[], Vector2> onCapturedPhotoSent;
    public static Action<ulong, string, Vector2> onEncodedCapturedPhotoSent;
    public static Action<ulong, Vector3, Vector3> onGoalKeeperPositionUpdated;
    public static Action<ulong> onShootAnimationPlayed;
    public static Action<ulong, Vector3> onFootballUpdated;
    public static Action onPhotoCaptured;

    public static Action onUseCameraSelected;
    public static Action<ulong, bool> onStrikerSelected;
    public static Action<ulong, bool> onGoalKeeperSelected;
    public static Action<ulong> onShootTimerStarted;
    public static Action<ulong> onShootTimerEnded;
    public static Action<ulong> onNextRoundStarted;
    public static Action<ulong, int, int> onScoreUpdated;
    public static Action<FootballPlayerData> onFootballDataSent;
    public static Action<ulong> onResetMatchStarted;
    public static Action<ulong, int> onPartOfDayChanged;
    public static Action<ulong, int> onWeatherChanged;

    public static Action<ulong, ulong> onOtherPlayerDisconnected;

    #endregion

    public static Action<int> onControlTypeSelected;

    public static Action onDebugButtonClicked;
    public static Action<string> sendNetworkMessage;
    

}
