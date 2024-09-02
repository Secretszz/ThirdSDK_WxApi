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

		void IBridge.OpenPay(string orderInfo, IPayListener listener)
		{
			listener?.OnPayResult(0, "");
		}

		void IBridge.ShareImage(string imagePath, int scene, IShareListener listener)
		{
			listener?.OnFinishShare(true, "");
		}

		void IBridge.ShareImage(byte[] imageData, int scene, IShareListener listener)
		{
			listener?.OnFinishShare(true, "");
		}

		public void ShareLink(string linkUrl, int scene, IShareListener listener)
		{
			listener?.OnFinishShare(false, "not support");
		}

		public void ShareVideo(string videoUrl, int scene, IShareListener listener)
		{
			listener?.OnFinishShare(false, "not support");
		}

		public void WeChatAuth(string state, IAuthListener listener)
		{
			listener?.OnUserCancel(state);
		}
	}
}
#endif