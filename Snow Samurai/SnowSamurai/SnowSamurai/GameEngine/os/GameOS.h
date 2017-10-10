/*
	GameOS.h

	This class provides a framework for managing OS messages between
	this application and the Operating System. Each platform would have
	its own implementation.
*/

#pragma once
#include "../stdafx.h"


class GameOS
{
public:
	virtual void killApp()=0;
	virtual void processOSMessages()=0;
};