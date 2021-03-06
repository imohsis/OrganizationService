﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrgAPI.ViewModel;

namespace OrgAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")] 
    [ApiController]
    public class AccountController : ControllerBase
    {
        UserManager<IdentityUser> userManager;
        SignInManager<IdentityUser> signInManager;

        public AccountController(SignInManager<IdentityUser> _signInManager, UserManager<IdentityUser> _userManager)
        {
            signInManager = _signInManager;
            userManager = _userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    var user = new IdentityUser()
                    {
                        UserName = model.UserName,
                        Email = model.Email
                    };
                    var userResult = await userManager.CreateAsync(user, model.Password);
                    if (userResult.Succeeded)
                    {
                        return Ok(user);
                    }
                    else
                    {
                        foreach (var error in userResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);

                        }
                        return BadRequest(ModelState.Values);
                    }
                }
                return BadRequest(ModelState.Values);
            }
            catch (Exception Ex)
            {
                ModelState.AddModelError("", Ex.Message);
                return BadRequest(ModelState.Values);
            }
           
        }


    }  
}