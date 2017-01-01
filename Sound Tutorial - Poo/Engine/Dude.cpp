#include "Dude.h"
#include "Graphics.h"
#include "SpriteCodex.h"

void Dude::ClampToScreen()
{
	const int right = x + width;
	if( x < 0 )
	{
		x = 0;
	}
	else if( right >= Graphics::ScreenWidth )
	{
		x = (Graphics::ScreenWidth - 1) - width;
	}

	const int bottom = y + height;
	if( y < 0 )
	{
		y = 0;
	}
	else if( bottom >= Graphics::ScreenHeight )
	{
		y = (Graphics::ScreenHeight - 1) - height;
	}
}

void Dude::Draw( Graphics& gfx ) const
{
	SpriteCodex::DrawDude( gfx,x,y );
}

void Dude::Update( const Keyboard & kbd )
{
	if( kbd.KeyIsPressed( VK_RIGHT ) )
	{
		x += speed;
	}
	if( kbd.KeyIsPressed( VK_LEFT ) )
	{
		x -= speed;
	}
	if( kbd.KeyIsPressed( VK_DOWN ) )
	{
		y += speed;
	}
	if( kbd.KeyIsPressed( VK_UP ) )
	{
		y -= speed;
	}
}

int Dude::GetX() const
{
	return x;
}

int Dude::GetY() const
{
	return y;
}

int Dude::GetWidth() const
{
	return width;
}

int Dude::GetHeight() const
{
	return height;
}
