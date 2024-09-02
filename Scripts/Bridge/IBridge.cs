
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
		/// <param name="state">用于保持请求和回调的状态，授权请求后原样带回给第三方。
		/// 该参数可用于防止 csrf 攻击（跨站请求伪造攻击），建议第三方带上该参数，可设置为简单的随机数加 session 进行校验。
		/// 在state传递的过程中会将该参数作为url的一部分进行处理，因此建议对该参数进行url encode操作，防止其中含有影响url解析的特殊字符（如'#'、'&'等）导致该参数无法正确回传。
		/// </param>
		/// <param name="listener">验证回调</param>
		void WeChatAuth(string state, IAuthListener listener);
	}
}