#include "Smash.h"
#include "CartridgeHolderItem.h"

ACartridgeHolderItem::ACartridgeHolderItem()
{
	myCurrentCartridge = NULL;
}

ACartridgeHolderItem::~ACartridgeHolderItem()
{

}

void ACartridgeHolderItem::NotifyActorBeginOverlap(AActor* otherActor)
{
	UPrimitiveComponent* cartridgePrimitiveComponent = NULL;

	if (otherActor != NULL)
	{
		myCurrentCartridge = Cast<ACartridgeItem>(otherActor);
		if (myCurrentCartridge != NULL)
		{
			//Remove the item from the hand(s).
			if (myCurrentCartridge->GetPrimaryHand() != NULL)
			{
				myCurrentCartridge->GetPrimaryHand()->Drop();
			}
			if (myCurrentCartridge->GetSecondaryHand() != NULL)
			{
				myCurrentCartridge->GetSecondaryHand()->Drop();
			}
			
			cartridgePrimitiveComponent = Cast<UPrimitiveComponent>(myCurrentCartridge->GetRootComponent());
			if (cartridgePrimitiveComponent != NULL)
			{
				if (cartridgePrimitiveComponent->IsPhysicsStateCreated() == true)
				{
					cartridgePrimitiveComponent->RecreatePhysicsState();
				}

				cartridgePrimitiveComponent->AttachToComponent(this->GetRootComponent(), FAttachmentTransformRules(EAttachmentRule::SnapToTarget, EAttachmentRule::SnapToTarget, EAttachmentRule::KeepWorld, true), FName::FName("Root"));

				cartridgePrimitiveComponent->SetSimulatePhysics(false);
				cartridgePrimitiveComponent->SetEnableGravity(false);
				cartridgePrimitiveComponent->RecreatePhysicsState();
			}

			//Callback for when the cartridge is inserted.
			if (CartridgeHolder_OnCartridgeInserted.IsBound() == true)
			{
				CartridgeHolder_OnCartridgeInserted.Broadcast();
			}

			//Play vibration feedback.
			Super::VibrateTouchingHands(EVibrationType::VE_TOUCH);
		}
	}
}

void ACartridgeHolderItem::NotifyActorEndOverlap(AActor* otherActor)
{
	if (otherActor != NULL && myCurrentCartridge != NULL)
	{
		if (myCurrentCartridge == Cast<ACartridgeItem>(otherActor))
		{
			//Callback for when the cartridge is removed.
			if (CartridgeHolder_OnCartridgeRemove.IsBound() == true)
			{
				CartridgeHolder_OnCartridgeRemove.Broadcast();
			}

			myCurrentCartridge = NULL;
		}
	}
}

AActor* ACartridgeHolderItem::GetCurrentCartridge()
{
	return myCurrentCartridge;
}