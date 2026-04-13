using System.Text.RegularExpressions;
using VemProJogo.Times.Application.DTOs.Times;
using VemProJogo.Times.Application.Exceptions;

namespace VemProJogo.Times.Application.Rules;

public static class TimeBusinessRules
{
    private const int NameMaxLength = 120;
    private const int AcronymMaxLength = 10;
    private const int ResponsibleNameMaxLength = 120;
    private const int ResponsibleContactMaxLength = 60;
    private const int CrestUrlMaxLength = 300;

    private static readonly Regex MongoObjectIdRegex =
        new("^[a-fA-F0-9]{24}$", RegexOptions.Compiled);

    public static NormalizedTimeData ValidateAndNormalizeForCreate(CreateTimeRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new NormalizedTimeData
        {
            ChampionshipId = NormalizeRequiredObjectId(request.ChampionshipId, nameof(request.ChampionshipId)),
            Name = NormalizeRequired(request.Name, nameof(request.Name), NameMaxLength),
            Acronym = NormalizeOptional(request.Acronym, nameof(request.Acronym), AcronymMaxLength, value => value.ToUpperInvariant()),
            ResponsibleName = NormalizeOptional(request.ResponsibleName, nameof(request.ResponsibleName), ResponsibleNameMaxLength),
            ResponsibleContact = NormalizeOptional(request.ResponsibleContact, nameof(request.ResponsibleContact), ResponsibleContactMaxLength),
            CrestUrl = NormalizeAndValidateUrl(request.CrestUrl, nameof(request.CrestUrl))
        };
    }

    public static NormalizedTimeData ValidateAndNormalizeForUpdate(UpdateTimeRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new NormalizedTimeData
        {
            ChampionshipId = NormalizeRequiredObjectId(request.ChampionshipId, nameof(request.ChampionshipId)),
            Name = NormalizeRequired(request.Name, nameof(request.Name), NameMaxLength),
            Acronym = NormalizeOptional(request.Acronym, nameof(request.Acronym), AcronymMaxLength, value => value.ToUpperInvariant()),
            ResponsibleName = NormalizeOptional(request.ResponsibleName, nameof(request.ResponsibleName), ResponsibleNameMaxLength),
            ResponsibleContact = NormalizeOptional(request.ResponsibleContact, nameof(request.ResponsibleContact), ResponsibleContactMaxLength),
            CrestUrl = NormalizeAndValidateUrl(request.CrestUrl, nameof(request.CrestUrl))
        };
    }

    public static NormalizedPatchTimeData ValidateAndNormalizeForPatch(PatchTimeRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var hasAnyField =
            request.ChampionshipId is not null ||
            request.Name is not null ||
            request.Acronym is not null ||
            request.ResponsibleName is not null ||
            request.ResponsibleContact is not null ||
            request.CrestUrl is not null ||
            request.Active.HasValue;

        if (!hasAnyField)
        {
            throw new BusinessValidationException("Informe ao menos um campo para atualizar o time.");
        }

        return new NormalizedPatchTimeData
        {
            HasChampionshipId = request.ChampionshipId is not null,
            ChampionshipId = request.ChampionshipId is null
                ? null
                : NormalizeRequiredObjectId(request.ChampionshipId, nameof(request.ChampionshipId)),
            HasName = request.Name is not null,
            Name = request.Name is null
                ? null
                : NormalizeRequired(request.Name, nameof(request.Name), NameMaxLength),
            HasAcronym = request.Acronym is not null,
            Acronym = request.Acronym is null
                ? null
                : NormalizeOptional(request.Acronym, nameof(request.Acronym), AcronymMaxLength, value => value.ToUpperInvariant()),
            HasResponsibleName = request.ResponsibleName is not null,
            ResponsibleName = request.ResponsibleName is null
                ? null
                : NormalizeOptional(request.ResponsibleName, nameof(request.ResponsibleName), ResponsibleNameMaxLength),
            HasResponsibleContact = request.ResponsibleContact is not null,
            ResponsibleContact = request.ResponsibleContact is null
                ? null
                : NormalizeOptional(request.ResponsibleContact, nameof(request.ResponsibleContact), ResponsibleContactMaxLength),
            HasCrestUrl = request.CrestUrl is not null,
            CrestUrl = request.CrestUrl is null
                ? null
                : NormalizeAndValidateUrl(request.CrestUrl, nameof(request.CrestUrl)),
            HasActive = request.Active.HasValue,
            Active = request.Active
        };
    }

    private static string NormalizeRequiredObjectId(string value, string fieldName)
    {
        var normalized = NormalizeRequired(value, fieldName, 24);

        if (!MongoObjectIdRegex.IsMatch(normalized))
        {
            throw new BusinessValidationException($"O campo {fieldName} deve estar no formato de ObjectId.");
        }

        return normalized;
    }

    private static string NormalizeRequired(string value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessValidationException($"O campo {fieldName} e obrigatorio.");
        }

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
        {
            throw new BusinessValidationException($"O campo {fieldName} deve ter no maximo {maxLength} caracteres.");
        }

        return normalized;
    }

    private static string? NormalizeOptional(
        string? value,
        string fieldName,
        int maxLength,
        Func<string, string>? transform = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();

        if (normalized.Length > maxLength)
        {
            throw new BusinessValidationException($"O campo {fieldName} deve ter no maximo {maxLength} caracteres.");
        }

        return transform is null ? normalized : transform(normalized);
    }

    private static string? NormalizeAndValidateUrl(string? value, string fieldName)
    {
        var normalized = NormalizeOptional(value, fieldName, CrestUrlMaxLength);

        if (normalized is null)
        {
            return null;
        }

        if (!Uri.TryCreate(normalized, UriKind.Absolute, out _))
        {
            throw new BusinessValidationException($"O campo {fieldName} deve conter uma URL valida.");
        }

        return normalized;
    }
}

public sealed class NormalizedTimeData
{
    public string ChampionshipId { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string? Acronym { get; init; }
    public string? ResponsibleName { get; init; }
    public string? ResponsibleContact { get; init; }
    public string? CrestUrl { get; init; }
}

public sealed class NormalizedPatchTimeData
{
    public bool HasChampionshipId { get; init; }
    public string? ChampionshipId { get; init; }
    public bool HasName { get; init; }
    public string? Name { get; init; }
    public bool HasAcronym { get; init; }
    public string? Acronym { get; init; }
    public bool HasResponsibleName { get; init; }
    public string? ResponsibleName { get; init; }
    public bool HasResponsibleContact { get; init; }
    public string? ResponsibleContact { get; init; }
    public bool HasCrestUrl { get; init; }
    public string? CrestUrl { get; init; }
    public bool HasActive { get; init; }
    public bool? Active { get; init; }
}
