namespace MoneyTrackerWebApp.Utils
{
    public class JSONExceptionWrapper
    {
        private readonly Exception ex;
        private readonly JSONExceptionWrapper inner;

        public JSONExceptionWrapper(Exception ex)
        {
            ArgumentNullException.ThrowIfNull(ex);
            this.ex = ex;
            if(ex.InnerException != null)
            {
                inner = new JSONExceptionWrapper(ex.InnerException);
            }
        }

        public string Type => ex.GetType().FullName;
        public string? Source => ex.Source;
        public string? TargetSite => ex.TargetSite.GetType().FullName;
        public string Message => ex.Message;
        public string? CallStack => ex.StackTrace;

        public JSONExceptionWrapper InnerException => inner;

        public override string ToString()
        {
            return ex.ToString();
        }

    }
}
