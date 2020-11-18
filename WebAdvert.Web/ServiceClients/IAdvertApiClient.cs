using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvertApi.Models;

namespace WebAdvert.Web.ServiceClients
{
    public interface IAdvertApiClient
    {
        Task<AdvertRespose> Create(CreateAdvertModel model);
        Task<bool> Confirm(ConfirmAdvertRequest model);
    }
}
