//
//  GetMessageFromWXResp+responseWithTextOrMediaMessage.h
//  UnityFramework
//
//  Created by 晴天网络 on 2023/7/4.
//

#import "WXApiObject.h"

@interface GetMessageFromWXResp (responseWithTextOrMediaMessage)

+ (GetMessageFromWXResp *)responseWithText:(NSString *)text
                            OrMediaMessage:(WXMediaMessage *)message
                                     bText:(BOOL)bText;
@end
