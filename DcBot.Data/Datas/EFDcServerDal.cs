﻿using DcBot.Core.Core;
using DcBot.Data.Interfaces;

namespace DcBot.Data.Datas
{
    public class EFDcServerDal : EFRepositoryDal<DcServer>, IDcServerDal
    {
        public EFDcServerDal(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}