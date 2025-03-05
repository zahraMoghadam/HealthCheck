


using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class HealthCheckDynamicAttribute : Attribute
{
    public string Name { get; }

    public HealthCheckDynamicAttribute(string name)
    {
        Name = name;
    }
}


public class HealthCheckHelper
{
    public List<HealthCheckDynamicAttribute> GetHealthCheckDynamicAttributes()
    {
        List<HealthCheckDynamicAttribute> actionNames = new List<HealthCheckDynamicAttribute>();
        var controllers = GetSubClasses<ControllerBase>().ToList();

        foreach (var controller in controllers)
        {
            // دریافت تمامی متدهای کنترلر
            var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            foreach (var method in methods)
            {
                // چک کردن اینکه آیا attribute به متد اعمال شده است
                var attribute = method.GetCustomAttribute<HealthCheckDynamicAttribute>();
                if (attribute != null)
                {
                    // اگر attribute پیدا شد، آن را به لیست اضافه می‌کنیم
                    actionNames.Add(attribute);
                }
            }
        }

        return actionNames;
    }
    private static List<Type> GetSubClasses<T>()
    {
        return Assembly.GetCallingAssembly().GetTypes().Where(
            type => type.IsSubclassOf(typeof(T))).ToList();
    }
}

