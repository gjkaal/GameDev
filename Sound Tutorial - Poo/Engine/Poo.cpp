#include "Poo.h"
#include "Graphics.h"
#include "SpriteCodex.h"
#include <assert.h>

void Poo::Init( int in_x,int in_y,int in_vx,int in_vy )
{
	assert( initialized == false );
	x = in_x;
	y = in_y;
	vx = in_vx;
	vy = in_vy;
	initialized = true;
}

void Poo::Update()
{
	assert( initialized == true );
	x += vx;
	y += vy;

	const int right = x + width;
	if( x < 0 )
	{
		x = 0;
		vx = -vx;
	}
	else if( right >= Graphics::ScreenWidth )
	{
		x = (Graphics::ScreenWidth - 1) - width;
		vx = -vx;
	}

	const int bottom = y + height;
	if( y < 0 )
	{
		y = 0;
		vy = -vy;
	}
	else if( bottom >= Graphics::ScreenHeight )
	{
		y = (Graphics::ScreenHeight - 1) - height;
		vy = -vy;
	}
}

void Poo::ProcessConsumption( const Dude& dude )
{
	assert( initialized == true );
	const int duderight = dude.GetX() + dude.GetWidth();
	const int dudebottom = dude.GetY() + dude.GetHeight();
	const int pooright = x + width;
	const int poobottom = y + height;

	if( duderight >= x &&
		dude.GetX() <= pooright &&
		dudebottom >= y &&
		dude.GetY() <= poobottom )
	{
		isEaten = true;
	}
}

void Poo::Draw( Graphics& gfx ) const
{
	assert( initialized == true );
	SpriteCodex::DrawPoo( gfx,x,y );
}

bool Poo::IsEaten() const
{
	assert( initialized == true );
	return isEaten;
}
