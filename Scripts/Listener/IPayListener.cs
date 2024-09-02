
// *******************************************
// Company Name:	深圳市晴天互娱科技有限公司
//
// File Name:		IPayListener.cs
//
// Author Name:		Bridge
//
// Create Time:		2023/12/04 17:31:07
// *******************************************

namespace Bridge.WxApi
{
	/// <summary>
	/// 
	/// </summary>
	public interface IPayListener
	{
		void OnPayResult(int error_code, string error_msg);
	}
}