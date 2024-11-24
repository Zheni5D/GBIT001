using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public enum MESSAGE_TYPE
{
    GAME_RESTART,
    ADD_SCORE,
    GAME_OVER,
    TIME_STOP_ON,
    TIME_STOP_OFF,
    LEVEL_CLEAR,
    STAGE_CLEAR,
    MOV_nSTAGE,
    PAUSE_ON,
    PAUSE_OFF,
    PLAYER_GET_HURT,
    FURY_MODE_ON,
    FURY_MODE_OFF
}
public struct CommonMessage
{
    public int Mid;
    public Object content;
    public int intParam;
    public Transform target;
}
public class MessageCenter
{
    public static bool Enabled = true;
    private static System.Action<CommonMessage> _actions;
    //private static delegate Action XX 是错误的

    public static void AddListener( System.Action<CommonMessage> action)
    {
        _actions += action;
    }

    public static void RemoveListner(System.Action<CommonMessage> action)
    {
        _actions -= action;
    }

    public static void SendMessage(CommonMessage msg)
    {
        if(!Enabled)
            return;
        // if(msg.Mid ==(int) MESSAGE_TYPE.GAME_RESTART)
        // {
        //     double t = Time.realtimeSinceStartupAsDouble;
        //     _actions?.Invoke(msg);
        //     Debug.Log(Time.realtimeSinceStartupAsDouble - t);
        // }
        // else
        _actions?.Invoke(msg);
    }

}
