package com.wxapi.bridge;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Build;
import android.util.Log;

import com.tencent.mm.opensdk.modelmsg.SendAuth;
import com.tencent.mm.opensdk.modelmsg.SendMessageToWX;
import com.tencent.mm.opensdk.modelmsg.WXImageObject;
import com.tencent.mm.opensdk.modelmsg.WXMediaMessage;
import com.tencent.mm.opensdk.modelmsg.WXMusicObject;
import com.tencent.mm.opensdk.modelmsg.WXTextObject;
import com.tencent.mm.opensdk.openapi.IWXAPI;

import java.io.ByteArrayOutputStream;

public final class SendToWeChat {
    public final static String TAG = SendToWeChat.class.getName();
    private final IWXAPI api;

    public SendToWeChat(IWXAPI api){
        this.api = api;
    }

    /**
     * 发生文字信息给微信
     * @param content 文字内容
     * @param description 内容说明
     * @param mediaTagName 媒体信息标签
     * @param scene 目标场景
     */
    public void SendTextToWeChat(
            String content,
            String description,
            String mediaTagName,
            int scene){
        WXTextObject textObj = new WXTextObject();
        textObj.text = content;
        WXMediaMessage msg = new WXMediaMessage();
        msg.mediaObject = textObj;
        msg.description = description;
        msg.mediaTagName = mediaTagName;
        this.SendMessageToWX(msg, "text", scene);
    }

    /**
     * 发生图片到微信
     * @param bmp 图片实例
     * @param thumbSize 缩略图尺寸
     * @param scene 目标场景
     */
    public void SendImageToWeChat(
            Bitmap bmp,
            int thumbSize,
            int scene){
        WXImageObject imgObj = new WXImageObject(bmp);
        WXMediaMessage msg = new WXMediaMessage();
        msg.mediaObject = imgObj;
        //设置缩略图
        byte[] thumbData = bmpToByteArray(bmp, true, WXAPIManager.IMAGE_MAX_BYTES, thumbSize);
        bmp.recycle();
        msg.thumbData = thumbData;
        this.SendMessageToWX(msg, "img", scene);
    }

    /**
     * 发生本地图片到微信
     * @param path 本地图片地址
     * @param thumbSize 缩略图尺寸
     * @param scene 目标场景
     */
    public void SendImageToWeChat(
            String path,
            int thumbSize,
            int scene){
        WXImageObject imgObj = new WXImageObject();
        imgObj.setImagePath(path);
        WXMediaMessage msg = new WXMediaMessage();
        msg.mediaObject = imgObj;

        //设置缩略图
        Bitmap bmp = BitmapFactory.decodeFile(path);
        byte[] thumbData = bmpToByteArray(bmp, true, WXAPIManager.IMAGE_MAX_BYTES, thumbSize);
        bmp.recycle();
        msg.thumbData = thumbData;
        this.SendMessageToWX(msg, "img", scene);
    }

    /**
     * 发生音乐到微信
     * @param musicUrl 音乐网络地址
     * @param bmp 缩略图
     * @param title 抬头
     * @param description 说明
     * @param thumbSize 缩略图尺寸
     * @param scene 目标场景
     */
    public void SendMusicToWeChat(
            String musicUrl,
            Bitmap bmp,
            String title,
            String description,
            int thumbSize,
            int scene){
        WXMusicObject music = new WXMusicObject();
        music.musicUrl = musicUrl;
        WXMediaMessage msg = new WXMediaMessage();
        msg.mediaObject = music;
        msg.title = title;
        msg.description = description;
        byte[] thumbData = bmpToByteArray(bmp, true, WXAPIManager.IMAGE_MAX_BYTES, thumbSize);
        bmp.recycle();
        msg.thumbData = thumbData;
        this.SendMessageToWX(msg, "music", scene);
    }

    /**
     * 发生本地图片到微信
     * @param scopeArray 权限列表
     * @param state 外部应用本身用来标识其请求的唯一性，验证完成后，将由微信终端回传，限制长度不超过1KB
     */
    public void SendAuthToWeChat(
            String[] scopeArray,
            String state){
        final SendAuth.Req req = new SendAuth.Req();
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O){
            req.scope = String.join(",", scopeArray);
        }else {
            int length = scopeArray.length;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++) {
                if (i < length - 1){
                    sb.append(scopeArray[i] + ",");
                }else {
                    sb.append(scopeArray[i]);
                }
            }
            req.scope = sb.toString();
        }

        req.state = state;
        api.sendReq(req);
    }

    /**
     * 发生媒体信息到微信
     * @param msg 媒体信息实例
     * @param transactionType 请求事务ID
     * @param scene 目标场景
     */
    private void SendMessageToWX(WXMediaMessage msg, String transactionType, int scene){
        SendMessageToWX.Req req = new SendMessageToWX.Req();
        req.transaction = buildTransaction(transactionType);
        req.message = msg;
        req.scene = scene;
        api.sendReq(req);
    }

    /**
     * 构建请求事务ID字符串
     * @param type 事务类型
     * @return 请求事务ID字符串
     */
    private String buildTransaction(final String type) {
        return (type == null) ? String.valueOf(System.currentTimeMillis()) : type + System.currentTimeMillis();
    }

    public static byte[] bmpToByteArray(final Bitmap bmp, final boolean needRecycle, final int maxLength, int thumbSize) {
        byte[] thumbData;
        do {
            Bitmap thumbBmp = Bitmap.createScaledBitmap(bmp, thumbSize, thumbSize, true);
            thumbData = bmpToByteArray(thumbBmp, needRecycle);
            thumbSize -= 10;
        }
        while (thumbData.length > maxLength);
        return thumbData;
    }

    public static byte[] bmpToByteArray(final Bitmap bmp, final boolean needRecycle) {
        ByteArrayOutputStream output = new ByteArrayOutputStream();
        bmp.compress(Bitmap.CompressFormat.PNG, 100, output);
        if (needRecycle) {
            bmp.recycle();
        }

        byte[] result = output.toByteArray();
        try {
            output.close();
        } catch (Exception e) {
            e.printStackTrace();
            Log.e(TAG, "bmpToByteArray: ", e);
        }

        return result;
    }
}
