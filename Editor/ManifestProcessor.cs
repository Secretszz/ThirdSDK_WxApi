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
    using Common;

    internal static class ManifestProcessor
    {
        [PostProcessBuild(10001)]
        public static void OnPostprocessBuild(BuildTarget target, string projectPath)
        {
            CopyNativeCode(projectPath);
            string packageName = PlayerSettings.applicationIdentifier;
            RefreshLaunchManifest(projectPath, packageName);
            RefreshManifest();
            ThirdSDKSettings settings = ThirdSDKSettings.Instance;
            // Objective-C 文件路径
            var objectiveCFilePath = Path.Combine(projectPath, Common.ManifestProcessor.NATIVE_CODE_DIR, "wxapi/WXAPIManager.java");
            // 读取 Objective-C 文件内容
            var objectiveCCode = new StringBuilder(File.ReadAllText(objectiveCFilePath));
            objectiveCCode = objectiveCCode.Replace("**APPID**", settings.WxAppId);
            // 将修改后的 Objective-C 代码写回文件中
            File.WriteAllText(objectiveCFilePath, objectiveCCode.ToString());
            RefreshActivityPackagePath(projectPath, packageName, "WXEntryActivity.java");
            RefreshActivityPackagePath(projectPath, packageName, "WXPayEntryActivity.java");
            
            Common.ManifestProcessor.ReplaceBuildDefinedCache[Common.ManifestProcessor.WX_DEPENDENCIES] = "api 'com.tencent.mm.opensdk:wechat-sdk-android:6.8.24'";
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
                LogBuildLaunchFailed(packageName);
                return;
            }

            XElement elemManifest = manifest.Element("manifest");
            if (elemManifest == null)
            {
                LogBuildLaunchFailed(packageName);
                return;
            }

            XElement elemApplication = elemManifest.Element("application");
            if (elemApplication == null)
            {
                LogBuildLaunchFailed(packageName);
                return;
            }

            elemApplication.Add(CreateActivityElement(".wxapi.WXPayEntryActivity", packageName));

            elemApplication.Add(CreateActivityElement(".wxapi.WXEntryActivity", packageName));

            elemManifest.Save(manifestPath);
        }

        private static void RefreshManifest()
        {
            Common.ManifestProcessor.QueriesElements.Add(new XElement("package", new XAttribute(Common.ManifestProcessor.ns + "name", "com.tencent.mm")));
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
        
        private static void CopyNativeCode(string projectPath)
        {
            var sourcePath = ThirdSDKPackageManager.GetUnityPackagePath(PackageType.WeChat);
            if (string.IsNullOrEmpty(sourcePath))
            {
                // 这个不是通过ump下载的包，查找工程内部文件夹
                sourcePath = "Assets/ThirdSDK/WxApi";
            }

            sourcePath += "/Plugins/Android";
            Debug.Log("remotePackagePath===" + sourcePath);
            string targetPath = Path.Combine(projectPath, Common.ManifestProcessor.NATIVE_CODE_DIR, "wxapi");
            Debug.Log("targetPath===" + targetPath);
            FileTool.DirectoryCopy(sourcePath + "/wxapi", targetPath);
        }

        private static XElement CreateActivityElement(string name, string taskAffinity)
        {
            return new XElement("activity",
                    new XAttribute(Common.ManifestProcessor.ns + "name", name),
                    new XAttribute(Common.ManifestProcessor.ns + "label", "@string/app_name"),
                    new XAttribute(Common.ManifestProcessor.ns + "theme", "@android:style/Theme.Translucent.NoTitleBar"),
                    new XAttribute(Common.ManifestProcessor.ns + "exported", "true"),
                    new XAttribute(Common.ManifestProcessor.ns + "taskAffinity", taskAffinity),
                    new XAttribute(Common.ManifestProcessor.ns + "launchMode", "singleTask"));
        }

        private static void LogBuildLaunchFailed(string packageName)
        {
            Debug.LogWarning($@"LauncherManifest.xml is not valid.
    <!-- 微信API回调Activity-->
    <activity
            android:name="".wxapi.WXPayEntryActivity""
            android:theme=""@android:style/Theme.Translucent.NoTitleBar""
            android:exported=""true""
            android:taskAffinity=""{packageName}""
            android:launchMode=""singleTask"">
    </activity>

    <activity
            android:name="".wxapi.WXEntryActivity""
            android:theme=""@android:style/Theme.Translucent.NoTitleBar""
            android:exported=""true""
            android:taskAffinity=""{packageName}""
            android:launchMode=""singleTask"">
    </activity>");
        }
    }
}
#endif
