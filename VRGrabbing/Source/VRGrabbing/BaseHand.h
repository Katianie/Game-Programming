#pragma once
#include "BaseEnums.h"
#include "GameFramework/Actor.h"
#include "BaseHand.generated.h"

class ABaseItem;
class ABaseWeapon;
class ABaseCharacter;
class USkeletalMeshComponent;
class UPhysicsHandleComponent;
class UMotionControllerComponent;

UCLASS(config = Game)
class ABaseHand : public AActor
{
	GENERATED_BODY()

protected:
	ABaseCharacter* myOwningCharacter;

	//Hand mesh: 1st person view (seen only by self).
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Hand)
	USkeletalMeshComponent* myFirstPersonHandMesh;

	//VR Hand mesh: VR view (attached to the VR controller directly, no arm, just the hand).
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Hand)
	USkeletalMeshComponent* myVRHandMesh;

	//Motion controller (left hand).
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Hand)
	UMotionControllerComponent* myMotionController;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Hand)
	EControllerHand myControllerHand;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Hand)
	bool myIsItemEquipped;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Hand)
	bool myIsCurrentlyGrabbing;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Hand)
	bool myIsCurrentlyCrushing;

	//Current Distance of grabbed items from their respective controllers.
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Hand)
	float myDistanceFromController;

	//The farthest distance you can extend your grabbing.
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Hand)
	float myReachDistance;

	//Min Distance for Controller for grabbed objects
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Hand)
	float myMinDistanceFromController;

	//Max Distance for Controller for grabbed objects
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Hand)
	float myMaxDistanceFromController;

	//Whether to use motion controller location for aiming. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Hand)
	bool myIsUsingMotionControllers;

	UPROPERTY(VisibleAnywhere, BlueprintReadWrite, Category = Hand)
	FVector myControllerLocation;

	UPROPERTY(VisibleAnywhere, BlueprintReadWrite, Category = Hand)
	FRotator myControllerRotation;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Hand)
	FTransform myGrabbedObjectTransform;

	UPROPERTY(VisibleAnywhere, BlueprintReadWrite, Category = Hand)
	FTransform myGrabbedPhysicsHandleTransform;

	UPROPERTY(VisibleAnywhere, BlueprintReadWrite, Category = Hand)
	FTransform myControllerTransform;

	//Target location for grabbed object
	UPROPERTY(VisibleAnywhere, BlueprintReadWrite, Category = Hand)
	FVector myNewGrabbedLocation;

	//Difference between previous and old grabbed object locations
	UPROPERTY(VisibleAnywhere, BlueprintReadWrite, Category = Hand)
	FVector myDiffGrabbedLocation;

	//Difference between controller rotation and grabbed object rotation
	UPROPERTY(VisibleAnywhere, BlueprintReadWrite, Category = Hand)
	FQuat myDiffGrabbedRotation;

	//If not NULL, the object that is currently being grabbed.
	UPROPERTY(VisibleAnywhere, BlueprintReadWrite, Category = Hand)
	ABaseItem* myGrabbedItem;

	//If not NULL, the physics handle of the item currently being grabbed.
	UPROPERTY(VisibleAnywhere, BlueprintReadWrite, Category = Hand)
	UPhysicsHandleComponent* myGrabbedPhysicsHandle;

	//The currently grabbed component.
	UPROPERTY(VisibleAnywhere, BlueprintReadWrite, Category = Hand)
	UPrimitiveComponent* myGrabbedPrimitiveComponent;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Effects)
	UForceFeedbackEffect* myPickUpForceFeedback;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = Effects)
	UForceFeedbackEffect* myHitForceFeedback;

	ECollisionChannel myCollisionChannels[33];

	FVector myLastHitDistance;

public:
	///Constructor
	ABaseHand();

	///Virtual Override Functions
	virtual void BeginPlay() override;
	virtual void Tick(float deltaTime) override;

	///Functions
	bool Grab(AActor* actorThatIsGrabbing, bool showDebugLine = false);
	bool GrabItem(ABaseItem* grabbedItem, FHitResult hitResult);
	bool TouchItem(ABaseItem* grabbedItem, FHitResult hitResult);
	bool GrabWeapon(ABaseItem* grabbedItem);
	bool Crush(int playerStrength);
	void UseItem();
	void RemoveGrabbedItem();
	void Drop();
	bool EquipItem();
	bool UnEquipItem();
	AActor* CastCircleOfRays(double radius, FVector& lineTraceStart, FVector& lineTraceEnd, bool showDebugLines = false);
	AActor* CastCircleOfRays(double radius, FVector& lineTraceStart, FVector& lineTraceEnd, FHitResult& outHitResult, bool showDebugLines = false);
	AActor* CastRay(FVector& lineTraceStart, FVector& lineTraceEnd, bool showDebugLine = false);
	AActor* CastRay(FVector& lineTraceStart, FVector& lineTraceEnd, FHitResult& outHitResult, bool showDebugLine = false);
	AActor* LineTrace(FVector& lineTraceStart, FVector& lineTraceEnd, FHitResult& outHitResult);
	void VibrateController(EVibrationType vibrationType);

	///Getters
	USkeletalMeshComponent* GetFirstPersonHandMesh();
	USkeletalMeshComponent* GetVRHandMesh();
	UMotionControllerComponent* GetMotionController();
	EControllerHand GetControllerHand();
	bool GetIsItemEquipped();
	bool GetIsCurrentlyGrabbing();
	bool GetIsCurrentlyCrushing();
	float GetDistanceFromController();
	float GetReachDistance();
	float GetMinDistanceFromController();
	float GetMaxDistanceFromController();
	bool GetIsUsingMotionControllers();
	FVector GetControllerLocation();
	FRotator GetControllerRotation();
	FTransform GetGrabbedPhysicsHandleTransform();
	FTransform GetControllerTransform();
	FVector GetControllerRelativeLocation();
	FRotator GetControllerRelativeRotation();
	FVector GetNewGrabbedLocation();
	FVector GetLastHitDiffrence();
	FVector GetDiffGrabbedLocation();
	FQuat GetDiffGrabbedRotation();
	ABaseItem* GetGrabbedItem();
	UPrimitiveComponent* GetGrabbedPrimitiveComponent();
	UPhysicsHandleComponent* GetGrabbedPhysicsHandle();
	ABaseCharacter* GetOwningCharacter();
	float GetOwnerStrength();

	///Setters
	void SetMinDistanceFromController(float minDistance);
	void SetMaxDistanceFromController(float maxDistance);
	void SetIsUsingMotionControllers(bool isUsingMotionControllers);
	void SetControllerLocation(FVector controllerLocation);
	void SetControllerRotation(FRotator controllerRotation);
	void SetDistanceFromController(float distanceFromController);
	void SetGrabbedItem(ABaseItem* grabbedItem);
	void SetIsItemEquipped(bool isItemEquipped);
	void SetIsCurrentlyGrabbing(bool isCurrentlyGrabbing);
	void SetGrabbedPrimitiveComponent(UPrimitiveComponent* grabbedPrimitiveComponent);
	void SetGrabbedObjectTransform(FTransform grabbedObjectTransform);
	void SetControllerTransform(FTransform controllerTransform);
	void SetIsCurrentlyCrushing(bool isCurrentlyCrushing);
	void SetOwningCharacter(ABaseCharacter* owningCharacter);
};