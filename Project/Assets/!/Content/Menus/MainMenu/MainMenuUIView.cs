using System;
using Core.Events;
using Core.Initialization;
using Core.View.UI;
using Global;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Menus.MainMenu
{
    public class MainMenuUIView : UIView<MainMenuMessenger>
    {
        [SerializeField] private Button PlayButton;
        
        [Inject] private readonly EventQueue EventQueue = null!;
        [Inject] private readonly UIRoot UIRoot = null!;
        [Inject] private readonly ScenesController ScenesController = null!;
        
        private void OnPlayButtonClicked()
        {
            Messenger.PlayButtonClick();

            // EventQueue.Execute<MainMenuToGameEvent>();
            // MainMenuPlayButton.onClick.RemoveListener(OnPlayButtonClicked);
        }
        
        protected override void OnShow()
        {
            PlayButton.onClick.AddListener(OnPlayButtonClicked);
        }


        protected override void OnHide()
        {
            PlayButton.onClick.RemoveListener(OnPlayButtonClicked);
        }

        protected override void OnUnregister(MainMenuMessenger uiMessenger) { }
        protected override void OnRegister(MainMenuMessenger uiMessenger) { }
    }
    
    public sealed class MainMenuMessenger : UIMessenger
    {
        public event Action OnPlayButtonClicked;
        
        public void PlayButtonClick()
        {
            OnPlayButtonClicked?.Invoke();
        }
    }
}
