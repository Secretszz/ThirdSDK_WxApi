package com.bridge.wxapi.callback;

/**
 * 支付回调接口
 */
public interface PayCallBack {
    void onPayResult(int error_code, String error_msg);
}