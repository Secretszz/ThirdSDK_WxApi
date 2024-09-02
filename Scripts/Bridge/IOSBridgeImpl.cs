// *******************************************
// Company Name:	深圳市晴天互娱科技有限公司
//
// File Name:		IOSBridgeImpl.cs
//
// Author Name:		Bridge
//
// Create Time:		2023/12/04 17:24:53
// *******************************************

#if UNITY_IOS
namespace Bridge.WxApi
{
	using System;
	using UnityEngine;
	using System.Runtime.InteropServices;
	using AOT;

	/// <summary>
	/// 
	/// </summary>
	internal class iOSBridgeImpl : IBridge
	{
		/// <summary>
		/// 初始化sdk
		/// </summary>
		void IBridge.InitBridge()
		{
			wx_init();
		}

		/// <summary>
		/// 是否安装了微信客户端
		/// </summary>
		/// <returns></returns>
		bool IBridge.IsWXAppInstalled()
		{
			return wx_isWXAppInstalled();
		}

		/// <summary>
		/// 拉起微信客服
		/// </summary>
		/// <param name="groupId">企业ID</param>
		/// <param name="kfid">客服ID</param>
		bool IBridge.OpenCustomerServiceChat(string groupId, string kfid)
		{
			return wx_openCustomerServiceChat(groupId, kfid);
		}

		/// <summary>
		/// 拉起支付
		/// </summary>
		/// <param name="orderInfo">订单信息</param>
		/// <param name="listener">支付回调</param>
		void IBridge.OpenPay(string orderInfo, IPayListener listener)
		{
			_payListener = listener;
			wx_Purchase(orderInfo, purchaseDoneProductId);
		}

		/// <summary>
		/// 分享图片到微信
		/// </summary>
		/// <param name="imagePath">图片路径</param>
		/// <param name="scene">分享场景</param>
		/// <param name="listener">分享回调</param>
		void IBridge.ShareImage(string imagePath, int scene, IShareListener listener)
		{
			_shareListener = listener;
			wx_shareImage(imagePath, scene, OnFinishSavePhoto);
		}

		/// <summary>
		/// 分享图片到微信
		/// </summary>
		/// <param name="imageData">图片数据</param>
		/// <param name="scene">分享场景</param>
		/// <param name="listener">分享回调</param>
		void IBridge.ShareImage(byte[] imageData, int scene, IShareListener listener)
		{
			try
			{
				_shareListener = listener;
				int length = imageData.Length;
				IntPtr buffer = Marshal.AllocHGlobal(length);
				Marshal.Copy(imageData, 0, buffer, length);
				wx_shareImageWithDatas(buffer, length, scene, OnFinishSavePhoto);
			}
			catch (Exception e)
			{
				Debug.LogError("字节流转指针解析错误：" + e.Message);
				_shareListener?.OnFinishShare(false, e.Message);
				_shareListener = null;
			}
		}
				
		/// <summary>
		/// 分享链接
		/// </summary>
		/// <param name="linkUrl">链接地址</param>
		/// <param name="scene">分享场景</param>
		/// <param name="listener">拉起分享窗口事件</param>
		void IBridge.ShareLink(string linkUrl, int scene, IShareListener listener)
		{
			listener?.OnFinishShare(false, "not support");
		}

		/// <summary>
		/// 分享视频
		/// </summary>
		/// <param name="videoUrl">视频地址</param>
		/// <param name="scene">分享场景</param>
		/// <param name="listener">拉起分享窗口事件</param>
		void IBridge.ShareVideo(string videoUrl, int scene, IShareListener listener)
		{
			listener?.OnFinishShare(false, "not support");
		}

		/// <summary>
		/// 登录
		/// </summary>
		/// <param name="state">
		/// 用于保持请求和回调的状态，授权请求后原样带回给第三方。
		/// 该参数可用于防止 csrf 攻击（跨站请求伪造攻击），建议第三方带上该参数，可设置为简单的随机数加 session 进行校验。
		/// 在state传递的过程中会将该参数作为url的一部分进行处理，因此建议对该参数进行url encode操作，防止其中含有影响url解析的特殊字符（如'#'、'&'等）导致该参数无法正确回传。
		/// </param>
		/// <param name="listener">验证回调</param>
		void IBridge.WeChatAuth(string state, IAuthListener listener)
		{
			_authListener = listener;
			wx_Auth(state, OnFinishAuth);
		}

		/// <summary>
		/// 支付回调监听
		/// </summary>
		private static IPayListener _payListener;
		
		/// <summary>
		/// 分享回调监听
		/// </summary>
		private static IShareListener _shareListener;
		
		/// <summary>
		/// 权限回调监听
		/// </summary>
		private static IAuthListener _authListener;

		/// <summary>
		/// 初始化
		/// </summary>
		[DllImport("__Internal")]
		private static extern void wx_init();

		/// <summary>
		/// 打开客服界面
		/// </summary>
		/// <param name="groupId"></param>
		/// <param name="kfid"></param>
		/// <returns></returns>
		[DllImport("__Internal")]
		private static extern bool wx_openCustomerServiceChat(string groupId, string kfid);

		/// <summary>
		/// 是否下载了微信客户端
		/// </summary>
		/// <returns></returns>
		[DllImport("__Internal")]
		private static extern bool wx_isWXAppInstalled();

		/// <summary>
		/// 支付
		/// </summary>
		/// <param name="orderInfo"></param>
		/// <param name="onPayCallback"></param>
		[DllImport("__Internal")]
		private static extern void wx_Purchase(string orderInfo, WXAPIU3DBridgeCallback_onPayCallback onPayCallback);

		/// <summary>
		/// 分享图片
		/// </summary>
		/// <param name="path">图片本地路径</param>
		/// <param name="scene">分享场景</param>
		/// <param name="onShareCallback">分享回调</param>
		[DllImport("__Internal")]
		private static extern void wx_shareImage(string path, int scene, WXAPIU3DBridgeCallback_onShareCallback onShareCallback);

		/// <summary>
		/// 分享图片
		/// </summary>
		/// <param name="datas">图片数据字节流指针</param>
		/// <param name="length">图片数据长度</param>
		/// <param name="scene">分享场景</param>
		/// <param name="onShareCallback">分享回调</param>
		[DllImport("__Internal")]
		private static extern void wx_shareImageWithDatas(IntPtr datas, int length, int scene, WXAPIU3DBridgeCallback_onShareCallback onShareCallback);

		/// <summary>
		/// 请求权限
		/// </summary>
		/// <param name="state">权限类型</param>
		/// <param name="onAuthCallback">完成请求回调</param>
		[DllImport("__Internal")]
		private static extern void wx_Auth(string state, WXAPIU3DBridgeCallback_onAuthCallback onAuthCallback);

		/// <summary>
		/// iOS桥接支付回调事件
		/// </summary>
		private delegate void WXAPIU3DBridgeCallback_onPayCallback(int error_code, string error_msg);
		
		/// <summary>
		/// iOS桥接分享回调事件
		/// </summary>
		private delegate void WXAPIU3DBridgeCallback_onShareCallback(bool success, string err_msg);
		
		/// <summary>
		/// iOS桥接授权回调事件
		/// </summary>
		private delegate void WXAPIU3DBridgeCallback_onAuthCallback(int errCode, string errStr, string code, string state);

		/// <summary>
		/// iOS桥接支付回调事件
		/// </summary>
		/// <param name="error_code"></param>
		/// <param name="error_msg"></param>
		[MonoPInvokeCallback(typeof(WXAPIU3DBridgeCallback_onPayCallback))]
		private static void purchaseDoneProductId(int error_code, string error_msg)
		{
			_payListener?.OnPayResult(error_code, error_msg);
			_payListener = null;
		}

		/// <summary>
		/// iOS桥接分享回调事件
		/// </summary>
		/// <param name="success"></param>
		/// <param name="err_msg"></param>
		[MonoPInvokeCallback(typeof(WXAPIU3DBridgeCallback_onShareCallback))]
		private static void OnFinishSavePhoto(bool success, string err_msg)
		{
			_shareListener?.OnFinishShare(success, err_msg);
		}

		/// <summary>
		/// iOS桥接分享回调事件
		/// </summary>
		/// <param name="errCode"></param>
		/// <param name="errStr"></param>
		/// <param name="code"></param>
		/// <param name="state"></param>
		[MonoPInvokeCallback(typeof(WXAPIU3DBridgeCallback_onAuthCallback))]
		private static void OnFinishAuth(int errCode, string errStr, string code, string state)
		{
			if (errCode == 0)
			{
				// 用户同意
				_authListener.OnUserAuth(code, state);
			} 
			else if (errCode == -4)
			{
				// 用户拒绝
				_authListener.OnUserDenied(state);
			} 
			else if (errCode == -2)
			{
				// 用户取消
				_authListener.OnUserCancel(state);
			}
			else 
			{
				// 发生错误
				_authListener.OnError(errCode, errStr, state);
			}
		}
	}
}
#endif