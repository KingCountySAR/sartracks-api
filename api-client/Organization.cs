using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SarData.Api.Client
{
  public class Organization : IHasId
  {
    public Guid Id { get; set; }

    public string Name { get; set; }

    public bool HasDirectMembers { get; set; } = true;

    [ForeignKey("ParentId")]
    public Organization Parent { get; set; }
    public Guid? ParentId { get; set; }

    public ICollection<Organization> Children { get; set; } = new List<Organization>();

    [ForeignKey("JurisdictionId")]
    public Organization Jurisdiction { get; set; }
    public Guid JurisdictionId { get; set; }
  }
}
