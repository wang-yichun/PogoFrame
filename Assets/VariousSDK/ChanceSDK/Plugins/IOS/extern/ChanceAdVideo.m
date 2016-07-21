//
//  ChanceAdVideo.m
//  ChanceDemo
//
//  Created by PogoRock_Developer_02 on 16/7/14.
//  Copyright © 2016年 ChuKong-Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "ChanceAd.h"
//#import "CSInterstitial.h"
#import "CSVideoAD.h"
//#import "UnityInterface.h"
#import "CSSplashAd.h"


@interface ChanceAdVideo : NSObject<CSVideoADDelegate>
@end

@implementation ChanceAdVideo

- (id)init
{
    id object = [super init];
    
    NSLog(@"ChanceAdVideo init");
    
    return object;
}

#pragma mark - CSVideoADDelegate

// 查询视频广告的结果
- (void)csVideoAD:(CSVideoAD *)csVideoAD receiveResultOfQuery:(CSError *)error
{
    if (CSErrorCode_ExistVideoAD == error.code) {
     //   [[[UIAlertView alloc] initWithTitle:@"存在视频广告" message:error.localizedDescription delegate:nil cancelButtonTitle:nil otherButtonTitles:@"OK", nil] show];
        UnitySendMessage("Chance_Utility","VideoHasCanPlayVideo_Callback","yes");
    }
    else {
     //   [[[UIAlertView alloc] initWithTitle:@"视频广告不存在" message:error.localizedDescription delegate:nil cancelButtonTitle:nil otherButtonTitles:@"OK", nil] show];
            UnitySendMessage("Chance_Utility","VideoHasCanPlayVideo_Callback","no");
    }
}
// 视频广告请求成功
- (void)csVideoADRequestVideoADSuccess:(CSVideoAD *)csVideoAD
{
    NSLog(@"----------%s", __PRETTY_FUNCTION__);
//     UnitySendMessage("Chance_Utility","VideoHasCanPlayVideo_Callback","yes");
}

// 视频广告请求失败
- (void)csVideoAD:(CSVideoAD *)csVideoAD requestVideoADError:(CSError *)csError
{
    NSLog(@"----------%s  %@", __PRETTY_FUNCTION__, csError.localizedDescription);
//    UnitySendMessage("Chance_Utility","VideoHasCanPlayVideo_Callback","no");
}

// 视频文件准备好了（注：不可以在这里播放视频广告）
// isCache为YES表示视频文件为缓存
- (void)csVideoAD:(CSVideoAD *)csVideoAD videoFileIsReady:(BOOL)isCache
{
    if (isCache==YES) {
        UnitySendMessage("Chance_Utility","VideoLoadPlayVideo_Callback","yes");
    }
    else{
        UnitySendMessage("Chance_Utility","VideoLoadPlayVideo_Callback","no");

    }
}

// 视频广告将要播放
// 返回YES表示播放视频广告，返回NO表示暂不播放。未实现按YES处理
- (BOOL)csVideoADWillPlayVideoAD:(CSVideoAD *)csVideoAD
{
    NSLog(@"----------%s", __PRETTY_FUNCTION__);
    return YES;
}

// 视频广告被取消播放
- (void)csVideoADCancelPlayVideo:(CSVideoAD *)csVideoAD
{
    UnitySendMessage("Chance_Utility", "VideoPlay_Callback_isCompletePlay", "no");
}

// 视频广告展现失败
- (void)csVideoAD:(CSVideoAD *)csVideoAD showVideoADError:(CSError *)error
{
     UnitySendMessage("Chance_Utility","VideoHasCanPlayVideo_Callback","no");
}

// 视频广告展现成功，即开始播放视频广告
- (void)csVideoAD:(CSVideoAD *)csVideoAD showVideoADSuccess:(BOOL)replay
{
    NSLog(@"----------%s", __PRETTY_FUNCTION__);
}

// 点击视频广告的关闭按钮
- (void)csVideoAD:(CSVideoAD *)csVideoAD clickCloseButtonAndWillClose:(BOOL)close
{
    if(close){
        UnitySendMessage("Chance_Utility", "VideoPlay_Callback_isCompletePlay", "no");
    }

}

// 视频广告退出播放
- (void)csVideoADExitPlayVideo:(CSVideoAD *)csVideoAD
{
   //     UnitySendMessage("Joying_Utility", "VideoPlay_Callback_isCompletePlay", "yes");
}

// 视频广告播放完成（广告不会自动关闭）（real为NO表示播放中点击关闭按钮触发的播放完成）
- (void)csVideoAD:(CSVideoAD *)csVideoAD replayVideoAD:(BOOL)replay realFinished:(BOOL)real
{
    if (real == YES){
        UnitySendMessage("Chance_Utility", "VideoPlay_Callback_isCompletePlay", "yes");
    } else {
        UnitySendMessage("Chance_Utility", "VideoPlay_Callback_isCompletePlay", "no");
    }
}

// 视频广告播放过程中点击了下载按钮
- (void)csVideoADClickDownload:(CSVideoAD *)csVideoAD
{
    NSLog(@"----------%s", __PRETTY_FUNCTION__);
}


ChanceAdVideo *m_ChanceAdVideo = NULL;

#if defined(_cplusplus)
extern "C"{
#endif
    
    extern void UnitySendMessage(const char* obj, const char* method, const char* msg);
    
    void init () {
        if (m_ChanceAdVideo == NULL) {
            m_ChanceAdVideo = [[ChanceAdVideo alloc] init];
        }
        NSLog(@"NSLog: init");
    }
    
    void chance_init(char * publisherId, char * placementId) {
        
        [ChanceAd startSession:[NSString stringWithUTF8String:publisherId]];
        
        [CSSplashAd sharedInstance].placementID = [NSString stringWithUTF8String:placementId];
        
        [[CSSplashAd sharedInstance] showSplashInWindow: [[UIApplication sharedApplication] keyWindow]];
        
        [CSVideoAD sharedInstance].delegate = m_ChanceAdVideo;
    }
    
    void queryVideoAD(){
        [[CSVideoAD sharedInstance] queryVideoAD];
    }
    
    void loadCSVideoAD(){
        [[CSVideoAD sharedInstance] loadVideoADWithOrientation:true
                                      andDownloadVideoOnlyWifi:true];
    }
    
    void playVideoAD(){
        // 请求视频广告
        [CSVideoAD sharedInstance].rewardVideo = YES;
        [[CSVideoAD sharedInstance] showVideoADWithOrientation:YES];
    }
#if defined(_cplusplus)
}
#endif

@end
