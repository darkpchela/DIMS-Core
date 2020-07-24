﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DIMS_Core.BusinessLayer.Interfaces.Admin;
using DIMS_Core.BusinessLayer.Models.Admin;
using DIMS_Core.DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DIMS_Core.BusinessLayer.Services
{
    public class UserProfileViewService : IUserProfileViewService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public UserProfileViewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<VUserProfileModel>> GetAllUserProfileViews()
        {
            var result = unitOfWork.VUserProfileRepository.GetAll();
            var mappedResult = mapper.ProjectTo<VUserProfileModel>(result);

            return await mappedResult.ToListAsync();
        }
        public async Task<VUserProfileModel> GetUserProfileViewById(int userId)
        {
            var entity = await unitOfWork.VUserProfileRepository.GetByIdAsync(userId);
            var vUserModel = mapper.Map<VUserProfileModel>(entity);
            return vUserModel;
        }

        public async Task<VUserProfileModel> GetUserProfileViewByEmail(string email)
        {
            var entity = await unitOfWork.VUserProfileRepository.GetVUserProfileByEmail(email);
            var vUserModel = mapper.Map<VUserProfileModel>(entity);
            return vUserModel;
        }


        public async Task<IEnumerable<VUserProfileModel>> GetUserProfileViewsByDirection(string direction)
        {
            var entity = unitOfWork.VUserProfileRepository.GetVUserProfilesByDirection(direction);
            var vUserModel = mapper.ProjectTo<VUserProfileModel>(entity);
            return await vUserModel.ToListAsync();
        }
    }
}