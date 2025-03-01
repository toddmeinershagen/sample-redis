using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using MethodBoundaryAspect.Fody.Attributes;
using Newtonsoft.Json;

public partial class CompanyServicesProvider
{
    public class LogAttribute : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs arg)
        {
            base.OnEntry(arg);

            var builder = new StringBuilder();
            builder.AppendLine($"OnEntry:   {arg.Method.Name}");
            for (var index = 0; index < arg.Arguments.Count(); index++)
            {
                var parameters = arg.Method.GetParameters();
                var parameter = parameters[index];
                var name = parameter.Name;
                var type = parameter.ParameterType.Name;
                var value = arg.Arguments[index];
                builder.AppendLine($"           {name}:  {value}");
            }
            Console.WriteLine(builder.ToString());
        }

        public override void OnExit(MethodExecutionArgs arg)
        {
            Console.WriteLine($"OnExit:     {arg.Method.Name}");
            base.OnExit(arg);
        }

        public override void OnException(MethodExecutionArgs arg)
        {
            Console.WriteLine($"OnException:  {arg.Exception}");
            base.OnException(arg);
        }

        private string GetJson(object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented);
        }
    }
}