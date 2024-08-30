// *******************************************
// Company Name:	深圳市晴天互娱科技有限公司
//
// File Name:		WXAPI.cs
//
// Author Name:		Bridge
//
// Create Time:		2023/12/04 17:24:20
// *******************************************

namespace WxApi
{
	/// <summary>
	/// 
	/// </summary>
	public static class WXAPI
	{
		private static IBridge _bridge;

		/// <summary>
		/// SDK桥接文件
		/// </summary>
		private static IBridge bridgeImpl
		{
			get
			{
				if (_bridge == null)
				{
#if UNITY_IOS && !UNITY_EDITOR
					_bridge = new iOSBridgeImpl();
#elif UNITY_ANDROID && !UNITY_EDITOR
					_bridge = new AndroidBridgeImpl();
#else
					_bridge = new EditorBridge();
#endif
				}

				return _bridge;
			}
		}

		/// <summary>
		/// 应用ID
		/// </summary>
		public const string AppId = "";
		
		/// <summary>
		/// 深度链接
		/// </summary>
		public const string UniversalLink = "https://domain/project/";

		/// <summary>
		/// 初始化sdk
		/// </summary>
		public static void InitWXAPI()
		{
			bridgeImpl.InitBridge(AppId, UniversalLink);
		}

		/// <summary>
		/// 是否下载了微信
		/// </summary>
		/// <returns></returns>
		public static bool IsWXAppInstalled()
		{
			return bridgeImpl.IsWXAppInstalled();
		}

		/// <summary>
		/// 打开微信客服
		/// </summary>
		/// <param name="groupId"></param>
		/// <param name="kfid"></param>
		/// <returns></returns>
		public static bool OpenCustomerServiceChat(string groupId, string kfid)
		{
			return bridgeImpl.OpenCustomerServiceChat(groupId, kfid);
		}

		/// <summary>
		/// 拉起支付
		/// </summary>
		/// <param name="orderInfo">订单信息</param>
		/// <param name="listener">支付回调</param>
		public static void OpenPay(string orderInfo, IPayListener listener)
		{
			bridgeImpl.OpenPay(orderInfo, listener);
		}

		/// <summary>
		/// 分享图片到微信
		/// </summary>
		/// <param name="imagePath">图片路径</param>
		/// <param name="scene">分享场景</param>
		/// <param name="listener">分享回调</param>
		public static void ShareImage(string imagePath, int scene, IShareListener listener)
		{
			bridgeImpl.ShareImage(imagePath, scene, listener);
		}

		/// <summary>
		/// 分享图片到微信
		/// </summary>
		/// <param name="imageData">图片数据</param>
		/// <param name="scene">分享场景</param>
		/// <param name="listener">分享回调</param>
		public static void ShareImage(byte[] imageData, int scene, IShareListener listener)
		{
			bridgeImpl.ShareImage(imageData, scene, listener);
		}

		/// <summary>
		/// 分享链接
		/// </summary>
		/// <param name="linkUrl">链接地址</param>
		/// <param name="scene">分享场景</param>
		/// <param name="listener">拉起分享窗口事件</param>
		public static void ShareLink(string linkUrl, int scene, IShareListener listener)
		{
			bridgeImpl.ShareLink(linkUrl, scene, listener);
		}

		/// <summary>
		/// 分享视频
		/// </summary>
		/// <param name="videoUrl">视频地址</param>
		/// <param name="scene">分享场景</param>
		/// <param name="listener">拉起分享窗口事件</param>
		public static void ShareVideo(string videoUrl, int scene, IShareListener listener)
		{
			bridgeImpl.ShareVideo(videoUrl, scene, listener);
		}
	}
}
