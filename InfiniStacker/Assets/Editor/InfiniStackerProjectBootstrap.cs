using System;
using System.IO;
using System.Linq;
using System.Reflection;
using InfiniStacker.Bootstrap;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace InfiniStacker.Editor
{
    [InitializeOnLoad]
    public static class InfiniStackerProjectBootstrap
    {
        private const string GameScenePath = "Assets/Scenes/Game.unity";
        private static bool _ran;

        static InfiniStackerProjectBootstrap()
        {
            EditorApplication.delayCall += RunOnce;
        }

        private static void RunOnce()
        {
            if (_ran)
            {
                return;
            }

            _ran = true;

            EnsureTmpEssentials();
            EnsureInputSystemEnabled();
            EnsurePortraitOrientation();
            EnsureGameSceneExists();
            EnsureBuildSettingsContainsGameScene();
        }

        private static void EnsureTmpEssentials()
        {
            if (TMP_Settings.defaultFontAsset != null)
            {
                return;
            }

            var utilType = Type.GetType("TMPro.EditorUtilities.TMP_PackageUtilities, Unity.TextMeshPro.Editor");
            var importMethod = utilType?.GetMethod("ImportProjectResources", BindingFlags.Public | BindingFlags.Static);
            importMethod?.Invoke(null, null);
            AssetDatabase.Refresh();
        }

        private static void EnsureInputSystemEnabled()
        {
            var settingsAssets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset");
            if (settingsAssets == null || settingsAssets.Length == 0)
            {
                return;
            }

            var serializedObject = new SerializedObject(settingsAssets[0]);
            var inputHandler = serializedObject.FindProperty("activeInputHandler");
            if (inputHandler == null || inputHandler.intValue == 1)
            {
                return;
            }

            // 1 = Input System Package (New)
            inputHandler.intValue = 1;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssets();
        }

        private static void EnsurePortraitOrientation()
        {
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
        }

        private static void EnsureGameSceneExists()
        {
            if (File.Exists(GameScenePath))
            {
                return;
            }

            var sceneDir = Path.GetDirectoryName(GameScenePath);
            if (!string.IsNullOrEmpty(sceneDir) && !Directory.Exists(sceneDir))
            {
                Directory.CreateDirectory(sceneDir);
                AssetDatabase.Refresh();
            }

            var currentScene = EditorSceneManager.GetActiveScene();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var bootstrap = new GameObject("GameBootstrap");
            bootstrap.AddComponent<GameBootstrap>();

            EditorSceneManager.SaveScene(scene, GameScenePath);

            if (currentScene.IsValid() && !string.IsNullOrWhiteSpace(currentScene.path) && File.Exists(currentScene.path))
            {
                EditorSceneManager.OpenScene(currentScene.path, OpenSceneMode.Single);
            }
            else
            {
                EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            }
        }

        private static void EnsureBuildSettingsContainsGameScene()
        {
            var scenes = EditorBuildSettings.scenes.ToList();
            if (scenes.Any(s => s.path == GameScenePath))
            {
                return;
            }

            scenes.Add(new EditorBuildSettingsScene(GameScenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
