using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedAssist.TelegramBot.Worker.Services.Api;

public class SpecialityUpdateRequest
{
    public required string Code { get; set; }
}