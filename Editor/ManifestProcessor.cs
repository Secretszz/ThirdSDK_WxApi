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
    using UnityEditor.Build;
#if UNITY_2018_1_OR_NEWER
    using UnityEditor.Build.Reporting;
#endif
    using UnityEngine;

#if UNITY_2018_1_OR_NEWER
    public class ManifestProcessor : IPreprocessBuildWithReport
#else
    public class ManifestProcessor : IPreprocessBuild
#endif
    {
        private const string MANIFEST_RELATIVE_PATH = "Plugins/Android/LauncherManifest.xml";

        private XNamespace ns = "http://schemas.android.com/apk/res/android";

        public int callbackOrder => 0;

#if UNITY_2018_1_OR_NEWER
        public void OnPreprocessBuild(BuildReport report)
#else
        public void OnPreprocessBuild(BuildTarget target, string path)
#endif
        {
            string packageName = UnityEditor.PlayerSettings.applicationIdentifier;

            RefreshActivityPackagePath(packageName);

            string manifestPath = Path.Combine(Application.dataPath, MANIFEST_RELATIVE_PATH);

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

        private void RefreshActivityPackagePath(string packageName)
        {
            // Objective-C 文件路径
            string objectiveCFilePath = Path.Combine(Application.dataPath, "WxApi/Plugins/Android/WXEntryActivity.java");
            // 读取 Objective-C 文件内容
            StringBuilder objectiveCCode = new StringBuilder(File.ReadAllText(objectiveCFilePath));
            objectiveCCode = objectiveCCode.Replace("**PACKAGE**", $"package {packageName}.wxapi;");
            // 将修改后的 Objective-C 代码写回文件中
            File.WriteAllText(objectiveCFilePath, objectiveCCode.ToString());
            
            // Objective-C 文件路径
            objectiveCFilePath = Path.Combine(Application.dataPath, "WxApi/Plugins/Android/WXPayEntryActivity.java");
            // 读取 Objective-C 文件内容
            objectiveCCode = new StringBuilder(File.ReadAllText(objectiveCFilePath));
            objectiveCCode = objectiveCCode.Replace("**PACKAGE**", $"package {packageName}.wxapi;");
            // 将修改后的 Objective-C 代码写回文件中
            File.WriteAllText(objectiveCFilePath, objectiveCCode.ToString());
        }

        private XElement CreateActivityElement(string name, string theme, bool exported, string taskAffinity, string launchMode)
        {
            return new XElement("activity",
                    new XAttribute(ns + "name", name),
                    new XAttribute(ns + "label", "@string/app_name"),
                    new XAttribute(ns + "theme", theme),
                    new XAttribute(ns + "exported", exported ? "true" : "false"),
                    new XAttribute(ns + "taskAffinity", taskAffinity),
                    new XAttribute(ns + "launchMode", launchMode));
        }

        private void LogBuildLaunchFailed()
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
