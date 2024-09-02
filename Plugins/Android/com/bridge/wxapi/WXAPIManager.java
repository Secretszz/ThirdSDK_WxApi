package com.bridge.wxapi;

import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.graphics.Bitmap;
import android.util.Log;

import androidx.annotation.NonNull;

import com.tencent.mm.opensdk.constants.Build;
import com.tencent.mm.opensdk.constants.ConstantsAPI;
import com.tencent.mm.opensdk.modelbase.BaseReq;
import com.tencent.mm.opensdk.modelbase.BaseResp;
import com.tencent.mm.opensdk.modelbiz.WXOpenCustomerServiceChat;
import com.tencent.mm.opensdk.modelmsg.SendAuth;
import com.tencent.mm.opensdk.modelmsg.SendMessageToWX;
import com.tencent.mm.opensdk.modelpay.PayReq;
import com.tencent.mm.opensdk.openapi.IWXAPI;
import com.tencent.mm.opensdk.openapi.WXAPIFactory;
import com.wxapi.bridge.callback.AuthCallback;
import com.wxapi.bridge.callback.PayCallBack;
import com.wxapi.bridge.callback.ShareCallBack;

import org.json.JSONException;
import org.json.JSONObject;

public class WXAPIManager {
    private static final String TAG = WXAPIManager.class.getName();
    private static final int THUMB_SIZE = 150;
    public static String APP_ID;
    public final static int IMAGE_MAX_BYTES = 65535;
    private static WXAPIManager instance;

    private IWXAPI wxApi;
    private ShareCallBack shareCallBack;
    private AuthCallback authCallback;
    private PayCallBack payCallBack;
    private SendToWeChat sender;

    public static WXAPIManager getInstance(){
        if (instance == null){
            instance = new WXAPIManager();
        }
        return instance;
    }

    /**
     * 初始化微信API
     */
    public void initWXAPIManager(Activity activity){
        APP_ID = "**APPID**";
        wxApi = WXAPIFactory.createWXAPI(activity, APP_ID, true);
        wxApi.registerApp(APP_ID);
        activity.registerReceiver(new BroadcastReceiver() {
            @Override
            public void onReceive(Context context, Intent intent) {

                // 将该app注册到微信
                wxApi.registerApp(APP_ID);
            }
        }, new IntentFilter(ConstantsAPI.ACTION_REFRESH_WXAPP));
        sender = new SendToWeChat(wxApi);
    }

    /**
     * check WeChat app installation status
     * @return true if installed
     */
    public boolean isWXAppInstalled(){
        return wxApi.isWXAppInstalled();
    }

    /**
     * 拉起微信客服对话
     * @param corpID 公司ID
     * @param kfid 客服ID
     * @return 微信版本是否支持拉起微信客服
     */
    public boolean openCustomerServiceChat(final String corpID, final String kfid){

        if (wxApi == null){
            Log.d(TAG, "openCustomerServiceChat: wxApi is null");
            return false;
        }

        if (wxApi.getWXAppSupportAPI() < Build.SUPPORT_OPEN_CUSTOMER_SERVICE_CHAT){
            Log.e(TAG, "当前微信版本:" + wxApi.getWXAppSupportAPI() + "不支持微信客服");
            return false;
        }

        WXOpenCustomerServiceChat.Req req = new WXOpenCustomerServiceChat.Req();
        req.corpId = corpID;
        req.url = "https://work.weixin.qq.com/kfid/" + kfid;
        wxApi.sendReq(req);
        return true;
    }

    /**
     * 开始调用微信支付API
     * @param orderInfo 订单信息
     * @param payCallBack 支付回调
     */
    public void openWechatPay(String orderInfo, PayCallBack payCallBack){
        if (checkWXApi()){
            payCallBack.onPayResult(-1, "wxApi error");
            return;
        }
        try {
            this.payCallBack = payCallBack;
            JSONObject obj = new JSONObject(orderInfo);
            final PayReq payReq = new PayReq();
            payReq.appId = obj.getString("appid");
            payReq.partnerId = obj.getString("partnerid"); //商户ID
            payReq.prepayId = obj.getString("prepayid");  //预支付订单
            payReq.nonceStr = obj.getString("noncestr");  //异步通知url
            payReq.timeStamp = obj.getString("timestamp");
            payReq.packageValue = obj.getString("package");
            payReq.sign = obj.getString("sign"); //签名由服务器加签后返回，无需客户端处理
            wxApi.sendReq(payReq);
        }catch (JSONException ex){
            Log.e(TAG, ex.getMessage(), ex);
            payCallBack.onPayResult(-1, ex.getMessage());
        }
    }

    /**
     * 分享图片到微信
     * @param bmp 图片
     * @param targetScene 目标场景
     * @param callback 完成分享事件
     */
    public void shareImage(Bitmap bmp, int targetScene, ShareCallBack callback){
        if (checkWXApi()){
            callback.onFinishShare(false, "wxApi error");
            return;
        }

        if (bmp == null){
            Log.d(TAG, "ShareImage: Bitmap === load image failed");
            if (callback != null){
                callback.onFinishShare(false, "ShareImage: Bitmap === load image failed");
            }
            return;
        }
        shareCallBack = callback;
        sender.SendImageToWeChat(bmp, THUMB_SIZE, targetScene);
    }

    /**
     * 发送微信授权
     * @param scopeArray 授权类型数组
     * @param state 用于保持请求和回调的状态，授权请求后原样带回给第三方。
     *              该参数可用于防止 csrf 攻击（跨站请求伪造攻击），
     *              建议第三方带上该参数，可设置为简单的随机数加 session 进行校验。
     *              在state传递的过程中会将该参数作为url的一部分进行处理，
     *              因此建议对该参数进行url encode操作，
     *              防止其中含有影响url解析的特殊字符（如'#'、'&'等）导致该参数无法正确回传。
     * @param callback 授权完成回调事件
     */
    public void sendWeChatAuth(String[] scopeArray, String state, AuthCallback callback){
        if (checkWXApi()){
            callback.onError(-1, "wxApi error", state);
            return;
        }

        this.authCallback = callback;
        sender.SendAuthToWeChat(scopeArray, state);
    }

    /**
     * 用户微信信息授权情况返回
     * @param authResp 授权回调
     */
    public void onAuthResp(@NonNull SendAuth.Resp authResp){
        if (authResp.errCode == 0){
            // 用户同意
            this.authCallback.onUserAuth(authResp.code, authResp.state);
        } else if (authResp.errCode == -4){
            // 用户拒绝
            this.authCallback.onUserDenied(authResp.state);
        } else if (authResp.errCode == -2){
            // 用户取消
            this.authCallback.onUserCancel(authResp.state);
        } else {
            // 发生错误
            this.authCallback.onError(authResp.errCode, authResp.errStr, authResp.state);
        }
    }

    /**
     * 检查WXAPI是否不可用
     * @return 不可用返回true
     */
    private boolean checkWXApi(){
        return wxApi == null || !wxApi.isWXAppInstalled();
    }

    public void onEntryActivityReq(@NonNull BaseReq req){
        Log.d(TAG, "onReq: " + + req.getType());
        switch (req.getType()) {
            case ConstantsAPI.COMMAND_GETMESSAGE_FROM_WX:
                Log.e(TAG, "COMMAND_GETMESSAGE_FROM_WX");
                break;
            case ConstantsAPI.COMMAND_SHOWMESSAGE_FROM_WX:
                Log.e(TAG, "COMMAND_SHOWMESSAGE_FROM_WX");
                break;
            default:
                break;
        }
    }

    public void onEntryActivityResp(@NonNull BaseResp resp) {

        // 暂时只接入了微信分享，所以只需要看看返回的代码是不是OK
        Log.d(TAG, "resp.errCode: " + resp.errCode);
        Log.d(TAG, "resp.getType: " + resp.getType());

        if (resp.getType() == ConstantsAPI.COMMAND_SENDMESSAGE_TO_WX){
            Log.d(TAG, "onResp: COMMAND_SENDMESSAGE_TO_WX");
            SendMessageToWX.Resp sendResp = (SendMessageToWX.Resp)resp;
            Log.d(TAG, "sendResp===: " + sendResp.errStr);
            if (shareCallBack != null){
                shareCallBack.onFinishShare(sendResp.errCode == BaseResp.ErrCode.ERR_OK, "");
            }
        }

        if (resp.getType() == ConstantsAPI.COMMAND_OPEN_CUSTOMER_SERVICE_CHAT){
            Log.d(TAG, "onResp: COMMAND_OPEN_CUSTOMER_SERVICE_CHAT");
            Log.d(TAG, "customerResp===: " + resp.errStr);
        }

        if (resp.getType() == ConstantsAPI.COMMAND_PAY_BY_WX) {
            Log.e(TAG, "COMMAND_PAY_BY_WX");
            if (payCallBack != null){
                payCallBack.onPayResult(resp.errCode, resp.errStr);
            }
        }

        /*
        Log.d(TAG, "onResp: " + resp.getType());
        if (resp.getType() == ConstantsAPI.COMMAND_SUBSCRIBE_MESSAGE) {
            Log.d(TAG, "onResp: COMMAND_SUBSCRIBE_MESSAGE");
            SubscribeMessage.Resp subscribeMsgResp = (SubscribeMessage.Resp) resp;
            String text = String.format("openid=%s\ntemplate_id=%s\nscene=%d\naction=%s\nreserved=%s",
                    subscribeMsgResp.openId, subscribeMsgResp.templateID, subscribeMsgResp.scene, subscribeMsgResp.action, subscribeMsgResp.reserved);

            Toast.makeText(this, text, Toast.LENGTH_LONG).show();
        }

        if (resp.getType() == ConstantsAPI.COMMAND_LAUNCH_WX_MINIPROGRAM) {
            Log.d(TAG, "onResp: COMMAND_LAUNCH_WX_MINIPROGRAM");
           WXLaunchMiniProgram.Resp launchMiniProgramResp = (WXLaunchMiniProgram.Resp) resp;
           String text = String.format("openid=%s\nextMsg=%s\nerrStr=%s",
                   launchMiniProgramResp.openId, launchMiniProgramResp.extMsg,launchMiniProgramResp.errStr);

            Toast.makeText(this, text, Toast.LENGTH_LONG).show();
        }

        if (resp.getType() == ConstantsAPI.COMMAND_OPEN_BUSINESS_VIEW) {
            Log.d(TAG, "onResp: COMMAND_OPEN_BUSINESS_VIEW");
            WXOpenBusinessView.Resp launchMiniProgramResp = (WXOpenBusinessView.Resp) resp;
            String text = String.format("openid=%s\nextMsg=%s\nerrStr=%s\nbusinessType=%s",
                    launchMiniProgramResp.openId, launchMiniProgramResp.extMsg,launchMiniProgramResp.errStr,launchMiniProgramResp.businessType);

            Toast.makeText(this, text, Toast.LENGTH_LONG).show();
        }

        if (resp.getType() == ConstantsAPI.COMMAND_OPEN_BUSINESS_WEBVIEW) {
            Log.d(TAG, "onResp: COMMAND_OPEN_BUSINESS_WEBVIEW");
            WXOpenBusinessWebview.Resp response = (WXOpenBusinessWebview.Resp) resp;
            String text = String.format("businessType=%d\nresultInfo=%s\nret=%d",response.businessType,response.resultInfo,response.errCode);

            Toast.makeText(this, text, Toast.LENGTH_LONG).show();
        }
         */

        if (resp.getType() == ConstantsAPI.COMMAND_SENDAUTH) {
            Log.d(TAG, "onResp: COMMAND_SENDAUTH");
            onAuthResp((SendAuth.Resp)resp);
        }
    }
}