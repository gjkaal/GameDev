/******************************************************************************************
 *	Chili DirectX Framework Version 16.07.20											  *
 *	Game.cpp																			  *
 *	Copyright 2016 PlanetChili.net <http://www.planetchili.net>							  *
 *																						  *
 *	This file is part of The Chili DirectX Framework.									  *
 *																						  *
 *	The Chili DirectX Framework is free software: you can redistribute it and/or modify	  *
 *	it under the terms of the GNU General Public License as published by				  *
 *	the Free Software Foundation, either version 3 of the License, or					  *
 *	(at your option) any later version.													  *
 *																						  *
 *	The Chili DirectX Framework is distributed in the hope that it will be useful,		  *
 *	but WITHOUT ANY WARRANTY; without even the implied warranty of						  *
 *	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the						  *
 *	GNU General Public License for more details.										  *
 *																						  *
 *	You should have received a copy of the GNU General Public License					  *
 *	along with The Chili DirectX Framework.  If not, see <http://www.gnu.org/licenses/>.  *
 ******************************************************************************************/
#include "MainWindow.h"
#include "Game.h"
#include "SplashScreen.h"
#include "Explosion.h"
#include "Bitmaps.h"

// chrone to calculate frames
#include <chrono>


Game::Game(MainWindow& wnd)
	:
	wnd(wnd),
	gfx(wnd),
	// init sound files
	soundBG(L"Groovy.wav"),
	bitMaps()
{
	// initialize framespeed, higher number = lower animation speed
	// 1000 = 1000ms = 1s
	frameSpeed = 100;
	soundBG.Play();

}

void Game::Go()
{
	gfx.BeginFrame();
	UpdateModel();
	ComposeFrame();
	gfx.EndFrame();
}

void Game::UpdateModel()
{
	// Calculate current frame and update using frame current count down
	// framecount is changed every framespeed milliseconds
	using namespace std::chrono;
		milliseconds ms = duration_cast< milliseconds >(
			system_clock::now().time_since_epoch()
			);
	frameCurrent = (ms.count() / frameSpeed) % MAXINT;

}

void Game::ComposeFrame()
{
	/*gfx.DrawHLine(100, 100, 600, 255, 255, 255);
	gfx.DrawVLine(100, 100, 400, 255, 255, 255);
	gfx.DrawHLine(100, 500, 600, 255, 255, 255);
	gfx.DrawVLine(700, 100, 400, 255, 255, 255);

	gfx.DrawRect(250, 300, 400, 450, 255, 255, 255);
	gfx.DrawRect(252, 302, 398, 448, 255, 255, 255);

	gfx.WriteLine("  Marjolein kaal     ", Ascii::cm, 120, 120, 8, 8);*/
	gfx.DrawBitmap(SplashScreen::imgData, 0, 0, 800, 600);
	gfx.WriteLine("                       ", Ascii::cm, 120, 104, 16, 16);
	gfx.WriteLine("      a game by        ", Ascii::cm, 120, 120, 16, 16);
	gfx.WriteLine("    Marjolein kaal     ", Ascii::cm, 120, 136, 16, 16);
	gfx.WriteLine("                       ", Ascii::cm, 120, 152, 16, 16);

	// gfx.DrawCharacter(Ascii::cm, 65, 120, 120, 16, 16);
	int explosionFrame = frameCurrent % Explosion::frameCount;
	gfx.DrawFrame(
		Explosion::frameData, 
		explosionFrame * Explosion::frameSize, 500, 500, 
		Explosion::XSize, 
		Explosion::YSize);

	int bitmapImg = Bitmaps::esher2;
	gfx.DrawImage(bitMaps.imageInfo, bitMaps.imageData, bitmapImg, 450, 450);


}

