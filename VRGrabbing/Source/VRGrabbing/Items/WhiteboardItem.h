// Copyright 1998-2015 Epic Games, Inc. All Rights Reserved.
#pragma once
#include "BaseItem.h"
#include "WhiteboardItem.generated.h"

UCLASS(config = Game)
class AWhiteboardItem : public ABaseItem
{
	GENERATED_BODY()

protected:
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Draw)
	UMaterial* myClearMaterial;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = Draw)
	UTextureRenderTarget2D* myRenderTarget;

	UStaticMeshComponent* myBoardComponent;

	///Protected functions
	void ConvexTriangulate2DRecursive(TArray<FCanvasUVTri>* uvTris, int b, int c, TArray<FVector2D> points);

public:
	///Constructor
	AWhiteboardItem();

	///Destructor
	virtual ~AWhiteboardItem();

	///Functions
	virtual void BeginPlay() override;
	virtual void Tick(float deltaTime) override;
	virtual void DrawMaterialToRenderTarget(UMaterial* material);
	virtual void ClearRenderTarget();
	virtual void DrawPolygon(TArray<FVector>* intersectionVertices, UMaterial* paintMaterial);
	virtual void DrawLine(FVector startLocation, FVector endLocation, float thickness, FLinearColor color);
	virtual FVector2D BoardWorldToRelative(FVector worldPos);
	virtual TArray<FVector2D>* BoardWorldToRelative(TArray<FVector> points);
	virtual FVector BoardRelativeToWorld(FVector2D relativeLocation);
	virtual TArray<FCanvasUVTri>* ConvexTriangulate2D(TArray<FVector2D> points);
	virtual void SortPoints(TArray<FVector2D>& points);
	virtual FVector2D Vector2DCenter(TArray<FVector2D> points);
	virtual bool PointLessThan(FVector2D a, FVector2D b, FVector2D center);
	virtual void EndPlay(const EEndPlayReason::Type endPlayReason) override;

	///Getters
	UStaticMeshComponent* GetDrawableBoard();
};