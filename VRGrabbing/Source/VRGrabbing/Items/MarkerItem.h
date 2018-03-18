// Copyright 1998-2015 Epic Games, Inc. All Rights Reserved.
#pragma once
#include "BaseWeapon.h"
#include "WhiteboardItem.h"
#include "MarkerItem.generated.h"

UCLASS(config = Game)
class AMarkerItem : public ABaseWeapon
{
	GENERATED_BODY()

protected:
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Draw)
	FLinearColor myMarkerColor;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Draw)
	float myThickness;

	AWhiteboardItem* myWhiteboard;
	UStaticMeshComponent* myMarkerTip;
	USceneComponent* myMarkerTipStart;
	USceneComponent* myMarkerTipEnd;
	UCapsuleComponent* myCapsuleCollisionComponent;
	FVector myPreviousDrawLocation;


public:
	///Constructor
	AMarkerItem();

	///Destructor
	virtual ~AMarkerItem();

	///Callbacks
	UFUNCTION()
	virtual void OnComponentBeginOverlap(UPrimitiveComponent* overlappedComp, AActor* otherActor, UPrimitiveComponent* otherComp, int32 otherBodyIndex, bool isFromSweep, const FHitResult& sweepResult);
	UFUNCTION()
	virtual void OnComponentEndOverlap(UPrimitiveComponent* overlappedComp, AActor* otherActor, UPrimitiveComponent* otherComp, int32 otherBodyIndex);

	///Functions
	virtual void BeginPlay() override;
	virtual void Tick(float deltaTime) override;
	virtual void NotifyActorBeginOverlap(AActor* otherActor) override;
	virtual void NotifyHit(UPrimitiveComponent* collisionComp, AActor* otherActor, UPrimitiveComponent* otherCollisionComp, bool bSelfMoved, FVector hitLocation, FVector hitNormal, FVector normalImpulse, const FHitResult& hitResult) override;

};