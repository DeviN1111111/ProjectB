public class UserModel
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Address { get; set; }
    public string Zipcode { get; set; }
    public string PhoneNumber { get; set; }
    public string City { get; set; }
    public string AccountStatus { get; set; }

    public UserModel(string name, string lastName, string email, string password, string address, string zipcode, string phoneNumber, string city)
    {
        Name = name;
        LastName = lastName;
        Email = email;
        Password = password;
        Address = address;
        Zipcode = zipcode;
        PhoneNumber = phoneNumber;
        City = city;
        AccountStatus = "User";
    }

    //public UserModel() { }
}