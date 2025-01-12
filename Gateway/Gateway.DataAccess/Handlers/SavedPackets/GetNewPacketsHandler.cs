using Dapper;
using Gateway.Domain.Entities;
using Gateway.Domain.Persistence;
using MediatR;

namespace Gateway.Domain.Handlers.SavedPackets;

public record GetNewPackets(int BatchLimit) : IRequest<SavedPacket[]>;
public class GetNewPacketsHandler(IConnectionProvider connectionProvider) : IRequestHandler<GetNewPackets, SavedPacket[]>
{
    private const string Query = @"SELECT * FROM public.saved_packet
                                   WHERE state = 'new'
                                   ORDER BY created_on ASC, priority ASC LIMIT :batchLimit";

    public async Task<SavedPacket[]> Handle(GetNewPackets request, CancellationToken cancellationToken)
    {
        using var cn = connectionProvider.GetConnection();
        var result = await cn.QueryAsync<SavedPacket>(Query, request);
        return result.ToArray();
    }
}
