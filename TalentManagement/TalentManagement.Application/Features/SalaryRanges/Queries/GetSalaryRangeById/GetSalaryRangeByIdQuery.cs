﻿namespace TalentManagement.Application.Features.SalaryRanges.Queries.GetSalaryRangeById
{
    public class GetSalaryRangeByIdQuery : IRequest<Response<SalaryRange>>
    {
        public Guid Id { get; set; }

        public class GetSalaryRangeByIdQueryHandler : IRequestHandler<GetSalaryRangeByIdQuery, Response<SalaryRange>>
        {
            private readonly ISalaryRangeRepositoryAsync _repository;

            public GetSalaryRangeByIdQueryHandler(ISalaryRangeRepositoryAsync repository)
            {
                _repository = repository;
            }

            public async Task<Response<SalaryRange>> Handle(GetSalaryRangeByIdQuery query, CancellationToken cancellationToken)
            {
                var entity = await _repository.GetByIdAsync(query.Id);
                if (entity == null) throw new ApiException($"SalaryRange Not Found.");
                return new Response<SalaryRange>(entity);
            }
        }
    }
}