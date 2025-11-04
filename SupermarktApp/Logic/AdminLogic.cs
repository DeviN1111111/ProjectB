using System.Reflection.Metadata.Ecma335;
using Spectre.Console;

public class AdminLogic
{
    public static readonly Color AsciiPrimary = Color.FromHex("#247BA0");
    public static List<UserModel> GetAllUsers()
    {
        List<UserModel> AllUsers = AdminAccess.GetAllUsers();
        return AllUsers;
    }

    public static bool DeleteUser(int UserID)
    {
        if (SessionManager.CurrentUser!.ID == UserID)// Voorkomt dat je jezelf kan deleten.
        {
            return false;
        }
        else
        {
            AdminAccess.DeleteUserByID(UserID);
            return true;
        }
    }

    public static bool ChangeRole(int UserID, string NewRole)
    {
        if (SessionManager.CurrentUser!.ID == UserID)// Voorkomt dat je jouw eigen role kan veranderen.
        {
            return false;
        }
        else
        {
            AdminAccess.ChangeRole(UserID, NewRole);
            return true;
        }
    }
}