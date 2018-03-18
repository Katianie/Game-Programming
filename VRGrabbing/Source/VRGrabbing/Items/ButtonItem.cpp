#include "Smash.h"
#include "ButtonItem.h"

AButtonItem::AButtonItem() : Super()
{
	myMaxButtonPress = 2.0f;
	myHasBeenTouchedOnce = false;
	myButtonComponent = NULL;

	//Enable Tick for BaseItem(s).
	PrimaryActorTick.bCanEverTick = true;
}

AButtonItem::~AButtonItem()
{

}

void AButtonItem::BeginPlay()
{
	Super::BeginPlay();

	//Get all the components in the Button Item blueprint.
	TArray<UActorComponent*> components = this->GetComponentsByClass(UStaticMeshComponent::StaticClass());

	for (int currComponentIndex = 0; currComponentIndex < components.Num(); currComponentIndex++)
	{
		if (components[currComponentIndex]->GetName().Equals("Button", ESearchCase::IgnoreCase) == true)
		{
			myButtonComponent = Cast<UStaticMeshComponent>(components[currComponentIndex]);
			break;
		}
	}

	if (myButtonComponent != NULL)
	{
		myButtonInitialPosition = myButtonComponent->GetRelativeTransform().GetLocation();
	}
}

void AButtonItem::Tick(float deltaTime)
{
	FTransform buttonWorldTransform;
	FVector buttonLocalSpacePos;
	FVector ownerLocalSpacePos;
	FVector localDiff;
	float buttonPressAmount;

	Super::Tick(deltaTime);

	if (myButtonComponent != NULL)
	{
		if (myPrimaryHand != NULL)
		{
			//Get the world space location of the button.
			buttonWorldTransform = myButtonComponent->GetComponentTransform();

			//Convert the location of the button and the location of the hand to local space.
			buttonLocalSpacePos = UKismetMathLibrary::InverseTransformDirection( buttonWorldTransform, myInitialOverlapPosition );
			ownerLocalSpacePos = UKismetMathLibrary::InverseTransformDirection( buttonWorldTransform, myPrimaryHand->GetControllerLocation() + (myPrimaryHand->GetControllerRotation().Vector() * myPrimaryHand->GetReachDistance()) );

			//Vector distance between button and hand in local space.
			localDiff = ownerLocalSpacePos - buttonLocalSpacePos;

			//Only interested in the z value difference.
			buttonPressAmount = FMath::Clamp(FMath::Abs(localDiff.Z), 0.0f, myMaxButtonPress);
			localDiff.Set(0.0f, 0.0f, buttonPressAmount);

			//Set the new relative position of button based on the hand and the start button position.
			myButtonComponent->SetRelativeLocation(myButtonInitialPosition - localDiff);

			//UE_LOG(LogTemp, Error, TEXT("buttonPressAmount:%f"), buttonPressAmount);
			if (buttonPressAmount >= myMaxButtonPress)
			{
				if (myHasBeenTouchedOnce == false)
				{
					//Fire button pressed delegate
					if (ButtonItem_OnPressed.IsBound() == true)
					{
						ButtonItem_OnPressed.Broadcast();
					}
					
					myHasBeenTouchedOnce = true;
					myButtonComponent->SetScalarParameterValueOnMaterials("State", 1.0f);
					Super::VibrateTouchingHands(EVibrationType::VE_TOUCH);
				}
			}
		}
		else
		{
			//Slowly reset the button position back to the initial position when not being touched.
			FVector newPosition = FMath::VInterpTo(myButtonComponent->GetRelativeTransform().GetLocation(), myButtonInitialPosition, deltaTime, 10.0f);
			myButtonComponent->SetRelativeLocation(newPosition);
		}
	}
}

void AButtonItem::OnPlayerTouched(ABaseHand* playerHand)
{
	if (playerHand != NULL)
	{
		myPrimaryHand = playerHand;
		myInitialOverlapPosition = myPrimaryHand->GetControllerLocation() + (myPrimaryHand->GetControllerRotation().Vector() * myPrimaryHand->GetReachDistance());
	}
}

void AButtonItem::OnPlayerDrop(ABaseHand* playerHand)
{
	Super::OnPlayerDrop(playerHand);

	if (myButtonComponent != NULL)
	{
		//Reset the state of the button.
		myButtonComponent->SetScalarParameterValueOnMaterials("State", 0.0);
		myHasBeenTouchedOnce = false;
	}
}