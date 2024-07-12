using System.ComponentModel;
using System.Reflection;

namespace GameStore.Domain.Extensions;
public static class EnumExtensions
{
    public static TEnum? GetEnumValueFromDescription<TEnum>(string? description)
        where TEnum : struct, Enum
    {
        if (!string.IsNullOrEmpty(description) && !string.IsNullOrWhiteSpace(description))
        {
            TEnum[] allEnums = (TEnum[])Enum.GetValues(typeof(TEnum));

            foreach (TEnum singleEnum in allEnums)
            {
                FieldInfo fieldInfo = singleEnum.GetType().GetField(singleEnum.ToString());
                if (fieldInfo != null)
                {
                    DescriptionAttribute descriptionAttribute = (DescriptionAttribute)fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute), false);

                    if (descriptionAttribute.Description == description)
                    {
                        return singleEnum;
                    }
                }
            }
        }

        return null;
    }

    public static string GetEnumDescription(Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field!, typeof(DescriptionAttribute));
        return attribute == null ? value.ToString() : attribute.Description;
    }

    public static List<string> GetListEnumDesctiptions<T>(T[] values)
        where T : Enum
    {
        var returnList = new List<string>();
        foreach (var value in values)
        {
            returnList.Add(GetEnumDescription(value));
        }

        return returnList;
    }
}
