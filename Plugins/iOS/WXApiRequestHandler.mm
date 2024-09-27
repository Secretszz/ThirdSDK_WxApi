//
//  WXApiRequestHandler.cpp
//  Unity-iPhone
//
//  Created by 晴天网络 on 2023/7/4.
//

#import "WXApi.h"
#import "WXApiRequestHandler.h"
#import "WXApiManager.h"
#import "SendMessageToWXReq+requestWithTextOrMediaMessage.h"
#import "WXMediaMessage+messageConstruct.h"

@implementation WXApiRequestHandler

#pragma mark - Public Methods
+ (void)sendText:(NSString *)text
         InScene:(enum WXScene)scene
      completion:(void (^ __nullable)(BOOL success))completion{
    SendMessageToWXReq *req = [SendMessageToWXReq requestWithText:text
                                                   OrMediaMessage:nil
                                                            bText:YES
                                                          InScene:scene];
    [WXApi sendReq:req completion:completion];
}

+ (void)sendImageData:(NSData *)imageData
              TagName:(NSString *)tagName
           MessageExt:(NSString *)messageExt
               Action:(NSString *)action
           ThumbImage:(UIImage *)thumbImage
              InScene:(enum WXScene)scene
           completion:(void (^ __nullable)(BOOL success))completion{
    WXImageObject *ext = [WXImageObject object];
    ext.imageData = imageData;
    
    WXMediaMessage *message = [WXMediaMessage messageWithTitle:nil
                                                   Description:nil
                                                        Object:ext
                                                    MessageExt:messageExt
                                                 MessageAction:action
                                                    ThumbImage:thumbImage
                                                      MediaTag:tagName];
    
    SendMessageToWXReq* req = [SendMessageToWXReq requestWithText:nil
                                                   OrMediaMessage:message
                                                            bText:NO
                                                          InScene:scene];
    
    [WXApi sendReq:req completion:completion];
}

+ (void)sendLinkURL:(NSString *)urlString
            TagName:(NSString *)tagName
              Title:(NSString *)title
        Description:(NSString *)description
         ThumbImage:(UIImage *)thumbImage
            InScene:(enum WXScene)scene
         completion:(void (^ __nullable)(BOOL success))completion{
    WXWebpageObject *ext = [WXWebpageObject object];
    ext.webpageUrl = urlString;

    WXMediaMessage *message = [WXMediaMessage messageWithTitle:title
                                                   Description:description
                                                        Object:ext
                                                    MessageExt:nil
                                                 MessageAction:nil
                                                    ThumbImage:thumbImage
                                                      MediaTag:tagName];
    
    SendMessageToWXReq* req = [SendMessageToWXReq requestWithText:nil
                                                   OrMediaMessage:message
                                                            bText:NO
                                                          InScene:scene];
    [WXApi sendReq:req completion:completion];
}

+ (void)sendMusicURL:(NSString *)musicURL
             dataURL:(NSString *)dataURL
               Title:(NSString *)title
         Description:(NSString *)description
          ThumbImage:(UIImage *)thumbImage
             InScene:(enum WXScene)scene
          completion:(void (^ __nullable)(BOOL success))completion{
    WXMusicObject *ext = [WXMusicObject object];
    ext.musicUrl = musicURL;
    ext.musicDataUrl = dataURL;

    WXMediaMessage *message = [WXMediaMessage messageWithTitle:title
                                                   Description:description
                                                        Object:ext
                                                    MessageExt:nil
                                                 MessageAction:nil
                                                    ThumbImage:thumbImage
                                                      MediaTag:nil];
    
    SendMessageToWXReq* req = [SendMessageToWXReq requestWithText:nil
                                                   OrMediaMessage:message
                                                            bText:NO
                                                          InScene:scene];
    
    [WXApi sendReq:req completion:completion];
}

+ (void)sendVideoURL:(NSString *)videoURL
               Title:(NSString *)title
         Description:(NSString *)description
          ThumbImage:(UIImage *)thumbImage
             InScene:(enum WXScene)scene
          completion:(void (^ __nullable)(BOOL success))completion{
    WXMediaMessage *message = [WXMediaMessage message];
    message.title = title;
    message.description = description;
    [message setThumbImage:thumbImage];
    
    WXVideoObject *ext = [WXVideoObject object];
    ext.videoUrl = videoURL;
    
    message.mediaObject = ext;
    
    SendMessageToWXReq* req = [SendMessageToWXReq requestWithText:nil
                                                   OrMediaMessage:message
                                                            bText:NO
                                                          InScene:scene];
    [WXApi sendReq:req completion:completion];
}

+ (void)sendEmotionData:(NSData *)emotionData
             ThumbImage:(UIImage *)thumbImage
                InScene:(enum WXScene)scene
             completion:(void (^ __nullable)(BOOL success))completion{
    WXMediaMessage *message = [WXMediaMessage message];
    [message setThumbImage:thumbImage];
    
    WXEmoticonObject *ext = [WXEmoticonObject object];
    ext.emoticonData = emotionData;
    
    message.mediaObject = ext;
    
    SendMessageToWXReq* req = [SendMessageToWXReq requestWithText:nil
                                                   OrMediaMessage:message
                                                            bText:NO
                                                          InScene:scene];
    [WXApi sendReq:req completion:completion];
}

+ (void)sendFileData:(NSData *)fileData
       fileExtension:(NSString *)extension
               Title:(NSString *)title
         Description:(NSString *)description
          ThumbImage:(UIImage *)thumbImage
             InScene:(enum WXScene)scene
          completion:(void (^ __nullable)(BOOL success))completion{
    WXMediaMessage *message = [WXMediaMessage message];
    message.title = title;
    message.description = description;
    [message setThumbImage:thumbImage];
    
    WXFileObject *ext = [WXFileObject object];
    ext.fileExtension = @"pdf";
    ext.fileData = fileData;
    
    message.mediaObject = ext;
    
    SendMessageToWXReq* req = [SendMessageToWXReq requestWithText:nil
                                                   OrMediaMessage:message
                                                            bText:NO
                                                          InScene:scene];
    [WXApi sendReq:req completion:completion];
}

+ (void)sendMiniProgramWebpageUrl:(NSString *)webpageUrl
                         userName:(NSString *)userName
                             path:(NSString *)path
                            title:(NSString *)title
                      Description:(NSString *)description
                       ThumbImage:(UIImage *)thumbImage
                      hdImageData:(NSData *)hdImageData
                  withShareTicket:(BOOL)withShareTicket
                  miniProgramType:(WXMiniProgramType)programType
                          InScene:(enum WXScene)scene
                       completion:(void (^ __nullable)(BOOL success))completion
{
    WXMiniProgramObject *ext = [WXMiniProgramObject object];
    ext.webpageUrl = webpageUrl;
    ext.userName = userName;
    ext.path = path;
    ext.hdImageData = hdImageData;
    ext.withShareTicket = withShareTicket;
    ext.miniProgramType = programType;
    
    WXMediaMessage *message = [WXMediaMessage messageWithTitle:title
                                                   Description:description
                                                        Object:ext
                                                    MessageExt:nil
                                                 MessageAction:nil
                                                    ThumbImage:thumbImage
                                                      MediaTag:nil];
    
    SendMessageToWXReq* req = [SendMessageToWXReq requestWithText:nil
                                                   OrMediaMessage:message
                                                            bText:NO
                                                          InScene:scene];
    
    [WXApi sendReq:req completion:completion];
}

+ (void)launchMiniProgramWithUserName:(NSString *)userName
                                 path:(NSString *)path
                                 type:(WXMiniProgramType)miniProgramType
                           completion:(void (^ __nullable)(BOOL success))completion
{
    WXLaunchMiniProgramReq *launchMiniProgramReq = [WXLaunchMiniProgramReq object];
    launchMiniProgramReq.userName = userName;
    launchMiniProgramReq.path = path;
    launchMiniProgramReq.miniProgramType = miniProgramType;
    
    return  [WXApi sendReq:launchMiniProgramReq completion:completion];
}



+ (void)sendAppContentData:(NSData *)data
                   ExtInfo:(NSString *)info
                    ExtURL:(NSString *)url
                     Title:(NSString *)title
               Description:(NSString *)description
                MessageExt:(NSString *)messageExt
             MessageAction:(NSString *)action
                ThumbImage:(UIImage *)thumbImage
                   InScene:(enum WXScene)scene
                completion:(void (^ __nullable)(BOOL success))completion{
    WXAppExtendObject *ext = [WXAppExtendObject object];
    ext.extInfo = info;
    ext.url = url;
    ext.fileData = data;

    WXMediaMessage *message = [WXMediaMessage messageWithTitle:title
                                                   Description:description
                                                        Object:ext
                                                    MessageExt:messageExt
                                                 MessageAction:action
                                                    ThumbImage:thumbImage
                                                      MediaTag:nil];
    
    SendMessageToWXReq* req = [SendMessageToWXReq requestWithText:nil
                                                   OrMediaMessage:message
                                                            bText:NO
                                                          InScene:scene];
    [WXApi sendReq:req completion:completion];

}

+ (void)addCardsToCardPackage:(NSArray *)cardIds
                     cardExts:(NSArray *)cardExts
                        appID:(NSString *)appID
                   completion:(void (^ __nullable)(BOOL success))completion
{
    NSMutableArray *cardItems = [NSMutableArray array];
    for (NSString *cardId in cardIds) {
        WXCardItem *item = [[WXCardItem alloc] init];
        item.cardId = cardId;
        item.appID = appID;
        [cardItems addObject:item];
    }
    
    for (NSInteger index = 0; index < cardItems.count; index++) {
        WXCardItem *item = cardItems[index];
        NSString *ext = cardExts[index];
        item.extMsg = ext;
    }
    
    AddCardToWXCardPackageReq *req = [[AddCardToWXCardPackageReq alloc] init];
    req.cardAry = cardItems;
    [WXApi sendReq:req completion:completion];
}

+ (void)chooseCard:(NSString *)appid
          cardSign:(NSString *)cardSign
          nonceStr:(NSString *)nonceStr
          signType:(NSString *)signType
         timestamp:(UInt32)timestamp
        completion:(void (^ __nullable)(BOOL success))completion
{
    WXChooseCardReq *chooseCardReq = [[WXChooseCardReq alloc] init];
    chooseCardReq.appID = appid;
    chooseCardReq.cardSign = cardSign;
    chooseCardReq.nonceStr = nonceStr;
    chooseCardReq.signType = signType;
    chooseCardReq.timeStamp = timestamp;
    [WXApi sendReq:chooseCardReq completion:completion];
    
}

+ (void)sendAuthRequestScope:(NSString *)scope
                       State:(NSString *)state
                      OpenID:(NSString *)openID
            InViewController:(UIViewController *)viewController
                  completion:(void (^ __nullable)(BOOL success))completion
{
    SendAuthReq* req = [[SendAuthReq alloc] init];
    req.scope = scope; // @"post_timeline,sns"
    req.state = state;
    req.openID = openID;
    
    [WXApi sendAuthReq:req
        viewController:viewController
              delegate:[WXApiManager sharedManager]
            completion:completion];
}

+ (void)openUrl:(NSString *)url
     completion:(void (^ __nullable)(BOOL success))completion
{
    OpenWebviewReq *req = [[OpenWebviewReq alloc] init];
    req.url = url;
    [WXApi sendReq:req completion:completion];
}

+ (void)chooseInvoice:(NSString *)appid
             cardSign:(NSString *)cardSign
             nonceStr:(NSString *)nonceStr
             signType:(NSString *)signType
            timestamp:(UInt32)timestamp
           completion:(void (^ __nullable)(BOOL success))completion
{
    WXChooseInvoiceReq *chooseInvoiceReq = [[WXChooseInvoiceReq alloc] init];
    chooseInvoiceReq.appID = appid;
    chooseInvoiceReq.cardSign = cardSign;
    chooseInvoiceReq.nonceStr = nonceStr;
    chooseInvoiceReq.signType = signType;
//    chooseCardReq.cardType = @"INVOICE";
    chooseInvoiceReq.timeStamp = timestamp;
//    chooseCardReq.canMultiSelect = 1;
    [WXApi sendReq:chooseInvoiceReq completion:completion];
}

+ (void)openCustomerServiceChat:(NSString *)corpId
                            url:(NSString *)url
                     completion:(void (^ __nullable)(BOOL success))completion{
    WXOpenCustomerServiceReq *req = [[WXOpenCustomerServiceReq alloc] init];
    req.corpid = corpId;
    req.url = url;
    [WXApi sendReq:req completion:completion];
}

+ (void)sendPayRequest:(NSDictionary<NSString*, NSString*> *)orderInfo
            completion:(void (^ __nullable)(BOOL success))completion{
	PayReq *req = [[PayReq alloc] init];
    req.partnerId = [orderInfo objectForKey:@"partnerid"]; //商户ID
    req.prepayId = [orderInfo objectForKey:@"prepayid"]; //预支付订单
    req.package = [orderInfo objectForKey:@"package"];
    req.nonceStr = [orderInfo objectForKey:@"noncestr"]; //异步通知url
    NSString* timestamp = [orderInfo objectForKey:@"timestamp"];
    UInt32 num;
    sscanf([timestamp UTF8String], "%u", &num);
    req.timeStamp = num;
    req.sign = [orderInfo objectForKey:@"sign"]; //签名由服务器加签后返回，无需客户端处理
    [WXApi sendReq:req completion:completion];
}
@end

