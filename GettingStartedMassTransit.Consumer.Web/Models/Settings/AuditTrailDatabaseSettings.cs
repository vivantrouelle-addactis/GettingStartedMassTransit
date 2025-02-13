namespace GettingStartedMassTransit.Consumer.Web.Models.Settings
{
    public class AuditTrailDatabaseSettings
    {
        public string ApplicationBetaCollectionName { get; set; }
        public string AuditTrailCollectionName { get; set; }
        public string ConnectionStrings { get; set; }
        public string DatabaseName { get; set; }
    }
}
