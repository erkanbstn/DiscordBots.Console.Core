﻿using DcBot.Core.Core;
using DcBot.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DcBot.Data.Datas
{
    public class EFUserDal : EFRepositoryDal<User>, IUserDal
    {
        private readonly AppDbContext _appDbContext;
        public EFUserDal(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}