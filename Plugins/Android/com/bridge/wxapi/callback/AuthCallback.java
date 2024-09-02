package com.bridge.wxapi.callback;

public interface AuthCallback {

    void onUserAuth(String code, String state);

    void onUserCancel(String state);

    void onUserDenied(String state);

    void onError(int errCode, String errStr, String state);
}
