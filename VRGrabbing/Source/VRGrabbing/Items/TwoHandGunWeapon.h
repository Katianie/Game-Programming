#pragma once
#include "BaseWeapon.h"
#include "TwoHandGunWeapon.generated.h"

UCLASS(config = Game)
class ATwoHandGunWeapon : public ABaseWeapon
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
	USceneComponent* myHandleComponent;
	USceneComponent* myGripComponent;
	USceneComponent* myGunRootComponent;

public:
	///Constructor
	ATwoHandGunWeapon();

	///Functions
	virtual void BeginPlay() override;
	virtual void OnPlayerReceive(ABaseHand* owningHand) override;
	virtual void Tick(float deltaTime) override;
	virtual void OnPlayerUse() override;
};