using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TMPro;
// 引入 Input System 的命名空间，防止找不到 TrackedPoseDriver
using UnityEngine.InputSystem.XR;

// Ensures the demo scene and Android settings are initialized when the project is opened.
[InitializeOnLoad]
public static class DemoSceneAutoSetup
{
    private const string ScenePath = "Assets/Scenes/Demo.unity";
    private const string ManifestPath = "Assets/Plugins/Android/AndroidManifest.xml";

    static DemoSceneAutoSetup()
    {
        EditorApplication.delayCall += EnsureProjectSetup;
    }

    private static void EnsureProjectSetup()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        ConfigureAndroidSdkVersions();
        EnsureAndroidManifest();
        EnsureDemoScene();
    }

    private static void ConfigureAndroidSdkVersions()
    {
        // 修复点 1：使用 AndroidApiLevel24
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;

        var api34 = typeof(AndroidSdkVersions).GetFields()
            .FirstOrDefault(f => f.Name is "AndroidApiLevel34" or "AndroidLevel34");
        if (api34 != null)
        {
            PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)api34.GetValue(null);
        }
    }

    private static void EnsureAndroidManifest()
    {
        if (File.Exists(ManifestPath))
        {
            return;
        }

        var manifest = @"<manifest xmlns:android=""http://schemas.android.com/apk/res/android"" package=""com.company.productname"">
    <uses-sdk android:minSdkVersion=""24"" android:targetSdkVersion=""34"" />
    <uses-feature android:name=""android.hardware.camera.ar"" android:required=""false"" />
    <uses-permission android:name=""android.permission.CAMERA"" />
    <uses-permission android:name=""android.permission.RECORD_AUDIO"" />
    <application android:label=""@string/app_name"" android:icon=""@mipmap/app_icon"">
        <activity android:name=""com.unity3d.player.UnityPlayerActivity""
            android:exported=""true""
            android:label=""@string/app_name""
            android:launchMode=""singleTask""
            android:hardwareAccelerated=""true""
            android:configChanges=""keyboard|keyboardHidden|navigation|orientation|screenLayout|screenSize|smallestScreenSize|uiMode"">
            <intent-filter>
                <action android:name=""android.intent.action.MAIN"" />
                <category android:name=""android.intent.category.LAUNCHER"" />
            </intent-filter>
        </activity>
    </application>
</manifest>";
        File.WriteAllText(ManifestPath, manifest);
        AssetDatabase.ImportAsset(ManifestPath);
    }

    private static void EnsureDemoScene()
    {
        Scene scene;
        if (File.Exists(ScenePath))
        {
            scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        }
        else
        {
            scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }

        var hasSession = SceneManager.GetActiveScene().GetRootGameObjects().Any(go => go.GetComponent<ARSession>() != null);
        var hasOrigin = SceneManager.GetActiveScene().GetRootGameObjects().Any(go => go.GetComponent<ARSessionOrigin>() != null);

        if (!hasSession || !hasOrigin)
        {
            BuildSceneObjects(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
        }
        else if (!File.Exists(ScenePath))
        {
            EditorSceneManager.SaveScene(scene, ScenePath);
        }
    }

    private static void BuildSceneObjects(Scene scene)
    {
        var sessionGo = new GameObject("AR Session", typeof(ARSession));
        Undo.RegisterCreatedObjectUndo(sessionGo, "Create AR Session");

        var originGo = new GameObject("AR Session Origin", typeof(ARSessionOrigin));
        Undo.RegisterCreatedObjectUndo(originGo, "Create AR Session Origin");

        // 修复点 2：使用 UnityEngine.InputSystem.XR.TrackedPoseDriver
        // 你的项目里已经装了 Input System，这是正确的现代写法
        var cameraGo = new GameObject("AR Camera", typeof(Camera), typeof(AudioListener), typeof(ARCameraManager), typeof(ARCameraBackground), typeof(UnityEngine.InputSystem.XR.TrackedPoseDriver));
        
        cameraGo.tag = "MainCamera";
        cameraGo.transform.SetParent(originGo.transform, false);
        var cam = cameraGo.GetComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;

        originGo.transform.position = Vector3.zero;
        sessionGo.transform.position = Vector3.zero;

        var canvasGo = new GameObject("HUD Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(HudController));
        canvasGo.transform.SetParent(originGo.transform, false);
        var canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = cam;
        canvas.planeDistance = 1f;
        var canvasRect = canvas.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(800, 320);
        canvasRect.localPosition = new Vector3(0, -0.4f, 1.2f);

        var subtitleGo = new GameObject("Subtitle", typeof(RectTransform), typeof(TextMeshProUGUI));
        subtitleGo.transform.SetParent(canvasGo.transform, false);
        var subtitleRect = subtitleGo.GetComponent<RectTransform>();
        subtitleRect.anchoredPosition = new Vector2(0, -40);
        subtitleRect.sizeDelta = new Vector2(760, 120);
        var subtitle = subtitleGo.GetComponent<TextMeshProUGUI>();
        subtitle.text = "Ready";
        subtitle.fontSize = 48;
        subtitle.alignment = TextAlignmentOptions.Center;
        subtitle.color = Color.white;

        var ringGo = new GameObject("Status Ring", typeof(RectTransform), typeof(Image));
        ringGo.transform.SetParent(canvasGo.transform, false);
        var ringRect = ringGo.GetComponent<RectTransform>();
        ringRect.sizeDelta = new Vector2(180, 180);
        ringRect.anchoredPosition = new Vector2(0, 80);
        var ring = ringGo.GetComponent<Image>();
        ring.color = new Color(0.2f, 0.6f, 1.0f);
        ring.type = Image.Type.Filled;
        ring.fillMethod = Image.FillMethod.Radial360;
        ring.fillAmount = 1f;

        var hud = canvasGo.GetComponent<HudController>();
        hud.GetType().GetField("hudCanvas", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.SetValue(hud, canvas);
        hud.GetType().GetField("subtitle", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.SetValue(hud, subtitle);
        hud.GetType().GetField("statusRing", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.SetValue(hud, ring);

        EditorSceneManager.MarkSceneDirty(scene);
    }
}