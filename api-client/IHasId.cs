using System;

namespace SarData.Api.Client
{
  public interface IHasId
  {
    Guid Id { get; }
  }
}
