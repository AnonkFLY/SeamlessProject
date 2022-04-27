using System.Diagnostics;
using System.Collections.Generic;
using DG.Tweening;
using IO;
using UI.BaseClass;
using UI.BaseClass.Interface;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace UI
{
    public class UIManager
    {
        private int _topIndex;
        private List<UIController> _uiPanelList;
        private Dictionary<UIType, UIController> _uiPanelAssets;
        private AssetManager _assetManager;
        private CanvasGroup _backImage;
        private Transform _canvesTrans;
        public UnityAction<UIController, UIController, bool> onTopChange; //old top panel and new top panel gameObject

        public List<UIController> UiPanelList { get => _uiPanelList; set => _uiPanelList = value; }

        public UIManager(AssetManager assetManager)
        {
            _canvesTrans = GameObject.FindObjectOfType<Canvas>().transform;
            _uiPanelList = new List<UIController>();
            _uiPanelAssets = new Dictionary<UIType, UIController>();
            _topIndex = _uiPanelList.Count - 1;
            _assetManager = assetManager;
            _backImage = _canvesTrans.GetChild(0).GetComponent<CanvasGroup>();

            onTopChange = new UnityAction<UIController, UIController, bool>(OnGetEvent);
        }
        public void OpenUI(UIType type, object data = null)
        {
            GameManager.Instance.OnMainThreadExecute(() =>
            {
                var openPanel = GetUIInstance(type);
                if (!openPanel)
                    UnityEngine.Debug.LogError("Failed to load ui" + type);
                if (data != null)
                    openPanel.GetComponent<IRequirementData>()?.InComingData(data);
                if (_topIndex >= 0)
                    onTopChange?.Invoke(_uiPanelList[_topIndex], openPanel, true);//OnMainThreadExecute(() => {  });
                else
                    onTopChange?.Invoke(null, openPanel, true);//OnMainThreadExecute(() => {  });
                PushUI(openPanel);
            });
        }
        public void CloseUI()
        {
            if (_topIndex <= -1)
                return;
            var topPanel = PopUI(_topIndex);
            //            UnityEngine.Debug.Log(_topIndex);
            if (_topIndex >= 0)
                onTopChange?.Invoke(topPanel, _uiPanelList[_topIndex], false);//OnMainThreadExecute(() => {  });
            else
                onTopChange?.Invoke(topPanel, null, false);//OnMainThreadExecute(() => {  });
        }
        public void CloseUI(UIController uIController)
        {
            var index = _uiPanelList.IndexOf(uIController);
            if (index == -1)
                return;
            if (index == _topIndex)
            {
                CloseUI();
                return;
            }
            var topPanel = PopUI(index);
            onTopChange?.Invoke(topPanel, null, false);
        }
        public void CloseUI(UIType type)
        {
            if (_uiPanelAssets.TryGetValue(type, out var getPanel))
                CloseUI(getPanel);
        }
        private UIController GetUIInstance(UIType type)
        {
            //Find Instance
            if (_uiPanelAssets.TryGetValue(type, out var panel))
                return panel;
            //if null then load
            panel = InstancePanel(type);
            return panel;
        }
        public UIController InstancePanel(UIType type)
        {
            var panel = _assetManager.GetUIPanelPrefab(type, _canvesTrans);
            _uiPanelAssets.Add(type, panel);
            panel.RegisterUIManager(this);
            return panel;
        }
        public T GetUIController<T>(UIType type) where T : UIController
        {
            return (T)GetUIInstance(type);
        }
        public DG.Tweening.Core.TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> SetBackOpen(bool isOpen, float timer = 1)
        {
            var alpha = isOpen ? 1 : 0;
            return _backImage.DOFade(alpha, timer);
        }
        public void CloseAllUI()
        {
            while (_topIndex >= 0)
            {
                CloseUI();
            }
        }
        private void PushUI(UIController panel)
        {
            //UnityEngine.Debug.Log($"打开了{panel.type}");
            panel.OnOpen();
            panel.transform.SetAsLastSibling();
            var index = _uiPanelList.IndexOf(panel);
            if (index != -1)
            {
                _uiPanelList.RemoveAt(index);
                _uiPanelList.Add(panel);
                return;
            }
            _uiPanelList.Add(panel);
            _topIndex++;
        }
        private UIController PopUI(int index)
        {
            if (index <= -1)
                return null;
            var topPanel = GetUIController(index);
            topPanel.OnClose();
            _uiPanelList.RemoveAt(index);
            _topIndex--;
            return topPanel;
        }
        private UIController GetUIController(int index)
        {
            if (index < 0)
                return null;
            return _uiPanelList[index];
        }


        private void OnGetEvent(UIController oldPanel, UIController newPanel, bool open)
        {
            if (open)
            {
                oldPanel?.GetComponent<ICoverable>()?.OnPause();
            }
            else
            {
                newPanel?.GetComponent<ICoverable>()?.OnRenew();
            }
        }
        public static void DefaultOC(CanvasGroup canvasGroup, bool open, float timer = 0)
        {

            var value = open ? 1 : 0;
            canvasGroup.interactable = open;
            canvasGroup.blocksRaycasts = open;
            if (timer == 0)
            {
                canvasGroup.alpha = value;
            }
            else
                canvasGroup.DOFade(value, timer);
            // GameManager.Instance.OnMainThreadExecute(() =>
            // {
            //     var value = open ? 1 : 0;
            //     canvasGroup.interactable = open;
            //     canvasGroup.blocksRaycasts = open;
            //     if (timer == 0)
            //     {
            //         canvasGroup.alpha = value;
            //     }
            //     else
            //         canvasGroup.DOFade(value, timer);
            // });
        }
    }
}