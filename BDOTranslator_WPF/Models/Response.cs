using Chromely.Core.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace BDOTranslator_WPF.Models
{
    public class Response
    {
        public static ChromelyResponse OK
        {
            get
            {
                return new ChromelyResponse()
                {
                    Data="",
                    Status = 200
                };
            }
        }
        public static ChromelyResponse NoContent
        {
            get
            {
                return new ChromelyResponse()
                {
                    Data = "No Content",
                    Status = 204
                };
            }
        }
        public static ChromelyResponse NotModified
        {
            get
            {
                return new ChromelyResponse()
                {
                    Data = "Not Modified",
                    Status = 304
                };
            }
        }
        public static ChromelyResponse NotFound
        {
            get
            {
                return new ChromelyResponse()
                {
                    Data = "Not Found",
                    Status = 404
                };
            }
        }
        public static ChromelyResponse Forbid
        {
            get
            {
                return new ChromelyResponse()
                {
                    Data = "Forbidden",
                    Status = 403
                };
            }
        }
        public static ChromelyResponse BadRequest
        {
            get
            {
                return new ChromelyResponse()
                {
                    Data = "Bad Request",
                    Status = 400
                };
            }
        }


        public static ChromelyResponse Success(object data)
        {
            return new ChromelyResponse() { 
                Status=200,
                Data= data
            };
        }
    }
}
