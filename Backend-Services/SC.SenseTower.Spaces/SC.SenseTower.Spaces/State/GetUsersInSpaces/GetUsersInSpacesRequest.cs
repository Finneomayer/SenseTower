using MediatR;

namespace SC.SenseTower.Spaces.State.GetUsersInSpaces;

/// <summary>
/// Запрос на получение пользователей в помещениях
/// </summary>
/// <param name = "GetCount" >максимальное число пользователей в ответе</param>
public record GetUsersInSpacesRequest(int? GetCount, string? AccessToken) : IRequest<GetUsersInSpacesResponse>;