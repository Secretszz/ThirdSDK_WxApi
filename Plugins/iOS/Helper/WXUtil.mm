//
//  WXUtil.m
//  UnityFramework
//
//  Created by 晴天网络 on 2023/7/4.
//

#import <Foundation/Foundation.h>
#import "WXUtil.h"

@implementation WXUtil

+(UIImage *) compressImageDataSize:(UIImage *) image
                     compressRatio:(CGFloat) rotio{
    if (image){
        NSData * imageData = UIImageJPEGRepresentation(image, rotio);
        NSLog(@"Image size === %lu byte", imageData.length);
        if (imageData){
            UIImage * compressedImage = [UIImage imageWithData:imageData];
            return compressedImage;
        }
    }
    return nil;
}

+(UIImage *) compressImageDataSize:(UIImage *) image
                  targetDataLength:(NSUInteger) length{
    if (image){
        CGSize targetSize = CGSizeMake(image.size.width * 0.9, image.size.height * 0.9);
        UIGraphicsBeginImageContext(targetSize);
        [image drawInRect:CGRectMake(0, 0, targetSize.width, targetSize.height)];
        UIImage * compressedImage = UIGraphicsGetImageFromCurrentImageContext();
        UIGraphicsEndImageContext();
        NSData * imageData = UIImageJPEGRepresentation(compressedImage, 1.0);
        while (imageData.length >= length && targetSize.width > 10 && targetSize.height > 10) {
            targetSize.width *= 0.9;
            targetSize.height *= 0.9;
            UIGraphicsBeginImageContext(targetSize);
            [compressedImage drawInRect:CGRectMake(0, 0, targetSize.width, targetSize.height)];
            compressedImage = UIGraphicsGetImageFromCurrentImageContext();
            UIGraphicsEndImageContext();
            imageData = UIImageJPEGRepresentation(compressedImage, 1.0);
            NSLog(@"Image size === %lu byte", imageData.length);
        }
        return compressedImage;
    }
    return nil;
}

@end
