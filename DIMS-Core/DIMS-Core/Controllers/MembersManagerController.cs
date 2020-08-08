﻿using AutoMapper;
using DIMS_Core.BusinessLayer.Interfaces;
using DIMS_Core.BusinessLayer.Models.BaseModels;
using DIMS_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DIMS_Core.Controllers
{


    public class MembersManagerController : Controller
    {
        private readonly IMapper mapper;
        private readonly IUserProfileService userProfileService;
        private readonly IVUserProfileService vUserProfileService;
        private readonly IVUserProgressService vUserProgressService;
        private readonly IDirectionService directionService;
        private readonly IUserIdentityService userIdentityService;

        public MembersManagerController(IUserProfileService userProfileService, IVUserProfileService vUserProfileService, IVUserProgressService vUserProgressService,
            IDirectionService directionService, IUserIdentityService userIdentityService ,IMapper mapper)
        {
            this.userProfileService = userProfileService;
            this.vUserProfileService = vUserProfileService;
            this.vUserProgressService = vUserProgressService;
            this.directionService = directionService;
            this.userIdentityService = userIdentityService;
            this.mapper = mapper;
        }

        [Authorize(Roles = "Admin, Mentor")]
        public async Task<IActionResult> MembersManageGrid()
        {
            MembersGridViewModel model = new MembersGridViewModel();
            var vUserProfiles = await vUserProfileService.GetAllAsync();
            model.vUserProfileViewModels = mapper.ProjectTo<vUserProfileViewModel>(vUserProfiles.AsQueryable());
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditUser(int UserId)
        {
            var directions = await directionService.GetAllAsync();
            ViewBag.directions = new SelectList(directions, "DirectionId", "Name");
            var userProfile = await userProfileService.GetEntityModelAsync(UserId);
            var mappedProfile = mapper.Map<UserProfileEditViewModel>(userProfile);
            return PartialView("MemberEditWindow", mappedProfile);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditUser(UserProfileEditViewModel model)
        {
            var existingModel = await userProfileService.GetEntityModelAsync(model.UserId.Value);
            mapper.Map(model, existingModel);
            await userProfileService.UpdateAsync(existingModel);

            return RedirectToAction("MembersManageGrid");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> RegistUser()
        {
            var directions = await directionService.GetAllAsync();
            ViewBag.directions = new SelectList(directions, "DirectionId", "Name");
            var model = new UserRegistViewModel();
            return PartialView("RegistUserWindow", model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task RegistUser(UserRegistViewModel model)
        {
            var userProfileModel = mapper.Map<UserProfileModel>(model);
            await userProfileService.CreateAsync(userProfileModel);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(int UserId, string FullName)
        {
            DeleteUserViewModel model = new DeleteUserViewModel(UserId, FullName);
            return PartialView("DeleteWindow", model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(DeleteUserViewModel model)
        {
            await userProfileService.DeleteAsync(model.UserId);
            return RedirectToAction("MembersManageGrid");
        }

        public IActionResult Progress(int UserId)
        {
            return RedirectToAction("MemberProgressGrid", "TaskManager", UserId);
        }
    }
}