//
//  CSVideoAD.h
//  CSVideoADSDK
//
//  Created by Chance_yangjh on 15/1/8.
//  Copyright (c) 2015年 ChuKong-Inc. All rights reserved.
//

#ifndef CSVideoAD_h
#define CSVideoAD_h
#import <Foundation/Foundation.h>
#import "CSError.h"

/**
 *  查询积分时的block
 *
 *  @param taskCoins taskCoins中的元素为NSDictionary类型（taskCoins为空表示无积分返回，为nil表示查询出错）
 *                   键值说明：taskContent  NSString   任务名称
 *                            coins        NSNumber   赚得金币数量
 *  @param error   查询失败原因。只在taskCoins为nil时有效
 */
typedef void (^CSVideoADGetCoin)(NSArray *taskCoins, CSError *error);

@protocol CSVideoADDelegate;

@interface CSVideoAD : NSObject

// 广告位ID
@property (nonatomic, copy) NSString *placementID;
// 是否为激励模式（默认为YES）
@property (nonatomic, assign) BOOL rewardVideo;
// 是否隐藏金币相关的文本提示（只激励模式有效）
@property (nonatomic, assign) BOOL hideCoinLabel;
// 服务器对接时，积分回调时转发的信息。长度限制80（不建议使用中文符号）
@property (nonatomic, copy) NSString *userInfo;

@property (nonatomic, weak) id <CSVideoADDelegate> delegate;

+ (CSVideoAD *)sharedInstance;

/**
 *	@brief	查看是否有视频广告（只激励模式有效）
 */
- (void)queryVideoAD;

/**
 *  加载视频广告
 *
 *  @param portrait 视频广告方向
 *  @param onlyWifi 是否只在wifi情况下预加载视频文件
 */
- (void)loadVideoADWithOrientation:(BOOL)portrait
          andDownloadVideoOnlyWifi:(BOOL)onlyWifi;

/**
 *	@brief	展现视频广告
 *
 *  @param portrait 视频广告方向
 */
- (void)showVideoADWithOrientation:(BOOL)portrait;

/**
 *	@brief	查询是否有任务完成（只激励模式有效）
 *
 *	@param 	blockGetCoin 	查询积分时的block
 */
- (void)getCoins:(CSVideoADGetCoin)blockGetCoin;

/**
 *	@brief	清空视频文件缓存
 */
- (void)clearVideoCaches;

@end


@protocol CSVideoADDelegate <NSObject>

@optional

// 查询视频广告的结果
- (void)csVideoAD:(CSVideoAD *)csVideoAD receiveResultOfQuery:(CSError *)csError;

// 视频广告请求成功
- (void)csVideoADRequestVideoADSuccess:(CSVideoAD *)csVideoAD;

// 视频广告请求失败
- (void)csVideoAD:(CSVideoAD *)csVideoAD requestVideoADError:(CSError *)csError;

// 视频文件准备好了（注：不可以在这里播放视频广告）
// isCache为YES表示视频文件为缓存
- (void)csVideoAD:(CSVideoAD *)csVideoAD videoFileIsReady:(BOOL)isCache;

// 视频广告将要播放
// 返回YES表示播放视频广告，返回NO表示暂不播放。未实现按YES处理
- (BOOL)csVideoADWillPlayVideoAD:(CSVideoAD *)csVideoAD;

// 视频广告取消播放
- (void)csVideoADCancelPlayVideo:(CSVideoAD *)csVideoAD;

// 视频广告展现失败
- (void)csVideoAD:(CSVideoAD *)csVideoAD showVideoADError:(CSError *)csError;

// 视频广告展现成功，即开始播放视频广告
- (void)csVideoAD:(CSVideoAD *)csVideoAD showVideoADSuccess:(BOOL)replay;

// 点击视频广告的关闭按钮（close为YES表示广告会被关闭）
- (void)csVideoAD:(CSVideoAD *)csVideoAD clickCloseButtonAndWillClose:(BOOL)close;

// 视频广告退出播放
- (void)csVideoADExitPlayVideo:(CSVideoAD *)csVideoAD;

// 视频广告播放完成（广告不会自动关闭）（real为NO表示播放中点击关闭按钮触发的播放完成）
- (void)csVideoAD:(CSVideoAD *)csVideoAD replayVideoAD:(BOOL)replay realFinished:(BOOL)real;

// 视频广告播放过程中点击了下载按钮
- (void)csVideoADClickDownload:(CSVideoAD *)csVideoAD;

@end
#endif
