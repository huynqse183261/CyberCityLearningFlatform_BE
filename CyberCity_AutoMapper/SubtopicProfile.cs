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
    public class SubtopicProfile: Profile
    {
        public SubtopicProfile()
        {
            CreateMap<SubtopicCreateDto, Subtopic>();
            CreateMap<Subtopic,SubtopicDetailDto>().ReverseMap();
            CreateMap<SubtopicUpdateDto,Subtopic>();
        }
    }
}
