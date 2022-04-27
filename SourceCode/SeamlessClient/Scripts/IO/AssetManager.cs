using System;
using System.Collections;
using System.Collections.Generic;
using UI.BaseClass;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace IO
{
    public class AssetManager : MonoBehaviour
    {
        [SerializeField]
        private UIAssets _uiAssets;
        [SerializeField]
        private BulletAssets _bulletAssets;
        [SerializeField]
        private WeaponAssets _weaponAssets;
        [SerializeField]
        private ItemManager _itemManager;

        public ItemManager ItemManager { get => _itemManager; }

        public GameObject GetWeapon(WeaponType type)
        {
            _weaponAssets.typeObjs.TryGetValue(type, out var getValue);
            return getValue;
        }
        public UIController GetUIPanelPrefab(UIType type, Transform canves)
        {
            _uiAssets.typeObjs.TryGetValue(type, out var gameObject);
            var result = GameObject.Instantiate(gameObject).GetComponent<UIController>();
            if (result.isOverLay)
            {
                result.transform.SetParent(canves.GetChild(2), false);
            }
            else
                result.transform.SetParent(canves.GetChild(1), false);
            return result;
        }
        public BulletBase CreateBullet(BulletType type, Transform root)
        {
            _bulletAssets.typeObjs.TryGetValue(type, out var gameObject);
            return GameObject.Instantiate(gameObject, root).GetComponent<BulletBase>();
        }
        public void LoadAsyncAsset<T>(string path, Action<ResourceRequest> callback) where T : UnityEngine.Object
        {
            StartCoroutine(LoadAsyncAssetCoroutine<T>(path, callback));
        }
        private IEnumerator LoadAsyncAssetCoroutine<T>(string path, Action<ResourceRequest> callback) where T : UnityEngine.Object
        {
            var r = Resources.LoadAsync<T>(path);
            yield return r;
            callback?.Invoke(r);
        }
    }

}