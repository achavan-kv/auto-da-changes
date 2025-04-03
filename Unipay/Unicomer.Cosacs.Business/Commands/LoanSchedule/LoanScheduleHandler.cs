/* 
Author: Suresh -IGT
Date: Feb 2st
Description:save Customer API 
 */

using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Unicomer.Cosacs.Business.Commands.LoanProcess;
using Unicomer.Cosacs.Model;
using Unicomer.Cosacs.Model.ViewModels;

namespace Unicomer.Cosacs.Business.Commands.LoanSchedule
{
    public class LoanScheduleHandler : IRequestHandler<LoanScheduleCommand, LoanResponseModel>
    {
        private readonly IMediator mediator;

        public LoanScheduleHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<LoanResponseModel> Handle(LoanScheduleCommand request, CancellationToken cancellationToken)
        {
            var command = new LoanProcessCommand
            {
                CosacsAccountId = request.CosacsAccountId,
                ProcessRef = ApplicationSettingManager.Schedule
            };
            command.SetApiName(request.GetRequest().RequestName);
            var response = await this.mediator.Send(command);

            return response;
        }
    }
}
