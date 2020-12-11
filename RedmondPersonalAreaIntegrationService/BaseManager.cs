using Microsoft.AspNetCore.Mvc;
using RedmondPersonalAreaIntegrationService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RedmondLoyaltyMiddleware.Integration
{
    public abstract class BaseManager : ControllerBase
    {
        public ActionResult LoadPrimaryPack(IEnumerable<BaseProcessingModel> models)
        {
            if (models == null) return BadRequest("Ошибка передачи аргументов");
            try
            {
                var provider = new LoyaltyDBProvider();
                provider.ExecuteNonQuery(GetPrimaryQuery(models));

                return Ok(models.Select(m => new PackResult() { IsSuccess = true, Id = m.Id }).ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        protected abstract string GetPrimaryQuery(IEnumerable<BaseProcessingModel> models);
        protected abstract string GetQuery(BaseProcessingModel model);

        public ActionResult LoadPack(IEnumerable<BaseProcessingModel> models)
        {
            if (models == null) return BadRequest("Ошибка передачи аргументов");
            var result = new List<PackResult>();

            foreach (var m in models)
            {
                try
                {
                    var provider = new LoyaltyDBProvider();
                    provider.ExecuteNonQuery(GetQuery(m));

                    result.Add(new PackResult() { IsSuccess = true, Id = m.Id });
                }
                catch (Exception e)
                {
                    result.Add(new PackResult() { IsSuccess = false, ErrorMessage = e.Message, Id = m.Id });
                }
            }

            return Ok(result);
        }
    }
}
