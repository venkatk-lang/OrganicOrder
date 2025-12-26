using UnityEditor;
using UnityEngine;
using System.Linq;
namespace IACGGames
{
    [CustomEditor(typeof(AudioLibrary))]
    public class AudioLibraryEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            AudioLibrary lib = (AudioLibrary)target;

            EditorGUILayout.LabelField("Mixer Groups", EditorStyles.boldLabel);
            lib.sfxAudioMixerGroup = (UnityEngine.Audio.AudioMixerGroup)
                EditorGUILayout.ObjectField("SFX Mixer", lib.sfxAudioMixerGroup, typeof(UnityEngine.Audio.AudioMixerGroup), false);

            lib.bgmAudioMixerGroup = (UnityEngine.Audio.AudioMixerGroup)
                EditorGUILayout.ObjectField("BGM Mixer", lib.bgmAudioMixerGroup, typeof(UnityEngine.Audio.AudioMixerGroup), false);

            EditorGUILayout.Space(15);

            EditorGUILayout.LabelField("SFX Entries", EditorStyles.boldLabel);

            SyncEnumWithList<SFXAudioID, SFXAudio>(
                lib.sfxEntries,
                id => new SFXAudio { id = id, volume = 1f }
            );

            DrawSFXEntries(lib.sfxEntries);

            EditorGUILayout.Space(20);

            EditorGUILayout.LabelField("BGM Entries", EditorStyles.boldLabel);

            SyncEnumWithList<BGMAudioID, BGMAudio>(
                lib.bgmEntries,
                id => new BGMAudio { id = id, volume = 1f }
            );

            DrawBGMEntries(lib.bgmEntries);

            if (GUI.changed)
                EditorUtility.SetDirty(lib);
        }

        private void SyncEnumWithList<TEnum, TEntry>(
            System.Collections.Generic.List<TEntry> list,
            System.Func<TEnum, TEntry> createFunc)
            where TEnum : System.Enum
            where TEntry : class
        {
            var enumValues = System.Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList();

            list.RemoveAll(entry =>
            {
                var entryID = (TEnum)typeof(TEntry).GetField("id").GetValue(entry);
                return !enumValues.Contains(entryID);
            });

            foreach (var id in enumValues)
            {
                bool exists = list.Any(entry =>
                {
                    var entryID = (TEnum)typeof(TEntry).GetField("id").GetValue(entry);
                    return entryID.Equals(id);
                });

                if (!exists)
                    list.Add(createFunc(id));
            }

            // Sort by enum order
            list.Sort((a, b) =>
            {
                var idA = (TEnum)typeof(TEntry).GetField("id").GetValue(a);
                var idB = (TEnum)typeof(TEntry).GetField("id").GetValue(b);
                return idA.CompareTo(idB);
            });
        }

        private void DrawSFXEntries(System.Collections.Generic.List<SFXAudio> entries)
        {
            foreach (var entry in entries)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.EnumPopup(entry.id, GUILayout.Width(120));
                EditorGUI.EndDisabledGroup();

                entry.clip = (AudioClip)EditorGUILayout.ObjectField(entry.clip, typeof(AudioClip), false);
                entry.volume = EditorGUILayout.Slider(entry.volume, 0f, 1f, GUILayout.Width(160));

                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawBGMEntries(System.Collections.Generic.List<BGMAudio> entries)
        {
            foreach (var entry in entries)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.EnumPopup(entry.id, GUILayout.Width(120));
                EditorGUI.EndDisabledGroup();

                entry.clip = (AudioClip)EditorGUILayout.ObjectField(entry.clip, typeof(AudioClip), false);
                entry.volume = EditorGUILayout.Slider(entry.volume, 0f, 1f, GUILayout.Width(160));

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}