namespace GettingStartedMassTransit.Consumer.Web.Models.Settings
{
    public class EventStoreDatabaseSettings
    {
        public string MessageEventCollectionName { get; set; }
        public string ConnectionStrings { get; set; }
        public string DatabaseName { get; set; }
    }
}
