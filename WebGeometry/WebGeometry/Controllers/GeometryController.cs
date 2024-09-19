using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using WebGeometry.Models;
using WebGeometry.Services;

namespace WebGeometry.Controllers
{
    public class GeometryController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Intersec([FromUri] List<Line> lines)
        {
            try
            {
                Point X = Geometry.Intersec(lines);
                return Ok(X);
            }
            catch (Exception e)
            {
                return BadRequest("Invalid data: " + e.Message);
            }
        }
    }
}
