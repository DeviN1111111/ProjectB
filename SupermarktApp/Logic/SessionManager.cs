public static class SessionManager
{
    public static UserModel? CurrentUser { get; set; }
     
    //tracks if we already sent an exit CartProduct email this session
    public static bool HasSentExitCartProductEmail { get; set; }
    public static void Logout()
    {   // clear the user session
        CurrentUser = null;
        HasSentExitCartProductEmail = false; 
    }
    public static void UpdateCurrentUser(int CurrentUserID)
    {
        UserModel newCurrentUser = UserAccess.GetUserByID(CurrentUserID);
        if (CurrentUser == null || newCurrentUser == null) return;
        CurrentUser = newCurrentUser;
    }
}