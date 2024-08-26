//
//  WXUtil.h
//  Unity-iPhone
//
//  Created by 晴天网络 on 2023/7/4.
//

@interface WXUtil : NSObject

+(UIImage *) compressImageDataSize:(UIImage *) image
                     compressRatio:(CGFloat) rotio;

+(UIImage *) compressImageDataSize:(UIImage *) image
                  targetDataLength:(NSUInteger) length;

@end
