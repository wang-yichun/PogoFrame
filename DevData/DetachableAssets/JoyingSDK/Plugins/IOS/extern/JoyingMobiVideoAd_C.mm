//
//  JoyingMobiVideoAd_C.m
//  VideoSample
//
//  Created by Ethan.W on 16/7/11.
//  Copyright © 2016年 HuaiNan. All rights reserved.
//

#import "JoyingMobiVideoAd.h"

@interface JoyingMobiVideoAd_C : NSObject

@end

@implementation JoyingMobiVideoAd_C

- (id)init
{
    id object = [super init];
    
    NSLog(@"JoyingMobiVideoAd_C init");
    
    return object;
}

@end

JoyingMobiVideoAd_C *m_JoyingMobiVideo_C = NULL;

extern "C"
{
    extern void UnitySendMessage(const char* obj, const char* method, const char* msg);
    
    void init () {
        if (m_JoyingMobiVideo_C == NULL) {
            m_JoyingMobiVideo_C = [[JoyingMobiVideoAd_C alloc] init];
        }
        
        NSLog(@"NSLog: init");
    }
    
    void initAppID (char* appId, char* appKey) {
        NSLog(@"NSLog: initAppID");
        NSString* appId_str = [NSString stringWithUTF8String:appId];
        NSString* appKey_str = [NSString stringWithUTF8String:appKey];
        
        NSLog(@"appId: %@", appId_str);
        NSLog(@"initAppID: %@", appKey_str);
        [JoyingMobiVideoAd initAppID:appId_str appKey:appKey_str];
    }
    
    void videoHasCanPlayVideo () {
        [JoyingMobiVideoAd videoHasCanPlayVideo:^(int isHaveVideoStatue) {
            NSString *isHaveVideoStatue_str = [NSString stringWithFormat:@"%d",isHaveVideoStatue];
            UnitySendMessage("Joying_Utility", "VideoHasCanPlayVideo_Callback", [isHaveVideoStatue_str UTF8String]);
        }];
    }
    
    void videoPlay_FullScreen() {
        [JoyingMobiVideoAd videoPlay:[[[UIApplication sharedApplication] keyWindow] rootViewController]
        videoPlayFinishCallBackBlock:^(BOOL isFinishPlay) {
            if (isFinishPlay){
                UnitySendMessage("Joying_Utility", "VideoPlay_Callback_isFinishPlay", "yes");
            } else {
                UnitySendMessage("Joying_Utility", "VideoPlay_Callback_isFinishPlay", "no");
            }
        }
        videoPlayConfigCallBackBlock:^(BOOL isLegal) {
            if (isLegal){
                UnitySendMessage("Joying_Utility", "VideoPlay_Callback_isLegal", "yes");
            } else {
                UnitySendMessage("Joying_Utility", "VideoPlay_Callback_isLegal", "no");
            }
        }];
    }
    
    void videoPlay_CustomRect() {

        UIViewController* controller = [[[UIApplication sharedApplication] keyWindow] rootViewController];
        
        [JoyingMobiVideoAd videoPlay:controller
                      videoSuperView:controller.view
                    videoPlayerFrame:CGRectMake(0, 40, controller.view.frame.size.width, (controller.view.frame.size.width)*4/7)
        videoPlayFinishCallBackBlock:^(BOOL isFinishPlay){
            if (isFinishPlay) {
                UnitySendMessage("Joying_Utility", "VideoPlay_Callback_isFinishPlay", "yes");
            } else {
                UnitySendMessage("Joying_Utility", "VideoPlay_Callback_isFinishPlay", "no");
            }
        }
        videoPlayConfigCallBackBlock:^(BOOL isLegal){
            if (isLegal) {
                UnitySendMessage("Joying_Utility", "VideoPlay_Callback_isLegal", "yes");
            } else {
                UnitySendMessage("Joying_Utility", "VideoPlay_Callback_isFinishPlay", "no");
            }
        }];
    }
}


