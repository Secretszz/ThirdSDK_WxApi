// *******************************************
// Company Name:	深圳市晴天互娱科技有限公司
//
// File Name:		AndroidBridgeImpl.cs
//
// Author Name:		Bridge
//
// Create Time:		2023/12/04 17:57:18
// *******************************************

#if UNITY_ANDROID
namespace Bridge.WxApi
{
	using Common;
	using Newtonsoft.Json;
	using UnityEngine;

	/// <summary>
	/// 
	/// </summary>
	internal class AndroidBridgeImpl : IBridge
	{
		private const string UnityPlayerClassName = "com.unity3d.player.UnityPlayer";
		private const string ManagerClassName = "com.bridge.wxapi.WXAPIManager";
		private static AndroidJavaObject api;
		private static AndroidJavaObject currentActivity;

		/// <summary>
		/// 初始化sdk
		/// </summary>
		void IBridge.InitBridge()
		{
			AndroidJavaClass unityPlayer = new AndroidJavaClass(UnityPlayerClassName);
			currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

			AndroidJavaClass jc = new AndroidJavaClass(ManagerClassName);
			api = jc.CallStatic<AndroidJavaObject>("getInstance");
			api.Call("initWXAPIManager", currentActivity);
		}

		/// <summary>
		/// 是否安装了微信客户端
		/// </summary>
		/// <returns></returns>
		bool IBridge.IsWXAppInstalled()
		{
			return api != null && api.Call<bool>("isWXAppInstalled");
		}

		/// <summary>
		/// 拉起微信客服
		/// </summary>
		/// <param name="groupId">企业ID</param>
		/// <param name="kfid">客服ID</param>
		bool IBridge.OpenCustomerServiceChat(string groupId, string kfid)
		{
			return api != null && api.Call<bool>("openCustomerServiceChat", groupId, kfid);
		}

		/// <summary>
		/// 拉起支付
		/// </summary>
		/// <param name="orderInfo">订单信息</param>
		/// <param name="listener">支付回调</param>
		void IBridge.OpenPay(string orderInfo, IBridgeListener listener)
		{
			currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
			{
				api?.Call("openWechatPay", JsonConvert.SerializeObject(orderInfo), new BridgeCallback(listener));
			}));
		}

		/// <summary>
		/// 分享图片到微信
		/// </summary>
		/// <param name="imagePath">图片路径</param>
		/// <param name="scene">分享场景</param>
		/// <param name="listener">分享回调</param>
		void IBridge.ShareImage(string imagePath, int scene, IBridgeListener listener)
		{
			api?.Call("shareImage", imagePath, scene, new BridgeCallback(listener));
		}

		/// <summary>
		/// 分享图片到微信
		/// </summary>
		/// <param name="imageData">图片数据</param>
		/// <param name="scene">分享场景</param>
		/// <param name="listener">分享回调</param>
		void IBridge.ShareImage(byte[] imageData, int scene, IBridgeListener listener)
		{
			api?.Call("shareImage", imageData, scene, new BridgeCallback(listener));
		}

		/// <summary>
		/// 分享链接
		/// </summary>
		/// <param name="linkUrl">链接地址</param>
		/// <param name="scene">分享场景</param>
		/// <param name="listener">拉起分享窗口事件</param>
		void IBridge.ShareLink(string linkUrl, int scene, IBridgeListener listener)
		{
			listener?.OnError(-1, "not support");
		}

		/// <summary>
		/// 分享视频
		/// </summary>
		/// <param name="videoUrl">视频地址</param>
		/// <param name="scene">分享场景</param>
		/// <param name="listener">拉起分享窗口事件</param>
		void IBridge.ShareVideo(string videoUrl, int scene, IBridgeListener listener)
		{
			listener?.OnError(-1, "not support");
		}

		/// <summary>
		/// 登录
		/// </summary>
		/// <param name="listener">验证回调</param>
		void IBridge.WeChatAuth(IBridgeListener listener)
		{
			api?.Call("sendWeChatAuth", new BridgeCallback(listener));
		}
	}
}
#endif