public interface IPowerConsumer
{
	ConsumerType GetConsumerType();
	float GetPowerDemand();
	void SupplyPower(float satisfaction);
}

public enum ConsumerType
{
	Offense,
	Defense,
	Mobility
}