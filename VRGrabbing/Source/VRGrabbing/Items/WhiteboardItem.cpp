#include "Smash.h"
#include "Runtime/Engine/Classes/Kismet/KismetMathLibrary.h"
#include "Runtime/Engine/Classes/Kismet/KismetRenderingLibrary.h"
#include "WhiteboardItem.h"

AWhiteboardItem::AWhiteboardItem()
{
	myBoardComponent = NULL;
}

AWhiteboardItem::~AWhiteboardItem()
{

}

void AWhiteboardItem::BeginPlay()
{
	FTimerHandle unusedHandle;

	Super::BeginPlay();

	//Get all the actor components in the Button Item blueprint.
	TArray<UActorComponent*> components = this->GetComponentsByClass(UActorComponent::StaticClass());
	for (int currComponentIndex = 0; currComponentIndex < components.Num(); currComponentIndex++)
	{
		if (components[currComponentIndex]->GetName().Equals("Board", ESearchCase::IgnoreCase) == true)
		{
			myBoardComponent = Cast<UStaticMeshComponent>(components[currComponentIndex]);
			break;
		}
	}

	//Delay calling DrawMaterialToRenderTarget() by 1 second.
	GetWorldTimerManager().SetTimer(unusedHandle, this, &AWhiteboardItem::ClearRenderTarget, 1.0f, false);
}

void AWhiteboardItem::Tick(float deltaTime)
{

}

void AWhiteboardItem::DrawMaterialToRenderTarget(UMaterial* material)
{
	if (myRenderTarget != NULL)
	{
		//Apply a specified material to the render target of the white-board.
		UKismetRenderingLibrary::DrawMaterialToRenderTarget(this->GetWorld(), myRenderTarget, material);
	}
}

void AWhiteboardItem::ClearRenderTarget()
{
	AWhiteboardItem::DrawMaterialToRenderTarget(myClearMaterial);
}

void AWhiteboardItem::DrawPolygon(TArray<FVector>* intersectionVertices, UMaterial* paintMaterial)
{
	FVector2D outSize;
	FDrawToRenderTargetContext outRenderTargetContext;
	UCanvas* outCanvas;
	TArray<FVector2D>* relativeInsterctVerts;
	TArray<FCanvasUVTri>* canvasUVTriPoints;

	if (intersectionVertices != NULL && paintMaterial != NULL)
	{
		relativeInsterctVerts = AWhiteboardItem::BoardWorldToRelative(*intersectionVertices);
		canvasUVTriPoints = AWhiteboardItem::ConvexTriangulate2D(*relativeInsterctVerts);

		UKismetRenderingLibrary::BeginDrawCanvasToRenderTarget(this->GetWorld(), myRenderTarget, outCanvas, outSize, outRenderTargetContext);
		if (outCanvas != NULL)
		{
			if (canvasUVTriPoints != NULL)
			{
				outCanvas->K2_DrawMaterialTriangle(paintMaterial, *canvasUVTriPoints);
				UKismetRenderingLibrary::EndDrawCanvasToRenderTarget(this->GetWorld(), outRenderTargetContext);

				delete canvasUVTriPoints;
			}
		}
	}
}

void AWhiteboardItem::DrawLine(FVector startLocation, FVector endLocation, float thickness, FLinearColor color)
{
	FVector2D boardRelativeStart;
	FVector2D boardRelativeEnd;
	FVector2D outSize;
	FDrawToRenderTargetContext outRenderTargetContext;
	UCanvas* outCanvas;
	
	boardRelativeStart = AWhiteboardItem::BoardWorldToRelative(startLocation);
	boardRelativeEnd = AWhiteboardItem::BoardWorldToRelative(endLocation);

	if (myRenderTarget != NULL)
	{
		UKismetRenderingLibrary::BeginDrawCanvasToRenderTarget(this->GetWorld(), myRenderTarget, outCanvas, outSize, outRenderTargetContext);
		if (outCanvas != NULL)
		{
			outCanvas->K2_DrawLine(boardRelativeStart, boardRelativeEnd, thickness, color);
			UKismetRenderingLibrary::EndDrawCanvasToRenderTarget(this->GetWorld(), outRenderTargetContext);	
		}
	}
}

FVector2D AWhiteboardItem::BoardWorldToRelative(FVector worldPos)
{
	FVector2D retVal = FVector2D::ZeroVector;
	FVector2D renderTargetSize;
	FVector minRelativeBounds;
	FVector maxRelativeBounds;
	FVector relativePos;
	FVector relativeBounds;

	if (myRenderTarget != NULL && myBoardComponent != NULL)
	{
		//Get the dimensions of the render target.
		renderTargetSize.X = myRenderTarget->GetSurfaceWidth();
		renderTargetSize.Y = myRenderTarget->GetSurfaceHeight();

		//Get the local space relative location of the specified world location
		relativePos = UKismetMathLibrary::InverseTransformLocation(this->GetActorTransform(), worldPos);

		//Get the bounds for the board component.
		myBoardComponent->GetLocalBounds(minRelativeBounds, maxRelativeBounds);

		//Factor in the scale of the component.
		relativeBounds = myBoardComponent->RelativeScale3D * (maxRelativeBounds - minRelativeBounds);

		//Offset to corner
		relativePos = FVector(relativePos.X, relativePos.Y + (relativeBounds.Y / 2), relativePos.Z + (relativeBounds.Z / 2));

		//Normalize
		relativePos = FVector(relativePos.X, relativePos.Y / relativeBounds.Y, relativePos.Z / relativeBounds.Z);

		//Invert the top left
		relativePos = FVector(relativePos.X, 1 - relativePos.Y, 1 - relativePos.Z);

		//Scale the position according to the size of the render target.
		retVal = FVector2D(relativePos.Y, relativePos.Z) * renderTargetSize;
	}

	return retVal;
}

TArray<FVector2D>* AWhiteboardItem::BoardWorldToRelative(TArray<FVector> points)
{
	FVector2D currPoint;

	TArray<FVector2D>* tmpPositions = new TArray<FVector2D>();
	for (int currPointIndex = 0; currPointIndex < points.Num(); currPointIndex++)
	{
		currPoint = AWhiteboardItem::BoardWorldToRelative(points[currPointIndex]);
		tmpPositions->Add(currPoint);
	}

	return tmpPositions;
}

FVector AWhiteboardItem::BoardRelativeToWorld(FVector2D relativeLocation)
{
	FVector retVal = FVector::ZeroVector;
	FVector2D renderTargetSize;
	FVector minRelativeBounds;
	FVector maxRelativeBounds;
	FVector relativeBounds;
	FVector worldPos;

	if (myRenderTarget != NULL && myBoardComponent != NULL)
	{
		//Get the dimensions of the render target.
		renderTargetSize.X = myRenderTarget->GetSurfaceWidth();
		renderTargetSize.Y = myRenderTarget->GetSurfaceHeight();

		worldPos = FVector(renderTargetSize.X / relativeLocation.X, renderTargetSize.Y / relativeLocation.Y, 0.0f);

		//Invert to bottom right.
		worldPos = FVector(1 - worldPos.X, 1 - worldPos.Y, 0.0f);

		//Get the bounds for the board component.
		myBoardComponent->GetLocalBounds(minRelativeBounds, maxRelativeBounds);

		//Factor in the scale of the component.
		relativeBounds = myBoardComponent->RelativeScale3D * (maxRelativeBounds - minRelativeBounds);

		worldPos = FVector((worldPos.X * relativeBounds.Y) - (relativeBounds.Y / 2.0f), (worldPos.Y * relativeBounds.Z) - (relativeBounds.Z / 2.0f), 0.0f);

		retVal = UKismetMathLibrary::TransformLocation(myBoardComponent->GetComponentTransform(), FVector(0.0f, worldPos.X, worldPos.Y));
	}

	return retVal;
}

TArray<FCanvasUVTri>* AWhiteboardItem::ConvexTriangulate2D(TArray<FVector2D> points)
{
	TArray<FCanvasUVTri>* tmpUVTris = NULL;

	AWhiteboardItem::SortPoints(points);

	if (points.Num() > 2)
	{
		tmpUVTris = new TArray<FCanvasUVTri>();

		AWhiteboardItem::ConvexTriangulate2DRecursive(tmpUVTris, 1, 2, points);
	}

	return tmpUVTris;
}

void AWhiteboardItem::ConvexTriangulate2DRecursive(TArray<FCanvasUVTri>* uvTris, int b, int c, TArray<FVector2D> points)
{
	FCanvasUVTri uvTriToAdd;

	if (uvTris != NULL && b <= (points.Num() - 1) && c <= (points.Num() - 1))
	{
		uvTriToAdd.V0_Pos = points[0];
		uvTriToAdd.V1_Pos = points[b];
		uvTriToAdd.V2_Pos = points[c];

		uvTris->Add(uvTriToAdd);

		AWhiteboardItem::ConvexTriangulate2DRecursive(uvTris, c, c + 1, points);
	}
}

FVector2D AWhiteboardItem::Vector2DCenter(TArray<FVector2D> points)
{
	FVector2D retVal = FVector2D::ZeroVector;

	for (int currPointIndex = 0; currPointIndex < points.Num(); currPointIndex++)
	{
		retVal = retVal + points[currPointIndex];
	}

	retVal = retVal / points.Num();
	return retVal;
}

bool AWhiteboardItem::PointLessThan(FVector2D a, FVector2D b, FVector2D center)
{
	if (UKismetMathLibrary::CrossProduct2D(a - center, b - center) < 0.0f)
	{
		return true;
	}
	
	return false;
}

void AWhiteboardItem::SortPoints(TArray<FVector2D>& points)
{
	FVector2D centerPoint = AWhiteboardItem::Vector2DCenter(points);
	FVector2D jugglerPoint;

	for (int currPointIndex = 1; currPointIndex < points.Num(); currPointIndex++)
	{
		while (currPointIndex > 0 && AWhiteboardItem::PointLessThan(points[currPointIndex - 1], points[currPointIndex], centerPoint) == true)
		{
			jugglerPoint = points[currPointIndex];
			points[currPointIndex] = points[currPointIndex - 1];
			points[currPointIndex - 1] = jugglerPoint;

			currPointIndex--;
		}
	}
}

void AWhiteboardItem::EndPlay(const EEndPlayReason::Type endPlayReason)
{
	this->ClearRenderTarget();
}

UStaticMeshComponent* AWhiteboardItem::GetDrawableBoard()
{
	return myBoardComponent;
}