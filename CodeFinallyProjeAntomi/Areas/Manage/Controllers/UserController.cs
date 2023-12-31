﻿using CodeFinallyProjeAntomi.DataAccesLayer;
using CodeFinallyProjeAntomi.Models;
using CodeFinallyProjeAntomi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeFinallyProjeAntomi.Areas.Manage.Controllers
{
    [Area("manage")]
    //[Authorize(Roles ="SuperAdmin")]
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public UserController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int currenPage=1)
        {
            List<AppUser> users =await _userManager.Users
                .Where(u=>u.UserName !=User.Identity.Name)
                .ToListAsync();

            foreach (var item in users)
            {
                item.Roles=   await _userManager.GetRolesAsync(item);
            }

            return View(PageNatedList<AppUser>.Create(users.AsQueryable(),currenPage,4,4));
        }

         public async Task<IActionResult> SetActive(string? id,int currentPage)
         {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            AppUser appUser=await _userManager.FindByIdAsync(id);

            if (appUser == null) return BadRequest();

            bool active = appUser.IsActive;

            appUser.IsActive = !active;

            await _userManager.UpdateAsync(appUser);

            List<AppUser> users = await _userManager.Users
             .Where(u => u.UserName != User.Identity.Name)
             .ToListAsync();

            foreach (var item in users)
            {
                item.Roles = await _userManager.GetRolesAsync(item);
            }

            return PartialView("_changeRolPartial", PageNatedList<AppUser>.Create(users.AsQueryable(), currentPage, 4, 4));
        }

        public async Task<IActionResult> ResetPassword(string id,int currentPage)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            AppUser appUser = await _userManager.FindByIdAsync(id);

            if (appUser == null) return BadRequest();

            string token = await _userManager.GeneratePasswordResetTokenAsync(appUser);

            await _userManager.ResetPasswordAsync(appUser,token,"Salam123%_");

            List<AppUser> users = await _userManager.Users
           .Where(u => u.UserName != User.Identity.Name)
           .ToListAsync();

            foreach (var item in users)
            {
                item.Roles = await _userManager.GetRolesAsync(item);
            }

            return PartialView("_changeRolPartial", PageNatedList<AppUser>.Create(users.AsQueryable(), currentPage, 4, 4));
        }

    }
}
