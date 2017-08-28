using Microsoft.AspNetCore.Mvc;
using Monkey.Filters;
using Puppy.Web.Constants;

namespace Monkey.Areas.Developers.Controllers.Base
{
    [Area(Constants.Endpoint.DevelopersArea.Root)]
    [ServiceFilter(typeof(ApiExceptionFilter))]
    [ServiceFilter(typeof(ApiModelValidateFilter))]
    [Produces(ContentType.Json, ContentType.Xml)]
    public class DevelopersApiController : Controller
    {
    }
}