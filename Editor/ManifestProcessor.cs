// *******************************************
// Company Name:	深圳市晴天互娱科技有限公司
//
// File Name:		ManifestProcessor.cs
//
// Author Name:		Bridge
//
// Create Time:		2023/12/04 19:13:02
// *******************************************

#if UNITY_ANDROID
namespace Bridge.WxApi
{
    using System.Text;
    using System.IO;
    using System.Xml.Linq;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using Editor;

    public static class ManifestProcessor
    {
        private const string MANIFEST_RELATIVE_PATH = "Plugins/Android/LauncherManifest.xml";

        private static XNamespace ns = "http://schemas.android.com/apk/res/android";

        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget target, string projectPath)
        {
            string packageName = PlayerSettings.applicationIdentifier;

            RefreshLaunchManifest(projectPath, packageName);
            ThirdSDKSettings settings = ThirdSDKSettings.Instance;
            // Objective-C 文件路径
            var objectiveCFilePath = $"{projectPath}/unityLibrary/src/main/java/com/bridge/wxapi/WXAPIManager.java";
            // 读取 Objective-C 文件内容
            var objectiveCCode = new StringBuilder(File.ReadAllText(objectiveCFilePath));
            objectiveCCode = objectiveCCode.Replace("**APPID**", settings.WxAppId);
            // 将修改后的 Objective-C 代码写回文件中
            File.WriteAllText(objectiveCFilePath, objectiveCCode.ToString());
            RefreshActivityPackagePath(projectPath, packageName, "WXEntryActivity.java");
            RefreshActivityPackagePath(projectPath, packageName, "WXPayEntryActivity.java");
        }

        private static void RefreshLaunchManifest(string projectPath, string packageName)
        {
            string manifestPath = $"{projectPath}/launcher/src/main/AndroidManifest.xml";

            XDocument manifest = null;
            try
            {
                manifest = XDocument.Load(manifestPath);
            }
#pragma warning disable 0168
            catch (IOException e)
#pragma warning restore 0168
            {
                LogBuildLaunchFailed();
                return;
            }

            XElement elemManifest = manifest.Element("manifest");
            if (elemManifest == null)
            {
                LogBuildLaunchFailed();
                return;
            }

            XElement elemApplication = elemManifest.Element("application");
            if (elemApplication == null)
            {
                LogBuildLaunchFailed();
                return;
            }

            elemApplication.Add(CreateActivityElement(".wxapi.WXPayEntryActivity", "@android:style/Theme.Translucent.NoTitleBar", true, packageName, "singleTask"));

            elemApplication.Add(CreateActivityElement(".wxapi.WXEntryActivity", "@android:style/Theme.Translucent.NoTitleBar", true, packageName, "singleTask"));

            elemManifest.Save(manifestPath);
        }

        private static void RefreshActivityPackagePath(string projectPath, string packageName, string fileName)
        {
            string objectiveCFilePath = Path.Combine(projectPath, "unityLibrary/src/main/java", fileName);
            StringBuilder objectiveCCode;
            if (File.Exists(objectiveCFilePath))
            {
                objectiveCCode = new StringBuilder(File.ReadAllText(objectiveCFilePath));
                File.Delete(objectiveCFilePath);
            }
            else
            {
                objectiveCFilePath = Path.Combine(Application.dataPath, "ThirdSDK/WxApi/Plugins/Android", fileName);
                objectiveCCode = new StringBuilder(File.ReadAllText(objectiveCFilePath));
            }
            objectiveCCode = objectiveCCode.Replace("**PACKAGE**", $"package {packageName}.wxapi;");
            objectiveCFilePath = Path.Combine(projectPath, $"unityLibrary/src/main/java/{packageName.Replace(".", "/")}/wxapi");
            if (!Directory.Exists(objectiveCFilePath))
            {
                Directory.CreateDirectory(objectiveCFilePath);
            }
            objectiveCFilePath = Path.Combine(objectiveCFilePath, fileName);
            File.WriteAllText(objectiveCFilePath, objectiveCCode.ToString());
        }

        private static XElement CreateActivityElement(string name, string theme, bool exported, string taskAffinity, string launchMode)
        {
            return new XElement("activity",
                    new XAttribute(ns + "name", name),
                    new XAttribute(ns + "label", "@string/app_name"),
                    new XAttribute(ns + "theme", theme),
                    new XAttribute(ns + "exported", exported ? "true" : "false"),
                    new XAttribute(ns + "taskAffinity", taskAffinity),
                    new XAttribute(ns + "launchMode", launchMode));
        }

        private static void LogBuildLaunchFailed()
        {
            Debug.LogWarning(@"LauncherManifest.xml is not valid.
    <!-- 微信API回调Activity-->
    <activity
            android:name="".wxapi.WXPayEntryActivity""
            android:theme=""@android:style/Theme.Translucent.NoTitleBar""
            android:exported=""true""
            android:taskAffinity=""com.zhandoushaonv.android""
            android:launchMode=""singleTask"">
    </activity>

    <activity
            android:name="".wxapi.WXEntryActivity""
            android:theme=""@android:style/Theme.Translucent.NoTitleBar""
            android:exported=""true""
            android:taskAffinity=""com.zhandoushaonv.android""
            android:launchMode=""singleTask"">
    </activity>");
        }
    }
}
#endif
