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
		private const string PayCallBackClassName = "com.bridge.wxapi.callback.PayCallBack";
		private const string ShareCallBackClassName = "com.bridge.wxapi.callback.ShareCallBack";
		private const string AuthCallBackClassName = "com.bridge.wxapi.callback.AuthCallback";
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
		void IBridge.OpenPay(string orderInfo, IPayListener listener)
		{
			currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
			{
				api?.Call("openWechatPay", JsonConvert.SerializeObject(orderInfo), new PayCallback(listener));
			}));
		}

		/// <summary>
		/// 分享图片到微信
		/// </summary>
		/// <param name="imagePath">图片路径</param>
		/// <param name="scene">分享场景</param>
		/// <param name="listener">分享回调</param>
		void IBridge.ShareImage(string imagePath, int scene, IShareListener listener)
		{
			api?.Call("shareImage", GetBitmap(imagePath), scene, new ShareCallback(listener));
		}

		/// <summary>
		/// 分享图片到微信
		/// </summary>
		/// <param name="imageData">图片数据</param>
		/// <param name="scene">分享场景</param>
		/// <param name="listener">分享回调</param>
		void IBridge.ShareImage(byte[] imageData, int scene, IShareListener listener)
		{
			api?.Call("shareImage", ByteArrayToBitmap(imageData), scene, new ShareCallback(listener));
		}

		/// <summary>
		/// 分享链接
		/// </summary>
		/// <param name="linkUrl">链接地址</param>
		/// <param name="scene">分享场景</param>
		/// <param name="listener">拉起分享窗口事件</param>
		void IBridge.ShareLink(string linkUrl, int scene, IShareListener listener)
		{
			listener?.OnError(-1, "not support");
		}

		/// <summary>
		/// 分享视频
		/// </summary>
		/// <param name="videoUrl">视频地址</param>
		/// <param name="scene">分享场景</param>
		/// <param name="listener">拉起分享窗口事件</param>
		void IBridge.ShareVideo(string videoUrl, int scene, IShareListener listener)
		{
			listener?.OnError(-1, "not support");
		}

		/// <summary>
		/// 登录
		/// </summary>
		/// <param name="listener">验证回调</param>
		void IBridge.WeChatAuth(ILoginListener listener)
		{
			api?.Call("sendWeChatAuth", new AuthCallback(listener));
		}

		private static AndroidJavaObject GetBitmap(string imagePath)
		{
			if (string.IsNullOrEmpty(imagePath))
			{
				Debug.LogError("ShareImage: imagePath === path is empty === " + imagePath);
				return null;
			}

			AndroidJavaObject imageFile = new AndroidJavaObject("java.io.File", imagePath);
			if (!imageFile.Call<bool>("exists"))
			{
				Debug.LogError("ShareImage: imagePath === file is empty === " + imagePath);
				return null;
			}

			AndroidJavaObject inputStream = new AndroidJavaObject("java.io.FileInputStream", imageFile);
			AndroidJavaClass bitmapFactory = new AndroidJavaClass("android.graphics.BitmapFactory");
			var bitmap = bitmapFactory.CallStatic<AndroidJavaObject>("decodeStream", inputStream);
			inputStream.Call("close");
			return bitmap;
		}

		/**
	    * 字节转图片
        * @param b 图片字节数据
        * @return 图片
	    */
		private static AndroidJavaObject ByteArrayToBitmap(byte[] b)
		{
			if (b.Length != 0)
			{
				AndroidJavaClass bitmapFactory = new AndroidJavaClass("android.graphics.BitmapFactory");
				return bitmapFactory.CallStatic<AndroidJavaObject>("decodeByteArray", b, 0, b.Length);
			}

			return null;
		}

		/// <summary>
		/// 支付回调
		/// </summary>
		private class PayCallback : AndroidJavaProxy
		{
			public PayCallback(IPayListener listener) : base(PayCallBackClassName)
			{
				this.listener = listener;
			}

			private IPayListener listener;

			/**
			 * 支付结果
			 * @param error_code 支付返回的结果码
			 * 0:  用户支付成功
			 * -1: 可能的原因：签名错误、未注册APPID、项目设置APPID不正确、注册的APPID与设置的不匹配、其他异常等
			 * -2: 用户不支付了，点击取消，返回APP
			 */
			public void onPayResult(int error_code, string error_msg)
			{
				listener?.OnPayResult(error_code, error_msg);
			}
		}

		/// <summary>
		/// 分享回调
		/// </summary>
		private class ShareCallback : AndroidJavaProxy
		{
			public ShareCallback(IShareListener listener) : base(ShareCallBackClassName)
			{
				this.listener = listener;
			}

			private IShareListener listener;

			public void onSuccess()
			{
				listener?.OnSuccess();
			}

			public void onCancel()
			{
				listener?.OnCancel();
			}

			public void onError(int errCode, string errMsg)
			{
				listener?.OnError(errCode, errMsg);
			}
		}

		/// <summary>
		/// 请求授权回调
		/// </summary>
		private class AuthCallback : AndroidJavaProxy
		{
			public AuthCallback(ILoginListener listener) : base(AuthCallBackClassName)
			{
				this.listener = listener;
			}

			private ILoginListener listener;

			/// <summary>
			/// 登录成功
			/// </summary>
			/// <param name="accessToken"></param>
			public void onSuccess(string accessToken)
			{
				listener?.OnSuccess(accessToken);
			}

			/// <summary>
			/// 用户取消
			/// </summary>
			public void onCancel()
			{
				listener.OnCancel();
			}

			/// <summary>
			/// 错误
			/// </summary>
			/// <param name="errCode"></param>
			/// <param name="errStr"></param>
			public void onError(int errCode, string errStr)
			{
				listener.OnError(errCode, errStr);
			}
		}
	}
}
#endif