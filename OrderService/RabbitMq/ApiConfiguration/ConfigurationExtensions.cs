using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;


namespace OrderService.RabbitMq.ApiConfiguration
{
    public static class ConfigurationExtensions
    {
        public static string GetKeyValue<Tin>(this IConfiguration config, Expression<Func<Tin, string>> key)
        {
            var typeName = typeof(Tin).FullName;
            typeName = typeName.Substring(typeName.IndexOf("CONFIGKEYS"));
            typeName = typeName.Replace('+', '.');
            var parameterName = (key.Body as MemberExpression).Member.Name;
            typeName = typeName + '.' + parameterName;
            return config[typeName];
        }
    }
}
