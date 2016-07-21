//
//  ChanceAdVideo.h
//  ChanceDemo
//
//  Created by PogoRock_Developer_02 on 16/7/14.
//  Copyright © 2016年 ChuKong-Inc. All rights reserved.
//

#ifndef ChanceAdVideo_h
#define ChanceAdVideo_h

extern "C"
{
    void init();
    
    void chance_init(char * publisherId, char * placementId);
    
    void playVideoAD();
    void loadCSVideoAD();
    void queryVideoAD();
}

#endif /* ChanceAdVideo_h */
