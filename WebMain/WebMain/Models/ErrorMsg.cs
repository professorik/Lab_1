using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMain.Models
{
    public class ErrorMsg
    {
        public string Message { get; set; }
        public ErrorMsg()
        {
            Message = "";
        }

        public ErrorMsg(string message)
        {
            Message = message;
        }
    }
}