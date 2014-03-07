using Posh.Socrata.Service.DAL;
using Posh.Socrata.Service.Entity;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Posh.Socrata.Service.Controllers
{
    public class CommentController : ApiController
    {
        private IRepository repository = null;
        private string secertKey = "POSHSocrataComments";

        public CommentController()
        {
            repository = new SqlRepository();
        }

        // GET api/values
        [HttpGet]
        public HttpResponseMessage GetComments(string key, string Id, string cityName, string reportName, bool IsCommentCount = false)
        {
            if (key == secertKey)
            {
                if (!IsCommentCount)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, repository.GetCommentsByCityAndReport(Id, cityName, reportName));
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, repository.GetCommentsCountByCityAndReportName(Id, cityName, reportName));
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Secret key is invalid");
            }
        }

        // POST api/values
        [HttpPost]
        public HttpResponseMessage AddComment(string key, CommentData comment)
        {
            if (key == secertKey)
            {
                if (comment != null)
                {
                    if (repository.AddComments(comment))
                    {
                        return Request.CreateResponse(HttpStatusCode.Created, comment);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.ExpectationFailed, "Comment not added");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Pass valid comment to be added");
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Secret key is invalid");
            }
        }

        /// <summary>
        /// Gets notification for any change in dataset
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetNotifications(string key)
        {
            if (key == secertKey)
            {
                return Request.CreateResponse(HttpStatusCode.OK, repository.GetNotification());
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Secret key is invalid");
            }
        }
    }
}