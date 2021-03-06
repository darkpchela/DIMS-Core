﻿using AutoMapper;
using DIMS_Core.BusinessLayer.Interfaces;
using DIMS_Core.BusinessLayer.Models.BaseModels;
using DIMS_Core.Models.ProgressModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DIMS_Core.Controllers
{
    [Authorize]
    public class ProgressController : Controller
    {
        private IVTaskService vTaskService { get; set; }
        private IVUserTaskService vUserTaskService { get; set; }
        private IVUserTrackService vUserTrackService { get; set; }
        private IVUserProfileService vUserProfileService { get; set; }
        private IVUserProgressService vUserProgressService { get; set; }
        private IUserTaskService userTaskService { get; set; }
        private ITaskTrackService taskTrackService { get; set; }
        private IMapper mapper { get; set; }

        public ProgressController(IVUserTaskService vUserTaskService, IVUserProgressService vUserProgressService, IUserTaskService userTaskService,
            IVTaskService vTaskService, ITaskTrackService taskTrackService, IVUserTrackService vUserTrackService, IVUserProfileService vUserProfileService,
            IMapper mapper)
        {
            this.vUserTaskService = vUserTaskService;
            this.vUserProgressService = vUserProgressService;
            this.userTaskService = userTaskService;
            this.vTaskService = vTaskService;
            this.taskTrackService = taskTrackService;
            this.vUserTrackService = vUserTrackService;
            this.vUserProfileService = vUserProfileService;
            this.mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Mentor")]
        public async Task<IActionResult> MemberProgressGrid(int UserId, string UserName)
        {
            var allVUserProgress = await vUserProgressService.GetAllAsync();
            var currentUserProgress = allVUserProgress.Where(up => up.UserId == UserId);

            var model = new MembersProgressViewModel();
            model.vUserProgressModels = currentUserProgress;
            model.UserName = UserName;

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Mentor, Member")]
        public async Task<IActionResult> MembersTasksManageGrid(int? UserId)
        {
            if (UserId is null)
            {
                UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value); ;
            }

            var currentUserProfile = await vUserProfileService.GetEntityModelAsync(UserId.Value);

            var allUserTask = await userTaskService.GetAllAsync();
            var allVUserTask = await vUserTaskService.GetAllAsync();

            var model = new MembersTasksViewModel();
            model.UserName = currentUserProfile.FullName.Split(' ').First();
            model.userTaskModels = (from vut in allVUserTask
                                    join ut in allUserTask on new { uid = vut.UserId, tid = vut.TaskId } equals new { uid = ut.UserId, tid = ut.TaskId }
                                    where ut.UserId == UserId
                                    select new UserTaskViewModel
                                    {
                                        UserTaskId = ut.UserTaskId.Value,
                                        TaskName = vut.TaskName,
                                        DeadlineDate = vut.DeadlineDate,
                                        StartDate = vut.StartDate,
                                        State = vut.State
                                    }).ToList();

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> TaskTracksManageGrid()
        {
            int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var allVUserTrack = await vUserTrackService.GetAllAsync();
            var currentVUserTrack = allVUserTrack.Where(ut => ut.UserId == UserId);
            return View(currentVUserTrack);
        }

        [HttpGet]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> CreateNote(int UserTaskId)
        {
            var currentUserTask = await userTaskService.GetEntityModelAsync(UserTaskId);
            var allVTasks = await vTaskService.GetAllAsync();
            var taskName = allVTasks.First(vt => vt.TaskId == currentUserTask.TaskId).Name;
            var model = new CreateNoteViewModel();
            model.TaskName = taskName;
            return PartialView("CreateNoteWindow", model);
        }

        [HttpPost]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> CreateNote(CreateNoteViewModel model)
        {
            var trackModel = mapper.Map<TaskTrackModel>(model);
            await taskTrackService.CreateAsync(trackModel);
            return RedirectToAction("MembersTasksManageGrid");
        }

        [HttpGet]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> EditNote(int TaskTrackId)
        {
            var model = await taskTrackService.GetEntityModelAsync(TaskTrackId);
            return PartialView("EditNoteWindow", model);
        }

        [HttpPost]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> EditNote(TaskTrackModel model)
        {
            await taskTrackService.UpdateAsync(model);
            return RedirectToAction("TaskTracksManageGrid");
        }

        [HttpGet]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> DeleteNote(int TaskTrackId)
        {
            var model = await vUserTrackService.GetEntityModelAsync(TaskTrackId);
            return PartialView("DeleteNoteWindow", model);
        }

        [HttpPost]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> DeleteNote(VUserTrackModel model)
        {
            await taskTrackService.DeleteAsync(model.TaskTrackId);
            return RedirectToAction("TaskTracksManageGrid");
        }

        [Authorize(Roles = "Admin, Mentor")]
        public async Task<IActionResult> SetSuccess(int UserTaskId)
        {
            var userTaskModel = await userTaskService.GetEntityModelAsync(UserTaskId);
            userTaskModel.StateId = 2;
            await userTaskService.UpdateAsync(userTaskModel);
            return RedirectToAction("MembersTasksManageGrid", new { userTaskModel.UserId });
        }

        [Authorize(Roles = "Admin, Mentor")]
        public async Task<IActionResult> SetFail(int UserTaskId)
        {
            var userTaskModel = await userTaskService.GetEntityModelAsync(UserTaskId);
            userTaskModel.StateId = 3;
            await userTaskService.UpdateAsync(userTaskModel);
            return RedirectToAction("MembersTasksManageGrid", new { userTaskModel.UserId });
        }
    }
}