﻿using AutoMapper;
using DIMS_Core.BusinessLayer.Models.BaseModels;
using DIMS_Core.DataAccessLayer.Entities;

namespace DIMS_Core.BusinessLayer.MappingProfiles
{
    public class TaskTrackModelProfile : Profile
    {
        public TaskTrackModelProfile()
        {
            CreateMap<TaskTrack, TaskTrackModel>().ReverseMap();
        }
    }
}