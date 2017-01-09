#pragma once
namespace Mc {
	class IImageMap {
	public:
		virtual ~IImageMap() {}
		virtual unsigned int* FrameData()=0;
		virtual int* FrameInfo()=0;
	};
}