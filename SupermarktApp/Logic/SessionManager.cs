public static class SessionManager
{
    public static UserModel? CurrentUser { get; private set; }
     
    //tracks if we already sent an exit cart email this session
    public static bool HasSentExitCartEmail { get; private set; }

    public static void Login(UserModel user)
    {
        CurrentUser = user;
        HasSentExitCartEmail = false; // new session, reset to false
    }
    public static void Logout()
    {
        CurrentUser = null;
        HasSentExitCartEmail = false;  // clean reset here
    }
    public static void UpdateCurrentUser(int CurrentUserID)
    {
        UserModel newCurrentUser = UserAccess.GetUserByID(CurrentUserID);
        if (CurrentUser == null || newCurrentUser == null) return;
        CurrentUser = newCurrentUser;
    }
}