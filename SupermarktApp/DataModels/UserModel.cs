public class UserModel
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Zipcode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime Birthdate { get; set; }
    public string City { get; set; } = string.Empty;
    public string AccountStatus { get; set; } = "User";
    public int AccountPoints { get; set; }
    public bool TwoFAEnabled { get; set; }
    public string? TwoFACode { get; set; }
    public DateTime? TwoFAExpiry { get; set; }
    public DateTime? LastBirthdayGift { get; set; }


    public UserModel(string name, string lastName, string email, string password, string address, string zipcode, string phoneNumber, DateTime birthdate, string city, bool twoFAEnabled = false, string accountStatus = "User", int accountPoints = 0)
    {
        Name = name;
        LastName = lastName;
        Email = email;
        Password = password;
        Address = address;
        Zipcode = zipcode;
        PhoneNumber = phoneNumber;
        Birthdate = birthdate;
        City = city;
        AccountStatus = accountStatus;
        AccountPoints = accountPoints;
        TwoFAEnabled = twoFAEnabled;
    }
    public UserModel() { }
}