using System.Collections.Generic;
using UnityEngine;

namespace IACGGames.UISystem
{
    public enum UIState
    {
        MainMenu,
        GameHUD,
        LevelComplete
    }
    public class UIManager : Singleton<UIManager>
    {
        [Header("Panels (assign inspector)")]
        public GameHUD gameHUD;
        public MainMenu mainMenu;
        public LevelCompletePanel LevelComplete;
        private Dictionary<UIState, UIPanelBase> panels;
        private UIState currentState;
        protected override void Awake()
        {
            base.Awake();

            panels = new Dictionary<UIState, UIPanelBase>()
        {
            { UIState.MainMenu, mainMenu },
            { UIState.GameHUD, gameHUD },
            { UIState.LevelComplete, LevelComplete }
        };

            foreach (var item in panels)
            {
                item.Value.gameObject.SetActive(false);
            }
        }

        public void Show(UIState state,float animTime)
        {
          
            if (panels.ContainsKey(currentState))
                panels[currentState].Hide();

            panels[state].Show(animTime);
            currentState = state;
        }
        public void Init() // called from start of GameManager - put gamemanager stript in gamescene   
        {
            Show(UIState.MainMenu, 0);
        }
    }
}

