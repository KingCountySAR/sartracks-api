using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SarData.Api.Client
{
  public class Jurisdiction : IHasId
  {
    public Guid Id { get; set; }
    public string Name { get; set; }

    [ForeignKey("AgencyId")]
    public Organization Agency { get; set; }
    public Guid? AgencyId { get; set; }


    [ForeignKey("ParentId")]
    public Jurisdiction Parent { get; set; }
    public Guid? ParentId { get; set; }

    public ICollection<Jurisdiction> Children { get; set; } = new List<Jurisdiction>();
  }
}
