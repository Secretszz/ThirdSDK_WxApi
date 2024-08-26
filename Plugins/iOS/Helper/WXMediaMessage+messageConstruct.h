//
//  WXMediaMessage+messageConstruct.h
//  UnityFramework
//
//  Created by 晴天网络 on 2023/7/4.
//

#import "WXApiObject.h"

@interface WXMediaMessage (messageConstruct)

+ (WXMediaMessage *)messageWithTitle:(NSString *)title
                         Description:(NSString *)description
                              Object:(id)mediaObject
                          MessageExt:(NSString *)messageExt
                       MessageAction:(NSString *)action
                          ThumbImage:(UIImage *)thumbImage
                            MediaTag:(NSString *)tagName;
@end
