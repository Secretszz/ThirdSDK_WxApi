**PACKAGE**

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;

import com.tencent.mm.opensdk.modelbase.BaseReq;
import com.tencent.mm.opensdk.modelbase.BaseResp;
import com.tencent.mm.opensdk.openapi.IWXAPI;
import com.tencent.mm.opensdk.openapi.IWXAPIEventHandler;
import com.tencent.mm.opensdk.openapi.WXAPIFactory;
import com.wxapi.bridge.WXAPIManager;

public class WXEntryActivity extends Activity implements IWXAPIEventHandler{
    private IWXAPI api;
    public WXEntryActivity(){}

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        api = WXAPIFactory.createWXAPI(this, WXAPIManager.APP_ID, false);

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
        setIntent(intent);
        api.handleIntent(intent, this);
    }

    @Override
    public void onReq(BaseReq req) {
        WXAPIManager.getInstance().onEntryActivityReq(req);
        this.finish();
    }

    @Override
    public void onResp(BaseResp resp) {
        WXAPIManager.getInstance().onEntryActivityResp(resp);
        this.finish();
    }
}