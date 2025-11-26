using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WEBSGI.Models
{
    public class InlineFileResult : FileContentResult
    {
        public InlineFileResult(byte[] fileContents, string contentType)
            : base(fileContents, contentType)
        {
        }

        public string FileName { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var response = context.HttpContext.Response;
            response.AddHeader("Content-Disposition", "inline; filename=" + FileName);
            base.ExecuteResult(context);
        }
    }

}