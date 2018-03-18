#pragma once
#include "BaseItem.h"
#include "BaseWeapon.generated.h"

UCLASS(config = Game)
class ABaseWeapon : public ABaseItem
{
	GENERATED_BODY()

protected:
	//Type of weapon (One Handed, Two Handed, etc).
	UPROPERTY(EditDefaultsOnly, Category = Weapon)
	EWeaponType myWeaponType;

	//The Weapon Damage modifier (how much HP it drains per hit).
	UPROPERTY(EditDefaultsOnly, Category = Weapon)
	int myDamageModifier;

	//The Weapon Range modifier (how far the item can hit something).
	UPROPERTY(EditDefaultsOnly, Category = Weapon)
	int myRangeModifier;

public:
	///Constructor
	ABaseWeapon();

	///Functions
	virtual FText RetrieveStats() override;

	///Handlers
	virtual void BeginPlay() override;
	virtual void OnPlayerReceive(ABaseHand* owningHand) override;
	virtual void NotifyHit(UPrimitiveComponent* collisionComp, AActor* otherActor, UPrimitiveComponent* otherCollisionComp, bool bSelfMoved, FVector hitLocation, FVector hitNormal, FVector normalImpulse, const FHitResult& hitResult) override;

	///Getters
	EWeaponType GetWeaponType();

	///Setters
	void SetWeaponType(EWeaponType weaponType);

};