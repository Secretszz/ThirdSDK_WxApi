//
//  WeChatSDKManager.h
//  UnityFramework
//
//  Created by 晴天网络 on 2023/7/4.
//

#import "WXApiManager.h"
#import "WXApi.h"
#import "CommonApi.h"

@interface WeChatSDKManager : NSObject<WXApiManagerDelegate>{
    bool isInit;
}

@property (nonatomic, assign) U3DBridgeCallback_Success onSuccess;
@property (nonatomic, assign) U3DBridgeCallback_Cancel onCancel;
@property (nonatomic, assign) U3DBridgeCallback_Error onError;

// 初始化
-(void) initWeChat;

-(BOOL) isInstalledWeChat;

// 获取设备国家代码
-(NSString *) CountrylocaleIdenti;

-(void) shareImageToWX :(UIImage *) image;

+(enum WXScene)getWXScene:(int) sceneId;

+(instancetype)sharedManager;

-(BOOL)handleOpenURL:(NSURL *)url;

-(BOOL)handleOpenUniversalLink:(NSUserActivity *)userActivity;

@end
