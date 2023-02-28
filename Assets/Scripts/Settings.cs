using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public enum NetworkType
    {
        NetCode,
        PhotonPUN
    }

    public static NetworkType networkType;

}
