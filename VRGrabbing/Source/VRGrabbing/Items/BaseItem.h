// Copyright 1998-2015 Epic Games, Inc. All Rights Reserved.
#pragma once
#include "BaseEnums.h"
#include "BaseHand.h"
#include "DestructibleComponent.h"
#include "GameFramework/Actor.h"
#include "BaseItem.generated.h"

//Base class for pickup objects that can be placed in the world
UCLASS(config = Game)
class ABaseItem : public AActor
{
protected:
	GENERATED_BODY()

	AActor* myCollidedActor;

	//If not NULL, the hands that are currently holding the item.
	ABaseHand* myLeftHand;
	ABaseHand* myRightHand;

	//Reference to the hand that grabed it first.
	ABaseHand* myPrimaryHand;
	ABaseHand* mySecondaryHand;

	//If not NULL, the component that breaks.
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Pickup)
	UDestructibleComponent* myDestructibleComponent;

	//If not NULL, the object that is currently being grabbed.
	UPhysicsHandleComponent* myGrabbedPhysicsHandle;

	//keep a reference so we can switch back to it later.
	UDestructibleComponent* myDestructiblePhysicsComponent;

	//The mass of the item in kg. TODO: INCORPERATE INTO PHYSICS.
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Pickup)
	float myMass;

	//Is the item currently active/activated/being used.
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Pickup)
	bool myIsActive;

	//Is the item able to be touched with the hands.
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Pickup)
	bool myIsTouchable;

	//Is the item able to be picked up and grabbed with the hands.
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Pickup)
	bool myIsGrabbable;

	//Is the item able to be equipped.
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Pickup)
	bool myIsEquipable;

	//Is the item able to be harvested
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Pickup)
	bool myIsHarvestable;

	//Is the item able to be broken/crushed/smashed.
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Pickup)
	bool myIsBreakable;

	//True if the item should automatically equip when picked up (so you dont have to hold the trigger to continue hold).
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Pickup)
	bool myIsAutomaticallyEquipped;

	//True when the destructible component has broken/fractured in some way.
	bool myIsFractured;

	//True if the item has been touched at least once.
	bool myHasBeenTouchedOnce;

	//If true, will continue to call use while use button is held.
	UPROPERTY(EditDefaultsOnly, Category = Use)
	bool myIsFullAutomatic;

	//Name of the item.
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Description)
	FText myItemName;

	//Identifies which component is the root component and will attach to the primary hand (optional).
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Pickup)
	FText myGrabbableRootComponentName;

	//Identifies which component is the secondary component and can attach to the secondary hand (optional).
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Pickup)
	FText myGrabbableSecondaryComponentName;

	//Description of the Item.
	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category = Description)
	FText myItemDescription;

	//Texture used in the inventory.
	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category = Description)
	UTexture2D* myInventoryTexture;

	//Animation to use when the item is equipped
	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = Pickup)
	UAnimSequence* myOnEquipAnimationSequence;

	//Sound when player picks up this item.
	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category = Effects)
	USoundCue* myPickupItemSound;

	//Sound when player drops this item.
	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category = Effects)
	USoundCue* myDropItemSound;

	//Relative Transform for the item if and when it is first grabbed.
	UPROPERTY(EditDefaultsOnly, Category = Pickup)
	FTransform myOnGrabItemTransform;

	///Functions


public:

	///Constructor
	ABaseItem();

	///Destructor
	virtual ~ABaseItem();

	///Functions
	virtual void BeginPlay() override;
	virtual void Tick(float deltaTime) override;
	virtual FText RetrieveStats();
	virtual USceneComponent* FindSubComponentByName(FText componentName);
	virtual void VibrateTouchingHands(EVibrationType vibrationType);

	///Handlers
	virtual void NotifyActorBeginOverlap(AActor* otherActor) override;
	virtual void NotifyHit(UPrimitiveComponent* collisionComp, AActor* otherActor, UPrimitiveComponent* otherCollisionComp, bool bSelfMoved, FVector hitLocation, FVector hitNormal, FVector normalImpulse, const FHitResult& hitResult) override;

	UFUNCTION()
	virtual void NotifyFracture(const FVector& hitPosition, const FVector& hitDirection);	//Called when destructible component fractures.
	virtual void OnPlayerEquip();
	virtual void OnPlayerUse();
	virtual void OnPlayerTouched(ABaseHand* owningHand);
	virtual void OnPlayerReceive(ABaseHand* owningHand);
	virtual void OnPlayerDrop(ABaseHand* owningHand);
	virtual void OnPlayerCrush(ABaseHand* owningHand, int playerStrength);

	///Getters
	bool GetIsActive();
	bool GetIsHarvestable();
	bool GetIsBreakable();
	bool GetIsTouchable();
	bool GetIsGrabbable();
	bool GetIsEquipable();
	bool GetIsFractured();
	bool GetIsFullAutomatic();
	bool GetIsBeingTouched();
	bool GetIsAutomaticallyEquipped();
	float GetDestructibleStrength();
	float GetMass();
	FText GetItemName();
	FText GetItemDescription();
	AActor* GetCollidedActor();
	UTexture2D* GetInventoryTexture();
	ABaseHand* GetLeftHand();
	ABaseHand* GetRightHand();
	ABaseHand* GetPrimaryHand();
	ABaseHand* GetSecondaryHand();
	UAnimSequence* GetOnEquipAnimationSequence();
	UPhysicsHandleComponent* GetGrabbedPhysicsHandle();
	UDestructibleComponent* GetDestructibleComponent();
	FTransform GetOnGrabItemTransform();
	USceneComponent* GetGrabbableRootComponent();
	USceneComponent* GetGrabbableSecondaryComponent();

	///Setters
	void SetIsActive(bool isActive);
	void SetIsHarvestable(bool isHarvestable);
	void SetIsBreakable(bool isBreakable);
	void SetIsEquipable(bool isEquipable);
	void SetIsGrabbable(bool isGrabbable);
	void SetItemName(FText itemName);
	void SetItemName(FString itemName);
	void SetItemDescription(FText itemDescription);
	void SetItemDescription(FString itemDesctiption);
	void SetInventoryTexture(UTexture2D* inventoryTexture);
	void SetLeftOwningHand(ABaseHand* owningLeftHand);
	void SetRightOwningHand(ABaseHand* owningRightHand);
	void SetOnEquipAnimationSequence(UAnimSequence* onEquipAnimationSequence);
	void SetGrabbedPhysicsHandle(UPhysicsHandleComponent* physHandle);
	void SetOnGrabItemTransform(FTransform onGrabItemTransform);
};
