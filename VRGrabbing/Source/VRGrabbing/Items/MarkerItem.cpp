#include "Smash.h"
#include "Runtime/Engine/Classes/Kismet/KismetMathLibrary.h"
#include "Runtime/Engine/Classes/Kismet/KismetSystemLibrary.h"
#include "MarkerItem.h"

AMarkerItem::AMarkerItem()
{
	myWhiteboard = NULL;
	myMarkerTip = NULL;
	myCapsuleCollisionComponent = NULL;
	myPreviousDrawLocation = FVector(-1, -1, -1);
	myThickness = 5.0f;
}

AMarkerItem::~AMarkerItem()
{

}

void AMarkerItem::BeginPlay()
{
	FString currName;
	int numComponentsFound = 0;
	TArray<UActorComponent*> components;

	Super::BeginPlay();

	//Get all the scene components in the Marker Item blueprint.
	components = this->GetComponentsByClass(UActorComponent::StaticClass());
	for (int currComponentIndex = 0; numComponentsFound < 4 && currComponentIndex < components.Num(); currComponentIndex++)
	{
		currName = components[currComponentIndex]->GetName();
		if (currName.Equals("Tip", ESearchCase::IgnoreCase) == true)
		{
			myMarkerTip = Cast<UStaticMeshComponent>(components[currComponentIndex]);
			numComponentsFound++;
		}
		else if (currName.Equals("Capsule", ESearchCase::IgnoreCase) == true)
		{
			myCapsuleCollisionComponent = Cast<UCapsuleComponent>(components[currComponentIndex]);
			numComponentsFound++;
		}
		else if (currName.Equals("TipStart", ESearchCase::IgnoreCase) == true)
		{
			myMarkerTipStart = Cast<USceneComponent>(components[currComponentIndex]);
			numComponentsFound++;
		}
		else if (currName.Equals("TipEnd", ESearchCase::IgnoreCase) == true)
		{
			myMarkerTipEnd = Cast<USceneComponent>(components[currComponentIndex]);
			numComponentsFound++;
		}
	}

	if (myMarkerTip != NULL)
	{
		myMarkerTip->OnComponentBeginOverlap.AddDynamic(this, &AMarkerItem::OnComponentBeginOverlap);
		myMarkerTip->OnComponentEndOverlap.AddDynamic(this, &AMarkerItem::OnComponentEndOverlap);

		myMarkerTip->SetVectorParameterValueOnMaterials(FName("Color"), FVector(myMarkerColor.R, myMarkerColor.G, myMarkerColor.B));
	}
}

void AMarkerItem::NotifyActorBeginOverlap(AActor* otherActor)
{
	Super::NotifyActorBeginOverlap(otherActor);
}

void AMarkerItem::NotifyHit(UPrimitiveComponent* collisionComp, AActor* otherActor, UPrimitiveComponent* otherCollisionComp, bool bSelfMoved, FVector hitLocation, FVector hitNormal, FVector normalImpulse, const FHitResult& hitResult)
{
	Super::NotifyHit(collisionComp, otherActor, otherCollisionComp, bSelfMoved, hitLocation, hitNormal, normalImpulse, hitResult);

	AMarkerItem::OnComponentBeginOverlap(collisionComp, otherActor, otherCollisionComp, 0, false, hitResult);
}

void AMarkerItem::OnComponentBeginOverlap(UPrimitiveComponent* overlappedComp, AActor* otherActor, UPrimitiveComponent* otherComp, int32 otherBodyIndex, bool isFromSweep, const FHitResult& sweepResult)
{
	AWhiteboardItem* collidedWhiteboard;
	
	if (myCollidedActor != NULL)
	{
		collidedWhiteboard = Cast<AWhiteboardItem>(myCollidedActor);
		if (collidedWhiteboard != NULL && myCapsuleCollisionComponent != NULL)
		{
			//Marker collided with a white-board so retain the reference to the specific one the marker collided with.
			myWhiteboard = collidedWhiteboard;

			//Set the initial previous draw location.
			myPreviousDrawLocation = myCapsuleCollisionComponent->GetComponentLocation();			

			//Play vibration when dragging over segment.
			Super::VibrateTouchingHands(EVibrationType::VE_TOUCH);
		}
	}
}

void AMarkerItem::OnComponentEndOverlap(UPrimitiveComponent* overlappedComp, AActor* otherActor, UPrimitiveComponent* otherComp, int32 otherBodyIndex)
{
	if (otherActor != NULL && myWhiteboard == Cast<AWhiteboardItem>(otherActor))
	{
		//Remove the reference to the white-board since we are no longer interacting with it.
		myWhiteboard = NULL;
		myPreviousDrawLocation = FVector(-1, -1, -1);
	}
}

void AMarkerItem::Tick(float deltaTime)
{
	AWhiteboardItem* currentWhiteboard;
	TArray<AActor*> actorsToIgnore;
	FVector tipStartLocation;
	FVector tipEndLocation;
	FVector tipImpactPoint;
	FHitResult outHitResult;

	if (myPrimaryHand != NULL && myMarkerTipStart != NULL && myMarkerTipEnd != NULL)
	{
		tipStartLocation = myMarkerTipStart->GetComponentLocation();
		tipEndLocation = myMarkerTipEnd->GetComponentLocation();

		//TODO: ensure this is correctly detecting collision.
		if (UKismetSystemLibrary::SphereTraceSingle(this->GetWorld(), tipStartLocation, tipEndLocation, 0.6f, ETraceTypeQuery::TraceTypeQuery_MAX, false, actorsToIgnore, EDrawDebugTrace::None, outHitResult, true) == true)
		{
			currentWhiteboard = Cast<AWhiteboardItem>(outHitResult.GetActor());
			tipImpactPoint = outHitResult.ImpactPoint;

			if (currentWhiteboard == NULL)
			{
				currentWhiteboard = myWhiteboard;
			}

			if (currentWhiteboard != NULL)
			{
				//UE_LOG(LogTemp, Error, TEXT("AMarkerItem::Tick() before myPreviousDrawLocation:%s"), *(myPreviousDrawLocation.ToString()));
				
				//Only allow drawing when in range of the white-board.
				if (FVector::Dist(this->GetActorLocation(), currentWhiteboard->GetActorLocation()) > myPrimaryHand->GetReachDistance())
				{
					AMarkerItem::OnComponentEndOverlap(myCapsuleCollisionComponent, currentWhiteboard, NULL, 0);
				}
				else
				{
					currentWhiteboard->DrawLine(myPreviousDrawLocation, tipImpactPoint, myThickness, myMarkerColor);
					myPreviousDrawLocation = tipImpactPoint;
				}
			}
		}
	}
}