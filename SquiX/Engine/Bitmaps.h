
#pragma once
#include "stdint.h"
#include "McImageMap.h"

class Bitmaps: public Mc::IImageMap
{
public:
    Bitmaps();
	unsigned int* FrameData() override { return (unsigned int*) imageData; }
	int* FrameInfo() override { return (int*) imageInfo; }
	const static int books=0;
	const static int earth1=1;
	const static int escher2=2;
	const static int escher3=3;
	const static int escher4=4;
	const static int exit=5;
	const static int fire=6;
	const static int fishblue=7;
	const static int fractal=8;
	const static int leaves=9;
	const static int metal=10;
	const static int metal1=11;
	const static int neon=12;
	const static int pattern=13;
	const static int player1=14;
	const static int player2=15;
	const static int rock=16;
	const static int ruby1=17;
	const static int ruby2=18;
	const static int ruby3=19;
	const static int scales=20;
	const static int tiles1=21;
	const static int tiles2=22;
	const static int vertical=23;
	const static int wall1=24;
	const static int wall2=25;
	const static int wall3=26;
	const static int wall4=27;
	const static int waves=28;
	const static int wood1=29;
	const static int wood2=30;
	const static int wood3=31;
	const static int wood4=32;
	const static unsigned int imageData[135168];	
	const static int imageInfo[99];	
	const static int imageCount=33;	
};
