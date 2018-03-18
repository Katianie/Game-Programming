#pragma once
#include "BaseWeapon.h"
#include "OneHandGunWeapon.generated.h"

UCLASS(config = Game)
class AOneHandGunWeapon : public ABaseWeapon
{
	GENERATED_BODY()

protected:
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Projectile)
	float myBulletSpeed;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Projectile)
	float myBulletLifetime;
	
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Projectile)
	TSubclassOf<AStaticMeshActor> myProjectile;
	
	USceneComponent* myFirePointComponent;

public:
	///Constructor
	AOneHandGunWeapon();

	///Functions
	virtual void BeginPlay() override;
	virtual void OnPlayerUse() override;

};