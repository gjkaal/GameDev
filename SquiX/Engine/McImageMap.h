#pragma once
namespace Mc {
	class ImageMap {
	public:
		virtual unsigned int* FrameData();
		virtual int* FrameInfo();
	};
}