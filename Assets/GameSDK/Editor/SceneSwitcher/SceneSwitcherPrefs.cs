using UnityEditor;
namespace IACGGames
{
    public static class SceneSwitcherPrefs
    {
        const string Key = "SceneSwitcher_FetchAllScenes";

        public static bool FetchAllScenes
        {
            get => EditorPrefs.GetBool(Key, false);
            set => EditorPrefs.SetBool(Key, value);
        }
    }
}