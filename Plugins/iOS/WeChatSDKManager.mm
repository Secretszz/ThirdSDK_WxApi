//
//  WeChatSDKManager.m
//  UnityFramework
//
//  Created by 晴天网络 on 2023/7/4.
//

#import "WeChatSDKManager.h"
#import "WXApiRequestHandler.h"
#import "WXApi.h"
#import "WXUtil.h"
#import "JsonUtil.h"
#import "CommonApi.h"

@implementation WeChatSDKManager

#pragma mark - LifeCycle
+(instancetype)sharedManager {
    static dispatch_once_t onceToken;
    static WeChatSDKManager *instance;
    dispatch_once(&onceToken, ^{
        instance = [[WeChatSDKManager alloc] init];
    });
    return instance;
}

#pragma mark - Delegate

// 初始化
-(void) initWeChat{
    if(isInit){
        return;
    }
    isInit = true;
    NSString * appId = @"**APPID**";
    NSString * universalLink = @"**UNILINK**";
    [WXApi startLogByLevel:WXLogLevelDetail logBlock:^(NSString * _Nonnull log) {
        NSLog(@"===WechatSDK:%@", log);
    }];
    
    //向微信注册
    [WXApi registerApp:appId universalLink:universalLink];
    
//    [WXApi checkUniversalLinkReady:^(WXULCheckStep step, WXCheckULStepResult * _Nonnull result) {
//        NSLog(@"===%@,%u,%@,%@",@(step),result.success,result.errorInfo,result.suggestion);
//    }];
    
    NSString * msgStr = [NSString stringWithFormat:@"==initWeChat==appID=%@,universalLink=%@",appId,universalLink];
    NSLog(@"%@",msgStr);
}

-(BOOL) isInstalledWeChat{
    return [WXApi isWXAppInstalled];
}

-(BOOL)handleOpenURL:(NSURL *)url{
    return [WXApi handleOpenURL:url delegate:[WXApiManager sharedManager]];
}

-(BOOL)handleOpenUniversalLink:(NSUserActivity *)userActivity{
    return [WXApi handleOpenUniversalLink:userActivity delegate:[WXApiManager sharedManager]];
}

// 获取设备国家代码
-(NSString *) CountrylocaleIdenti{
    NSLocale *locale=[NSLocale currentLocale];
    NSString * currectLocaleCountryCode=[[NSLocale currentLocale] objectForKey:NSLocaleCountryCode];
    NSArray * countryArray=[NSLocale ISOCountryCodes];
    for(NSString* countryCode in countryArray)
    {
        if([countryCode isEqualToString:(currectLocaleCountryCode)])
        {
            return [locale displayNameForKey:NSLocaleCountryCode value:(countryCode)];
        }
    }
    return currectLocaleCountryCode;
}

+(enum WXScene)getWXScene:(int) sceneId{
    if (sceneId == 0) {
        return WXSceneSession;
    }
    else if (sceneId == 1){
        return WXSceneTimeline;
    }
    else if (sceneId == 2){
        return WXSceneFavorite;
    }
    else if (sceneId == 3){
        return WXSceneSpecifiedSession;
    }
    else if (sceneId == 4){
        return WXSceneState;
    }
    return WXSceneSession;
}

-(void) shareImageToWX :(UIImage *) image{
    NSMutableArray* sharingItems = [NSMutableArray new];
    
    [sharingItems addObject:image];
    
    UIActivityViewController *activityViewController =[[UIActivityViewController alloc] initWithActivityItems:sharingItems applicationActivities:nil];
    activityViewController.popoverPresentationController.sourceView = UnityGetGLViewController().view;
    activityViewController.popoverPresentationController.sourceRect = CGRectMake(UnityGetGLViewController().view.frame.size.width/2, UnityGetGLViewController().view.frame.size.height/4, 0, 0);
    activityViewController.excludedActivityTypes = @[ UIActivityTypePostToFacebook, // 脸书
                                                      UIActivityTypePostToTwitter, // 推特
                                                      UIActivityTypePostToWeibo, // 微博
                                                      UIActivityTypeMessage, // 信息
                                                      UIActivityTypeMail, // 邮件
                                                      UIActivityTypePrint, // 打印
                                                      UIActivityTypeCopyToPasteboard,
                                                      UIActivityTypeAssignToContact,
                                                      UIActivityTypeSaveToCameraRoll,
                                                      UIActivityTypeAddToReadingList,
                                                      UIActivityTypePostToFlickr,
                                                      UIActivityTypePostToVimeo,
                                                      UIActivityTypePostToTencentWeibo,
                                                      UIActivityTypeAirDrop,
                                                      UIActivityTypeOpenInIBooks,
                                                      @"com.burbn.instagram.shareextension"
    ];
    
    UIActivityViewControllerCompletionWithItemsHandler myblock = ^(UIActivityType _Nullable activitytype, BOOL completed, NSArray * __nullable returnedItems, NSError * __nullable activityError){
        
        NSLog(@"分享渠道%@", activitytype);
        if (completed) {
            NSLog(@"分享成功");
            if (self.onSuccess != nil) {
                self.onSuccess([activitytype UTF8String]);
            }
        }else{
            NSLog(@"分享失败");
            if (self.onError != nil) {
                self.onError(-1, [activitytype UTF8String]);
            }
        }
    };
    activityViewController.completionWithItemsHandler = myblock;

    [UnityGetGLViewController() presentViewController:activityViewController animated:YES completion:nil];
}

- (void)managerDidRecvShowMessageReq:(ShowMessageFromWXReq *)request{

}

- (void)managerDidRecvLaunchFromWXReq:(LaunchFromWXReq *)request{
    
}

- (void)managerDidRecvMessageResponse:(SendMessageToWXResp *)response{
    NSLog(@"code===%d, msg===%@, type===%d", response.errCode, response.errStr, response.type);
    if (response.errCode == WXSuccess) {
        self.onSuccess([response.errStr UTF8String]);
    } else if (response.errCode == WXErrCodeUserCancel){
        self.onCancel();
    } else{
        self.onError(response.errCode, [response.errStr UTF8String]);
    }
}

- (void)managerDidRecvAuthResponse:(SendAuthResp *)response{
    
}

- (void)managerDidRecvAddCardResponse:(AddCardToWXCardPackageResp *)response{
    
}

- (void)managerDidRecvChooseCardResponse:(WXChooseCardResp *)response{
    
}

- (void)managerDidRecvChooseInvoiceResponse:(WXChooseInvoiceResp *)response{
    
}

- (void)managerDidRecvSubscribeMsgResponse:(WXSubscribeMsgResp *)response{
    
}

- (void)managerDidRecvLaunchMiniProgram:(WXLaunchMiniProgramResp *)response{
    
}

- (void)managerDidRecvInvoiceAuthInsertResponse:(WXInvoiceAuthInsertResp *)response{
    
}

- (void)managerDidRecvNonTaxpayResponse:(WXNontaxPayResp *)response{
    
}

- (void)managerDidRecvPayInsuranceResponse:(WXPayInsuranceResp *)response{
    
}

- (void)managerDidRecvPayResponse:(PayResp *)response{
    NSLog(@"code===%d, msg===%@, type===%d", response.errCode, response.errStr, response.type);
    if (response.errCode == WXSuccess) {
        self.onSuccess([response.errStr UTF8String]);
    } else if (response.errCode == WXErrCodeUserCancel){
        self.onCancel();
    } else{
        self.onError(response.errCode, [response.errStr UTF8String]);
    }
}

@end

#if __cplusplus
extern "C" {
#endif
    /**
     初始化微信SDK
     */
    void wx_init() {
		return [[WeChatSDKManager sharedManager] initWeChat];
    }

    /**
     打开微信客服
     @param corpId 企业ID
     @param fkid 客服ID
     */
    bool wx_openCustomerServiceChat(const char * corpId, const char * kfid){
        
        NSString *strCorpId = [NSString stringWithUTF8String:corpId];
        NSString *strKfid = [NSString stringWithUTF8String:kfid];
        NSString * url = [NSString stringWithFormat:@"https://work.weixin.qq.com/kfid/%@", strKfid];
        [WXApiRequestHandler openCustomerServiceChat:strCorpId url:url completion:nil];
        return true;
    }

    /**
     is installed WeChat App
     */
    BOOL wx_isWXAppInstalled() {
        return [[WeChatSDKManager sharedManager] isInstalledWeChat];
    }
    
    void wx_Purchase(const char * orderInfo, U3DBridgeCallback_Success onSuccess, U3DBridgeCallback_Cancel onCancel, U3DBridgeCallback_Error onError){
        NSString* jParams = [NSString stringWithUTF8String:orderInfo];
        NSData* jsonData = [jParams dataUsingEncoding:NSUTF8StringEncoding];
        NSError* error;
        NSDictionary* dict = [NSJSONSerialization JSONObjectWithData:jsonData options:NSJSONReadingMutableContainers error:&error];
        if (error){
            NSLog(@"解析json错误：%@, %@", jParams, error);
            onError(-1, "解析订单错误");
            return;
        }
        [WeChatSDKManager sharedManager].onSuccess = onSuccess;
        [WeChatSDKManager sharedManager].onCancel = onCancel;
        [WeChatSDKManager sharedManager].onError = onError;
        [WXApiRequestHandler sendPayRequest:dict completion:nil];
    }

    /**
     Share image with device local url
     */
    void wx_shareImage(const char * imagePath, int scene, U3DBridgeCallback_Success onSuccess, U3DBridgeCallback_Cancel onCancel, U3DBridgeCallback_Error onError){
        [WeChatSDKManager sharedManager].onSuccess = onSuccess;
        [WeChatSDKManager sharedManager].onCancel = onCancel;
        [WeChatSDKManager sharedManager].onError = onError;
        
        NSString *strImagePath = [NSString stringWithUTF8String:imagePath];
        // NSLog(@"imagePath===%@", strImagePath);
        UIImage * image = [UIImage imageWithContentsOfFile:strImagePath];
        
        //[WeChatSDKManager.sharedManager shareImageToWX:image];
        
        NSData * imageData = UIImageJPEGRepresentation(image, 1.0);
        NSLog(@"share image path data size === %lu byte", imageData.length);
        NSString * tagName = @"战斗少女跑酷";
        NSString * messgaeExt = @"";
        NSString * action = @"";
        UIImage * thumbImage = [WXUtil compressImageDataSize:image targetDataLength:65534];
        WXScene wxScene = [WeChatSDKManager getWXScene:scene];
        [WXApiRequestHandler sendImageData:imageData TagName:tagName MessageExt:messgaeExt Action:action ThumbImage:thumbImage InScene:wxScene completion:^(BOOL success) {
            if (success) {
                WeChatSDKManager.sharedManager.onSuccess("");
            } else{
                WeChatSDKManager.sharedManager.onError(-1, "");
            }
        }];
    }
    
	/**
	 share image whit image byte[] data
	 */
    void wx_shareImageWithDatas(const Byte* datas, int length, int scene, U3DBridgeCallback_Success onSuccess, U3DBridgeCallback_Cancel onCancel, U3DBridgeCallback_Error onError){
        [WeChatSDKManager sharedManager].onSuccess = onSuccess;
        [WeChatSDKManager sharedManager].onCancel = onCancel;
        [WeChatSDKManager sharedManager].onError = onError;
        
        NSData * imageData = [NSData dataWithBytes:datas length:length];
        // NSLog(@"share image datas data size === %lu byte", imageData.length);
        UIImage * image = [UIImage imageWithData:imageData];
        
        //[WeChatSDKManager.sharedManager shareImageToWX:image];
        
        NSString * tagName = @"战斗少女跑酷";
        NSString * messgaeExt = @"";
        NSString * action = @"";
        UIImage * thumbImage = [WXUtil compressImageDataSize:image targetDataLength:65534];
        WXScene wxScene = [WeChatSDKManager getWXScene:scene];
        [WXApiRequestHandler sendImageData:imageData TagName:tagName MessageExt:messgaeExt Action:action ThumbImage:thumbImage InScene:wxScene completion:^(BOOL success) {
            if (success) {
                WeChatSDKManager.sharedManager.onSuccess("");
            } else{
                WeChatSDKManager.sharedManager.onError(-1, "");
            }
        }];
    }
    
    void wx_Auth(U3DBridgeCallback_Success onSuccess, U3DBridgeCallback_Cancel onCancel, U3DBridgeCallback_Error onError){
        [WeChatSDKManager sharedManager].onSuccess = onSuccess;
        [WeChatSDKManager sharedManager].onCancel = onCancel;
        [WeChatSDKManager sharedManager].onError = onError;
        
    }
#if __cplusplus
}
#endif
