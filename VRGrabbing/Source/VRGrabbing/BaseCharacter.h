// Copyright 1998-2016 Epic Games, Inc. All Rights Reserved.
#pragma once
#include "BaseEnums.h"
#include "GameFramework/Character.h"
#include "BaseCharacter.generated.h"

class ABaseItem;
class ABaseHand;
class UAnimBlueprint;
class UInputComponent;
class USceneComponent;
class UCameraComponent;
class USkeletalMeshComponent;

UCLASS(config=Game)
class ABaseCharacter : public ACharacter
{
	GENERATED_BODY()

protected:
	//Character mesh: 1st person view (arms; seen only by self).
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Character)
	USkeletalMeshComponent* myFirstPersonMesh;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Character)
	float myBaseTurnRate;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Character)
	float myBaseLookUpRate;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Character)
	float myStrength;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Hand)
	TSubclassOf<ABaseHand> myLeftHandBlueprint;
	
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Hand)
	TSubclassOf<ABaseHand> myRightHandBlueprint;
	
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Hand)
	ABaseHand* myLeftHand;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Hand)
	ABaseHand* myRightHand;

	//First person camera attached to the character.
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Camera, meta = (AllowPrivateAccess = "true"))
	UCameraComponent* myFirstPersonCamera;

	bool myIsLeftTriggerPressed;
	bool myIsRightTriggerPressed;

public:
	///Constructor
	ABaseCharacter();

	///Virtual Override Functions
	virtual void BeginPlay() override;
	virtual void SetupPlayerInputComponent(UInputComponent* playerInputComponent) override;
	virtual void Tick(float deltaTime) override;
	
	///Functions
	virtual void OnLeftTriggerPress();
	virtual void OnLeftTriggerHeld();
	virtual void OnRightTriggerPress();
	virtual void OnRightTriggerHeld();
	virtual void OnLeftGripPress();
	virtual void OnRightGripPress();
	virtual void OnLeftTriggerRelease();
	virtual void OnRightTriggerRelease();
	virtual void OnLeftGripRelease();
	virtual void OnRightGripRelease();
	virtual void OnUseLeftHandItem();
	virtual void OnUseRightHandItem();
	virtual void OnEquipLeftHandItem();
	virtual void OnEquipRightHandItem();
	virtual void OnUnEquipLeftHandItem();
	virtual void OnUnEquipRightHandItem();
	virtual void OnGrabLeftHand();
	virtual void OnGrabRightHand();
	virtual void OnCrushLeftHandItem();
	virtual void OnCrushRightHandItem();
	virtual void OnResetVR();
	virtual void MoveForward(float val);
	virtual void MoveStafing(float val);
	virtual void Turn(float rate);
	virtual void LookUp(float rate);
	virtual void VibrateLeftController(EVibrationType vibrationType);
	virtual void VibrateRightController(EVibrationType vibrationType);

	///Getters
	USkeletalMeshComponent* GetFirstPersonMesh();
	ABaseHand* GetLeftHand();
	ABaseHand* GetRightHand();
	float GetBaseTurnRate();
	float GetBaseLookUpRate();
	float GetStrength();
	bool GetAreBothHandsGrabbing();

	///Setters
	void SetBaseTurnRate(float baseTurnRate);
	void SetBaseLookUpRate(float baseLookUpRate);
};

