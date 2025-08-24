using System.Reflection;
using fcg.GameService.Application.Handlers;
using fcg.GameService.Application.Interfaces;
using fcg.GameService.Application.UseCases;
using fcg.GameService.Application.Validators;
using fcg.GameService.Presentation.DTOs.Game.Requests;
using fcg.GameService.Presentation.DTOs.GameLibrary.Requests;
using FluentValidation;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace fcg.GameService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IGameUseCase, GameUseCase>();
        services.AddScoped<IGameLibraryUseCase, GameLibraryUseCase>();

        services.AddScoped<IValidator<CreateGameDTO>, CreateGameDTOValidator>();
        services.AddScoped<IValidator<UpdateGameDTO>, UpdateGameDTOValidator>();
        services.AddScoped<IValidator<UpdateTagsDTO>, UpdateTagsDTOValidator>();
        services.AddScoped<IValidator<CreateGameLibraryDTO>, CreateGameLibraryDTOValidator>();
        services.AddScoped<IValidator<AddGameToLibraryDTO>, AddGameToLibraryDTOValidator>();
        services.AddScoped<IValidator<RemoveGameFromLibraryDTO>, RemoveGameFromLibraryDTOValidator>();

        services.AddExceptionHandler<CustomExceptionHandler>();

        return services;
    }
}
