public static class SessionManager
{
    public static UserModel? CurrentUser { get; set; }

    public static void UpdateCurrentUser(int CurrentUserID)
    {
        UserModel newCurrentUser = UserAccess.GetUserByID(CurrentUserID);
        if (CurrentUser == null || newCurrentUser == null) return;
        CurrentUser = newCurrentUser;
    }
}