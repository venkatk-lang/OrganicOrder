using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
namespace IACGGames
{
    [Overlay(typeof(SceneView), "Scene Switcher")]
    public sealed class SceneSwitcherOverlay : Overlay
    {
        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement
            {
                style =
            {
                flexDirection = FlexDirection.Row
            }
            };

            // --- Toggle ---
            var allScenesToggle = new ToolbarToggle
            {
                text = "All",
                value = SceneSwitcherPrefs.FetchAllScenes
            };

            allScenesToggle.RegisterValueChangedCallback(evt =>
            {
                SceneSwitcherPrefs.FetchAllScenes = evt.newValue;
            });

            root.Add(allScenesToggle);

            // --- Button ---
            var scenesButton = new ToolbarButton(ShowMenu)
            {
                text = "Scenes"
            };

            root.Add(scenesButton);

            return root;
        }

        void ShowMenu()
        {
            SceneSwitcherMenu.Show();
        }
    }
}