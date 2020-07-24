﻿using DIMS_Core.BusinessLayer.Models.User;
using System.Threading.Tasks;

namespace DIMS_Core.BusinessLayer.Interfaces
{
    public interface IUserProfileService
    {
        Task GetUserProfile(int userId);
        Task DeleteUserProfile(int userProfileId);

        Task UpdateUserProfile(UserProfileModel userModel);

        Task CreateUserProfile(UserProfileModel userModel);
    }
}