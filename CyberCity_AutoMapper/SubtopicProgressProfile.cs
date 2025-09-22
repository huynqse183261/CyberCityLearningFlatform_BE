using AutoMapper;
using CyberCity.Doman.Models;
using CyberCity.DTOs.Subtopics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCity.AutoMapper
{
    public class SubtopicProgressProfile: Profile
    {
        public SubtopicProgressProfile()
        {
            CreateMap<SubtopicProgress, SubtopicProgressDto>();
            CreateMap<SubtopicProgressUpdateDto, SubtopicProgress>();
        }
    }
}
