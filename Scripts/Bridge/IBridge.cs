
// *******************************************
// Company Name:	深圳市晴天互娱科技有限公司
//
// File Name:		IBridge.cs
//
// Author Name:		Bridge
//
// Create Time:		2023/12/04 17:26:08
// *******************************************

namespace Bridge.WxApi
{
	using Common;
	
	/// <summary>
	/// 
	/// </summary>
	internal interface IBridge
	{
		/// <summary>
		/// 初始化sdk
		/// </summary>
		void InitBridge();

		/// <summary>
		/// 是否安装了微信客户端
		/// </summary>
		/// <returns></returns>
		bool IsWXAppInstalled();

		/// <summary>
		/// 拉起微信客服
		/// </summary>
		/// <param name="groupId">企业ID</param>
		/// <param name="kfid">客服ID</param>
		bool OpenCustomerServiceChat(string groupId, string kfid);

		/// <summary>
		/// 拉起支付
		/// </summary>
		/// <param name="orderInfo">订单信息</param>
		/// <param name="listener">支付回调</param>
		void OpenPay(string orderInfo, IPayListener listener);

		/// <summary>
		/// 分享图片到微信
		/// </summary>
		/// <param name="imagePath">图片路径</param>
		/// <param name="scene">分享场景</param>
		/// <param name="listener">分享回调</param>
		void ShareImage(string imagePath, int scene, IShareListener listener);

		/// <summary>
		/// 分享图片到微信
		/// </summary>
		/// <param name="imageData">图片数据</param>
		/// <param name="scene">分享场景</param>
		/// <param name="listener">分享回调</param>
		void ShareImage(byte[] imageData, int scene, IShareListener listener);

		/// <summary>
		/// 分享链接
		/// </summary>
		/// <param name="linkUrl">链接地址</param>
		/// <param name="scene">分享场景</param>
		/// <param name="listener">拉起分享窗口事件</param>
		void ShareLink(string linkUrl, int scene, IShareListener listener);

		/// <summary>
		/// 分享视频
		/// </summary>
		/// <param name="videoUrl">视频地址</param>
		/// <param name="scene">分享场景</param>
		/// <param name="listener">拉起分享窗口事件</param>
		void ShareVideo(string videoUrl, int scene, IShareListener listener);

		/// <summary>
		/// 登录
		/// </summary>
		/// <param name="listener">验证回调</param>
		void WeChatAuth(ILoginListener listener);
	}
}