**PACKAGE**

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;

import com.tencent.mm.opensdk.modelbase.BaseReq;
import com.tencent.mm.opensdk.modelbase.BaseResp;
import com.tencent.mm.opensdk.openapi.IWXAPI;
import com.tencent.mm.opensdk.openapi.IWXAPIEventHandler;
import com.tencent.mm.opensdk.openapi.WXAPIFactory;
import com.bridge.wxapi.WXAPIManager;

public class WXPayEntryActivity extends Activity implements IWXAPIEventHandler {
    private IWXAPI api;
    public WXPayEntryActivity(){}

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        api = WXAPIFactory.createWXAPI(this, WXAPIManager.APP_ID, true);

        try {
            Intent intent = getIntent();
            api.handleIntent(intent, this);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    @Override
    protected void onNewIntent(Intent intent) {
        super.onNewIntent(intent);
        this.setIntent(intent);
        this.api.handleIntent(intent, this);
    }

    /**
     * App对微信发生消息中事件
     * @param req App对微信发送消息的消息实例
     */
    @Override
    public void onReq(BaseReq req) {
        WXAPIManager.getInstance().onEntryActivityReq(req);
        this.finish();
    }

    /**
     * 微信返回消息给APP事件
     * @param resp 微信返回给App的消息实例
     */
    @Override
    public void onResp(BaseResp resp) {
        WXAPIManager.getInstance().onEntryActivityResp(resp);
        this.finish();
    }
}
