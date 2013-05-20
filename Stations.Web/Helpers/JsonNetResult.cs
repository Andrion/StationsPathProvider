namespace Stations.Web
{
    using Newtonsoft.Json;
    using System;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Json net result class.
    /// </summary>
    public class JsonNetResult : JsonResult
    {
        #region Private Fields

        /// <summary>
        /// Json content type.
        /// </summary>
        private const String _jsonContentType = "application/json";

        /// <summary>
        /// Html content type.
        /// </summary>
        private const String _htmlContentType = "text/html";

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public JsonNetResult()
        {
            this.Formatting = Formatting.None;
            this.HttpStatusCode = 200;
        }

        /// <summary>
        /// Constructor with data.
        /// </summary>
        /// <param name="data">Data.</param>
        public JsonNetResult(Object data)
            : this()
        {
            this.Data = data;
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// Serializer settings.
        /// </summary>
        public JsonSerializerSettings SerializerSettings { get; set; }

        /// <summary>
        /// Formatting.
        /// </summary>
        public Formatting Formatting { get; set; }

        /// <summary>
        /// Http status code.
        /// </summary>
        public Int32 HttpStatusCode { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Executes result.
        /// </summary>
        /// <param name="context">Controller context.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw (new ArgumentNullException("context"));
            }

            String httpMethod = context.HttpContext.Request.HttpMethod;

            if ((this.JsonRequestBehavior == JsonRequestBehavior.DenyGet) && (String.Equals(httpMethod, "GET", StringComparison.OrdinalIgnoreCase)))
            {
                throw (new InvalidOperationException("Get is not supported in json result."));
            }

            HttpResponseBase response = context.HttpContext.Response;

            response.ContentType = !String.IsNullOrEmpty(this.ContentType)
                ? this.ContentType
                : _jsonContentType;

            // Fix for accept application/json by IE.
            if (context.HttpContext.Request.Browser.Browser.Equals("IE", StringComparison.InvariantCultureIgnoreCase)
                && !context.HttpContext.Request.IsAjaxRequest() 
                && response.ContentType.Equals(_jsonContentType, StringComparison.InvariantCultureIgnoreCase))
            {
                response.ContentType = _htmlContentType;
            }

            if (this.ContentEncoding != null)
            {
                response.ContentEncoding = this.ContentEncoding;
            }
            if (this.Data == null)
            {
                return;
            }

            var writer = new JsonTextWriter(response.Output)
            {
                Formatting = this.Formatting
            };

            JsonSerializer serializer = JsonSerializer.Create(this.SerializerSettings);

            serializer.Serialize(writer, this.Data);
            writer.Flush();

            response.StatusCode = this.HttpStatusCode;
        }

        #endregion Public Methods
    }

    /// <summary>
    /// File json net result class.
    /// </summary>
    public class FileJsonNetResult : JsonNetResult
    {
        #region Public Methods

        /// <summary>
        /// Executes result.
        /// </summary>
        /// <param name="context">Controller context.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context.HttpContext.Request.Browser.Browser.Contains("IE"))
            {
                context.HttpContext.Response.Write("<textarea>");
                base.ExecuteResult(context);
                context.HttpContext.Response.Write("</textarea>");
                context.HttpContext.Response.ContentType = "text/html";
            }
            else base.ExecuteResult(context);
        }

        #endregion Public Methods

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FileJsonNetResult() { }

        /// <summary>
        /// Constructor with data.
        /// </summary>
        /// <param name="data">Data.</param>
        public FileJsonNetResult(Object data)
            : base(data) { }

        #endregion Constructors
    }
}