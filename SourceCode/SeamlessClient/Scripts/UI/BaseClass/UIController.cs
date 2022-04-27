using UnityEngine;

namespace UI.BaseClass
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIController : MonoBehaviour, IGetAssetsType<UIType>
    {
        public UIType type;
        public bool isOverLay = false;
        [SerializeField] protected UIManager uiManager;
        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] protected RectTransform rectTrans;
        public abstract void OnOpen();
        public abstract void OnClose();
        public virtual void RegisterUIManager(UIManager uiManager)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            rectTrans = GetComponent<RectTransform>();
            this.uiManager = uiManager;
        }

        UIType IGetAssetsType<UIType>.GetAssetsType()
        {
            return type;
        }
    }
}
