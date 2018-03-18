// Copyright 1998-2015 Epic Games, Inc. All Rights Reserved.
#pragma once
#include "BaseWeapon.h"
#include "WhiteboardItem.h"
#include "Runtime/Engine/Classes/Components/BoxComponent.h"
#include "EraserItem.generated.h"

UCLASS(config = Game)
class AEraserItem : public ABaseWeapon
{
	GENERATED_BODY()

protected:
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Draw)
	UMaterial* myPaintMaterial;

	AWhiteboardItem* myWhiteboard;
	UStaticMeshComponent* myEraserFront;
	UBoxComponent* myBoxCollisionComponent;


public:
	///Constructor
	AEraserItem();

	///Destructor
	virtual ~AEraserItem();

	///Callbacks
	UFUNCTION()
	virtual void OnComponentBeginOverlap(UPrimitiveComponent* overlappedComp, AActor* otherActor, UPrimitiveComponent* otherComp, int32 otherBodyIndex, bool isFromSweep, const FHitResult& sweepResult);
	UFUNCTION()
	virtual void OnComponentEndOverlap(UPrimitiveComponent* overlappedComp, AActor* otherActor, UPrimitiveComponent* otherComp, int32 otherBodyIndex);

	///Functions
	virtual void BeginPlay() override;
	virtual void Tick(float deltaTime) override;
	virtual TArray<FVector>* FindIntersectionVertices();
	virtual void NotifyActorBeginOverlap(AActor* otherActor) override;
	virtual void NotifyHit(UPrimitiveComponent* collisionComp, AActor* otherActor, UPrimitiveComponent* otherCollisionComp, bool bSelfMoved, FVector hitLocation, FVector hitNormal, FVector normalImpulse, const FHitResult& hitResult) override;
};