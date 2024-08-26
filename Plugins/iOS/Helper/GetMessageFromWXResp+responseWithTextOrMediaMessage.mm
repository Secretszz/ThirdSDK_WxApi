//
//  GetMessageFromWXResp+responseWithTextOrMediaMessage.m
//  UnityFramework
//
//  Created by 晴天网络 on 2023/7/4.
//

#import "GetMessageFromWXResp+responseWithTextOrMediaMessage.h"

@implementation GetMessageFromWXResp (responseWithTextOrMediaMessage)

+ (GetMessageFromWXResp *)responseWithText:(NSString *)text
                         OrMediaMessage:(WXMediaMessage *)message
                                  bText:(BOOL)bText {
    GetMessageFromWXResp *resp = [[GetMessageFromWXResp alloc] init];
    resp.bText = bText;
    if (bText)
        resp.text = text;
    else
        resp.message = message;
    return resp;
}

@end
