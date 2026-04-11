using System;
using Microsoft.CodeAnalysis;

namespace Resultant.Analyzers;

public static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor ValueAccessedWithoutCheck = new(
        id: "RES001",
        title: "Value accessed without checking IsSuccess",
        messageFormat: "Accessing 'Value' without checking 'IsSuccess' may throw",
        category: "Resultant.Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ResultIgnored = new(
        id: "RES002",
        title: "Result return value is ignored",
        messageFormat: "The Result returned by '{0}' is not used",
        category: "Resultant.Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ErrorsAccessedOnSuccess = new(
        id: "RES003",
        title: "Errors accessed on success path",
        messageFormat: "Accessing 'Errors' on a success path is likely a mistake",
        category: "Resultant.Usage",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);
}
