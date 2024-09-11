// *******************************************
// Company Name:	深圳市晴天互娱科技有限公司
//
// File Name:		EditorBridge.cs
//
// Author Name:		Bridge
//
// Create Time:		2023/12/04 17:33:45
// *******************************************

#if (UNITY_IOS || UNITY_ANDROID) && UNITY_EDITOR
namespace Bridge.WxApi
{
	using Common;
	
	/// <summary>
	/// 
	/// </summary>
	internal class EditorBridge : IBridge
	{
		/// <summary>
		/// 初始化sdk
		/// </summary>
		void IBridge.InitBridge()
		{
		}

		bool IBridge.IsWXAppInstalled()
		{
			return false;
		}

		bool IBridge.OpenCustomerServiceChat(string groupId, string kfid)
		{
			return false;
		}

		void IBridge.OpenPay(string orderInfo, IBridgeListener listener)
		{
			listener?.OnSuccess("");
		}

		void IBridge.ShareImage(string imagePath, int scene, IBridgeListener listener)
		{
			listener?.OnSuccess("");
		}

		void IBridge.ShareImage(byte[] imageData, int scene, IBridgeListener listener)
		{
			listener?.OnSuccess("");
		}

		public void ShareLink(string linkUrl, int scene, IBridgeListener listener)
		{
			listener?.OnSuccess("");
		}

		public void ShareVideo(string videoUrl, int scene, IBridgeListener listener)
		{
			listener?.OnSuccess("");
		}

		public void WeChatAuth(IBridgeListener listener)
		{
			listener?.OnCancel();
		}
	}
}
#endif