using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;


namespace CUI.UI
{

    [AddComponentMenu("UI/CustomButton", 30)]
    public class CustomButton : Selectable, IPointerClickHandler, ISubmitHandler, IDisposable, IPoolItem<object>, IEnterTrigger
    {
        public delegate void VoidDelegateObj(CustomButton go);

        #region 按钮通用信息 
        // public CustomButtonGroup btnGroup;
        public bool isFindInParents;
        public int btnIndex = 0;//按键编号 
        public Text txtLabel;
        private Transform _lockNode;
        public Transform LockNode
        {
            get
            {
                if (_lockNode == null)
                    _lockNode = transform.Find("locknode");
                return _lockNode;
            }
        }
        // private UI_RedTip _RedTip;
        // public UI_RedTip RedTip
        // {
        //     get
        //     {
        //         if (_RedTip == null)
        //         {
        //             Transform redtNode = transform.Find("Redpoint");
        //             if (redtNode != null)
        //                 _RedTip = UGUITool.AddMissComponent<UI_RedTip>(redtNode.gameObject);
        //             else
        //                 Log.Error("未绑定红点:" + goName);
        //         }
        //         return _RedTip;
        //     }
        // }

        public string goName { get { return gameObject.name; } }//按键名称  
        public object Data { set; get; }
        [Space(10)]
        public uint ClickInterval = 0;
        public uint LongPressTime = 0;
        private uint m_LongPressTimer = 0;
        public VoidDelegateObj onClickCustom;
        public Action btnOnClickNewGuide;
        public VoidDelegateObj onLongPress;
        public VoidDelegateObj onPressDown;
        public VoidDelegateObj onPressUp;
        public Action OnClickFailed = null;
        [HideInInspector]
        public bool IsNormalSound = true;


        private bool m_IsLock;
        public bool IsLock
        {
            get
            {
                return m_IsLock;
            }
            set
            {
                SetLock(value);
                m_IsLock = value;
            }
        }

        public virtual void SetLabel(string name)
        {
            if (txtLabel)
                txtLabel.text = name;
        }



        public virtual void SetIcon(Sprite spr)
        {
            if (image)
            {
                image.sprite = spr;
            }
        }

        public virtual void SetLock(bool islock)
        {
            interactable = !islock;
        }

        // public void SetGroup(CustomButtonGroup group)
        // {
        //     btnGroup = group;
        //     btnGroup.RegisterBtn(this);
        // }
        #endregion






        protected CustomButton()
        { }

        protected override void Awake()
        {
            base.Awake();
            // if (btnGroup == null && isFindInParents)
            //     btnGroup = GetComponentInParent<CustomButtonGroup>();
            // if (btnGroup != null)
            //     btnGroup.RegisterBtn(this);
        }

        public void Press()
        {
            if (!IsActive() || !IsInteractable())
            {
                OnClickFailed?.Invoke();
                return;
            }


            UISystemProfilerApi.AddMarker("CustomButton.onClick", this);
            DoClickAction();

        }

        // Trigger all registered callbacks.
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            //Press();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (LongPressTime > 0)
            {
                m_LongPressTimer = TimerHeap.AddTimer(LongPressTime, 0, () =>
                {
                    m_LongPressTimer = 0;
                    if (onLongPress != null)
                        onLongPress(this);
                });
            }
            else
            {
                Press();
            }
            if (onPressDown != null)
                onPressDown(this);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (LongPressTime > 0)
            {
                if (m_LongPressTimer == 0) return;
                TimerHeap.DelTimer(m_LongPressTimer);
                Press();
            }
            if (onPressUp != null)
                onPressUp(this);
        }

        protected virtual void DoClickAction()
        {
            // if (!IsLock)
            // {
            //     //专门为新手指引干的事
            //     btnOnClickNewGuide?.Invoke();
            //     if (onClickCustom != null)
            //         onClickCustom.Invoke(this);
            //     if (btnGroup != null)
            //         btnGroup.OnNotifyClick(this);
            //     if (ClickInterval > 0)
            //     {
            //         IsLock = true;
            //         TimerHeap.AddTimer(ClickInterval, 0, () =>
            //         {
            //             if (this != null)
            //                 IsLock = false;
            //         });
            //     }
            //     //播放音效
            //     if (IsNormalSound)
            //         ToolKit.PlayUISound(1);
            // }
        }


        public virtual void OnSubmit(BaseEventData eventData)
        {
            Press();

            // if we get set disabled during the press
            // don't run the coroutine.
            if (!IsActive() || !IsInteractable())
                return;

            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(OnFinishSubmit());
        }

        private IEnumerator OnFinishSubmit()
        {
            float fadeTime = colors.fadeDuration;
            float elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }

        public override bool IsInteractable()
        {
            return base.IsInteractable();
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
        }

        protected override void OnDestroy()
        {
            Dispose();
            base.OnDestroy();
        }

        public void Show(bool isShow)
        {
            gameObject.SetActive(isShow);
        }

        public void Dispose()
        {
            this.image = null;
            this.targetGraphic = null;
            onClickCustom = null;
            btnOnClickNewGuide = null;
            // if (btnGroup)
            //     btnGroup.UnregisterBtn(this);
        }

        public void BindData(object data)
        {
            SetLabel(data as string);
        }

        public void SetEnterState(EnterTriggerState state, Action clickfailed)
        {
            if (state == EnterTriggerState.Hide)
            {
                Show(false);
                OnClickFailed = null;
                if (LockNode != null)
                    LockNode.gameObject.SetActive(false);
            }
            else if (state == EnterTriggerState.Disable)
            {
                Show(true);
                OnClickFailed = clickfailed;
                interactable = false;
                if (LockNode != null)
                    LockNode.gameObject.SetActive(true);
            }
            else
            {
                Show(true);
                OnClickFailed = null;
                interactable = true;
                if (LockNode != null)
                    LockNode.gameObject.SetActive(false);
            }
        }
    }


}
