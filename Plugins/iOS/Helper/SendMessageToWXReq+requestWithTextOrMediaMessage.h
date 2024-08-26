//
//  SendMessageToWXReq+requestWithTextOrMediaMessage.h
//  Unity-iPhone
//
//  Created by 晴天网络 on 2023/7/4.
//

#import "WXApiObject.h"

@interface SendMessageToWXReq (requestWithTextOrMediaMessage)

+ (SendMessageToWXReq *)requestWithText:(NSString *)text
                         OrMediaMessage:(WXMediaMessage *)message
                                  bText:(BOOL)bText
                                InScene:(enum WXScene)scene;
@end
