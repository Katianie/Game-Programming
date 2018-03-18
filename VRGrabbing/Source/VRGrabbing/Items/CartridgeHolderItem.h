// Copyright 1998-2015 Epic Games, Inc. All Rights Reserved.
#pragma once
#include "BaseItem.h"
#include "CartridgeItem.h"
#include "CartridgeHolderItem.generated.h"

DECLARE_DYNAMIC_MULTICAST_DELEGATE(FCartridgeHolderInsertSignatrue);
DECLARE_DYNAMIC_MULTICAST_DELEGATE(FCartridgeHolderRemoveSignatrue);

UCLASS(config = Game)
class ACartridgeHolderItem : public ABaseItem
{
	GENERATED_BODY()
private:
	FCartridgeHolderInsertSignatrue CartridgeHolder_OnCartridgeInserted;
	FCartridgeHolderRemoveSignatrue CartridgeHolder_OnCartridgeRemove;

	ACartridgeItem* myCurrentCartridge;

public:
	ACartridgeHolderItem();

	~ACartridgeHolderItem();

	virtual void NotifyActorBeginOverlap(AActor* otherActor) override;
	virtual void NotifyActorEndOverlap(AActor* otherActor) override;

	AActor* GetCurrentCartridge();
};