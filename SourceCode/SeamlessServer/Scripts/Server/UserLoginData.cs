using AnonCommandSystem;

public struct UserLoginData : ICommandParameter<UserLoginData>
{
    public string userName;
    public string passWordMD5;

    public string[] GetParameteCompletion(string preInput)
    {
        return null;
    }

    public bool TryParse(string input, out UserLoginData getValue)
    {
        var userPass = input.Split('-');
        if (userPass.Length != 2)
        {
            getValue = this;
            return false;
        }
        userName = userPass[0];
        userName = userPass[1];
        getValue = this;
        return true;
    }
}