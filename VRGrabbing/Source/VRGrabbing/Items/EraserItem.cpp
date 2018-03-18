#include "Smash.h"
#include "Runtime/Engine/Classes/Kismet/KismetMathLibrary.h"
#include "EraserItem.h"

AEraserItem::AEraserItem()
{
	myWhiteboard = NULL;
	myEraserFront = NULL;
	myBoxCollisionComponent = NULL;
}

AEraserItem::~AEraserItem()
{

}

void AEraserItem::BeginPlay()
{
	FString currName;
	TArray<UActorComponent*> components;

	Super::BeginPlay();

	//Get all the scene components in the Eraser Item blueprint.
	components = this->GetComponentsByClass(UActorComponent::StaticClass());
	for (int currComponentIndex = 0; (myEraserFront == NULL || myBoxCollisionComponent == NULL) && currComponentIndex < components.Num(); currComponentIndex++)
	{
		currName = components[currComponentIndex]->GetName();

		if (currName.Equals("Front", ESearchCase::IgnoreCase) == true)
		{
			myEraserFront = Cast<UStaticMeshComponent>(components[currComponentIndex]);
			
		}
		else if (currName.Equals("Box", ESearchCase::IgnoreCase) == true)
		{
			myBoxCollisionComponent = Cast<UBoxComponent>(components[currComponentIndex]);
		}
	}

	if (myEraserFront != NULL)
	{
		myEraserFront->OnComponentBeginOverlap.AddDynamic(this, &AEraserItem::OnComponentBeginOverlap);
		myEraserFront->OnComponentEndOverlap.AddDynamic(this, &AEraserItem::OnComponentEndOverlap);
	}
}

void AEraserItem::NotifyActorBeginOverlap(AActor* otherActor)
{
	Super::NotifyActorBeginOverlap(otherActor);
}

void AEraserItem::NotifyHit(UPrimitiveComponent* collisionComp, AActor* otherActor, UPrimitiveComponent* otherCollisionComp, bool bSelfMoved, FVector hitLocation, FVector hitNormal, FVector normalImpulse, const FHitResult& hitResult)
{
	Super::NotifyHit(collisionComp, otherActor, otherCollisionComp, bSelfMoved, hitLocation, hitNormal, normalImpulse, hitResult);

	AEraserItem::OnComponentBeginOverlap(collisionComp, otherActor, otherCollisionComp, 0, false, hitResult);
}

void AEraserItem::OnComponentBeginOverlap(UPrimitiveComponent* overlappedComp, AActor* otherActor, UPrimitiveComponent* otherComp, int32 otherBodyIndex, bool isFromSweep, const FHitResult& sweepResult)
{
	AWhiteboardItem* collidedWhiteboard = Cast<AWhiteboardItem>(otherActor);

	if (collidedWhiteboard != NULL)
	{
		//Eraser collided with a white-board so retain the reference to the specific one the eraser collided with.
		myWhiteboard = collidedWhiteboard;

		//Play vibration feedback.
		Super::VibrateTouchingHands(EVibrationType::VE_TOUCH);
	}
}

void AEraserItem::OnComponentEndOverlap(UPrimitiveComponent* overlappedComp, AActor* otherActor, UPrimitiveComponent* otherComp, int32 otherBodyIndex)
{
	if (otherActor != NULL && myWhiteboard == Cast<AWhiteboardItem>(otherActor))
	{
		//Remove the reference to the white-board since we are no longer interacting with it.
		myWhiteboard = NULL;
	}
}

void AEraserItem::Tick(float deltaTime)
{
	TArray<FVector>* intersectionVertices = this->FindIntersectionVertices();
	if (intersectionVertices != NULL && myPrimaryHand != NULL && myWhiteboard != NULL)
	{
		//Only allow erasing when in range of the white-board.
		if (FVector::Dist(this->GetActorLocation(), myWhiteboard->GetActorLocation()) > myPrimaryHand->GetReachDistance())
		{
			AEraserItem::OnComponentEndOverlap(myBoxCollisionComponent, myWhiteboard, NULL, 0);
		}
		else
		{
			myWhiteboard->DrawPolygon(intersectionVertices, myPaintMaterial);
		}

		delete intersectionVertices;
	}
}

TArray<FVector>* AEraserItem::FindIntersectionVertices()
{
	UStaticMeshComponent* drawableBoard;
	FPlane surfacePlane;
	FRotator boxRotation;
	FVector boxLocation;
	FVector boxExtent;
	FVector outLinePlaneIntersection;
	float outLinePlaneIntersectionTime;
	TArray<FVector>* intersectionVertices = NULL;
	FVector tmpVertices[8];
	int tmpEdges[24];

	if (myBoxCollisionComponent != NULL && myWhiteboard != NULL)
	{
		drawableBoard = myWhiteboard->GetDrawableBoard();
		if (drawableBoard != NULL)
		{
			boxLocation = myBoxCollisionComponent->GetComponentLocation();
			boxRotation = myBoxCollisionComponent->GetComponentRotation();
			boxExtent = myBoxCollisionComponent->GetUnscaledBoxExtent();

			//Find and calculate the vertices and edges of the box extent.
			tmpVertices[0] = boxRotation.RotateVector(boxExtent) + boxLocation;
			tmpVertices[1] = boxRotation.RotateVector(boxExtent) - boxLocation;
			tmpVertices[2] = boxRotation.RotateVector(FVector(-boxExtent.X, boxExtent.Y, boxExtent.Z)) + boxLocation;
			tmpVertices[3] = boxRotation.RotateVector(FVector(boxExtent.X, -boxExtent.Y, boxExtent.Z)) + boxLocation;
			tmpVertices[4] = boxRotation.RotateVector(FVector(boxExtent.X, boxExtent.Y, -boxExtent.Z)) + boxLocation;
			tmpVertices[5] = boxRotation.RotateVector(FVector(-boxExtent.X, -boxExtent.Y, boxExtent.Z)) + boxLocation;
			tmpVertices[6] = boxRotation.RotateVector(FVector(boxExtent.X, -boxExtent.Y, -boxExtent.Z)) + boxLocation;
			tmpVertices[7] = boxRotation.RotateVector(FVector(-boxExtent.X, boxExtent.Y, -boxExtent.Z)) + boxLocation;

			tmpEdges[0] = 0;
			tmpEdges[1] = 3;
			tmpEdges[2] = 0;
			tmpEdges[3] = 4;
			tmpEdges[4] = 0;
			tmpEdges[5] = 2;
			tmpEdges[6] = 6;
			tmpEdges[7] = 3;
			tmpEdges[8] = 6;
			tmpEdges[9] = 4;
			tmpEdges[10] = 6;
			tmpEdges[11] = 1;
			tmpEdges[12] = 3;
			tmpEdges[13] = 5;
			tmpEdges[14] = 4;
			tmpEdges[15] = 7;
			tmpEdges[16] = 2;
			tmpEdges[17] = 7;
			tmpEdges[18] = 2;
			tmpEdges[19] = 5;
			tmpEdges[20] = 1;
			tmpEdges[21] = 5;
			tmpEdges[22] = 1;
			tmpEdges[23] = 7;

			intersectionVertices = new TArray<FVector>();
			for (int index = 0; index <= 11; index++)
			{
				//Get the plane of the drawable part of the white-board.
				surfacePlane = UKismetMathLibrary::MakePlaneFromPointAndNormal(drawableBoard->GetComponentLocation(), drawableBoard->GetForwardVector());

				//Calculate the plane intersection of the eraser and the drawable part of the white-board.
				if (UKismetMathLibrary::LinePlaneIntersection(tmpVertices[tmpEdges[2 * index]], tmpVertices[tmpEdges[2 * index + 1]], surfacePlane, outLinePlaneIntersectionTime, outLinePlaneIntersection) == true)
				{
					intersectionVertices->Add(outLinePlaneIntersection);
				}
			}
		}

	}

	return intersectionVertices;
}

