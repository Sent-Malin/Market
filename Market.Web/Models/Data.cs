namespace Market_Web.Models
{
    public class Data
    {
        public string? errorMessage { get; set; }
        public Errors typeError { get; set; }
        public Data(string errorMessage="", Errors typeError=Errors.None)
        {
            this.errorMessage = errorMessage;
            this.typeError = typeError;
        }
        public Data()
        {
        }
    }
}
