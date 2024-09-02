// *******************************************
// Company Name:	深圳市晴天互娱科技有限公司
//
// File Name:		IOSProcessor.cs
//
// Author Name:		Bridge
//
// Create Time:		2023/12/26 18:23:11
// *******************************************

#if UNITY_IOS
namespace Bridge.WxApi
{
	using System.IO;
	using UnityEditor;
	using UnityEditor.Callbacks;
	using UnityEditor.iOS.Xcode;
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// 
	/// </summary>
	internal static class IOSProcessor
	{
		[PostProcessBuild(10002)]
		public static void OnPostProcessBuild(BuildTarget target, string pathToBuildProject)
		{
			if (target == BuildTarget.iOS)
			{
				var projPath = pathToBuildProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
				var proj = new PBXProject();
				proj.ReadFromFile(projPath);
#if UNITY_2019_3_OR_NEWER
				var targetGUID = proj.GetUnityFrameworkTargetGuid();
#else
				var targetGUID = proj.TargetGuidByName("Unity-iPhone");
#endif
				proj.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-ObjC -all_load");
				
				proj.AddFrameworkToProjectEx(targetGUID, "Security.framework", false);
				proj.AddFrameworkToProjectEx(targetGUID, "CoreGraphics.framework", false);
				proj.AddFrameworkToProjectEx(targetGUID, "WebKit.framework", false);

				proj.WriteToFile(projPath);
				
				var plistPath = Path.Combine(pathToBuildProject, "Info.plist");
				var plist = new PlistDocument();
				plist.ReadFromFile(plistPath);
				var rootDic = plist.root;
			
				var items = new[]
				{
						"weixin",
						"weixinULAPI",
						"weixinURLParamsAPI"
				};
			
				PlistElementArray plistElementList = rootDic.GetElementArray("LSApplicationQueriesSchemes");
				List<string> list = plistElementList.values.ToList(x => x.AsString());
				foreach (var t in items)
				{
					if (!list.Contains(t))
					{
						plistElementList.AddString(t);
					}
				}
			
				var array = rootDic.GetElementArray("CFBundleURLTypes");
				PlistElementDict wxURLType = array.AddDict();
				wxURLType.SetString("CFBundleTypeRole", "Editor");
				wxURLType.SetString("CFBundleURLName", "weixin");
				wxURLType.CreateArray("CFBundleURLSchemes").AddString(WXAPI.AppId);
				plist.WriteToFile(plistPath);
			}
		}

		private static void AddFrameworkToProjectEx(this PBXProject proj, string targetGuid, string framework, bool weak)
		{
			if (!proj.ContainsFramework(targetGuid, framework))
			{
				proj.AddFrameworkToProject(targetGuid, framework, weak);
			}
		}

		private static PlistElementArray GetElementArray(this PlistElementDict rootDict, string key)
		{
			return rootDict.values.TryGetValue(key, out var element) ? element.AsArray() : rootDict.CreateArray(key);
		}
		
		private static List<T> ToList<TSource, T>(this IEnumerable<TSource> source, Func<TSource, T> predicate, Func<TSource, bool> selector = null)
		{
			List<T> result = new List<T>();
			foreach (var item in source)
			{
				if (selector == null || selector(item))
					result.Add(predicate(item));
			}
			return result;
		}
	}
}
#endif
