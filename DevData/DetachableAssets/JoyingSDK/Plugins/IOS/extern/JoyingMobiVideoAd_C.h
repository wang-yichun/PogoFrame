//
//  JoyingMobiVideoAd_C.h
//  VideoSample
//
//  Created by Ethan.W on 16/7/11.
//  Copyright © 2016年 HuaiNan. All rights reserved.
//

#ifndef JoyingMobiVideoAd_C_h
#define JoyingMobiVideoAd_C_h


extern "C"
{
    
    void init();
    void initAppID(char* appId, char* appKey);
    void videoHasCanPlayVideo();
    void videoPlay_FullScreen();
    void videoPlay_CustomRect();
}

#endif /* JoyingMobiVideoAd_C_h */
