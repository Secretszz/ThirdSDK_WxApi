
// *******************************************
// Company Name:	深圳市晴天互娱科技有限公司
//
// File Name:		IAuthListener.cs
//
// Author Name:		Bridge
//
// Create Time:		2024/03/08 10:10:34
// *******************************************

/// <summary>
/// 
/// </summary>
public interface IAuthListener
{
	/// <summary>
	/// 用户同意
	/// </summary>
	/// <param name="code"></param>
	/// <param name="state"></param>
	void OnUserAuth(string code, string state);
	
	/// <summary>
	/// 用户取消
	/// </summary>
	/// <param name="state"></param>
	void OnUserCancel(string state);
	
	/// <summary>
	/// 用户拒绝
	/// </summary>
	/// <param name="state"></param>
	void OnUserDenied(string state);
		
	/// <summary>
	/// 用户错误
	/// </summary>
	/// <param name="errCode"></param>
	/// <param name="errStr"></param>
	/// <param name="state"></param>
	void OnError(int errCode, string errStr, string state);
}
