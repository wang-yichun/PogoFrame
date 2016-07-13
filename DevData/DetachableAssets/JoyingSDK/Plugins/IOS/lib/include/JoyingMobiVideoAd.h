//
//  Video.h
//  Video
//
//  Created by joying on 15/11/26.
//  Copyright (c) 2015年 Joying. All rights reserved.
//  version：3.0.5
//
#import <UIKit/UIKit.h>
@class JoyNativeView;
/*
 * 视频下载失败通知。userinfo有下载失败的视频的名字
 */
extern NSString *VideoFileDownloadFailNotification;

/*
 * 视频即将播放
 */
extern NSString *VideoWillPlayNotification;

/*
 * 播放过程中下载视频超时检测
 */
extern NSString *VideoDownloadOverTimeNotification;

/*
 * 全屏按钮按下后的通知，一般用于状态栏的隐藏与显示
 */
extern NSString *VideoFullButtonPressNotification;


@class BannerView;
@interface JoyingMobiVideoAd : UIWebView

#pragma mark - 公共接口

/**
 * 初始化appid和secretId
 * 您需要先在掌盈官网注册并创建一个应用，在项目初始化的时候调用本方法传入对应的值
 */
+(void)initAppID:(NSString *)appId appKey:(NSString *)appKey;

/*
 * 是否还有视频可以播放  ［可选］
 * isHaveVideoStatue的值目前有两个
 * 0：有视频可以播放
 * 1：暂时没有可播放视频
 * 2：网络状态不好
 */
+(void)videoHasCanPlayVideo:(void(^)(int isHaveVideoStatue))backCallBlock;

/*
 * 关闭视频  ［可选］
 */
+(void)videoCloseVideoPlayer;

/*
 * 点击关闭按钮不显示『中途退出没有奖励』弹出框  ［可选］ 默认显示
 */
+(void)videoCloseAlertViewWhenWantExit:(BOOL)isClose;



#pragma mark - 全屏播放视频的相关接口

/*
 *
 * 开始播放视频 ［全屏］ ［必须］
 *
 * 传入当前的viewController。视频将会以viewController presentMoviePlayerViewControllerAnimated:VideoController的方式呈现
 * Unity3D或者其他游戏引擎最好传入[[[UIApplication sharedApplication] keyWindow] rootViewController]
 *
 * VideoPlayFinishCallBackBlock是视频播放完成后马上回调,isFinishPlay为true则是用户完全播放，为false则是用户中途退出
 *
 * VideoPlayConfigCallBackBlock会在sdk的服务器最终确认这次播放是否有效后回掉（播放完成后有网络请求，网络不好可能有延时）。
 *
 * 注意：  isLegal在（app有联网，并且注册的appkey后台审核通过）的情况下才返回yes, 否则都是返回no.
 */
+(void)videoPlay:(UIViewController *)viewController videoPlayFinishCallBackBlock:(void(^)(BOOL isFinishPlay))block videoPlayConfigCallBackBlock:(void(^)(BOOL isLegal))configBlock;


#pragma mark - 非全屏，嵌入式视频

/*
 * 开始播放视频［非全屏］［必须］
 *
 * 传入当前的viewController。Unity3D或者其他游戏引擎最好传入[[[UIApplication shareApplication] keyWindow] rootViewController]
 * VideoPlayerFrame是视频的frame,superView是用于 ［addSubView：视频view］的，一般情况下可以传入viewController.view
 *
 * 注意：  isLegal在（app有联网，并且注册的appkey后台审核通过）的情况下才返回yes, 否则都是返回no.
 */
+(void)videoPlay:(UIViewController *)viewController videoSuperView:(UIView *)superView videoPlayerFrame:(CGRect)VideoPlayerFrame videoPlayFinishCallBackBlock:(void(^)(BOOL isFinish))block videoPlayConfigCallBackBlock:(void(^)(BOOL isLegal))configBlock;

#pragma mark - 插页视频
/**
 * 开始播放插页视频 ［插页］［只适用于竖屏］
 *
 * 传入当前viewController.superView是用于 ［addSubView：视频view］的，一般情况下可以传入viewController.view
 *
 */
+(void)videoSpotPlay:(UIViewController *)viewController videoSuperView:(UIView *)superView videoPlayFinishCallBackBlock:(void(^)(BOOL isFinish))block videoPlayConfigCallBackBlock:(void(^)(BOOL isLegal))configBlock;




/**
 * 设置是否强制横屏，默认是强制横屏 [全屏] ［可选］
 */
+(void)videoIsForceLandscape:(BOOL)isForce;

/**
 *
 *开始展示banner条［可选］
 *
 * VideoPlayerFrame是视频的frame,
 * closeBlock是关闭回调
 */
+(BannerView *)videoBannerPlayerFrame:(CGRect)VideoPlayerFrame videoBannerPlayCloseCallBackBlock:(void(^)(BOOL isLegal))closeBlock;



/*
 * 设置左旋转全屏还是右旋转全屏［非全屏］［可选］
 * 0:复原
 * 1:左旋转全屏
 * 2:右旋转全屏
 */
+(void)videoIslandscapeLeftOrRight:(int)vidoeShowStuts;

/*
 * 开发者需要自定义界面的时候用到，隐藏右下角的<全屏>按钮
 */
+(void)videoHiddenFullScreenButtonView;

/**
 *
 * 按钮事件      ［可选］
 *
 * 开发者需要自定义界面的时候用到，默认的广告界面的按钮跟“剩余多少秒”的view会隐藏.
 * 开发者需要自定义button与“剩余多少秒”的view
 */
+(void)videoHiddenDefaultButtonView;

/*
 * 剩余时间，还没开始播放时返回－1。开发者需要设置一个定时器。每秒获取一次
 */
+(int)videoGetRestPlayerTime;

/**
 * 关闭
 */
+(void)videobtnClosePressed;

/**
 * 静音或者开启
 */
+(void)videoButtonVoicePressed;

/**
 * 下载
 */
+(void)videoDownloadPressed;

/**
 * 全屏
 */
+(void)videoButtonFullScreenPressed;


/*
 *暂停视频播放
 */
+(void)pauseVideoPlay;

/*
 *继续视频播放
 */
+(void)continueVideoPlay;

/** 设置关闭弹出框的内容
 *  注：默认内容为（确认退出观看视频？）， 但是有些开发者需要做点不一样的提示，如：中途退出无法获得奖励哟～
 */
+(void)videosetCloseAlertContent:(NSString *)content;

/**
 * wifi模式下缓存视频文件（可选）
 * sdk已经加了wifi判断，开发者不用再做判断
 */
+(void)videoCacheVideoFile;

/**
 * 隐藏前贴片详情页的重放按钮 (可选)
 */
+(void)hideDetailViewReplayBtn:(BOOL)ynHide;

/**
 * 是否有可播放的缓存视频
 */
+(BOOL)videoIsCacheVideoFile;


/**
 * 设置播放界面视频下载超时时间 ［可选］ 
 * 默认15秒
 */
+(void)videoDownloadOverTime:(float)time;


#pragma mark - 原生信息流视频 ------>

/**
 *
 * 设置原生广告的tableview
 *
 */
+(void)videoInitNativeTableView:(UITableView *)tableView;

//是否可以显示信息流
+(BOOL)isHaveNative;

+(BOOL)isHaveNativeHeight;

+(void)relative;

/**
 * 关闭信息流
 *
 *
 */
+(void)closeNativeView;


/**
 * 获取信息流cell的高度
 *
 *
 */
+(float)getNativeCellHeight;

/**
 * 获取信息流视图
 *
 *
 */
+(JoyNativeView*)getNativeMovieView;

/**
 * 预加载
 *
 *
 */
+(void)preloadNative;

+(void)refreshNative;
+(void)videoInitRootViewController:(UIViewController *)viewController;
/**
 * 开始播放视频 ［原生视频］
 *
 * 传入当前viewController.
 *
 */
+(void)videoNativePlayFinishCallBackBlock:(void(^)(BOOL isFinish))block videoPlayConfigCallBackBlock:(void(^)(BOOL isLegal))configBlock;

#pragma mark - 原生固定视频 <-----

@end
