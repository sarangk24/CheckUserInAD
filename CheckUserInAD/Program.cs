using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using System.DirectoryServices;
using Microsoft.SharePoint;
namespace CheckUserInAD
{
    class Program
    {
        static void Main(string[] args)
        {
            

            var adUserName = "s-lighthouse-softwar";//ConfigurationManager.AppSettings["ADUserName"];
            var adPassword = "1Welcome";//ConfigurationManager.AppSettings["ADPassword"];
            var adServerName = "mpdomvp1.domain.internal";// ConfigurationManager.AppSettings["ADServerName"];
            var userToSearch=string.Empty;
            while (true)
            {
                Console.WriteLine("Press \n 1.User Check By UserName \n 2.Group Check \n 3.User Check By SurName \n 4. User's Manager");
                int result = 1;

                int.TryParse(Console.ReadLine(), out result);
                if (result < 5)
                {
                    Console.WriteLine("Please enter user/group name without domain and suffixes like i:0# :");
                    userToSearch = Console.ReadLine();
                }
                switch (result)
                {
                    case 1: CheckIfUserExists(adServerName, adUserName, adPassword, userToSearch);

                        break;
                    case 2: CheckIfGroupAndItsSubmembersExists(adServerName, adUserName, adPassword, userToSearch);

                        break;
                    case 3: FindUserByName(adServerName, adUserName, adPassword, userToSearch);
                        break;
                    case 4: CheckManager(adServerName, adUserName, adPassword, userToSearch);
                        break;

                    default:
                        Console.WriteLine("Please enter proper choice");
                        Console.ReadLine();
                        break;
                }
            }
            
                      
        }



        private static void FindUserByName(string adServerName, string adUserName, string adPassword, string userToSearch)
        {
            Console.WriteLine("Please wait....");
            try
            {

                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, adServerName, adUserName, adPassword))
                {
                    //searching details based on the surname
                    var user = new UserPrincipal(context);
                    user.Surname = userToSearch;

                    PrincipalSearcher pS = new PrincipalSearcher();
                    pS.QueryFilter = user;


                    PrincipalSearchResult<Principal> results = pS.FindAll();

                    var principals = results.ToList();

                    foreach (var principal in principals)
                    {
                        var member = principal.GetUnderlyingObject() as DirectoryEntry;

                        if (member != null && IsActive(member))
                            Console.WriteLine(string.Format("User Details: {0} ---  User Email-> {1}-- UserName -> {2}", principal.DisplayName,UserPrincipal.FindByIdentity(context,principal.SamAccountName).EmailAddress,principal.SamAccountName));
                        else
                            Console.WriteLine("User doesnt exist or inactive :"+principal.DisplayName+ "User Name :-"+principal.SamAccountName);
                    }
                        Console.WriteLine("Press any key to exit.");
                        Console.ReadLine();
                        Console.Clear();
                    
                   


                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception :" + ex.Message);
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                Console.Clear();

            } 
        
             
           
        
        
        
        }

        private static void CheckManager(string adServerName, string adUserName, string adPassword, string userToSearch)
        {
            Console.WriteLine("Please wait....");
            try
            {

                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, adServerName, adUserName, adPassword))
                {
                    //check if group is active
                    var userPrincipal = UserPrincipal.FindByIdentity(context, userToSearch);
                    var member = userPrincipal.GetUnderlyingObject() as DirectoryEntry;
                    if (userPrincipal != null)
                    {

                        var de = userPrincipal.GetUnderlyingObject() as DirectoryEntry;
                            if (IsActive(de))
                                Console.WriteLine(userPrincipal.DisplayName + " Manager Name------->" + de.Properties["Manager"].Value);
                            else
                                Console.WriteLine("Not active :" + userPrincipal.SamAccountName);

                        
                    }
                    else
                    {
                        Console.WriteLine("Group doesnot exist");
                    }

                    Console.WriteLine("Press any key to exit.");
                    Console.ReadLine();
                    Console.Clear();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception :" + ex.Message);
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                Console.Clear();

            } 
                
        
        
        
        }



        private static void CheckIfGroupAndItsSubmembersExists(string adServerName, string adUserName, string adPassword, string groupToSearch)
        {
            Console.WriteLine("Please wait....");
            try
            {

                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, adServerName, adUserName, adPassword))
                {
                    //check if group is active
                    var groupPrincipal = GroupPrincipal.FindByIdentity(context, groupToSearch);
                    if (groupPrincipal != null)
                    {
                        foreach (Principal principal in groupPrincipal.Members)
                        {
                            var de = principal.GetUnderlyingObject() as DirectoryEntry;
                            if (IsActive(de))
                                Console.WriteLine(principal.DisplayName + "------->" + principal.SamAccountName);
                            else
                                Console.WriteLine("Not active :" + principal.SamAccountName);

                        }
                    }
                    else
                    {
                        Console.WriteLine("Group doesnot exist");
                    }

                    Console.WriteLine("Press any key to exit.");
                    Console.ReadLine();
                    Console.Clear();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception :" + ex.Message);
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                Console.Clear();

            } 
        
        
        
        }

        private static void CheckIfUserExists(string adServerName,string adUserName,string adPassword,string userToSearch)
        {
            Console.WriteLine("Please wait....");
            try
            {

                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, adServerName, adUserName, adPassword))
                {
                    var userPrincipal = UserPrincipal.FindByIdentity(context, userToSearch);
                    var member = userPrincipal.GetUnderlyingObject() as DirectoryEntry;
                  
                    //check if user status is active 
                    if (userPrincipal != null && IsActive(member))
                        Console.WriteLine(string.Format("User Details: {0} ---  User Email-> {1}", userPrincipal.DisplayName,userPrincipal.EmailAddress));
                    else
                        Console.WriteLine("User doesnt exist or inactive");
                    Console.WriteLine("Press any key to exit.");
                    Console.ReadLine();
                    Console.Clear();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception :" + ex.Message);
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                Console.Clear();

            } 
        
        
        
       
        }


        




        private static bool IsActive(DirectoryEntry de)
        {
            if (de.NativeGuid == null) return false;

            int flags = (int)de.Properties["userAccountControl"].Value;

            return !Convert.ToBoolean(flags & 0x0002);
        }


        


    }
}