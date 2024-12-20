// *******************************************
// Company Name:	深圳市晴天互娱科技有限公司
//
// File Name:		OpenHarmonyImpl.cs
//
// Author Name:		Bridge
//
// Create Time:		2024/10/25 16:10:SS
// *******************************************

#if UNITY_OPENHARMONY

namespace Bridge.WxApi
{
    using Common;
    using UnityEngine;

    public class OpenHarmonyImpl : IBridge
    {
        private const string ManagerClassName = "WeChatSDKManager";
        private static OpenHarmonyJSObject api;
        private OpenHarmonyJSCallback messageCallback;

        private BridgeCallback authCallback;
        private BridgeCallback payCallback;
        private BridgeCallback shareCallback;

        /// <summary>
        /// 初始化sdk
        /// </summary>
        void IBridge.InitBridge()
        {
            OpenHarmonyJSClass jc = new OpenHarmonyJSClass(ManagerClassName);
            api = jc.CallStatic<OpenHarmonyJSObject>("getInstance");
            api.Call("init");
        }

        /// <summary>
        /// 是否安装了微信客户端
        /// </summary>
        /// <returns></returns>
        bool IBridge.IsWXAppInstalled()
        {
            return api.Call<bool>("isWXAppInstalled");
        }

        /// <summary>
        /// 拉起微信客服
        /// </summary>
        /// <param name="groupId">企业ID</param>
        /// <param name="kfid">客服ID</param>
        bool IBridge.OpenCustomerServiceChat(string groupId, string kfid)
        {
            return api.Call<bool>("OpenCustomerServiceChat", groupId, kfid);
        }

        /// <summary>
        /// 拉起支付
        /// </summary>
        /// <param name="orderInfo">订单信息</param>
        /// <param name="listener">支付回调</param>
        void IBridge.OpenPay(string orderInfo, IBridgeListener listener)
        {
            payCallback = new BridgeCallback(listener);
            api.Call("pay", orderInfo, new OpenHarmonyJSCallback(payCallback.Callback));
        }

        /// <summary>
        /// 分享图片到微信
        /// </summary>
        /// <param name="imagePath">图片路径</param>
        /// <param name="scene">分享场景</param>
        /// <param name="listener">分享回调</param>
        void IBridge.ShareImage(string imagePath, int scene, IBridgeListener listener)
        {
            shareCallback = new BridgeCallback(listener);
            api.Call("sendImageToWX", imagePath, null, scene, new OpenHarmonyJSCallback(shareCallback.Callback));
        }

        /// <summary>
        /// 分享图片到微信
        /// </summary>
        /// <param name="imageData">图片数据</param>
        /// <param name="scene">分享场景</param>
        /// <param name="listener">分享回调</param>
        void IBridge.ShareImage(byte[] imageData, int scene, IBridgeListener listener)
        {
            shareCallback = new BridgeCallback(listener);
            api.Call("sendImageToWX", "", imageData, scene, new OpenHarmonyJSCallback(shareCallback.Callback));
        }

        /// <summary>
        /// 分享链接
        /// </summary>
        /// <param name="linkUrl">链接地址</param>
        /// <param name="scene">分享场景</param>
        /// <param name="listener">拉起分享窗口事件</param>
        void IBridge.ShareLink(string linkUrl, int scene, IBridgeListener listener)
        {
            listener?.OnError(-1, "not support");
        }

        /// <summary>
        /// 分享视频
        /// </summary>
        /// <param name="videoUrl">视频地址</param>
        /// <param name="scene">分享场景</param>
        /// <param name="listener">拉起分享窗口事件</param>
        void IBridge.ShareVideo(string videoUrl, int scene, IBridgeListener listener)
        {
            listener?.OnError(-1, "not support");
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="listener">验证回调</param>
        void IBridge.WeChatAuth(IBridgeListener listener)
        {
            authCallback = new BridgeCallback(listener);
            api.Call("auth", new OpenHarmonyJSCallback(authCallback.Callback));
        }
    }
}

#endif
