using System.Diagnostics.CodeAnalysis;

namespace HsNsH.SuperMarket.CatalogService.Domain.Shared.Commons;

public static class Check
{
    public static T NotNull<T>(T value, [NotNull] string parameterName)
    {
        if (value == null)
        {
            throw new ArgumentNullException(parameterName);
        }

        return value;
    }

    public static T NotNull<T>(T value, [NotNull] string parameterName, string message)
    {
        if (value == null)
        {
            throw new ArgumentNullException(parameterName, message);
        }

        return value;
    }

    public static string NotNull(string value, [NotNull] string parameterName, int maxLength = int.MaxValue, int minLength = 0)
    {
        if (value == null)
        {
            throw new ArgumentException($"{parameterName} can not be null!", parameterName);
        }

        if (value.Length > maxLength)
        {
            throw new ArgumentException($"{parameterName} length must be equal to or lower than {maxLength}!", parameterName);
        }

        if (minLength > 0 && value.Length < minLength)
        {
            throw new ArgumentException($"{parameterName} length must be equal to or bigger than {minLength}!", parameterName);
        }

        return value;
    }

    public static string NotNullOrWhiteSpace(string value, [NotNull] string parameterName, int maxLength = int.MaxValue, int minLength = 0)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{parameterName} can not be null, empty or white space!", parameterName);
        }

        if (value.Length > maxLength)
        {
            throw new ArgumentException($"{parameterName} length must be equal to or lower than {maxLength}!", parameterName);
        }

        if (minLength > 0 && value.Length < minLength)
        {
            throw new ArgumentException($"{parameterName} length must be equal to or bigger than {minLength}!", parameterName);
        }

        return value;
    }

    public static string NotNullOrEmpty(string value, [NotNull] string parameterName, int maxLength = int.MaxValue, int minLength = 0)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException($"{parameterName} can not be null or empty!", parameterName);
        }

        if (value.Length > maxLength)
        {
            throw new ArgumentException($"{parameterName} length must be equal to or lower than {maxLength}!", parameterName);
        }

        if (minLength > 0 && value.Length < minLength)
        {
            throw new ArgumentException($"{parameterName} length must be equal to or bigger than {minLength}!", parameterName);
        }

        return value;
    }


    public static string Length(string value, [NotNull] string parameterName, int maxLength, int minLength = 0)
    {
        if (minLength > 0)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(parameterName + " can not be null or empty!", parameterName);
            }

            if (value.Length < minLength)
            {
                throw new ArgumentException($"{parameterName} length must be equal to or bigger than {minLength}!", parameterName);
            }
        }

        if (value != null && value.Length > maxLength)
        {
            throw new ArgumentException($"{parameterName} length must be equal to or lower than {maxLength}!", parameterName);
        }

        return value;
    }

    public static Int16 Positive(Int16 value, [NotNull] string parameterName)
    {
        return value switch
        {
            0 => throw new ArgumentException($"{parameterName} is equal to zero"),
            < 0 => throw new ArgumentException($"{parameterName} is less than zero"),
            _ => value
        };
    }

    public static Int32 Positive(Int32 value, [NotNull] string parameterName)
    {
        return value switch
        {
            0 => throw new ArgumentException($"{parameterName} is equal to zero"),
            < 0 => throw new ArgumentException($"{parameterName} is less than zero"),
            _ => value
        };
    }

    public static Int64 Positive(Int64 value, [NotNull] string parameterName)
    {
        return value switch
        {
            0 => throw new ArgumentException($"{parameterName} is equal to zero"),
            < 0 => throw new ArgumentException($"{parameterName} is less than zero"),
            _ => value
        };
    }

    public static float Positive(float value, [NotNull] string parameterName)
    {
        return value switch
        {
            0 => throw new ArgumentException($"{parameterName} is equal to zero"),
            < 0 => throw new ArgumentException($"{parameterName} is less than zero"),
            _ => value
        };
    }

    public static double Positive(double value, [NotNull] string parameterName)
    {
        return value switch
        {
            0 => throw new ArgumentException($"{parameterName} is equal to zero"),
            < 0 => throw new ArgumentException($"{parameterName} is less than zero"),
            _ => value
        };
    }

    public static decimal Positive(decimal value, [NotNull] string parameterName)
    {
        return value switch
        {
            0 => throw new ArgumentException($"{parameterName} is equal to zero"),
            < 0 => throw new ArgumentException($"{parameterName} is less than zero"),
            _ => value
        };
    }

    public static Int16 Range(Int16 value, [NotNull] string parameterName, Int16 minimumValue, Int16 maximumValue = Int16.MaxValue)
    {
        if (value < minimumValue || value > maximumValue)
        {
            throw new ArgumentException($"{parameterName} is out of range min: {minimumValue} - max: {maximumValue}");
        }

        return value;
    }

    public static Int32 Range(Int32 value, [NotNull] string parameterName, Int32 minimumValue, Int32 maximumValue = Int32.MaxValue)
    {
        if (value < minimumValue || value > maximumValue)
        {
            throw new ArgumentException($"{parameterName} is out of range min: {minimumValue} - max: {maximumValue}");
        }

        return value;
    }

    public static Int64 Range(Int64 value, [NotNull] string parameterName, Int64 minimumValue, Int64 maximumValue = Int64.MaxValue)
    {
        if (value < minimumValue || value > maximumValue)
        {
            throw new ArgumentException($"{parameterName} is out of range min: {minimumValue} - max: {maximumValue}");
        }

        return value;
    }


    public static float Range(float value, [NotNull] string parameterName, float minimumValue, float maximumValue = float.MaxValue)
    {
        if (value < minimumValue || value > maximumValue)
        {
            throw new ArgumentException($"{parameterName} is out of range min: {minimumValue} - max: {maximumValue}");
        }

        return value;
    }


    public static double Range(double value, [NotNull] string parameterName, double minimumValue, double maximumValue = double.MaxValue)
    {
        if (value < minimumValue || value > maximumValue)
        {
            throw new ArgumentException($"{parameterName} is out of range min: {minimumValue} - max: {maximumValue}");
        }

        return value;
    }


    public static decimal Range(decimal value, [NotNull] string parameterName, decimal minimumValue, decimal maximumValue = decimal.MaxValue)
    {
        if (value < minimumValue || value > maximumValue)
        {
            throw new ArgumentException($"{parameterName} is out of range min: {minimumValue} - max: {maximumValue}");
        }

        return value;
    }

    public static T NotDefaultOrNull<T>(T? value, [NotNull] string parameterName) where T : struct
    {
        if (value == null)
        {
            throw new ArgumentException($"{parameterName} is null!", parameterName);
        }

        if (value.Value.Equals(default(T)))
        {
            throw new ArgumentException($"{parameterName} has a default value!", parameterName);
        }

        return value.Value;
    }
}