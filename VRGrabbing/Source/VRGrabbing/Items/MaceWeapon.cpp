#include "Smash.h"
#include "Ace.h"
#include "MaceWeapon.h"

///Constructor
AMaceWeapon::AMaceWeapon() : Super()
{
	myWeaponType = EWeaponType::VE_ONE_HANDED;
	myDamageModifier = 9.0;
	myRangeModifier = 3.0;
}

///Functions
void AMaceWeapon::BeginPlay()
{
	Super::BeginPlay();
}
