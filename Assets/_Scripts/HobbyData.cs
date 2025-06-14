
public class HobbyData
{
    private string _name;
    private int _daysFrequency;

    public void SetName(string name)
    {
        _name = name;
    }

    public void SetDaysFrequency(int daysFrequency)
    {
        _daysFrequency = daysFrequency;
    }

    public string GetName()
    {
        return _name;
    }

    public int GetFrequency()
    {
        return _daysFrequency;
    }
}
