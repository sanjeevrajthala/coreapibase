using AutoMapper;
using CoreBase.Dtos;
using CoreBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBase.Profiles
{
    public class CommandsProfile:Profile
    {
        public CommandsProfile()
        {
            //source -> target
            CreateMap<Command, CommandReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<CommandUpdateDto, Command>();
            CreateMap<Command, CommandUpdateDto>();
        }
    }
}
