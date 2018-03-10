using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SarData.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
  [Route("api/[controller]")]
  public class JurisdictionsController : Controller
  {
    protected readonly StoreContext data;

    public JurisdictionsController(StoreContext data)
    {
      this.data = data;
    }

    // GET api/values
    [HttpGet]
    public async Task<System.Collections.IEnumerable> Get()
    {
      return await data.Jurisdictions.Select(f => new { f.Id, f.Name, f.ParentId }).ToListAsync();
    }

    // GET api/values/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
      return "value";
    }

    // POST api/values
    [HttpPost]
    public void Post([FromBody]string value)
    {
    }

    // PUT api/values/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody]string value)
    {
    }

    // DELETE api/values/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
  }
}
