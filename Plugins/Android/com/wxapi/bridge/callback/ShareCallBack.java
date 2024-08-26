package com.wxapi.bridge.callback;

/**
 * 分享回调接口
 */
public interface ShareCallBack {
    void onFinishShare(boolean success, String err_msg);
}