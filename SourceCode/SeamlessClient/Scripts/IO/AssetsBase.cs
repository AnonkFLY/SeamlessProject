using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetsBase<T> : ScriptableObject
{
    [SerializeField]
    GameObject[] objs;
    public Dictionary<T, GameObject> typeObjs = new Dictionary<T, GameObject>();
    private void OnEnable()
    {

        for (int i = 0; i < objs.Length; i++)
        {
            var type = objs[i].GetComponent<IGetAssetsType<T>>();
            if (!typeObjs.ContainsKey(type.GetAssetsType()))
                typeObjs.Add(type.GetAssetsType(), objs[i]);
        }

    }
}
