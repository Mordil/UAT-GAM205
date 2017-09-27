public class ComputerTankManager : BaseTankManager
{
    // private float _timeOfLastHealthGain;

    //public void RegenerateHealth()
    //{
    //    if (_timeOfLastHealthGain == 0)
    //    {
    //        _timeOfLastHealthGain = Time.time;
    //        return;
    //    }

    //    float timeDiff = Time.time - _timeOfLastHealthGain;

    //    // if it has been at least 1 second
    //    if (timeDiff >= 1)
    //    {
    //        _timeOfLastHealthGain = Time.time;
    //        _currentHealth += (int)(_settings.HealthRegenRate * timeDiff);
    //        _currentHealth = Mathf.Clamp(_currentHealth, 0, _settings.MaxHealth);
    //    }
    //}
}
