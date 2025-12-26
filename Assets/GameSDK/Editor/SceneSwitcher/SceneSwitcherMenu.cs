using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;
using System.Linq;
namespace IACGGames
{
    public static class SceneSwitcherMenu
    {
        public static void Show()
        {
            var menu = new GenericMenu();
            var activePath = EditorSceneManager.GetActiveScene().path;

            string[] scenes = SceneSwitcherPrefs.FetchAllScenes
                ? Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories)
                : EditorBuildSettings.scenes
                    .Where(s => s.enabled && File.Exists(s.path))
                    .Select(s => s.path)
                    .ToArray();

            if (scenes.Length == 0)
            {
                menu.AddDisabledItem(new GUIContent("No scenes found"));
                menu.ShowAsContext();
                return;
            }

            foreach (var path in scenes)
            {
                string name = Path.GetFileNameWithoutExtension(path);
                bool isActive = path == activePath;

                menu.AddItem(
                    new GUIContent(name),
                    isActive,
                    () =>
                    {
                        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                            EditorSceneManager.OpenScene(path);
                    });
            }

            menu.ShowAsContext();
        }
    }
}