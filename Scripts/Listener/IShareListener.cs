
// *******************************************
// Company Name:	深圳市晴天互娱科技有限公司
//
// File Name:		IShareListener.cs
//
// Author Name:		Bridge
//
// Create Time:		2023/12/04 17:49:25
// *******************************************

namespace Bridge.WxApi
{
	/// <summary>
	/// 
	/// </summary>
	public interface IShareListener
	{
		void OnFinishShare(bool success, string err_msg);
	}
}