using System.Runtime.Serialization;

namespace FeeCollectorApplication.Models
{
    [DataContract]
    public class PieChart
    {
        public PieChart(string isPending, double value)
        {
            this.y = value;
            this.label = isPending;
        }

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "label")]
        public string label = "";

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public double y = 0;
    }
}
