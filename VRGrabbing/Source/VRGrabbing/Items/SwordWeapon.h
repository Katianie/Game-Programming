#pragma once
#include "BaseWeapon.h"
#include "SwordWeapon.generated.h"

UCLASS(config = Game)
class ASwordWeapon : public ABaseWeapon
{
	GENERATED_BODY()

public:
	///Constructor
	ASwordWeapon();

	///Functions
	virtual void BeginPlay() override;

};