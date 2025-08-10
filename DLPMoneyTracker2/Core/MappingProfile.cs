using AutoMapper;
using DLPMoneyTracker.Core.ReportDTOs;
using DLPMoneyTracker2.Reports.IncomeStatement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker2.Core;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<IncomeStatementItemDTO, IncomeStatementItemVM>();
    }
}
