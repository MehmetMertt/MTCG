using MCTG.Classes;


User userA = new User("Mehmet", "StarkesPassword123");
User userB = new User("Max", "StaerkeresPassword123");
userA.BuyPackage();
foreach (var c in userA.Stack.Container)
{
    c.PrintInfo();
}

