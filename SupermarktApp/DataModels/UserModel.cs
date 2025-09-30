public class UserModel
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Adress { get; set; }
    public int HouseNumber { get; set; }
    public string Zipcode { get; set; }
    public string PhoneNumber { get; set; }
    public string City { get; set; }
    public bool IsAdmin { get; set; }

    public UserModel(string name, string lastName, string email, string password, string adress, int houseNumber, string zipcode, string phoneNumber, string city)
    {
        Name = name;
        LastName = lastName;
        Email = email;
        Password = password;
        Adress = adress;
        HouseNumber = houseNumber;
        Zipcode = zipcode;
        PhoneNumber = phoneNumber;
        City = city;
        IsAdmin = false;
    }

    public UserModel() { }
}