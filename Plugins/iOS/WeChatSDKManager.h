//
//  WeChatSDKManager.h
//  UnityFramework
//
//  Created by 晴天网络 on 2023/7/4.
//

#import "WXApiManager.h"
#import "WXApi.h"

typedef void(*WXAPIU3DBridgeCallback_onPayCallback)(int code, const char* message);
typedef void(*WXAPIU3DBridgeCallback_onShareCallback)(bool success, const char* message);
typedef void(*WXAPIU3DBridgeCallback_onAuthCallback)(int errCode, const char* errStr, const char* code, const char* state);

@interface WeChatSDKManager : NSObject<WXApiManagerDelegate>{
    bool isInit;
}

@property (nonatomic, assign) WXAPIU3DBridgeCallback_onPayCallback onPayCallback;
@property (nonatomic, assign) WXAPIU3DBridgeCallback_onShareCallback onShareCallback;
@property (nonatomic, assign) WXAPIU3DBridgeCallback_onAuthCallback onAuthCallback;

// 初始化
-(void) initWeChat:(NSString *) appId
     universalLink:(NSString *) universalLink;

-(BOOL) isInstalledWeChat;

// 获取设备国家代码
-(NSString *) CountrylocaleIdenti;

-(void) shareCompletion:(BOOL) success;

-(void) shareImageToWX :(UIImage *) image;

+(enum WXScene)getWXScene:(int) sceneId;

+(instancetype)sharedManager;

-(BOOL)handleOpenURL:(NSURL *)url;

-(BOOL)handleOpenUniversalLink:(NSUserActivity *)userActivity;

@end
