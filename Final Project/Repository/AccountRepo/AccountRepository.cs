using Final_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace Final_Project.Repository.AccountRepo
{
    public class AccountRepository
    {

        private readonly ProjContext db;
        private readonly UserManager<Account> userManager;
        public AccountRepository(ProjContext db , UserManager<Account> userManager)
        {

            this.db = db;
            this.userManager = userManager;

        }

        public Employee FindEmployee(string email, string password)
        {
            return db.Employees.FirstOrDefault(e => e.Email == email && e.PasswordHash == password);
             
        }

    }
}
