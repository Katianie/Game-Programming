#include "Smash.h"
#include "Ace.h"
#include "SwordWeapon.h"

///Constructor
ASwordWeapon::ASwordWeapon() : Super()
{
	myWeaponType = EWeaponType::VE_ONE_HANDED;
	myDamageModifier = 3.0;
	myRangeModifier = 7.0;
}

///Functions
void ASwordWeapon::BeginPlay()
{
	Super::BeginPlay();
}