namespace blogpost_website_api.Respons
{
    public class respons
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public respons(bool Success, string Message , object Data = null) 
        {
            this.Success = Success;
            this.Message = Message;
            this.Data = Data;
        }

        public respons(bool Success ,object Data = null) 
        {
            this.Success = Success;
            this.Data= Data;
        }
    }
}
