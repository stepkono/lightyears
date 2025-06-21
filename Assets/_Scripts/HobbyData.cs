
public class HobbyData
{
    private string _name;
    public bool IntervalSet { get; private set; } = false; 
    private bool _reminder = false;  
    private int _daysInterval;

    /*SETTERS*/
    public void SetName(string name)
    {
        _name = name;
    }
    public void SetDaysInterval(int daysFrequency)
    {
        _daysInterval = daysFrequency;
    }
    public void ActivateInterval()
    {
        IntervalSet = true;
    }
    public void SwitchReminder()
    {
        if (IntervalSet && _reminder == false)
        {
            _reminder = true;
        }
        else if (!_reminder)
        {
            _reminder = false;
        }
    }
    
    /*GETTER*/
    public string GetName()
    {
        return _name;
    }
    public int GetDaysInterval()
    {
        return _daysInterval;
    }
}
