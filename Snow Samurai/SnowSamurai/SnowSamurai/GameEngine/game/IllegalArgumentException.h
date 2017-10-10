/*	
	IllegalArgumentException.h

	This class represents an exception thrown when an illegal
	argument is given to a method.
*/

#pragma once
#include "../resource.h"
#include "../stdafx.h"

class IllegalArgumentException:exception
{
public:
	// INLINED CONSTRUCTOR
	IllegalArgumentException(string *errorMessage)
	{
		exception(errorMessage->c_str());
	}

	// INLINED DESTRUCTOR
	~IllegalArgumentException(){}
};