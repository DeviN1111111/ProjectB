public static class SessionManager
{
    public static UserModel? CurrentUser { get; set; }
     
    //tracks if we already sent an exit cart email this session
    public static bool HasSentExitCartEmail { get; set; }
    public static void Logout()
    {   // clear the user session
        CurrentUser = null;
        HasSentExitCartEmail = false; 
    }
    public static void UpdateCurrentUser(int CurrentUserID)
    {
        UserModel newCurrentUser = UserAccess.GetUserByID(CurrentUserID);
        if (CurrentUser == null || newCurrentUser == null) return;
        CurrentUser = newCurrentUser;
    }
}