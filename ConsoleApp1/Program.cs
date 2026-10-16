using DataLayer.DataAccess;

var user = UserData.GetUserByID(1);

Console.WriteLine(user.Name);