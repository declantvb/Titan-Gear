public interface IPowerConsumer
{
	float GetPowerDemand();
	void SupplyPower(float satisfaction);
}