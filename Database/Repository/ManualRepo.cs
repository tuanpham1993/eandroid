using Database.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Repository
{
  public class ManualRepo : GenericRepo<Manual>
  {
    public ManualRepo(EAppContext db) : base(db)
    {
    }
  }
}
