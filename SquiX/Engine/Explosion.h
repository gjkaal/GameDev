
#pragma once
#include "stdint.h"

class Explosion
{
public:
// frame n = frame_00 starting at position [0];
// frame n = frame_01 starting at position [4096];
// frame n = frame_02 starting at position [8192];
// frame n = frame_03 starting at position [12288];
// frame n = frame_04 starting at position [16384];
// frame n = frame_05 starting at position [20480];
// frame n = frame_06 starting at position [24576];
// frame n = frame_07 starting at position [28672];
// frame n = frame_08 starting at position [32768];
// frame n = frame_09 starting at position [36864];
// frame n = frame_10 starting at position [40960];
// frame n = frame_11 starting at position [45056];
// frame n = frame_12 starting at position [49152];
// frame n = frame_13 starting at position [53248];
// frame n = frame_14 starting at position [57344];
// frame n = frame_15 starting at position [61440];
const static unsigned int frameData[65536];	
const static int frameCount=16;	
const static int frameSize=4096;	
const static int XSize=64;	
const static int YSize=64;	
};
