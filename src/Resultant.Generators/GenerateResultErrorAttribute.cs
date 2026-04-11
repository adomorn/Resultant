using System;

namespace Resultant.Generators;

/// <summary>
/// Marks a partial record for source generation of ResultError factory methods.
/// The generator will produce static factory methods that create Result.Fail&lt;T&gt; instances.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class GenerateResultErrorAttribute : Attribute
{
}
