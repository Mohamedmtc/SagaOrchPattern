using System.ComponentModel.DataAnnotations;
using System;
using Newtonsoft.Json;
using Automatonymous;
using System.ComponentModel.DataAnnotations.Schema;

namespace SagaOrchPattern.Order.Models
{
    public class OutBox
    {
        [Key]
        public Guid OutBoxId { get; set; } = Guid.NewGuid();

        public string Event { get; set; }

        [NotMapped]
        public string ExchangeName
        {
            get
            {
                string data = Event.Substring(Event.LastIndexOf('.'));
                var newdata = data.Replace(".", ":");
                var exchangename = Event.Replace(data, newdata);
                return exchangename;
            }
        }

        public string Message { get; set; }

        public static OutBox CreateInstance<T>(object message)
        {
            OutBox outBox = new OutBox();
            var assemblyQualifiedName = typeof(T).AssemblyQualifiedName.Split(',');
            outBox.Event = $"{assemblyQualifiedName[0].Trim()}, {assemblyQualifiedName[1].Trim()}";
            outBox.Message = JsonConvert.SerializeObject(message);
            return outBox;
        }


    }


}
