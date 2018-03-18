#pragma once
#include "BaseWeapon.h"
#include "MaceWeapon.generated.h"

UCLASS(config = Game)
class AMaceWeapon : public ABaseWeapon
{
	GENERATED_BODY()

public:
	///Constructor
	AMaceWeapon();

	///Functions
	virtual void BeginPlay() override;

};